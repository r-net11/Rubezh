using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Procedure
	{
		public Procedure()
		{
			Name = "Новая процедура";
			Variables = new List<Variable>();
			Arguments = new List<Variable>();
			Steps = new List<ProcedureStep>();
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<ProcedureStep> Steps { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public List<Variable> Variables { get; set; }

		[DataMember]
		public List<Variable> Arguments { get; set; }

		[DataMember]
		public bool IsActive { get; set; }

		public void Start(List<Variable> arguments)
		{
			
		}
	}
}