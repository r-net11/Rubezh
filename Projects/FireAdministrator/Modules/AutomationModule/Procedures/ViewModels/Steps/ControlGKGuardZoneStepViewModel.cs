using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlGKGuardZoneStepViewModel: BaseStepViewModel
	{
		ControlGKGuardZoneArguments ControlGKGuardZoneArguments { get; set; }
		public ArgumentViewModel GKGuardZoneParameter { get; private set; }

		public ControlGKGuardZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlGKGuardZoneArguments = stepViewModel.Step.ControlGKGuardZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<GuardZoneCommandType>();
			GKGuardZoneParameter = new ArgumentViewModel(ControlGKGuardZoneArguments.GKGuardZoneParameter, stepViewModel.Update);
			GKGuardZoneParameter.SelectedObjectType = ObjectType.GuardZone;
			GKGuardZoneParameter.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlGKGuardZoneArguments.GuardZoneCommandType;
			UpdateContent();
		}

		public ObservableCollection<GuardZoneCommandType> Commands { get; private set; }
		GuardZoneCommandType _selectedCommand;
		public GuardZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKGuardZoneArguments.GuardZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}
	
		public override void UpdateContent()
		{
			GKGuardZoneParameter.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.GuardZone, false));
		}

		public override string Description
		{
			get
			{
				return "Зона: " + GKGuardZoneParameter.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
