
namespace FiresecAPI.Models
{
	public static class ConfigurationDriverHelper
	{
		public static void AddPlainEnumProprety(Driver driver, byte no, string propertyName, byte offset, string parameter1Name, string parameter2Name, int startValue = 0)
		{
			var property = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = "0",
				Offset = offset
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
				IsInternalDeviceParameter = true,
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = DriverPropertyTypeEnum.BoolType,
				Offset = offset
			};
			driver.Properties.Add(property);
		}

		public static void AddIntProprety(Driver driver, byte no, string propertyName, byte offset, int defaultValue, int min, int max)
		{
			var property = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Offset = offset,
				Default = defaultValue.ToString(),
				Min = (short)min,
				Max = (short)max
			};
			driver.Properties.Add(property);
		}

		public static void AddPropertyParameter(DriverProperty property, string name, int value)
		{
			var parameter = new DriverPropertyParameter()
			{
				Name = name,
				Value = value.ToString()
			};
			property.Parameters.Add(parameter);
		}

		public static void AddAlternativePropertyParameter(DriverProperty property, string name, string alternativeName, int value)
		{
			var parameter = new DriverPropertyParameter()
			{
				Name = name,
				AlternativeName = alternativeName,
				Value = value.ToString()
			};
			property.Parameters.Add(parameter);
		}
	}
}