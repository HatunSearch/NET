// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HatunSearch.Entities.Globalization
{
	[JsonConverter(typeof(LocalizationDictionaryJsonConverter))]
	public sealed class LocalizationDictionary
	{
		private readonly IEnumerable<KeyValuePair<string, string>> collection = null;
		private readonly Thread currentThread = Thread.CurrentThread;

		public LocalizationDictionary(IEnumerable<KeyValuePair<string, string>> collection) => this.collection = collection;

		public sealed class LocalizationDictionaryJsonConverter : JsonConverter<LocalizationDictionary>
		{
			public override LocalizationDictionary ReadJson(JsonReader reader, Type objectType, LocalizationDictionary existingValue, bool hasExistingValue, JsonSerializer serializer) =>
				throw new NotImplementedException();
			public override void WriteJson(JsonWriter writer, LocalizationDictionary value, JsonSerializer serializer) => serializer.Serialize(writer, value?.Value);
		}

		public static implicit operator string(LocalizationDictionary dictionary) => dictionary.Value;

		public override string ToString() => Value;

		private string CurrentLanguage => currentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
		public string Value => collection.FirstOrDefault(i => i.Key == CurrentLanguage).Value;
	}
}