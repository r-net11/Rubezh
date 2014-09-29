using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	public class XCodeReaderSettings
	{
		public XCodeReaderSettings()
		{
			SetGuardSettings = new XCodeReaderSettingsPart();
			ResetGuardSettings = new XCodeReaderSettingsPart();
			AlarmSettings = new XCodeReaderSettingsPart();

			SetGuardSettings.CodeReaderEnterType = XCodeReaderEnterType.CodeAndOne;
			ResetGuardSettings.CodeReaderEnterType = XCodeReaderEnterType.CodeAndTwo;
		}

		public XCodeReaderSettingsPart SetGuardSettings { get; set; }
		public XCodeReaderSettingsPart ResetGuardSettings { get; set; }
		public XCodeReaderSettingsPart AlarmSettings { get; set; }
	}

	public class XCodeReaderSettingsPart
	{
		public XCodeReaderSettingsPart()
		{
			CodeReaderEnterType = XCodeReaderEnterType.None;
		}

		public XCodeReaderEnterType CodeReaderEnterType { get; set; }
		public Guid CodeUID { get; set; }
	}
}