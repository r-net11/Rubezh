using AutoMapper;

namespace Integration.Service.Converters
{
	internal class StringToIntConverter : TypeConverter<string, int>
	{
		protected override int ConvertCore(string source)
		{
			if (string.IsNullOrEmpty(source))
				return default(int); //TODO: can throw an Exception for non-nullable int.

			return int.Parse(source);
		}
	}
}
