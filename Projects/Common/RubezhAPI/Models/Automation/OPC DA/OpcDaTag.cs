using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class OpcDaTag: OpcDaElement
	{
		[DataMember]
		public Guid Uid { get; set; }
		[DataMember]
		public string TagId { get; set; }

		public override bool IsTag
		{
			get { return true; }
		}

		public bool IsChecked { get; set; }
	}
}