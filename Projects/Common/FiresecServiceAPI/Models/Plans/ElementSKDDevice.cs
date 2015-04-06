﻿using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementSKDDevice : ElementBasePoint, IElementReference
	{
		public ElementSKDDevice()
		{
			DeviceUID = Guid.Empty;
			PresentationName = "Устройство Страж";
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		public override ElementBase Clone()
		{
			ElementSKDDevice elementBase = new ElementSKDDevice();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementSKDDevice)element).DeviceUID = DeviceUID;
		}

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return DeviceUID; }
			set { DeviceUID = value; }
		}

		#endregion
	}
}