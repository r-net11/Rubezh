using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XZoneState
	{
		[DataMember]
		public short No { get; set; }

		[DataMember]
		public XStateType StateType { get; set; }

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}
	}
}