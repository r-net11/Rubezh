using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ZoneState
	{
		[DataMember]
		public Zone Zone { get; set; }

		[DataMember]
		public StateType StateType { get; set; }

		public ZoneState()
		{
			StateType = StateType.No;
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}
	}
}