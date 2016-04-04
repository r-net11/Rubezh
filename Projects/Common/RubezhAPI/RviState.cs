using RubezhAPI.Models;
using System;
using System.Collections.Generic;
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
		public Guid CameraUid { get; set; }
		[DataMember]
		public RviStatus Status { get; set; }
		[DataMember]
		public bool IsOnGuard { get; set; }
		[DataMember]
		public bool IsRecordOnline { get; set; }
		[DataMember]
		public List<RviStream> RviStreams { get; set; }
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
		public RviState(Camera camera, RviStatus rviStatus, bool isOnGuard, bool isRecordOnline, List<RviStream> rviStreams)
		{
			CameraUid = camera.UID;
			Status = rviStatus;
			IsOnGuard = isOnGuard;
			IsRecordOnline = isRecordOnline;
			RviStreams = rviStreams;
		}
	}
}