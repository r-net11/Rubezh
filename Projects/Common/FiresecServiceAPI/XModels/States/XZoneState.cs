using System;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using System.Collections.Generic;

namespace XFiresecAPI
{
	[DataContract]
	public class XZoneState
	{
		public XZone Zone { get; set; }

		[DataMember]
		public short No { get; set; }

		[DataMember]
		public List<XStateType> States { get; set; }

		[DataMember]
		public StateType StateType { get; set; }

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}
	}
}