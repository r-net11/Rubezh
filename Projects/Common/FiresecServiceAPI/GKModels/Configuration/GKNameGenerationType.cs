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

		[Description("Тип + адрес + (описание)")]
		DriverTypePlusAddressPlusDescription,

		[Description("Описание")]
		Description,
	}
}