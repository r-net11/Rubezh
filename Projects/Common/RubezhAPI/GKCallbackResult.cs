﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using RubezhAPI.Journal;

namespace RubezhAPI.GK
{
	[DataContract]
	public class GKCallbackResult
	{
		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public GKStates GKStates { get; set; }

		public GKCallbackResult()
		{
			JournalItems = new List<JournalItem>();
			GKStates = new GKStates();
		}
	}
}