using System;

namespace FiresecAPI.Models
{
	public class ZoneState
	{
		public Zone Zone { get; set; }
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