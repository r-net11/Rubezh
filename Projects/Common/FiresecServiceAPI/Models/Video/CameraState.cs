using Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.GK;

namespace StrazhAPI.Models
{
	[DataContract]
	public class CameraState : IDeviceState
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		public event Action StateChanged;

		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}

		public CameraState(Camera camera)
		{
			Camera = camera;
			StateClasses = new List<XStateClass>();
			StateClass = XStateClass.Norm;
		}

		public Camera Camera { get; private set; }

		#region IDeviceState<XStateClass> Members

		XStateClass IDeviceState.StateClass
		{
			get { return StateClass; }
		}

		string IDeviceState.Name
		{
			get { return StateClass.ToDescription(); }
		}

		#endregion IDeviceState<XStateClass> Members
	}
}