using System.ComponentModel;

namespace StrazhAPI.Automation
{
	public enum ObjectType
	{
		[Description("СКД-устройство")]
		SKDDevice,

		[Description("СКД-зона")]
		SKDZone,

		[Description("Видеоустройство")]
		VideoDevice,

		[Description("Точка доступа")]
		Door,

		[Description("Организация")]
		Organisation,

		[Description("Пользователь")]
		User,

		[Description("Сотрудник")]
		Employee,

		[Description("Посетитель")]
		Visitor
	}
}