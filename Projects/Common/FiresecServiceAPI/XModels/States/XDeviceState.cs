using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace XFiresecAPI
{
    public class XDeviceState
    {
		public XDeviceState()
		{
			States = new List<XStateType>();
			StateType = StateType.Norm;
		}

		public XDevice Device { get; set; }
		public bool IsConnectionLost { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XStateType> States { get; set; }

		public StateType StateType { get; set; }

		public bool IsDisabled
		{
			get { return States.Any(x => x == XStateType.Off); }
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}
    }
}