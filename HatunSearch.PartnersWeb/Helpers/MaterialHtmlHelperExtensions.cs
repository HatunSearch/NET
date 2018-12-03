// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using HatunSearch.PartnersWeb.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Helpers
{
	public static class MaterialHtmlHelperExtensions
	{
		public enum MaterialTextfieldType : byte
		{
			Filled = 1,
			MultipleLinesFilled = 2
		}

		public static MvcHtmlString MakeDropDown<TModel, TProperty, TValue>(this HtmlHelper<TModel> helper, MaterialTextfieldType type, Expression<Func<TModel, TProperty>> expression,
			IEnumerable<TProperty> items, Func<TProperty, string> displayNameSelector, TValue value = default(TValue)) where TProperty : IDTO<TValue>
		{
			TagBuilder tbTextfield = new TagBuilder("md:textfield"), tbSelect = new TagBuilder("select"), tbLabel = new TagBuilder("label"), tbHelperText = null;
			MemberInfo memberInfo = (expression.Body as MemberExpression)?.Member;
			DisplayNameAttribute displayNameAttribute = memberInfo.GetCustomAttribute<DisplayNameAttribute>();
			LocalizationProvider localization = helper.ViewBag.LocalizationProvider as LocalizationProvider;
			string name = memberInfo.Name;
			string displayName = localization[displayNameAttribute?.DisplayName ?? name];
			IDictionary<string, string> textfieldAttributes = tbTextfield.Attributes;
			textfieldAttributes.Add("type", type.ToString().ToLower());
			string selectName = $"cbo{name}";
			IDictionary<string, string> selectAttributes = tbSelect.Attributes;
			selectAttributes.Add("id", selectName);
			selectAttributes.Add("name", name);
			if (items != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TProperty item in items)
					stringBuilder.Append($"<option {(item.Id.Equals(value) ? "selected " : string.Empty)}value=\"{item.Id}\">{displayNameSelector(item)}</option>");
				tbSelect.InnerHtml = stringBuilder.ToString();
			}
			if (helper.ViewBag.Errors is IDictionary<string, string> errors)
			{
				if (errors.ContainsKey(name))
				{
					textfieldAttributes.Add("hasErrors", "true");
					tbHelperText = new TagBuilder("md:helpertext");
					string error = errors[name];
					tbHelperText.SetInnerText(localization[error]);
				}
			}
			tbLabel.Attributes.Add("for", selectName);
			tbLabel.SetInnerText(displayName);
			string textfieldInnerHtml = $"{tbSelect.ToString()}{tbLabel.ToString()}{tbHelperText?.ToString() ?? string.Empty}";
			tbTextfield.InnerHtml = textfieldInnerHtml;
			return new MvcHtmlString(tbTextfield.ToString());
		}
		public static MvcHtmlString MakeTextfield<TModel, TProperty>(this HtmlHelper<TModel> helper, MaterialTextfieldType type, Expression<Func<TModel, TProperty>> expression,
			TProperty value = default(TProperty))
		{
			MemberInfo memberInfo = (expression.Body as MemberExpression)?.Member;
			DisplayNameAttribute displayNameAttribute = memberInfo.GetCustomAttribute<DisplayNameAttribute>();
			DataTypeAttribute dataTypeAttribute = memberInfo.GetCustomAttribute<DataTypeAttribute>();
			LocalizationProvider localization = helper.ViewBag.LocalizationProvider as LocalizationProvider;
			string name = memberInfo.Name;
			string displayName = localization[displayNameAttribute?.DisplayName ?? name];
			string inputType = null;
			bool isSpellcheckingDisabled = false;
			switch (dataTypeAttribute?.DataType)
			{
				case DataType.EmailAddress:
					inputType = "email";
					isSpellcheckingDisabled = true;
					break;
				case DataType.Password:
					inputType = "password";
					break;
				case DataType.PhoneNumber:
					inputType = "tel";
					isSpellcheckingDisabled = true;
					break;
				case DataType.Url:
				default:
					inputType = "text";
					if (dataTypeAttribute?.DataType == DataType.Url) isSpellcheckingDisabled = true;
					break;
			}
			RequiredAttribute requiredAttribute = memberInfo.GetCustomAttribute<RequiredAttribute>();
			StringLengthAttribute stringLengthAttribute = memberInfo.GetCustomAttribute<StringLengthAttribute>();
			IDictionary<string, object> attributes = new Dictionary<string, object>();
			if (requiredAttribute != null) attributes.Add("required", "required");
			if (stringLengthAttribute != null) attributes.Add("maxlength", stringLengthAttribute.MaximumLength);
			if (isSpellcheckingDisabled) attributes.Add("spellcheck", false);
			return MakeTextfield(helper, type, name, displayName, inputType, attributes, value);
		}
		public static MvcHtmlString MakeTextfield(this HtmlHelper helper, MaterialTextfieldType type, string name, string displayName, string inputType, IDictionary<string, object> attributes,
			object value = null)
		{
			TagBuilder tbTextfield = new TagBuilder("md:textfield"), tbInput = null, tbLabel = new TagBuilder("label"), tbHelperText = null;
			LocalizationProvider localization = helper.ViewBag.LocalizationProvider as LocalizationProvider;
			tbInput = new TagBuilder(type == MaterialTextfieldType.Filled ? "input" : "textarea");
			IDictionary<string, string> textfieldAttributes = tbTextfield.Attributes;
			if (type == MaterialTextfieldType.Filled) textfieldAttributes.Add("type", type.ToString().ToLower());
			else textfieldAttributes.Add("type", type.ToString().Replace("MultipleLines", string.Empty).ToLower());
			string inputName = $"txt{name}";
			IDictionary<string, string> inputAttributes = tbInput.Attributes;
			inputAttributes.Add("id", inputName);
			inputAttributes.Add("name", name);
			if (type == MaterialTextfieldType.Filled) inputAttributes.Add("type", inputType);
			if (attributes != null)
			{
				foreach (KeyValuePair<string, object> attribute in attributes)
					inputAttributes.Add(attribute.Key, attribute.Value.ToString());
			}
			if (value != null) inputAttributes.Add("value", value.ToString());
			if (helper.ViewBag.Errors is IDictionary<string, string> errors)
			{
				if (errors.ContainsKey(name))
				{
					textfieldAttributes.Add("hasErrors", "true");
					tbHelperText = new TagBuilder("md:helpertext");
					string error = errors[name];
					tbHelperText.SetInnerText(localization[error]);
				}
			}
			tbLabel.Attributes.Add("for", inputName);
			tbLabel.SetInnerText(displayName);
			string inputHtml = null;
			inputHtml = type == MaterialTextfieldType.Filled? tbInput.ToString(TagRenderMode.SelfClosing) : tbInput.ToString();
			string textfieldInnerHtml = $"{inputHtml}{tbLabel.ToString()}{tbHelperText?.ToString() ?? string.Empty}";
			tbTextfield.InnerHtml = textfieldInnerHtml;
			return new MvcHtmlString(tbTextfield.ToString());
		}
	}
}