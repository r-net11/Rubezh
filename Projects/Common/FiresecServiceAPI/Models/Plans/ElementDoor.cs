using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using System;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementDoor : ElementBasePoint, IElementReference
	{
		public ElementDoor()
		{
			DoorUID = Guid.Empty;
			PresentationName = "Точка доступа";
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

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return DoorUID; }
			set { DoorUID = value; }
		}

		#endregion IElementReference Members
	}
}