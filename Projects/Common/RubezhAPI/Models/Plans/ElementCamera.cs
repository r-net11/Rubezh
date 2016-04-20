using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementCamera : ElementBasePoint, IElementReference, IMultipleVizualization
	{
		public ElementCamera()
		{
			CameraUID = Guid.Empty;
			PresentationName = "Камера";
		}

		[DataMember]
		public Guid CameraUID { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		[DataMember()]
		public int Rotation { get; set; }

		public double RotationRad
		{
			get { return Math.PI * ((double)Rotation / 180); }
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