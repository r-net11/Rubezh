using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementGKDoor : ElementBasePoint, IElementReference
	{
		public ElementGKDoor()
		{
			DoorUID = Guid.Empty;
			PresentationName = "ГК точка доступа";
		}

		[DataMember]
		public Guid DoorUID { get; set; }

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		[XmlIgnore]
		public Guid ItemUID
		{
			get { return DoorUID; }
			set { DoorUID = value; }
		}

		#endregion
	}
}