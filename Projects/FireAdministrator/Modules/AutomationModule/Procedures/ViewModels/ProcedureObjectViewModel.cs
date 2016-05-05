using FiresecAPI.Automation;
using ObjectType = FiresecAPI.Automation.ObjectType;

namespace AutomationModule.Procedures.ViewModels
{
	public class ProcedureObjectViewModel
	{
		ProcedureObject ProcedureObject { get; set; }
		public ProcedureObjectViewModel(ProcedureObject procedureObject)
		{
			ProcedureObject = procedureObject;
			ObjectType = procedureObject.ObjectType;
		}

		public ObjectType ObjectType { get; private set; }
	}
}
