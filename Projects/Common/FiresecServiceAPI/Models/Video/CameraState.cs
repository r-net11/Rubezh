using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;

namespace FiresecAPI.Models
{
	[DataContract]
	public class CameraState : IDeviceState<XStateClass>
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
		XStateClass IDeviceState<XStateClass>.StateType
		{
			get { return StateClass; }
		}
		#endregion
	}
}
