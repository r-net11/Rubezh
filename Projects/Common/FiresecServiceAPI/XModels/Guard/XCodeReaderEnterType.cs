using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum XCodeReaderEnterType
	{
		[Description("<Нет>")]
		None,

		[Description("* Код #")]
		CodeOnly,

		[Description("* Код * 1")]
		CodeAndOne,

		[Description("* Код * 2 #")]
		CodeAndTwo
	}
}