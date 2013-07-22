using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ZoneState
	{
		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public StateType StateType { get; set; }

		public Zone Zone { get; set; }

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