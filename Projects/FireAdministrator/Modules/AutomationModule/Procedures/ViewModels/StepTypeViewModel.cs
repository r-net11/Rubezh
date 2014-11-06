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
			ImageSource = "/Controls;component/StepIcons/Step.png";
			if ((procedureStepType == ProcedureStepType.Arithmetics) || (procedureStepType == ProcedureStepType.SetValue)
				|| (procedureStepType == ProcedureStepType.IncrementValue) || (procedureStepType == ProcedureStepType.FindObjects)
				|| (procedureStepType == ProcedureStepType.GetObjectProperty) || (procedureStepType == ProcedureStepType.Random)
				|| (procedureStepType == ProcedureStepType.ChangeList) || (procedureStepType == ProcedureStepType.GetListCount)
				|| (procedureStepType == ProcedureStepType.GetListItem) || (procedureStepType == ProcedureStepType.PlaySound)
				|| (procedureStepType == ProcedureStepType.AddJournalItem) || (procedureStepType == ProcedureStepType.SendEmail)
				|| (procedureStepType == ProcedureStepType.ShowMessage)
				|| (procedureStepType == ProcedureStepType.ControlVisualGet) || (procedureStepType == ProcedureStepType.ControlVisualSet)
				|| (procedureStepType == ProcedureStepType.Exit) || (procedureStepType == ProcedureStepType.Pause)
				|| (procedureStepType == ProcedureStepType.ProcedureSelection) || (procedureStepType == ProcedureStepType.CheckPermission)
				|| (procedureStepType == ProcedureStepType.For) || (procedureStepType == ProcedureStepType.While)
				|| (procedureStepType == ProcedureStepType.Break) || (procedureStepType == ProcedureStepType.Continue)
				|| (procedureStepType == ProcedureStepType.GetJournalItem))
				ImageSource = "/Controls;component/StepIcons/" + procedureStepType + ".png";
			if ((procedureStepType == ProcedureStepType.ControlCamera) || (procedureStepType == ProcedureStepType.ControlDirection)
				|| (procedureStepType == ProcedureStepType.ControlDoor) || (procedureStepType == ProcedureStepType.ControlGKDevice)
				|| (procedureStepType == ProcedureStepType.ControlGKFireZone) || (procedureStepType == ProcedureStepType.ControlGKGuardZone)
				|| (procedureStepType == ProcedureStepType.ControlSKDDevice) || (procedureStepType == ProcedureStepType.ControlSKDZone))
				ImageSource = "/Controls;component/StepIcons/Control.png";
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