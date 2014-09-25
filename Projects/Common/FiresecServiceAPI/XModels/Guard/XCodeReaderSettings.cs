using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
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

		[DataMember]
		public XCodeReaderSettingsPart AlarmSettings { get; set; }

		[DataMember]
		public XCodeReaderSettingsPart SetGuardSettings { get; set; }

		[DataMember]
		public XCodeReaderSettingsPart ResetGuardSettings { get; set; }
	}

	[DataContract]
	public class XCodeReaderSettingsPart
	{
		public XCodeReaderSettingsPart()
		{
			CodeReaderEnterType = XCodeReaderEnterType.None;
		}

		[DataMember]
		public XCodeReaderEnterType CodeReaderEnterType { get; set; }

		[DataMember]
		public Guid CodeUID { get; set; }
	}
}