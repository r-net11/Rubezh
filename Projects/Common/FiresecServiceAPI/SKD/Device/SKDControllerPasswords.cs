using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDControllerPasswords
	{
		public SKDControllerPasswords()
		{
			LocksPasswords = new List<SKDLocksPassword>();
		}

		[DataMember]
		public List<SKDLocksPassword> LocksPasswords { get; set; }
	}
}
