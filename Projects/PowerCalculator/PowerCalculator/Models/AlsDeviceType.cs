using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PowerCalculator.Models
{
	public enum AlsDeviceType
	{
		[Description("Дымовой извещатель")]
		SmokeDetector,

		[Description("Тепловой извещатель")]
		HeatDetector
	}
}