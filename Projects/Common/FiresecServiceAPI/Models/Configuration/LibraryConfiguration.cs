using System.Runtime.Serialization;
using FiresecAPI.Models.Layouts;
using System.Collections.Generic;
using System;

namespace FiresecAPI.Models
{
	[DataContract]
	public class LayoutsConfiguration : VersionedConfiguration
	{
		public LayoutsConfiguration()
		{
			Layouts = new List<Layout>();
		}

		[DataMember]
		public List<Layout> Layouts { get; set; }

		public override bool ValidateVersion()
		{
			var result = true;
			if (Layouts == null)
			{
				Layouts = new List<Layout>();
				result = false;
			}
			foreach (var layout in Layouts)
				if (layout.UID == Guid.Empty)
					layout.UID = Guid.NewGuid();
			return result;
		}

		public void Update()
		{
		}
	}
}