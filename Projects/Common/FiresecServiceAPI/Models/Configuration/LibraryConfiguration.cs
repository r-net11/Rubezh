using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models.Layouts;

namespace FiresecAPI.Models
{
	[DataContract]
	public class LayoutsConfiguration : VersionedConfiguration
	{
		public LayoutsConfiguration()
		{
			Root = new LayoutFolder();
		}

		[DataMember]
		public LayoutFolder Root { get; set; }

		public override bool ValidateVersion()
		{
			var result = true;
			if (Root == null)
			{
				Root = new LayoutFolder();
				result = false;
			}
			return result;
		}

		public void Update()
		{
		}
	}
}