using AutoMapper;

namespace Integration.Service.Converters
{
	internal class StringToBoolNullableConverter : TypeConverter<string, bool?>
	{
		protected override bool? ConvertCore(string source)
		{
			return source == null ? (bool?)null : source == "1";
		}
	}
}
