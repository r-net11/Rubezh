using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XPim : XBase
	{
		public XPim()
		{
			UID = BaseUID;// Guid.NewGuid();
			InternalState = new XPimState(this);
		}

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Pim; } }

		public XPimState InternalState { get; set; }
		public override XBaseState BaseState
		{
			get { return InternalState; }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		public override string PresentationName
		{
			get { return Name; }
		}
	}
}