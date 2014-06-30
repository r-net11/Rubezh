using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementXDevice : ElementBasePoint, IElementReference
	{
		public ElementXDevice()
		{
			XDeviceUID = Guid.Empty;
		}

		[DataMember]
		public Guid XDeviceUID { get; set; }

		public override ElementBase Clone()
		{
			ElementXDevice elementBase = new ElementXDevice();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementXDevice)element).XDeviceUID = XDeviceUID;
		}

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		public Guid ItemUID
		{
			get { return XDeviceUID; }
			set { XDeviceUID = value; }
		}

		#endregion
	}
}