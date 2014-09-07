using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ControlDirectionStepViewModel : BaseViewModel, IStepViewModel
	{
		ControlDirectionArguments ControlDirectionArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }
		Procedure Procedure { get; set; }

		public ControlDirectionStepViewModel(ControlDirectionArguments controlDirectionArguments, Procedure procedure)
		{
			ControlDirectionArguments = controlDirectionArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<DirectionCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlDirectionArguments.Variable1);
			OnPropertyChanged(() => Commands);
			SelectDirectionCommand = new RelayCommand(OnSelectDirection);
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
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		DirectionViewModel _selectedDirection;
		public DirectionViewModel SelectedDirection
		{
			get { return _selectedDirection; }
			set
			{
				_selectedDirection = value;
				Variable1.UidValue = Guid.Empty;
				if (_selectedDirection != null)
				{
					Variable1.UidValue = _selectedDirection.Direction.UID;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedDirection);
			}
		}

		public RelayCommand SelectDirectionCommand { get; private set; }
		private void OnSelectDirection()
		{
			var directionSelectationViewModel = new DirectionSelectionViewModel(SelectedDirection != null ? SelectedDirection.Direction : null);
			if (DialogService.ShowModalWindow(directionSelectationViewModel))
			{
				SelectedDirection = directionSelectationViewModel.SelectedDirection;
			}
		}

		public void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.Direction, false));
			if (Variable1.UidValue != Guid.Empty)
			{
				var direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == Variable1.UidValue);
				SelectedDirection = direction != null ? new DirectionViewModel(direction) : null;
				SelectedCommand = ControlDirectionArguments.DirectionCommandType;
			}
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
