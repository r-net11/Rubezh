using AutoMapper;

namespace Integration.Service.Converters
{
	internal class StringToIntNullableConverter : TypeConverter<string, int?>
	{
		protected override int? ConvertCore(string source)
		{
			int result;
			if (int.TryParse(source, out result))
			{
				return result;
			}

			return null;
		}
	}
}
