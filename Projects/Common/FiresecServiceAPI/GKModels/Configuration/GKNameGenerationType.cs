using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип генерации названия компонентка в ГК
	/// </summary>
	public enum GKNameGenerationType
	{
		[Description("Тип + адрес")]
		DriverTypePlusAddress,

		[Description("Тип + адрес + (примечание)")]
		DriverTypePlusAddressPlusDescription,

		[Description("Примечание")]
		Description,

		[Description("Проектный адрес")]
		ProjectAddress,

		[Description("Примечание или проектный адрес")]
		DescriptionOrProjectAddress,

		[Description("Проектный адрес или примечание")]
		ProjectAddressOrDescription,
	}
}