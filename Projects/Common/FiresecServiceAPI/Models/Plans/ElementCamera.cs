using StrazhAPI.Plans.Elements;
using StrazhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementCamera : ElementBasePoint, IElementReference
	{
		public ElementCamera()
		{
			CameraUID = Guid.Empty;
            PresentationName = Resources.Language.Models.Plans.ElementCamera.PresentationName;
		}

		[DataMember]
		public Guid CameraUID { get; set; }

		public override ElementBase Clone()
		{
			var elementBase = new ElementCamera();
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

		#endregion IElementReference Members
	}
}