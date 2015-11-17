﻿using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class UIArguments
	{
		public UIArguments()
		{
			LayoutFilter = new ProcedureLayoutCollection();
			ForAllClients = true;
		}

		[DataMember]
		public ProcedureLayoutCollection LayoutFilter { get; set; }

		[DataMember]
		public bool ForAllClients { get; set; }
	}
}