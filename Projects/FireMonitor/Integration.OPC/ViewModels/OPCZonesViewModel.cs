using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Integration.OPC.Models;
using Localization.Integration.Errors;
using StrazhAPI.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Integration.OPC.ViewModels
{
	public class OPCZonesViewModel : ViewPartViewModel
	{
		public List<OPCZone> ZonesOPC { get; set; }

		private OPCZone _selectedOPCZone;

		public OPCZone SelectedZoneOPC
		{
			get { return _selectedOPCZone; }
			set
			{
				_selectedOPCZone = value;
				OnPropertyChanged(() => SelectedZoneOPC);
				OnPropertyChanged(() => IsSeparatorEnabled);
			}
		}

		public bool IsSeparatorEnabled { get { return SelectedZoneOPC != null && SelectedZoneOPC.Type == OPCZoneType.Guard; } }

		public RelayCommand GuardOffCommand { get; set; }
		public RelayCommand GuardOnCommand { get; set; }
		public RelayCommand ShowPropertiesCommand { get; set; }
		public RelayCommand ShowOnPlanCommand { get; set; }
		public RelayCommand ShowJournalCommand { get; set; }

		public void OnGuardOff()
		{
			FiresecManager.FiresecService.UnsetGuard(SelectedZoneOPC.No);
		}

		public void OnGuardOn()
		{
			FiresecManager.FiresecService.SetGuard(SelectedZoneOPC.No);
		}

		public void Initialize(IEnumerable<OPCZone> zones)
		{
			GuardOffCommand = new RelayCommand(OnGuardOff, () => SelectedZoneOPC != null && SelectedZoneOPC.Type == OPCZoneType.Guard);
			GuardOnCommand = new RelayCommand(OnGuardOn, () => SelectedZoneOPC != null && SelectedZoneOPC.Type == OPCZoneType.Guard);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, () => SelectedZoneOPC != null);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, () => SelectedZoneOPC != null);
			ShowJournalCommand = new RelayCommand(OnShowJournal, () => SelectedZoneOPC != null);
			ZonesOPC = new List<OPCZone>(zones);
			SelectedZoneOPC = ZonesOPC.FirstOrDefault();
		}

		public void OnShowProperties()
		{
			var propertiesDialog = new PropertiesViewModel(SelectedZoneOPC);
			DialogService.ShowWindow(propertiesDialog);
		}

		public void OnShowOnPlan()
		{
			MessageBoxService.ShowError(CommonErrors.FunctionIsNotRealised_Error); //TODO: Implement show on plan function
		}

		public void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs
			{
				OPCZone = SelectedZoneOPC.ToDTO(),
			};
			ServiceFactoryBase.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}
	}
}
