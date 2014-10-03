using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class GKDoor : GKBase
	{
		public GKDevice EnterDevice { get; set; }
		public GKDevice ExitDevice { get; set; }
		public GKDevice LockDevice { get; set; }
		public GKDevice LockControlDevice { get; set; }

		[DataMember]
		public GKDoorType DoorType { get; set; }

		[DataMember]
		public int Delay { get; set; }

		[DataMember]
		public int EnterLevel { get; set; }

		[DataMember]
		public Guid EnterDeviceUID { get; set; }

		[DataMember]
		public Guid ExitDeviceUID { get; set; }

		[DataMember]
		public Guid LockDeviceUID { get; set; }

		[DataMember]
		public Guid LockControlDeviceUID { get; set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Door; } }
	}
}