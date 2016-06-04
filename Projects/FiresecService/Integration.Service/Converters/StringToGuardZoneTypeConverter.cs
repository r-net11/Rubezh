using AutoMapper;
using StrazhAPI.Enums;
using System;

namespace Integration.Service.Converters
{
	internal class StringToGuardZoneTypeConverter : TypeConverter<string, GuardZoneType?>
	{
		protected override GuardZoneType? ConvertCore(string source)
		{
			GuardZoneType result;
			if (Enum.TryParse(source, out result))
			{
				return result;
			}

			return null;
		}
	}
}
