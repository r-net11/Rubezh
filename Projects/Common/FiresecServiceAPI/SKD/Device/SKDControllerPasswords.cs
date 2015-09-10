using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FiresecAPI.SKD
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
