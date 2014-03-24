using System;
using System.ComponentModel;

namespace FiresecAPI
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
