// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HatunSearch.Entities.Patterns
{
	[JsonConverter(typeof(DTOJsonConverter))]
	public abstract class DTO : IDTO
	{
		public sealed class DTOJsonConverter : JsonConverter
		{
			private readonly static IDictionary<Type, PropertyInfo[]> typeProperties = new Dictionary<Type, PropertyInfo[]>();

			public override bool CanConvert(Type objectType) => typeof(DTO).IsAssignableFrom(objectType);
			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => throw new NotImplementedException();
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				Type type = value.GetType();
				typeProperties.TryGetValue(type, out PropertyInfo[] properties);
				if (properties == null)
				{
					properties = type.GetProperties();
					typeProperties.Add(type, properties);
				}
				writer.WriteStartObject();
				foreach (PropertyInfo property in properties)
				{
					object propertyValue = property.GetValue(value);
					Type propertyType = property.PropertyType;
					JsonIgnoreAttribute jsonIgnore = property.GetCustomAttribute<JsonIgnoreAttribute>();
					if (propertyValue != null && jsonIgnore == null && (!propertyType.IsGenericType || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() != typeof(CompositeKey<,>))))
					{
						writer.WritePropertyName(property.Name);
						serializer.Serialize(writer, typeof(IDTO).IsAssignableFrom(propertyType) ? (propertyValue as IDTO).Id : propertyValue);
					}
				}
				writer.WriteEndObject();
			}
		}

		public object Id { get; set; }
	}
	public abstract class DTO<TId> : DTO, IDTO<TId>
	{
		public new TId Id
		{
			get => base.Id != null ? (TId)base.Id : default;
			set => base.Id = value;
		}
	}
	public abstract class DTO<TId1, TId2> : DTO, IDTO<TId1, TId2>
	{
		public DTO() { Id = new CompositeKey<TId1, TId2>(); }

		protected new CompositeKey<TId1, TId2> Id
		{
			get => base.Id as CompositeKey<TId1, TId2>;
			set => base.Id = value;
		}

		CompositeKey<TId1, TId2> IDTO<TId1, TId2>.Id => Id;
		CompositeKey<TId1, TId2> IDTO<CompositeKey<TId1, TId2>>.Id
		{
			get => Id;
			set => Id = value;
		}
	}
}