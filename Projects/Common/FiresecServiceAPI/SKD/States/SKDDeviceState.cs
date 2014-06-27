using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDeviceState : IDeviceState<XStateClass>
	{
		public SKDDeviceState()
		{
			Clear();
			StateClasses = new List<XStateClass>();
			AdditionalStates = new List<XAdditionalState>();
			InternalStateClasses = new List<XStateClass>();
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

		[DataMember]
		public List<XAdditionalState> AdditionalStates { get; set; }

		public SKDDevice Device { get; private set; }

		public List<XStateClass> InternalStateClasses { get; set; }
		public bool IsSuspending { get; set; }
		public bool IsInitialState { get; set; }
		public bool IsConnectionLost { get; set; }
		public bool IsDBMissmatch { get; set; }

		public void Clear()
		{
			IsInitialState = true;
			IsConnectionLost = false;
			IsDBMissmatch = false;
		}

		public void CopyToState(SKDDeviceState state)
		{
			state.UID = UID;
			state.StateClasses = StateClasses.ToList();
			state.StateClass = StateClass;
			state.AdditionalStates = AdditionalStates.ToList();
		}

		public void CopyTo(SKDDeviceState state)
		{
			state.UID = UID;
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}

		#region IDeviceState<XStateClass> Members
		XStateClass IDeviceState<XStateClass>.StateType
		{
			get { return StateClass; }
		}
		#endregion
	}
}