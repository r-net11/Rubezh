using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementCamera : ElementBasePoint, IElementReference
	{
		public ElementCamera()
		{
			CameraUID = Guid.Empty;
			PresentationName = "Камера";
		}

		[DataMember]
		public Guid CameraUID { get; set; }

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementCamera)element).CameraUID = CameraUID;
		}
		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		public Guid ItemUID
		{
			get { return CameraUID; }
			set { CameraUID = value; }
		}

		#endregion
	}
}