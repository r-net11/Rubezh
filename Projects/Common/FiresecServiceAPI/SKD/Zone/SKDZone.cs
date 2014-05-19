using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDZone
	{
		public SKDZone()
		{
			UID = Guid.NewGuid();
			Children = new List<SKDZone>();
			PlanElementUIDs = new List<Guid>();
			Devices = new List<SKDDevice>();
		}

		public SKDZone Parent { get; set; }
		public SKDZoneState State { get; set; }
		public List<SKDDevice> Devices { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<SKDZone> Children { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool IsRootZone { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }
		
		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;
	}
}