using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XCode : XBase, INamedBase, IIdentity
	{
		public XCode()
		{
			UID = Guid.NewGuid();
			Name = "Новый код";
		}

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Code; } }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public ushort No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Password { get; set; }

		public override string PresentationName
		{
			get { return Name; }
		}
	}
}