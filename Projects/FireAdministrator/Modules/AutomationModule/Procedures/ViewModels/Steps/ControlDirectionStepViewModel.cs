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
	public class ControlDirectionStepViewModel : BaseStepViewModel
	{
		ControlDirectionArguments ControlDirectionArguments { get; set; }
		public ArithmeticParameterViewModel DirectionParameter { get; private set; }

		public ControlDirectionStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlDirectionArguments = stepViewModel.Step.ControlDirectionArguments;
			Commands = ProcedureHelper.GetEnumObs<DirectionCommandType>();
			DirectionParameter = new ArithmeticParameterViewModel(ControlDirectionArguments.DirectionParameter, stepViewModel.Update);
			DirectionParameter.ObjectType = ObjectType.Direction;
			DirectionParameter.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlDirectionArguments.DirectionCommandType;
			UpdateContent();
		}

		public ObservableCollection<DirectionCommandType> Commands { get; private set; }
		DirectionCommandType _selectedCommand;
		public DirectionCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlDirectionArguments.DirectionCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			DirectionParameter.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.Direction, false));
		}

		public override string Description
		{
			get
			{
				return "Направление: " + DirectionParameter.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
