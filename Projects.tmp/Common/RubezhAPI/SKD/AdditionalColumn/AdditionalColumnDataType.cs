using System;
using System.ComponentModel;

namespace RubezhAPI.SKD
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