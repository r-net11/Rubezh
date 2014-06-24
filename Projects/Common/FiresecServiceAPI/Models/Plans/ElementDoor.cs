using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementDoor : ElementBasePoint
	{
		public ElementDoor()
		{
			DoorUID = Guid.Empty;
		}

		[DataMember]
		public Guid DoorUID { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementDoor();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementDoor)element).DoorUID = DoorUID;
		}

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}
	}
}