using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class Ip4AddressInfo
	{
		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public string Mask { get; set; }
	}
}