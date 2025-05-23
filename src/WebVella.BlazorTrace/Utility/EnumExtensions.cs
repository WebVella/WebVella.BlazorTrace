using System.ComponentModel;
using System.Globalization;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace;

public static class EnumExtensions
{
	public static string ToDescriptionString<TEnum>(this TEnum e) where TEnum : IConvertible
	{
		string description = "";

		if (e is Enum)
		{
			Type type = e.GetType();
			var name = type.GetEnumName(e.ToInt32(CultureInfo.InvariantCulture));
			if (String.IsNullOrWhiteSpace(name)) return description;

			var memInfo = type.GetMember(name);
			var soAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (soAttributes.Length > 0)
			{
				// we're only getting the first description we find
				// others will be ignored
				description = ((DescriptionAttribute)soAttributes[0]).Description;
			}
		}

		return description;
	}

}
