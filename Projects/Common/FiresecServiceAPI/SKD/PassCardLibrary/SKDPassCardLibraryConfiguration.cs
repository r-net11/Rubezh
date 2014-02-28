using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.PassCardLibrary
{
	[DataContract]
	public class SKDPassCardLibraryConfiguration : VersionedConfiguration
	{
		public SKDPassCardLibraryConfiguration()
		{
			Templates = new List<PassCardTemplate>();
		}

		[DataMember]
		public List<PassCardTemplate> Templates { get; set; }

		public override bool ValidateVersion()
		{
			var result = true;
			if (Templates == null)
			{
				Templates = new List<PassCardTemplate>();
				result = false;
			}
			return result;
		}
	}
}
