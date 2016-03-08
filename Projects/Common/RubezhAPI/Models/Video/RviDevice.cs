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
		public void OnStatusChanged()
		{
			if (StatusChanged != null)
				StatusChanged();
		}
		public event Action StatusChanged;
		[XmlIgnore]
		public RviStatus Status { get; set; }
		[XmlIgnore]
		public string ImageSource { get { return "/Controls;component/RviDevicesIcons/Device.png"; } }
	}
}