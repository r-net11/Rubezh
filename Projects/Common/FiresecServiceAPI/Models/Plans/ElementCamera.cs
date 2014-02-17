using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementCamera : ElementBasePoint
	{
		public ElementCamera()
		{
			CameraUID = Guid.Empty;
		}

		[DataMember]
		public Guid CameraUID { get; set; }

		public override ElementBase Clone()
		{
			ElementCamera elementBase = new ElementCamera();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementCamera)element).CameraUID = CameraUID;
		}
		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}
	}
}