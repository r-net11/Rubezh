using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementGKDevice : ElementBasePoint, IElementReference
	{
		public ElementGKDevice()
		{
			DeviceUID = Guid.Empty;
			PresentationName = "Устройство";
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		[XmlIgnore]
		public Guid ItemUID
		{
			get { return DeviceUID; }
			set { DeviceUID = value; }
		}

		#endregion
	}
}