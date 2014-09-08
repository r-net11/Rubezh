using System.Collections.Generic;
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
			GlobalVariables = new List<Variable>();
		}

		[DataMember]
		public List<Procedure> Procedures { get; set; }

		[DataMember]
		public List<AutomationSchedule> AutomationSchedules { get; set; }

		[DataMember]
		public List<AutomationSound> AutomationSounds { get; set; }

		[DataMember]
		public List<Variable> GlobalVariables { get; set; }

		public void UpdateConfiguration()
		{
			foreach (var procedure in Procedures)
			{
				foreach (var step in procedure.Steps)
				{
					UpdateStep(step, procedure);
					InvalidateStep(step, procedure);
				}
			}
		}

		public void UpdateStep(ProcedureStep step, Procedure procedure)
		{

		}

		public void InvalidateStep(ProcedureStep step, Procedure procedure)
		{
			if (step.SoundArguments != null && step.SoundArguments.ProcedureLayoutCollection != null)
			{
				foreach (var layoutsUID in step.SoundArguments.ProcedureLayoutCollection.LayoutsUIDs)
				{
					//
				}
			}
		}
	}
}