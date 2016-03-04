using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class RviServer
	{
		[DataMember]
		public string Ip { get; set; }
		[DataMember]
		public int Port { get; set; }
		[DataMember]
		public string Protocol { get; set; }
		[DataMember]
		public string Url { get; set; }
		[DataMember]
		public List<RviDevice> RviDevices { get; set; }
		public void OnStatusChanged()
		{
			if (StatusChanged != null)
				StatusChanged();
		}
		public event Action StatusChanged;
		[XmlIgnore]
		public RviStatus Status { get; set; }
		[XmlIgnore]
		public string PresentationName
		{
			get { return string.Format("Сервер ({0}:{1})", Ip, Port); }
		}
		[XmlIgnore]
		public string ImageSource { get { return "/Controls;component/RviDevicesIcons/Server.png"; } }
	}
}