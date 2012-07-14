using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class MDUHelper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.MRO);
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
				Max = 255
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
				Max = 255
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
				Max = 255
			};
			driver.Properties.Add(property3);

			var property4 = new DriverProperty()
			{
				No = 0x86,
				Name = "Критическое время без обмена для перехода в защищаемое состояние",
				Caption = "Критическое время без обмена для перехода в защищаемое состояние",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property4);

			var property5 = new DriverProperty()
			{
				No = 0x85,
				Name = "Тип клапана",
				Caption = "Тип клапана",
				Default = "0"
			};
			var property5Parameter1 = new DriverPropertyParameter()
			{
				Name = "Клапан дымоудаления",
				Value = "0"
			};
			var property5Parameter2 = new DriverPropertyParameter()
			{
				Name = "Огнезащитный клапан",
				Value = "1"
			};
			property5.Parameters.Add(property5Parameter1);
			property5.Parameters.Add(property5Parameter2);
			driver.Properties.Add(property5);

			var property6 = new DriverProperty()
			{
				No = 0x85,
				Name = "Тип привода",
				Caption = "Тип привода",
				Default = "0",
				Offset = 1
			};
			var property6Parameter1 = new DriverPropertyParameter()
			{
				Name = "Резерв",
				Value = "0"
			};
			var property6Parameter2 = new DriverPropertyParameter()
			{
				Name = "Пружинный привод",
				Value = "1"
			};
			var property6Parameter3 = new DriverPropertyParameter()
			{
				Name = "Привод с ручным возвратом",
				Value = "2"
			};
			var property6Parameter4 = new DriverPropertyParameter()
			{
				Name = "Резерв",
				Value = "3"
			};
			property6.Parameters.Add(property6Parameter1);
			property6.Parameters.Add(property6Parameter2);
			property6.Parameters.Add(property6Parameter3);
			property6.Parameters.Add(property6Parameter4);
			driver.Properties.Add(property6);

			var property7 = new DriverProperty()
			{
				IsInternalDeviceParameter = true,
				No = 0x86,
				Name = "Перевод заслонки в указанное положение после подачи питания на модуль(Только для пружинного привода)",
				Caption = "Перевод заслонки в указанное положение после подачи питания на модуль(Только для пружинного привода)",
				DriverPropertyType = DriverPropertyTypeEnum.BoolType,
				Offset = 7
			};
			driver.Properties.Add(property7);
		}
	}
}