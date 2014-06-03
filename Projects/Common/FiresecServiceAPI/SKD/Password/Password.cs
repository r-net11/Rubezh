using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Password : OrganisationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PasswordString { get; set; }
	}
}