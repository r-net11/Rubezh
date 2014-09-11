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
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlDirectionStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlDirectionArguments = stepViewModel.Step.ControlDirectionArguments;
			Commands = ProcedureHelper.GetEnumObs<DirectionCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlDirectionArguments.Variable1, stepViewModel.Update);
			Variable1.ObjectType = ObjectType.Direction;
			Variable1.ExplicitType = ExplicitType.Object;
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
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.Direction, false));
		}

		public override string Description
		{
			get
			{
				return "Направление: " + Variable1.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
