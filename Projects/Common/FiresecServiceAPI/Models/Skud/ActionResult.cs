using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class ActionResult
	{
		[DataMember]
		public int RowCount { get; set; }
		[DataMember]
		public string Error { get; set; }
	}
}
