using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	[DataContract]
	public class GKUser
	{
		[DataMember]
		public int GKNo { get; set; }

		[DataMember]
		public uint Number { get; set; }

		[DataMember]
		public string FIO { get; set; }

		[DataMember]
		public bool IsActive { get; set; }

		[DataMember]
		public GKCardType CardType { get; set; }
	}
}