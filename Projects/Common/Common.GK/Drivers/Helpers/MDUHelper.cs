using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Commom.GK
{
	public class MDUHelper
	{
		public static void Create()
		{
			var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.MRO);

			var property1 = new XDriverProperty()
			{
				No = 0x82,
				Name = "Время переключения электропривода в положение ЗАКРЫТО",
				Caption = "Время переключения электропривода в положение ЗАКРЫТО",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			xDriver.Properties.Add(property1);

			var property2 = new XDriverProperty()
			{
				No = 0x83,
				Name = "Время переключения электропривода в положение ОТКРЫТО",
				Caption = "Время переключения электропривода в положение ОТКРЫТО",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			xDriver.Properties.Add(property2);

			var property3 = new XDriverProperty()
			{
				No = 0x84,
				Name = "Время задержки перед началом движения электропривода в положение ОТКРЫТО",
				Caption = "Время задержки перед началом движения электропривода в положение ОТКРЫТО",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			xDriver.Properties.Add(property3);

			var property4 = new XDriverProperty()
			{
				No = 0x86,
				Name = "Критическое время без обмена для перехода в защищаемое состояние",
				Caption = "Критическое время без обмена для перехода в защищаемое состояние",
				Default = 0,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			xDriver.Properties.Add(property4);

			var property5 = new XDriverProperty()
			{
				No = 0x85,
				Name = "Тип клапана",
				Caption = "Тип клапана",
				Default = 0
			};
			var property5Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Клапан дымоудаления",
				Value = 0
			};
			var property5Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Огнезащитный клапан",
				Value = 1
			};
			property5.Parameters.Add(property5Parameter1);
			property5.Parameters.Add(property5Parameter2);
			xDriver.Properties.Add(property5);

			var property6 = new XDriverProperty()
			{
				No = 0x85,
				Name = "Тип привода",
				Caption = "Тип привода",
				Default = 0,
				Offset = 1
			};
			var property6Parameter1 = new XDriverPropertyParameter()
			{
				Name = "Резерв",
				Value = 0
			};
			var property6Parameter2 = new XDriverPropertyParameter()
			{
				Name = "Пружинный привод",
				Value = 1
			};
			var property6Parameter3 = new XDriverPropertyParameter()
			{
				Name = "Привод с ручным возвратом",
				Value = 2
			};
			var property6Parameter4 = new XDriverPropertyParameter()
			{
				Name = "Резерв",
				Value = 3
			};
			property6.Parameters.Add(property6Parameter1);
			property6.Parameters.Add(property6Parameter2);
			property6.Parameters.Add(property6Parameter3);
			property6.Parameters.Add(property6Parameter4);
			xDriver.Properties.Add(property6);

			var property7 = new XDriverProperty()
			{
				No = 0x86,
				Name = "Перевод заслонки в указанное положение после подачи питания на модуль(Только для пружинного привода)",
				Caption = "Перевод заслонки в указанное положение после подачи питания на модуль(Только для пружинного привода)",
				DriverPropertyType = XDriverPropertyTypeEnum.BoolType,
				Offset = 7
			};
			xDriver.Properties.Add(property7);
		}
	}
}