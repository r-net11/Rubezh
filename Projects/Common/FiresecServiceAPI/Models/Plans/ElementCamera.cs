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

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return CameraUID; }
			set { CameraUID = value; }
		}

		#endregion
	}
}