using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using XFiresecAPI;

namespace FiresecAPI
{
	[DataContract]
	public class SKDZoneState
	{
		public SKDZoneState()
		{
			Clear();
			StateClasses = new List<XStateClass>();
			AdditionalStates = new List<XAdditionalState>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public List<XAdditionalState> AdditionalStates { get; set; }

		public SKDZone Zone { get; private set; }

		public SKDZoneState(SKDZone zone)
			: this()
		{
			Zone = zone;
			UID = zone.UID;
		}

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

		public void CopyToState(SKDZoneState state)
		{
			state.UID = UID;
			state.StateClasses = StateClasses.ToList();
			state.StateClass = StateClass;
			state.AdditionalStates = AdditionalStates.ToList();
		}

		public void CopyTo(SKDZoneState state)
		{
			state.UID = UID;
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}
	}
}