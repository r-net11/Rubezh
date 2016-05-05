using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Models.Layouts;

namespace StrazhAPI.Models
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
			foreach (var layout in Layouts)
			{
				if (layout.UID == Guid.Empty)
					layout.UID = Guid.NewGuid();
			}
			return result;
		}

		public void Update()
		{
		}
	}
}