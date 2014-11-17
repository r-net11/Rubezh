using System.Collections.Generic;
using System.Windows;
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
			if ((procedureStepType == ProcedureStepType.ControlCamera) || (procedureStepType == ProcedureStepType.ControlDirection)
				|| (procedureStepType == ProcedureStepType.ControlDoor) || (procedureStepType == ProcedureStepType.ControlGKDevice)
				|| (procedureStepType == ProcedureStepType.ControlGKFireZone) || (procedureStepType == ProcedureStepType.ControlGKGuardZone)
				|| (procedureStepType == ProcedureStepType.ControlSKDDevice) || (procedureStepType == ProcedureStepType.ControlSKDZone))
				ImageSource = "/Controls;component/StepIcons/Control.png";
			else
				ImageSource = "/Controls;component/StepIcons/" + procedureStepType + ".png";
		}

		public StepTypeViewModel(string folderName, List<StepTypeViewModel> children = null)
		{
			Name = folderName;
			IsFolder = true;
            ImageSource = "/Controls;component/Images/CFolder.png";

			if (children != null)
				foreach (var child in children)
					AddChild(child);
		}
	}
}