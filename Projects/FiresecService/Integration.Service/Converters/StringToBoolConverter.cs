using AutoMapper;

namespace Integration.Service.Converters
{
	internal class StringToBoolConverter : TypeConverter<string, bool>
	{
		protected override bool ConvertCore(string source)
		{
			return source == "1";
		}
	}
}
