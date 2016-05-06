using Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDDoorState : IDeviceState
	{
		public SKDDoorState()
		{
			StateClasses = new List<XStateClass>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public AccessState AccessState { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		public SKDDoor Door { get; private set; }

		public SKDDoorState(SKDDoor door)
			: this()
		{
			Door = door;
			UID = door.UID;
		}

		public event Action StateChanged;

		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}

		#region IDeviceState Members

		XStateClass IDeviceState.StateClass
		{
			get { return StateClass; }
		}

		string IDeviceState.Name
		{
			get { return StateClass.ToDescription(); }
		}

		#endregion IDeviceState Members
	}
}