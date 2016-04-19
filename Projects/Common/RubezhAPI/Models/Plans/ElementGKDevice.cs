using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementGKDevice : ElementBasePoint, IElementReference, IMultipleVizualization
	{
		public ElementGKDevice()
		{
			DeviceUID = Guid.Empty;
			PresentationName = "Устройство";
		}

		[DataMember]
		public Guid DeviceUID { get; set; }
		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

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