using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	public enum DelayRegime
	{
		[DescriptionAttribute("Выключено")]
		Off = 0,

		[DescriptionAttribute("Включено")]
		On = 1
	}
}