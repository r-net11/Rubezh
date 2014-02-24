using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XMPT
	{
		public XMPT()
		{
			BaseUID = Guid.NewGuid();
			StartLogic = new XDeviceLogic();
			StopLogic = new XDeviceLogic();
			AutomaticOffLogic = new XDeviceLogic();
			OffTableDeviceUIDs = new List<Guid>();
			OnTableDeviceUIDs = new List<Guid>();
			AutomaticTableDeviceUIDs = new List<Guid>();
			SirenaDeviceUIDs = new List<Guid>();

			OffTableDevices = new List<XDevice>();
			OnTableDevices = new List<XDevice>();
			AutomaticTableDevices = new List<XDevice>();
			SirenaDevices = new List<XDevice>();
		}

		public List<XDevice> OffTableDevices { get; set; }
		public List<XDevice> OnTableDevices { get; set; }
		public List<XDevice> AutomaticTableDevices { get; set; }
		public List<XDevice> SirenaDevices { get; set; }

		[DataMember]
		public Guid BaseUID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public XDeviceLogic StartLogic { get; set; }

		[DataMember]
		public XDeviceLogic StopLogic { get; set; }

		[DataMember]
		public XDeviceLogic AutomaticOffLogic { get; set; }

		[DataMember]
		public List<Guid> OffTableDeviceUIDs { get; set; }

		[DataMember]
		public List<Guid> OnTableDeviceUIDs { get; set; }

		[DataMember]
		public List<Guid> AutomaticTableDeviceUIDs { get; set; }

		[DataMember]
		public List<Guid> SirenaDeviceUIDs { get; set; }
	}
}