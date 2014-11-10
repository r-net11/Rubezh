using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
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

		public override ElementBase Clone()
		{
			var elementBase = new ElementGKDoor();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementGKDoor)element).DoorUID = DoorUID;
		}

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return DoorUID; }
			set { DoorUID = value; }
		}

		#endregion
	}
}