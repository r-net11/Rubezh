using Infrastructure.Common.TreeList;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.Generic;

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
			if ((procedureStepType == ProcedureStepType.ControlDirection)
				|| (procedureStepType == ProcedureStepType.ControlGKDevice) || (procedureStepType == ProcedureStepType.ControlGKDoor)
				|| (procedureStepType == ProcedureStepType.ControlGKFireZone) || (procedureStepType == ProcedureStepType.ControlGKGuardZone)
				|| (procedureStepType == ProcedureStepType.ControlPumpStation) || (procedureStepType == ProcedureStepType.ControlMPT)
				|| (procedureStepType == ProcedureStepType.ControlDelay) || (procedureStepType == ProcedureStepType.Ptz)
				|| (procedureStepType == ProcedureStepType.StartRecord) || (procedureStepType == ProcedureStepType.StopRecord) || (procedureStepType == ProcedureStepType.RviAlarm) || (procedureStepType == ProcedureStepType.RviOpenWindow)
				|| (procedureStepType == ProcedureStepType.ControlOpcDaTagGet) || (procedureStepType == ProcedureStepType.ControlOpcDaTagSet))
				ImageSource = "/Controls;component/StepIcons/Control.png";
			else if ((procedureStepType == ProcedureStepType.ExportJournal) || (procedureStepType == ProcedureStepType.ExportOrganisation)
				|| (procedureStepType == ProcedureStepType.ExportConfiguration) || (procedureStepType == ProcedureStepType.ExportOrganisationList))
				ImageSource = "/Controls;component/StepIcons/Export.png";
			else if ((procedureStepType == ProcedureStepType.ImportOrganisation) || (procedureStepType == ProcedureStepType.ImportOrganisationList))
				ImageSource = "/Controls;component/StepIcons/Import.png";
			else
				ImageSource = "/Controls;component/StepIcons/" + procedureStepType + ".png";
		}

		public StepTypeViewModel(string folderName, string imageSource, List<StepTypeViewModel> children = null)
		{
			Name = folderName;
			IsFolder = true;
			//ImageSource = "/Controls;component/Images/CFolder.png";
			ImageSource = imageSource;

			if (children != null)
				foreach (var child in children)
					AddChild(child);
		}
	}
}