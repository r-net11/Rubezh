using System;
using System.Runtime.Serialization;

namespace FS2Api
{
	[DataContract]
	public class FS2Journal
	{
		[DataMember]
		public DateTime SystemTime { get; set; }

		[DataMember]
		public DateTime DeviceTime { get; set; }

		[DataMember]
		public string Message { get; set; }
	}
}