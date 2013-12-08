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
			{
				if (layout.UID == Guid.Empty)
					layout.UID = Guid.NewGuid();
				if (layout.SplitterSize == 0)
					layout.SplitterSize = 4;
				if (layout.IPs == null)
					layout.IPs = new List<string>();
			}
			return result;
		}

		public void Update()
		{
		}
	}
}