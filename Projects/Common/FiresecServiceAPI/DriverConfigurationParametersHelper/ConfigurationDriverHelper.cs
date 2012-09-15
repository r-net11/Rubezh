
namespace FiresecAPI.Models
{
	public static class ConfigurationDriverHelper
	{
		public static void AddPlainEnumProprety(Driver driver, byte no, string propertyName, byte offset, string parameter1Name, string parameter2Name, int startValue = 0,
			int minBit = 0, int maxBit = 0, bool useMask = false, bool highByte = false, string dflt = "0" 
			)
		{
			var property = new DriverProperty()
			{
				IsAUParameter = true,
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = dflt,
				BitOffset = offset,
				MinBit = minBit,
				MaxBit = maxBit,
				UseMask = useMask,
				HighByte = highByte,
			};
			var parameter1 = new DriverPropertyParameter()
			{
				Name = parameter1Name,
				Value = startValue.ToString()
			};
			var parameter2 = new DriverPropertyParameter()
			{
				Name = parameter2Name,
				Value = (startValue + 1).ToString()
			};
			property.Parameters.Add(parameter1);
			property.Parameters.Add(parameter2);
			driver.Properties.Add(property);
		}

		public static void AddBoolProprety(Driver driver, byte no, string propertyName, byte offset)
		{
			var property = new DriverProperty()
			{
				IsAUParameter = true,
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = DriverPropertyTypeEnum.BoolType,
				BitOffset = offset
			};
			driver.Properties.Add(property);
		}

		public static void AddIntProprety(Driver driver, byte no, string propertyName, int offset, int defaultValue, int min, int max, bool useMask = false, bool highByte = false)
		{
			var property = new DriverProperty()
			{
				IsAUParameter = true,
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				BitOffset = offset,
				Default = defaultValue.ToString(),
				Min = (ushort)min,
				Max = (ushort)max,
				UseMask = useMask,
				HighByte = highByte
			};
			driver.Properties.Add(property);
		}

		public static void AddPropertyParameter(DriverProperty property, string name, int value)
		{
			var parameter = new DriverPropertyParameter()
			{
				Name = name,
				Value = value.ToString(),
			};
			property.Parameters.Add(parameter);
		}
	}
}