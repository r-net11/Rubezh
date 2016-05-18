using System.Runtime.Serialization;

namespace StrazhAPI.Integration.OPC
{
	[DataContract]
	public class OPCSettings
	{
		[DataMember]
		public bool IsActive { get; set; }

		[DataMember]
		public int HTTPClientPort { get; set; }

		[DataMember]
		public string OPCAddress { get; set; }

		[DataMember]
		public int OPCPort { get; set; }

		public OPCSettings()
		{
			OPCAddress = "localhost";
			OPCPort = 8087;
			HTTPClientPort = 8096;
			IsActive = true;
		}
	}
}
