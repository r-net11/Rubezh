using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class MDUHelper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.MDU);
			driver.HasConfigurationProperties = true;

			var property1 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x82,
				Name = "Время переключения в положение НОРМА, с",
				Caption = "Время переключения в положение НОРМА, с",
				Default = "180",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 250
			};
			driver.Properties.Add(property1);

			var property2 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x83,
				Name = "Время переключения электропривода в положение ЗАЩИТА, с",
				Caption = "Время переключения электропривода в положение ЗАЩИТА, с",
				Default = "180",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 250
			};
			driver.Properties.Add(property2);

			var property3 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x84,
				Name = "AU_Delay",
				Caption = "Время задержки перед началом движения электропривода в положение ЗАЩИТА, с",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 250
			};
			driver.Properties.Add(property3);

			var property4 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x86,
				Name = "Отказ обмена, с",
				Caption = "Отказ обмена, с",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 250
			};
			driver.Properties.Add(property4);

			var property6 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x85,
				Name = "Тип привода",
				Caption = "Тип привода",
				Default = "2",
				MinBit = 0,
				MaxBit = 3
			};
			
			var property6Parameter1 = new DriverPropertyParameter()
			{
				Name = "Реверсивный ДУ",
				Value = "0"
			};
			var property6Parameter2 = new DriverPropertyParameter()
			{
				Name = "Реверсивный ОЗ",
				Value = "1"
			};
			var property6Parameter3 = new DriverPropertyParameter()
			{
				Name = "Пружинный ДУ",
				Value = "2"
			};
			var property6Parameter4 = new DriverPropertyParameter()
			{
				Name = "Пружинный ОЗ",
				Value = "3"
			};
			var property6Parameter5 = new DriverPropertyParameter()
			{
				Name = "Ручной ДУ",
				Value = "4"
			};
			var property6Parameter6 = new DriverPropertyParameter()
			{
				Name = "Ручной ОЗ",
				Value = "5"
			};

			property6.Parameters.Add(property6Parameter1);
			property6.Parameters.Add(property6Parameter2);
			property6.Parameters.Add(property6Parameter3);
			property6.Parameters.Add(property6Parameter4);
			property6.Parameters.Add(property6Parameter5);
			property6.Parameters.Add(property6Parameter6);
			driver.Properties.Add(property6);

			ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x85, "начальное положение для привода пружинный ДУ", 7,
				"защита",
				"норма", 0, 0, 0, false, false, "0");

			ConfigurationDriverHelper.AddIntProprety(driver, 0x87, "ПАРАМЕТР 0x87", "AU_Test", 0, 0, 0, 255);

		//    var property7 = new DriverProperty()
		//    {
		//        IsAUParameter = true,
		//        No = 0x87,
		//        Name = "Установленные концевики",
		//        Caption = "Установленные концевики",
		//        ToolTip = "Установленные на приводе концевики. Применима только для ручного привода",
		//        Default = "0",
		//        MinBit = 0,
		//        MaxBit = 0
		//    };

		//    var property7Parameter1 = new DriverPropertyParameter()
		//    {
		//        Name = "Норма и защита",
		//        Value = "0"
		//    };
		//    var property7Parameter2 = new DriverPropertyParameter()
		//    {
		//        Name = "Только защита",
		//        Value = "1"
		//    };
		//    var property7Parameter3 = new DriverPropertyParameter()
		//    {
		//        Name = "Только норма",
		//        Value = "2"
		//    };

		//    property7.Parameters.Add(property7Parameter1);
		//    property7.Parameters.Add(property7Parameter2);
		//    property7.Parameters.Add(property7Parameter3);
		//    driver.Properties.Add(property7);

		//    var property8 = new DriverProperty()
		//    {
		//        IsAUParameter = true,
		//        No = 0x87,
		//        Name = "Контроль эл. магнита",
		//        Caption = "Контроль эл. магнита",
		//        ToolTip = "Отключение контроля только в сработавшем состоянии (в защите). Применима только для ручного привода"
		//        Default = "0",
		//        MinBit = 0,
		//        MaxBit = 0
		//    };

		//    var property8Parameter1 = new DriverPropertyParameter()
		//    {
		//        Name = "Вкл",
		//        Value = "0"
		//    };
		//    var property8Parameter2 = new DriverPropertyParameter()
		//    {
		//        Name = "Выкл",
		//        Value = "1"
		//    };

		//    property8.Parameters.Add(property8Parameter1);
		//    property8.Parameters.Add(property8Parameter2);
		//    driver.Properties.Add(property8);
		}
	}
}