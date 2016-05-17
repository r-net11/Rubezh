using System.Runtime.Serialization;

namespace StrazhAPI.Integration.OPC
{
	[DataContract]
	public class OPCSettings
	{
		[DataMember]
		public bool IsActive { get; set; }

		[DataMember]
		public string IPAddress { get; set; }

		[DataMember]
		public int Port { get; set; }

		public OPCSettings()
		{
			IPAddress = "localhost";
			Port = 8088;
			IsActive = true;
		}
	}
}
