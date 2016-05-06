using System;
using System.ComponentModel;

namespace StrazhAPI.SKD
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