using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class RviDevice
	{
		public RviDevice()
		{
			Cameras = new List<Camera>();
		}
		[DataMember]
		public Guid Uid { get; set; }
		[DataMember]
		public string Ip { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public List<Camera> Cameras { get; set; }
		[XmlIgnore]
		public RviStatus Status { get; set; }
		public event Action StatusChanged;
		public void OnStatusChanged()
		{
			if (StatusChanged != null)
				StatusChanged();
		}
	}
}