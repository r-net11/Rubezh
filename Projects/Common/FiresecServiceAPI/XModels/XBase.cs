using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using FiresecAPI;
using XFiresecAPI;

namespace XFiresecAPI
{
	[DataContract]
	public abstract class XBase
	{
		public XBase()
		{
			ClearDescriptor();
			BaseUID = Guid.NewGuid();
		}

		[DataMember]
		public Guid BaseUID { get; set; }

		public List<XBase> InputXBases { get; set; }
		public List<XBase> OutputXBases { get; set; }

		public XDevice KauDatabaseParent { get; set; }
		public XDevice GkDatabaseParent { get; set; }

		public ushort GKDescriptorNo { get; set; }
		public ushort KAUDescriptorNo { get; set; }

		public void ClearDescriptor()
		{
			InputXBases = new List<XBase>();
			OutputXBases = new List<XBase>();
		}

		public abstract XBaseObjectType ObjectType { get; }
		public string DescriptorPresentationName
		{
			get { return ObjectType.ToDescription() + " " + PresentationName; }
		}

		public abstract string PresentationName { get; }
		public XBaseState BaseState { get; set; }
		public XState State { get; set; }
	}
}