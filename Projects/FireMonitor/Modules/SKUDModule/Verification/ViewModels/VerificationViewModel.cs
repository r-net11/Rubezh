using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class VerificationViewModel : ViewPartViewModel
	{
		public VerificationViewModel()
		{
			DenyCommand = new RelayCommand(OnDeny);
			AllowCommand = new RelayCommand(OnAllow);
			Device = SKDManager.Devices.FirstOrDefault(x => x.UID == SKDManager.SKDConfiguration.SKDSystemConfiguration.ReaderDeviceUID);
			VerificationItemViewModel = new VerificationItemViewModel();

			ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Subscribe(OnNewJournal);
		}

		public void OnNewJournal(List<SKDJournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (journalItem.DeviceUID == Device.UID)
				{
					var verificationItemViewModel = new VerificationItemViewModel();
					verificationItemViewModel.EmployeeCardID = journalItem.CardNo.ToString();
					VerificationItemViewModel = verificationItemViewModel;
					OnPropertyChanged("VerificationItemViewModel");
					IsCommandEnabled = true;
				}
			}
		}

		public SKDDevice Device { get; private set; }
		public VerificationItemViewModel VerificationItemViewModel { get; private set; }

		bool _isCommandEnabled;
		public bool IsCommandEnabled
		{
			get { return _isCommandEnabled; }
			set
			{
				_isCommandEnabled = value;
				OnPropertyChanged("IsCommandEnabled");
			}
		}

		public RelayCommand DenyCommand { get; private set; }
		void OnDeny()
		{
			FiresecManager.FiresecService.SKDDenyReader(Device);
			IsCommandEnabled = false;
		}
		bool CanDeny()
		{
			return IsCommandEnabled;
		}

		public RelayCommand AllowCommand { get; private set; }
		void OnAllow()
		{
			FiresecManager.FiresecService.SKDAllowReader(Device);
			IsCommandEnabled = false;
		}
		bool CanAllow()
		{
			return IsCommandEnabled;
		}

		BackgroundWorker BackgroundWorker;
		void OnBackgroundWorker()
		{

		}

		int _commandTimer;
		public int CommandTimer
		{
			get { return _commandTimer; }
			set
			{
				_commandTimer = value;
				OnPropertyChanged("CommandTimer");
			}
		}
	}
}