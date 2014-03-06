using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class MPTViewModel : BaseViewModel
	{
		public XMPT MPT { get; private set; }
		public XState State
		{
			get { return MPT.State; }
		}

		public MPTViewModel(XMPT mpt)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			MPT = mpt;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in MPT.Devices)
			{
				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
				Devices.Add(deviceViewModel);
			}
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");
			OnPropertyChanged("HasOnDelay");
			OnPropertyChanged("HasHoldDelay");
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				MPT = MPT
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new MPTDetailsViewModel(MPT));
		}

		public bool HasOnDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOn) && State.OnDelay > 0; }
		}

		public string StartPresentationName
		{
			get { return XManager.GetPresentationZone(MPT.StartLogic); }
		}
	}
}