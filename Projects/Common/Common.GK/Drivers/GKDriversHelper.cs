using XFiresecAPI;

namespace Common.GK
{
	public static class GKDriversHelper
	{
		public static XDriverProperty AddPlainEnumProprety(XDriver xDriver, byte no, string propertyName, byte offset, string parameter1Name, string parameter2Name, int startValue = 0, ushort defaultValue = 0)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = defaultValue,
				Offset = offset,
				Mask = (short)((1 << offset) + (1 << (offset + 1)))
			};
			var parameter1 = new XDriverPropertyParameter()
			{
				Name = parameter1Name,
				Value = (ushort)startValue
			};
			var parameter2 = new XDriverPropertyParameter()
			{
				Name = parameter2Name,
				Value = (ushort)(startValue + 1)
			};
			property.Parameters.Add(parameter1);
			property.Parameters.Add(parameter2);
			xDriver.Properties.Add(property);
			return property;
		}

		public static XDriverProperty AddPlainEnumProprety2(XDriver xDriver, byte no, string propertyName, byte offset, string parameter1Name, string parameter2Name, int mask)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 0,
				Offset = 0,
				Mask = (byte)mask
			};
			var parameter1 = new XDriverPropertyParameter()
			{
				Name = parameter1Name,
				Value = (ushort)0
			};
			var parameter2 = new XDriverPropertyParameter()
			{
				Name = parameter2Name,
				Value = (ushort)mask
			};
			property.Parameters.Add(parameter1);
			property.Parameters.Add(parameter2);
			xDriver.Properties.Add(property);
			return property;
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

		public static XDriverProperty AddIntProprety(XDriver xDriver, byte no, string propertyName, byte offset, int defaultValue, int min, int max)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Offset = offset,
				Default = (ushort)defaultValue,
				Min = (ushort)min,
				Max = (ushort)max
			};
			xDriver.Properties.Add(property);
			return property;
		}

		public static void AddPropertyParameter(XDriverProperty property, string name, int value)
		{
			var parameter = new XDriverPropertyParameter()
			{
				Name = name,
				Value = (ushort)value
			};
			property.Parameters.Add(parameter);
		}

		public static void AddAvailableStateBits(XDriver driver, XStateBit stateType)
		{
			if (driver.AvailableStateBits.Count == 0)
			{
				driver.AvailableStateBits.Add(XStateBit.Norm);
				driver.AvailableStateBits.Add(XStateBit.Failure);
				driver.AvailableStateBits.Add(XStateBit.Ignore);
			}
			driver.AvailableStateBits.Add(stateType);
		}

		public static void AddControlAvailableStates(XDriver driver)
		{
			AddAvailableStateBits(driver, XStateBit.On);
		}

		public static void AddAvailableStateClasses(XDriver driver, XStateClass stateClass)
		{
			if (driver.AvailableStateClasses.Count == 0)
			{
				driver.AvailableStateClasses.Add(XStateClass.No);
				driver.AvailableStateClasses.Add(XStateClass.Norm);
				driver.AvailableStateClasses.Add(XStateClass.Failure);
				driver.AvailableStateClasses.Add(XStateClass.Ignore);
				driver.AvailableStateClasses.Add(XStateClass.Unknown);
			}
			driver.AvailableStateClasses.Add(stateClass);
		}
	}
}