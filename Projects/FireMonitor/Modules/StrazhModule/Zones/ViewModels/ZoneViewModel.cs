using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using StrazhAPI.SKD;
using StrazhModule.Zones;

namespace StrazhModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public SKDZone Zone { get; private set; }
		public SKDZoneState State { get { return Zone.State; } }

		public ZoneViewModel(SKDZone zone)
		{
			Zone = zone;
			State.StateChanged -= OnStateChanged;
			State.StateChanged += OnStateChanged;
			OnStateChanged();

			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			ZoneAccessStateNormalCommand = new RelayCommand(OnZoneAccessStateNormal, CanZoneAccessStateNormal);
			ZoneAccessStateCloseAlwaysCommand = new RelayCommand(OnZoneAccessStateCloseAlways, CanZoneAccessStateCloseAlways);
			ZoneAccessStateOpenAlwaysCommand = new RelayCommand(OnZoneAccessStateOpenAlways, CanZoneAccessStateOpenAlways);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ClearPromptWarningCommand = new RelayCommand(OnClearPromptWarning, CanClearPromptWarning);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
		}

		public string PresentationName
		{
			get { return Zone.Name; }
		}

		public string PresentationDescription
		{
			get { return Zone.Description; }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		private void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowSKDZone(Zone);
		}
		private bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowSKDZone(Zone);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs
			{
				SKDZone = Zone
			};
			ServiceFactoryBase.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			DialogService.ShowWindow(new ZoneDetailsViewModel(Zone));
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			ZoneCommander.Open(Zone);
		}
		bool CanOpen()
		{
			return ZoneCommander.CanOpen(Zone);
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			ZoneCommander.Close(Zone);
		}
		bool CanClose()
		{
			return ZoneCommander.CanClose(Zone);
		}

		public RelayCommand ZoneAccessStateNormalCommand { get; private set; }
		void OnZoneAccessStateNormal()
		{
			ZoneCommander.SetAccessStateToNormal(Zone);
		}
		bool CanZoneAccessStateNormal()
		{
			return ZoneCommander.CanSetAccessStateToNormal(Zone);
		}

		public RelayCommand ZoneAccessStateCloseAlwaysCommand { get; private set; }
		void OnZoneAccessStateCloseAlways()
		{
			ZoneCommander.SetAccessStateToCloseAlways(Zone);
		}
		bool CanZoneAccessStateCloseAlways()
		{
			return ZoneCommander.CanSetAccessStateToCloseAlways(Zone);
		}

		public RelayCommand ZoneAccessStateOpenAlwaysCommand { get; private set; }
		void OnZoneAccessStateOpenAlways()
		{
			ZoneCommander.SetAccessStateToOpenAlways(Zone);
		}
		bool CanZoneAccessStateOpenAlways()
		{
			return ZoneCommander.CanSetAccessStateToOpenAlways(Zone);
		}

		public RelayCommand ClearPromptWarningCommand { get; private set; }
		private void OnClearPromptWarning()
		{
			ZoneCommander.ClearPromptWarning(Zone);
		}
		private bool CanClearPromptWarning()
		{
			return ZoneCommander.CanClearPromptWarning(Zone);
		}

		public bool IsBold { get; set; }
	}
}