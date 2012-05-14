using XFiresecAPI;

namespace GKModule.Converter
{
	public static class CommonHelper
	{
		public static void AddPlainEnumProprety(XDriver xDriver, byte no, string propertyName, byte offset, string parameter1Name, string parameter2Name, int startValue = 0)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 0,
				Offset = offset
			};
			var parameter1 = new XDriverPropertyParameter()
			{
				Name = parameter1Name,
				Value = startValue
			};
			var parameter2 = new XDriverPropertyParameter()
			{
				Name = parameter2Name,
				Value = startValue + 1
			};
			property.Parameters.Add(parameter1);
			property.Parameters.Add(parameter2);
			xDriver.Properties.Add(property);
		}

		public static void AddBoolProprety(XDriver xDriver, byte no, string propertyName, byte offset)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
				Offset = offset
			};
			xDriver.Properties.Add(property);
		}

		public static void AddIntProprety(XDriver xDriver, byte no, string propertyName, byte offset, int defaultValue, int min, int max)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Offset = offset,
				Default = defaultValue,
				Min = (short)min,
				Max = (short)max
			};
			xDriver.Properties.Add(property);
		}

		public static void AddPropertyParameter(XDriverProperty property, string name, int value)
		{
			var parameter = new XDriverPropertyParameter()
			{
				Name = name,
				Value = value
			};
			property.Parameters.Add(parameter);
		}
	}
}