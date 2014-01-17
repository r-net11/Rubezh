using System.ComponentModel;

namespace FS2Api
{
	public enum HexMemoryType
	{
		[DescriptionAttribute("Код временного RAM загрузчика RS485")]
		RAM_RS485,

		[DescriptionAttribute("Код временного RAM загрузчика USB")]
		RAM_USB,

		[DescriptionAttribute("Код загрузчика (ROM)")]
		ROM,

		[DescriptionAttribute("ПользовательскоеПО (ARM)")]
		User_ARM,

		[DescriptionAttribute("ПО контроллера однопроводного интерфейса (AVR)")]
		Controller_AVR
	}
}