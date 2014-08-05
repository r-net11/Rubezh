using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum ExcuseDocumentEnum
	{
		[Description("Нет")]
		None,

		[Description("Больничный")]
		Hospital,

		[Description("Командировка")]
		Trip
	}
}