﻿using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace RubezhAPI.Models
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