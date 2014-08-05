using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class AutomationConfiguration
	{
		public AutomationConfiguration()
		{
			Procedures = new List<Procedure>();
			AutomationSchedules = new List<AutomationSchedule>();
			AutomationSounds = new List<AutomationSound>();
			GlobalVariables = new List<GlobalVariable>();
		}

		[DataMember]
		public List<Procedure> Procedures { get; set; }

		[DataMember]
		public List<AutomationSchedule> AutomationSchedules { get; set; }

		[DataMember]
		public List<AutomationSound> AutomationSounds { get; set; }

		[DataMember]
		public List<GlobalVariable> GlobalVariables { get; set; }

		public void UpdateConfiguration() //TODO???
		{
			foreach (var procedure in Procedures)
			{
				foreach (var step in procedure.Steps)
				{
					UpdateStep(step, procedure);
				}
			}
		}

		public void UpdateStep(ProcedureStep step, Procedure procedure)
		{

		}
	}
}