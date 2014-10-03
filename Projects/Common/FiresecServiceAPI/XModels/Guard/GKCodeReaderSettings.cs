using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKCodeReaderSettings
	{
		public GKCodeReaderSettings()
		{
			SetGuardSettings = new XCodeReaderSettingsPart();
			ResetGuardSettings = new XCodeReaderSettingsPart();
			AlarmSettings = new XCodeReaderSettingsPart();

			SetGuardSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne;
			ResetGuardSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndTwo;
		}

		[DataMember]
		public XCodeReaderSettingsPart SetGuardSettings { get; set; }

		[DataMember]
		public XCodeReaderSettingsPart ResetGuardSettings { get; set; }

		[DataMember]
		public XCodeReaderSettingsPart AlarmSettings { get; set; }
	}

	[DataContract]
	public class XCodeReaderSettingsPart
	{
		public XCodeReaderSettingsPart()
		{
			CodeReaderEnterType = GKCodeReaderEnterType.None;
		}

		[DataMember]
		public GKCodeReaderEnterType CodeReaderEnterType { get; set; }

		[DataMember]
		public Guid CodeUID { get; set; }
	}
}