// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Data;
using HatunSearch.Entities.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Http.ModelBinding
{
	public sealed class DTOModelBinder : IModelBinder
	{
		private readonly static IDictionary<PropertyInfo, IEnumerable<ValidationAttribute>> propertiesValidations = new Dictionary<PropertyInfo, IEnumerable<ValidationAttribute>>();
		private readonly static IDictionary<Type, PropertyInfo[]> typeProperties = new Dictionary<Type, PropertyInfo[]>();

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			Type modelType = bindingContext.ModelType;
			bool hasComposedKey = modelType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDTO<,>));
			typeProperties.TryGetValue(modelType, out PropertyInfo[] properties);
			if (properties == null)
			{
				properties = modelType.GetProperties();
				typeProperties.Add(modelType, properties);
			}
			IValueProvider valueProvider = bindingContext.ValueProvider;
			object result = bindingContext.Model ?? Activator.CreateInstance(modelType);
			IEnumerable<PropertyInfo> filteredProperties = properties.Where(i => i.CanWrite);
			if (hasComposedKey) filteredProperties = filteredProperties.Where(i => i.Name != "Id");
			foreach (PropertyInfo property in filteredProperties)
			{
				string name = property.Name;
				Type propertyType = property.PropertyType;
				ValueProviderResult propertyValueResult = valueProvider.GetValue(name);
				string propertyValue = propertyValueResult?.AttemptedValue;
				if (propertyValue != null && property.GetCustomAttribute<DataTypeAttribute>()?.DataType != DataType.Password) propertyValue = propertyValue.Trim();
				propertiesValidations.TryGetValue(property, out IEnumerable<ValidationAttribute> validationAttributes);
				if (validationAttributes == null)
				{
					validationAttributes = property.GetCustomAttributes().Where(i => typeof(ValidationAttribute).IsAssignableFrom(i.GetType())).Cast<ValidationAttribute>();
					propertiesValidations.Add(property, validationAttributes);
				}
				bool isValid = true;
				string errorMessage = null;
				if (validationAttributes.FirstOrDefault(i => i is RequiredAttribute) is RequiredAttribute || !string.IsNullOrEmpty(propertyValue))
				{
					foreach (ValidationAttribute validation in validationAttributes)
					{
						if (!validation.IsValid(propertyValue))
						{
							isValid = false;
							errorMessage = validation.ErrorMessage;
							break;
						}
					}
				}
				if (isValid)
				{
					if (typeof(IDTO).IsAssignableFrom(propertyType))
					{
						Type basePropertyType = propertyType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IDTO<>) || i.GetGenericTypeDefinition() == typeof(IDTO<,>)));
						if (basePropertyType.GetGenericTypeDefinition() == typeof(IDTO<>))
						{
							Type basePropertyTypeArgument = basePropertyType.GetGenericArguments()[0];
							IDTO propertyObjectValue = Activator.CreateInstance(propertyType) as IDTO;
							if (basePropertyTypeArgument == typeof(string)) propertyObjectValue.Id = propertyValue;
							else if (basePropertyTypeArgument == typeof(Guid) && propertyValue != null) propertyObjectValue.Id = Guid.Parse(propertyValue);
							else if (basePropertyTypeArgument == typeof(byte) && propertyValue != null) propertyObjectValue.Id = byte.Parse(propertyValue);
							property.SetValue(result, propertyObjectValue);
						}
						else
						{
							Type[] basePropertyTypeArguments = basePropertyType.GetGenericArguments();
							Type basePropertyTypeFirstArgument = basePropertyTypeArguments.First(), basePropertyTypeSecondArgument = basePropertyTypeArguments.Last();
							IDTO propertyObjectValue = Activator.CreateInstance(propertyType) as IDTO;
							Type idType = typeof(CompositeKey<,>).MakeGenericType(basePropertyTypeFirstArgument, basePropertyTypeSecondArgument);
							object idObject = Activator.CreateInstance(idType);
							object idFirstKey = null, idSecondKey = null;
							string[] propertyValues = propertyValue.Split('|');
							string firstPropertyValue = propertyValues.First(), secondPropertyValue = propertyValues.Last();
							if (basePropertyTypeFirstArgument == typeof(string)) idFirstKey = firstPropertyValue;
							else if (typeof(IDTO<string>).IsAssignableFrom(basePropertyTypeFirstArgument))
							{
								IDTO<string> idFirstKeyObject = Activator.CreateInstance(basePropertyTypeFirstArgument) as IDTO<string>;
								idFirstKeyObject.Id = firstPropertyValue;
								idFirstKey = idFirstKeyObject;
							}
							if (basePropertyTypeSecondArgument == typeof(string)) idSecondKey = secondPropertyValue;
							else if (typeof(IDTO<string>).IsAssignableFrom(basePropertyTypeSecondArgument))
							{
								IDTO<string> idSecondKeyObject = Activator.CreateInstance(basePropertyTypeSecondArgument) as IDTO<string>;
								idSecondKeyObject.Id = secondPropertyValue;
								idSecondKey = idSecondKeyObject;
							}
							idType.GetProperty("FirstKey").SetValue(idObject, idFirstKey);
							idType.GetProperty("SecondKey").SetValue(idObject, idSecondKey);
							propertyObjectValue.Id = idObject;
							property.SetValue(result, propertyObjectValue);
						}
					}
					else
					{
						if (propertyType == typeof(Guid) && propertyValue != null) property.SetValue(result, Guid.Parse(propertyValue));
						else property.SetValue(result, propertyValue);
					}
				}
				else bindingContext.ModelState.AddModelError(name, errorMessage);
			}
			return result;
		}
	}
}