using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public enum DriverType
	{
		[Description("Система")]
		System = 0,

		[Description("[RS-485] МЗЭП СОЭ 55-215/415")]
		MZEP55Network = 1,

		[Description("МЗЭП СОЭ 55-215/415")]
		MZEP55Counter = 2,

		[Description("[RS-485] Берегун 1-2")]
		BeregunNetwork = 3,

		[Description("Берегун 1-2")]
		BeregunCounter = 4,

        [Description("[RS-485] Меркурий 203")]
        Mercury203Counter = 5,

		[Description("Сеть Инкотекс")]
		IncotextNetwork = 6,

		[Description("Виртуальная cеть Инкотекс")]
		VirtualIncotextNetwork = 7,

		[Description("Виртуальный [RS-485] Меркурий 203")]
		VirtualMercury203Counter = 8,

		[Description("Виртуальная cеть [RS-485] МЗЭП СОЭ 55-215/415")]
		VirtualMZEP55Network = 9,

		[Description("Виртуальный МЗЭП СОЭ 55-215/415")]
		VirtualMZEP55Counter = 10,
	}
}