using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDDeviceState
	{
		public SKDDeviceState()
		{
			Clear();
		}

		public bool IsSuspending { get; set; }

		[DataMember]
		public Guid UID { get; set; }

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
		}
	}
}