using RubezhAPI.GK;

namespace GKProcessor
{
	public static class GKDriversHelper
	{
		public static GKDriverProperty AddPlainEnumProprety(GKDriver driver, byte no, byte offset, string propertyName, string parameter1Name, string parameter2Name)
		{
			var property = new GKDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = (ushort)(1 << offset),
				Mask = (short)((1 << offset) + (1 << (offset + 1)))
			};
			var parameter1 = new GKDriverPropertyParameter()
			{
				Name = parameter1Name,
				Value = (ushort)(1 << offset)
			};
			var parameter2 = new GKDriverPropertyParameter()
			{
				Name = parameter2Name,
				Value = (ushort)(2 << offset)
			};
			property.Parameters.Add(parameter1);
			property.Parameters.Add(parameter2);
			driver.Properties.Add(property);
			return property;
		}

		public static GKDriverProperty AddPlainEnumProprety2(GKDriver driver, byte no, string propertyName, byte offset, string parameter1Name, string parameter2Name, int mask)
		{
			var property = new GKDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 0,
				Mask = (byte)mask
			};
			var parameter1 = new GKDriverPropertyParameter()
			{
				Name = parameter1Name,
				Value = (ushort)0
			};
			var parameter2 = new GKDriverPropertyParameter()
			{
				Name = parameter2Name,
				Value = (ushort)mask
			};
			property.Parameters.Add(parameter1);
			property.Parameters.Add(parameter2);
			driver.Properties.Add(property);
			return property;
		}

		public static void AddBoolProprety(GKDriver driver, byte no, string propertyName)
		{
			var property = new GKDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = GKDriverPropertyTypeEnum.BoolType
			};
			driver.Properties.Add(property);
		}

		public static GKDriverProperty AddIntProprety(GKDriver driver, byte no, string propertyName, int defaultValue, int min, int max)
		{
			var property = new GKDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
				Default = (ushort)defaultValue,
				Min = (ushort)min,
				Max = (ushort)max,
			};
			driver.Properties.Add(property);
			return property;
		}
		public static void AddPropertyParameter(GKDriverProperty property, string name, int value)
		{
			var parameter = new GKDriverPropertyParameter()
			{
				Name = name,
				Value = (ushort)value
			};
			property.Parameters.Add(parameter);
		}

		public static void AddAvailableStateBits(GKDriver driver, GKStateBit stateType)
		{
			if (driver.AvailableStateBits.Count == 0)
			{
				driver.AvailableStateBits.Add(GKStateBit.Norm);
				driver.AvailableStateBits.Add(GKStateBit.Failure);
				driver.AvailableStateBits.Add(GKStateBit.Ignore);
			}
			driver.AvailableStateBits.Add(stateType);
		}

	    public static void AddRadioChanelProperties(GKDriver driver)
	    {
            AddIntProprety(driver, 0, "Младшее слово привязки", 1, 0, 255);
            AddIntProprety(driver, 1, "Старшее слово привязки", 0, 0, 255);

            var property1 = new GKDriverProperty()
            {
                No = 2,
                Name = "Секунда периода (не более ПЕРИОД - 1)",
                Caption = "Секунда периода (не более ПЕРИОД - 1)",
                Default = 0,
                DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
                IsLowByte = true,
                Min = 0,
                Max = 120
            };
            driver.Properties.Add(property1);

            var property2 = new GKDriverProperty()
            {
                No = 2,
                Name = "Окно",
                Caption = "Окно",
                Default = 1,
                DriverPropertyType = GKDriverPropertyTypeEnum.IntType,
                IsHieghByte = true,
                Min = 1,
                Max = 8
            };
            driver.Properties.Add(property2);
	    }

		public static void AddControlAvailableStates(GKDriver driver)
		{
			AddAvailableStateBits(driver, GKStateBit.On);
		}

		public static void AddAvailableStateClasses(GKDriver driver, XStateClass stateClass)
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

		public static void AddDefaultStateBitsClasses(GKDriver driver)
		{
			driver.AvailableStateBits.Add(GKStateBit.Norm);
			driver.AvailableStateBits.Add(GKStateBit.Failure);
			driver.AvailableStateBits.Add(GKStateBit.Ignore);
			driver.AvailableStateClasses.Add(XStateClass.No);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Ignore);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
		}
	}
}