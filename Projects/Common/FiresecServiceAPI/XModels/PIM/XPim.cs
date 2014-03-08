using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XPim : XBase
	{
		public XPim()
		{
			//UID = BaseUID;// Guid.NewGuid();
		}

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Pim; } }

		//[DataMember]
		//public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		public override string PresentationName
		{
			get { return Name; }
		}
	}
}