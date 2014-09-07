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
	public class ControlGKGuardZoneStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlGKGuardZoneArguments ControlGKGuardZoneArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlGKGuardZoneStepViewModel(ControlGKGuardZoneArguments controlGKGuardZoneArguments, Procedure procedure)
		{
			ControlGKGuardZoneArguments = controlGKGuardZoneArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<GuardZoneCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlGKGuardZoneArguments.Variable1);
			OnPropertyChanged(() => Commands);
			SelectZoneCommand = new RelayCommand(OnSelectZone);
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
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				Variable1.UidValue = Guid.Empty;
				if (_selectedZone != null)
				{
					Variable1.UidValue = _selectedZone.GuardZone.UID;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedZone);
			}
		}
		
		public RelayCommand SelectZoneCommand { get; private set; }
		private void OnSelectZone()
		{
			var zoneSelectationViewModel = new GuardZoneSelectionViewModel(SelectedZone != null ? SelectedZone.GuardZone : null);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				SelectedZone = zoneSelectationViewModel.SelectedZone;
			}
		}

		public void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.GuardZone, false));
			if (Variable1.UidValue != Guid.Empty)
			{
				var zone = XManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == Variable1.UidValue);
				SelectedZone = zone != null ? new ZoneViewModel(zone) : null;
				SelectedCommand = ControlGKGuardZoneArguments.GuardZoneCommandType;
			}
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
