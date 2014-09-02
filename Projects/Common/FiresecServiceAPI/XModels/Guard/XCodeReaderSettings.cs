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
			AttentionSettings = new XCodeReaderSettingsPart();
			Fire1Settings = new XCodeReaderSettingsPart();
			Fire2Settings = new XCodeReaderSettingsPart();
		}

		[DataMember]
		public XCodeReaderSettingsPart AttentionSettings { get; set; }

		[DataMember]
		public XCodeReaderSettingsPart Fire1Settings { get; set; }

		[DataMember]
		public XCodeReaderSettingsPart Fire2Settings { get; set; }
	}

	[DataContract]
	public class XCodeReaderSettingsPart
	{
		[DataMember]
		public XGuardZoneDeviceActionType GuardZoneDeviceActionType { get; set; }

		[DataMember]
		public Guid CodeUID { get; set; }
	}
}