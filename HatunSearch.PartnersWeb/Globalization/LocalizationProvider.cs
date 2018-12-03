// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace HatunSearch.PartnersWeb.Globalization
{
	public sealed class LocalizationProvider
	{
		private readonly Thread currentThread = Thread.CurrentThread;
		private readonly IDictionary<string, string> dictionary = new Dictionary<string, string>();

		public LocalizationProvider(string filePath) => FillDictionary(filePath);

		public string this[string id]
		{
			get
			{
				dictionary.TryGetValue($"{CurrentLanguage}${id}", out string result);
				return result;
			}
		}

		private void FillDictionary(string filePath)
		{
			XDocument document = XDocument.Load(filePath);
			XElement localization = document.Root;
			IEnumerable<XElement> languages = localization.Elements("language");
			foreach (XElement language in languages)
			{
				XAttribute languageIdAttribute = language.Attribute("id");
				string languageId = languageIdAttribute.Value.ToUpper();
				IEnumerable<XElement> items = language.Elements("item");
				foreach (XElement item in items)
				{
					XAttribute itemKeyAttribute = item.Attribute("key"), itemValueAttribute = item.Attribute("value");
					string value = itemValueAttribute?.Value ?? item.Value;
					dictionary.Add($"{languageId}${itemKeyAttribute.Value}", value);
				}
			}
		}
		public void Join(LocalizationProvider provider)
		{
			foreach (KeyValuePair<string, string> item in provider.dictionary)
				dictionary.Add(item);
		}

		public string CurrentLanguage => currentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
	}
}