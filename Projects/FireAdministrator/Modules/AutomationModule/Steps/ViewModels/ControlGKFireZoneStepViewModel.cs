using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlGKFireZoneStepViewModel: BaseStepViewModel
	{
		ControlGKFireZoneArguments ControlGKFireZoneArguments { get; set; }
		public ArgumentViewModel GKFireZoneArgument { get; private set; }

		public ControlGKFireZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlGKFireZoneArguments = stepViewModel.Step.ControlGKFireZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<ZoneCommandType>();
			GKFireZoneArgument = new ArgumentViewModel(ControlGKFireZoneArguments.GKFireZoneArgument, stepViewModel.Update, null);
			SelectedCommand = ControlGKFireZoneArguments.ZoneCommandType;
		}

		public ObservableCollection<ZoneCommandType> Commands { get; private set; }

		ZoneCommandType _selectedCommand;
		public ZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKFireZoneArguments.ZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			GKFireZoneArgument.Update(Procedure, ExplicitType.Object, objectType:ObjectType.Zone, isList:false);
		}

		public override string Description
		{
			get
			{
				return "Зона: " + GKFireZoneArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
