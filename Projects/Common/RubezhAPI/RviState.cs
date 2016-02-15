using RubezhAPI.Models;
using System;
using System.Runtime.Serialization;

namespace RubezhAPI
{
	[DataContract]
	public class RviState
	{
		[DataMember]
		public Guid RviDeviceUid { get; set; }
		[DataMember]
		public string RviServerUrl { get; set; }
		[DataMember]
		public RviStatus Status { get; set; }
		public RviState(RviDevice rviDevice, RviStatus rviStatus)
		{
			RviDeviceUid = rviDevice.Uid;
			Status = rviStatus;
		}
		public RviState(RviServer rviServer, RviStatus rviStatus)
		{
			RviServerUrl = rviServer.Url;
			Status = rviStatus;
		}
	}
}