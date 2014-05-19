using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Procedure
	{
		public Procedure()
		{
			Name = "Новая процедура";
			InputObjects = new List<ProcedureInputObject>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<ProcedureInputObject> InputObjects { get; set; }
	}
}