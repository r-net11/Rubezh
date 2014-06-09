using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Automation;
using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class StepTypeViewModel : TreeNodeViewModel<StepTypeViewModel>
	{
		public ProcedureStepType ProcedureStepType { get; private set; }
		public bool IsFolder { get; private set; }
		public string Name { get; private set; }

		public StepTypeViewModel(ProcedureStepType procedureStepType)
		{
			ProcedureStepType = procedureStepType;
			Name = procedureStepType.ToDescription();
			IsFolder = false;
		}

		public StepTypeViewModel(string folderName, List<StepTypeViewModel> children = null)
		{
			Name = folderName;
			IsFolder = true;

			if (children != null)
			{
				foreach (var child in children)
				{
					AddChild(child);
				}
			}
		}
	}
}