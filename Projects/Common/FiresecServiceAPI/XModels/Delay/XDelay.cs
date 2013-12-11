﻿using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDelay : XBase
	{
		public XDelay()
		{
			UID = BaseUID;// Guid.NewGuid();
			InternalState = new XDelayState(this);
		}

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Delay; } }

		public XDelayState InternalState { get; set; }
		public override XBaseState BaseState
		{
			get { return InternalState; }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ushort DelayTime { get; set; }

		[DataMember]
		public ushort SetTime { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		public override string PresentationName
		{
			get { return Name; }
		}
	}
}