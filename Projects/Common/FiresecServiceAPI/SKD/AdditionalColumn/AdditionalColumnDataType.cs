using System;
using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.SKD
{
	[Flags]
	public enum AdditionalColumnDataType
	{
		//[Description("Текстовый")]
        [LocalizedDescription(typeof(Resources.Language.SKD.AdditionalColumn.AdditionalColumnDataType), "Text")]
		Text,

        //[Description("Графический")]
        [LocalizedDescription(typeof(Resources.Language.SKD.AdditionalColumn.AdditionalColumnDataType), "Graphics")]
		Graphics
	}
}