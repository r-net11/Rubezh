using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Localization;

namespace FiresecAPI.Enums
{
	public enum RoundingType
	{
		//[Description("Не используется")]
        [LocalizedDescription(typeof(Resources.Language.Enums.RoundingType), "None")]
		None,

		//[Description("До минут")]
        [LocalizedDescription(typeof(Resources.Language.Enums.RoundingType), "RoundToMin")]
        RoundToMin,

		//[Description("До часов")]
        [LocalizedDescription(typeof(Resources.Language.Enums.RoundingType), "RoundToHour")]
        RoundToHour
	}
}
