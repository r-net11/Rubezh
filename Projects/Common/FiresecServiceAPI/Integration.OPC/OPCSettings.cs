using System.Runtime.Serialization;

namespace StrazhAPI.Integration.OPC
{
	[DataContract]
	public class OPCSettings
	{
		[DataMember]
		public bool IsActive { get; set; }

		[DataMember]
		public int IntegrationPort { get; set; }

		[DataMember]
		public string OPCAddress { get; set; }

		[DataMember]
		public int OPCPort { get; set; }

		public OPCSettings()
		{
			OPCAddress = "localhost";
			OPCPort = 8097;
			IntegrationPort = 8096;
			IsActive = true;
		}
	}
}
