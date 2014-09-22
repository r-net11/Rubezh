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
	public class ControlGKFireZoneStepViewModel: BaseStepViewModel
	{
		ControlGKFireZoneArguments ControlGKFireZoneArguments { get; set; }
		public ArgumentViewModel GKFireZoneParameter { get; private set; }

		public ControlGKFireZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlGKFireZoneArguments = stepViewModel.Step.ControlGKFireZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<ZoneCommandType>();
			GKFireZoneParameter = new ArgumentViewModel(ControlGKFireZoneArguments.GKFireZoneParameter, stepViewModel.Update);
			GKFireZoneParameter.SelectedObjectType = ObjectType.Zone;
			GKFireZoneParameter.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlGKFireZoneArguments.ZoneCommandType;
			UpdateContent();
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
			GKFireZoneParameter.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.Zone, false));
		}

		public override string Description
		{
			get
			{
				return "Зона: " + GKFireZoneParameter.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
