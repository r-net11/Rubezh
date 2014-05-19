using System;
using System.ComponentModel;

namespace FiresecAPI.SKD
{
	[Flags]
	public enum AdditionalColumnDataType
	{
		[Description("Текствовый")]
		Text,

		[Description("Графический")]
		Graphics
	}
}