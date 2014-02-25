using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace XFiresecAPI
{
	public enum MPTDeviceType
	{
		[DescriptionAttribute("Табличка Не входи")]
		DoNotEnterBoard,

		[DescriptionAttribute("Табличка Уходи")]
		ExitBoard,

		[DescriptionAttribute("Табличка Автоматика отключена")]
		AutomaticOffBoard,

		[DescriptionAttribute("Сирена")]
		Speaker,

		[DescriptionAttribute("Двери-окна")]
		Door,

		[DescriptionAttribute("Ручной запуск")]
		HandStart,

		[DescriptionAttribute("Ручной останов")]
		HandStop,

		[DescriptionAttribute("Ручное отключение автоматики")]
		HandAutomatic,

		[DescriptionAttribute("Пуск")]
		Bomb,

		[DescriptionAttribute("Неизвестно")]
		Unknown
	}
}