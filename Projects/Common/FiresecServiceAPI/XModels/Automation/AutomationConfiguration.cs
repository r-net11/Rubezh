using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class AutomationConfiguration
	{
		public AutomationConfiguration()
		{
			Procedures = new List<Procedure>();
		}

		[DataMember]
		public List<Procedure> Procedures { get; set; }
	}
}