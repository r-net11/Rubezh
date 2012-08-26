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
				IsInternalDeviceParameter = true,
				No = 0x82,
				Name = "Время переключения электропривода в положение ЗАКРЫТО",
				Caption = "Время переключения электропривода в положение ЗАКРЫТО",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 250
			};
			driver.Properties.Add(property1);

			var property2 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x83,
				Name = "Время переключения электропривода в положение ОТКРЫТО",
				Caption = "Время переключения электропривода в положение ОТКРЫТО",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 250
			};
			driver.Properties.Add(property2);

			var property3 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x84,
				Name = "Время задержки перед началом движения электропривода в положение ОТКРЫТО",
				Caption = "Время задержки перед началом движения электропривода в положение ОТКРЫТО",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 250
			};
			driver.Properties.Add(property3);

			var property4 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x86,
				Name = "Критическое время без обмена для перехода в защищаемое состояние",
				Caption = "Критическое время без обмена для перехода в защищаемое состояние",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 250
			};
			driver.Properties.Add(property4);

			var property6 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x85,
				Name = "Тип привода",
				Caption = "Тип привода",
				Default = "0",
				MinOffset = 0,
				MaxOffset = 3
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

			var property7 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x85,
				Name = "Перейти в защиту(иначе перейти в дежурное положение)",
				Caption = "Перейти в защиту(иначе перейти в дежурное положение)",
				DriverPropertyType = DriverPropertyTypeEnum.BoolType,
				Offset = 7,
				MinOffset = 7,
				MaxOffset = 8
			};
			driver.Properties.Add(property7);
		}
	}
}