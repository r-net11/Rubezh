using System;
using System.ComponentModel;

namespace FiresecAPI.SKD
{
	[Flags]
	public enum AdditionalColumnDataType
	{
		[Description("Текстовый")]
		Text,

		[Description("Графический")]
		Graphics
	}
}