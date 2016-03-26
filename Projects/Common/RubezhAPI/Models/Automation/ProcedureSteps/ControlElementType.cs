using System.ComponentModel;

namespace RubezhAPI.Automation
{
	public enum ControlElementType
	{
		[Description("Чтение")]
		Get,
		[Description("Установка")]
		Set
	}
}
