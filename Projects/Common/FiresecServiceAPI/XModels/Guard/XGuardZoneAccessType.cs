using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI.GK
{
	[DataContract]
	public enum XGuardZoneAccessType
	{
		[DescriptionAttribute("Просмотр")]
		Watch,

		[DescriptionAttribute("Взятие")]
		Get,

		[DescriptionAttribute("Снятие")]
		Set,

		[DescriptionAttribute("Взятие и снятие")]
		All
	}
}