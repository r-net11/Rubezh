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
		public string ImageSource { get; private set; }

		public StepTypeViewModel(ProcedureStepType procedureStepType)
		{
			ProcedureStepType = procedureStepType;
			Name = procedureStepType.ToDescription();
			IsFolder = false;
			ImageSource = ProcedureHelper.GetIconForProcedure(procedureStepType);
		}

		public StepTypeViewModel(string folderName, string imageSource, List<StepTypeViewModel> children = null)
		{
			Name = folderName;
			IsFolder = true;
			ImageSource = imageSource;

			if (children == null) return;

			foreach (var child in children)
				AddChild(child);
		}
	}
}