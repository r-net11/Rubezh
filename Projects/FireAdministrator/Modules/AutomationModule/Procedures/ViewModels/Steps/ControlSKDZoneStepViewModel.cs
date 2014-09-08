using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ControlSKDZoneStepViewModel: BaseStepViewModel
	{
		ControlSKDZoneArguments ControlSKDZoneArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlSKDZoneStepViewModel(ControlSKDZoneArguments controlSKDZoneArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ControlSKDZoneArguments = controlSKDZoneArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<SKDZoneCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlSKDZoneArguments.Variable1);
			OnPropertyChanged(() => Commands);
			SelectZoneCommand = new RelayCommand(OnSelectZone);
			UpdateContent();
		}

		public ObservableCollection<SKDZoneCommandType> Commands { get; private set; }

		SKDZoneCommandType _selectedCommand;
		public SKDZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlSKDZoneArguments.SKDZoneCommandType = value;
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
					Variable1.UidValue = _selectedZone.SKDZone.UID;
				}
				OnPropertyChanged(() => SelectedZone);
			}
		}
		
		public RelayCommand SelectZoneCommand { get; private set; }
		private void OnSelectZone()
		{
			var zoneSelectationViewModel = new SKDZoneSelectionViewModel(SelectedZone != null ? SelectedZone.SKDZone : null);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				SelectedZone = zoneSelectationViewModel.SelectedZone;
			}
		}

		public void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.SKDZone, false));
			if (Variable1.UidValue != Guid.Empty)
			{
				var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == Variable1.UidValue);
				SelectedZone = zone != null ? new ZoneViewModel(zone) : null;
				SelectedCommand = ControlSKDZoneArguments.SKDZoneCommandType;
			}
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}
