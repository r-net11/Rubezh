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
	public class ControlGKFireZoneStepViewModel: BaseStepViewModel
	{
		ControlGKFireZoneArguments ControlGKFireZoneArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlGKFireZoneStepViewModel(ControlGKFireZoneArguments controlGKFireZoneArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ControlGKFireZoneArguments = controlGKFireZoneArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<ZoneCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlGKFireZoneArguments.Variable1);
			OnPropertyChanged(() => Commands);
			SelectZoneCommand = new RelayCommand(OnSelectZone);
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
					Variable1.UidValue = _selectedZone.Zone.UID;
				}
				OnPropertyChanged(() => SelectedZone);
			}
		}
		
		public RelayCommand SelectZoneCommand { get; private set; }
		private void OnSelectZone()
		{
			var zoneSelectationViewModel = new ZoneSelectionViewModel(SelectedZone != null ? SelectedZone.Zone : null);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				SelectedZone = zoneSelectationViewModel.SelectedZone;
			}
		}

		public void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.Zone, false));
			if (Variable1.UidValue != Guid.Empty)
			{
				var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == Variable1.UidValue);
				SelectedZone = zone != null ? new ZoneViewModel(zone) : null;
				SelectedCommand = ControlGKFireZoneArguments.ZoneCommandType;
			}
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}
