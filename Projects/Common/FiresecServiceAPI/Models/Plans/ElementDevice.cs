using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementDevice : ElementBasePoint, IElementReference
	{
		public ElementDevice()
		{
			DeviceUID = Guid.Empty;
			PresentationName = "Устройство";
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public Guid AlternativeLibraryDeviceUID { get; set; }

		public override ElementBase Clone()
		{
			ElementDevice elementBase = new ElementDevice();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementDevice)element).DeviceUID = DeviceUID;
		}
		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members


		public Guid ItemUID
		{
			get { return DeviceUID; }
			set { DeviceUID = value; }
		}

		#endregion
	}
}