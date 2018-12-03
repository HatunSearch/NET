// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HatunSearch.Entities.DataAnnotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class UrlExAttribute : ValidationAttribute
	{
		private readonly static Regex regex = new Regex("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$");

		public override bool IsValid(object value) => value is string url ? regex.IsMatch(url) : false;
	}
}