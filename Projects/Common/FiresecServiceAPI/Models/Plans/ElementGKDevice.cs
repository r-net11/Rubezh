using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementGKDevice : ElementBasePoint, IElementReference
	{
		public ElementGKDevice()
		{
			DeviceUID = Guid.Empty;
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		public override ElementBase Clone()
		{
			ElementGKDevice elementBase = new ElementGKDevice();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementGKDevice)element).DeviceUID = DeviceUID;
		}

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		[XmlIgnore]
		public Guid ItemUID
		{
			get { return DeviceUID; }
			set { DeviceUID = value; }
		}

		#endregion
	}
}