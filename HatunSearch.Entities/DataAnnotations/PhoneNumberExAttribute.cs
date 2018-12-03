// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using PhoneNumbers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;

namespace HatunSearch.Entities.DataAnnotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class PhoneNumberExAttribute : ValidationAttribute
	{
		public PhoneNumberExAttribute(bool acceptFixedNumbersOnly = false, bool acceptMobileNumbersOnly = false)
		{
			AcceptFixedNumbersOnly = acceptFixedNumbersOnly;
			AcceptMobileNumbersOnly = acceptMobileNumbersOnly;
		}

		public override bool IsValid(object value)
		{
			if (value is string phoneNumber)
			{
				PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
				CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
				RegionInfo regionInfo = new RegionInfo(cultureInfo.Name);
				PhoneNumber phoneNumberObj = phoneNumberUtil.Parse(phoneNumber, regionInfo.TwoLetterISORegionName);
				bool result = phoneNumberUtil.IsValidNumber(phoneNumberObj);
				if (result && (AcceptFixedNumbersOnly || AcceptMobileNumbersOnly))
				{
					PhoneNumberType numberType = phoneNumberUtil.GetNumberType(phoneNumberObj);
					if (AcceptFixedNumbersOnly) result = numberType == PhoneNumberType.FIXED_LINE || numberType == PhoneNumberType.FIXED_LINE_OR_MOBILE;
					else result = numberType == PhoneNumberType.FIXED_LINE_OR_MOBILE || numberType == PhoneNumberType.MOBILE;
				}
				return result;
			}
			else return false;
		}

		public bool AcceptFixedNumbersOnly { get; private set; }
		public bool AcceptMobileNumbersOnly { get; private set; }
	}
}