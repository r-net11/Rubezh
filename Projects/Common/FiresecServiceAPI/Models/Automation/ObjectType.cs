using System.ComponentModel;
using Localization;

namespace FiresecAPI.Automation
{
	public enum ObjectType
	{
		//[DescriptionAttribute("СКД-устройство")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ObjectType), "SKDDevice")]
		SKDDevice,

		//[DescriptionAttribute("СКД-зона")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ObjectType), "SKDZone")]
        SKDZone,

		//[DescriptionAttribute("Видеоустройство")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ObjectType), "VideoDevice")]
		VideoDevice,

		//[DescriptionAttribute("Точка доступа")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ObjectType), "Door")]
        Door,

		//[DescriptionAttribute("Организация")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ObjectType), "Organisation")]
        Organisation,

		//[DescriptionAttribute("Пользователь")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ObjectType), "User")]
        User,

		//[DescriptionAttribute("Сотрудник")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ObjectType), "Employee")]
        Employee,

		//[DescriptionAttribute("Посетитель")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ObjectType), "Visitor")]
        Visitor
	}
}