﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDoorState : IDeviceState<XStateClass>
	{
		public SKDDoorState()
		{
			Clear();
			StateClasses = new List<XStateClass>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public List<XAdditionalState> AdditionalStates { get; set; }

		public SKDDoor Door { get; private set; }

		public SKDDoorState(SKDDoor door)
			: this()
		{
			Door = door;
			UID = door.UID;
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

		public void CopyToState(SKDDoorState state)
		{
			state.UID = UID;
			state.StateClasses = StateClasses.ToList();
			state.StateClass = StateClass;
		}

		public void CopyTo(SKDDoorState state)
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