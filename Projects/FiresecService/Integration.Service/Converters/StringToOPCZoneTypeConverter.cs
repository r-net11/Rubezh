using AutoMapper;
using StrazhAPI.Enums;
using System;

namespace Integration.Service.Converters
{
	internal class StringToOPCZoneTypeConverter : TypeConverter<string, OPCZoneType?>
	{
		protected override OPCZoneType? ConvertCore(string source)
		{
			OPCZoneType result;
			if (Enum.TryParse(source, out result))
				return result;

			return null;
		}
	}
}
