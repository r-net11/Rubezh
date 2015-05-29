using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDeviceState : IDeviceState
	{
		public SKDDeviceState()
		{
			Clear();
			StateClasses = new List<XStateClass>();
		}

		public SKDDeviceState(SKDDevice device)
			: this()
		{
			Device = device;
			UID = device.UID;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		[Obsolete("OpenAlwaysTimeIndex is deprecated, use AccessState instead")]
		[DataMember]
		public int OpenAlwaysTimeIndex { get; set; }

		[DataMember]
		public AccessState AccessState { get; set; }

		public SKDDevice Device { get; private set; }
		public bool IsSuspending { get; set; }
		public bool IsInitialState { get; set; }
		public bool IsConnectionLost { get; set; }

		public void Clear()
		{
			IsInitialState = true;
			IsConnectionLost = false;
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}

		#region IDeviceState<XStateClass> Members
		XStateClass IDeviceState.StateClass
		{
			get { return StateClass; }
		}
		string IDeviceState.Name
		{
			get { return StateClass.ToDescription(); }
		}
		#endregion
	}
}