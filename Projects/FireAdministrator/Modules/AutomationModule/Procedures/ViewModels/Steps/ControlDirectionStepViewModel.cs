using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlDirectionStepViewModel : BaseViewModel, IStepViewModel
	{
		ControlDirectionArguments ControlDirectionArguments { get; set; }
		public ControlDirectionStepViewModel(ControlDirectionArguments controlDirectionArguments)
		{
			ControlDirectionArguments = controlDirectionArguments;
			Commands = new ObservableCollection<DirectionCommandType>
			{
				DirectionCommandType.Automatic, DirectionCommandType.Manual, DirectionCommandType.Ignore, DirectionCommandType.TurnOn,
				DirectionCommandType.TurnOnNow, DirectionCommandType.ForbidStart, DirectionCommandType.TurnOff 
			};
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
				OnPropertyChanged(()=>SelectedCommand);
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
				ControlDirectionArguments.DirectionUid = Guid.Empty;
				if (_selectedDirection != null)
				{
					ControlDirectionArguments.DirectionUid = _selectedDirection.Direction.UID;
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
			if (ControlDirectionArguments.DirectionUid != Guid.Empty)
			{
				var direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == ControlDirectionArguments.DirectionUid);
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
