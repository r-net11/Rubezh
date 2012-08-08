using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ZoneState
	{
		public ZoneState()
		{
			StateType = StateType.No;
		}

		public bool IsOnGuard { get; set; }
		public Zone Zone { get; set; }

		[DataMember]
		public int No { get; set; }

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