using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using GKModule.Events;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTViewModel : BaseViewModel
	{
		public XMPT MPT { get; private set; }
		public XState State
		{
			get { return MPT.State; }
		}
		public MPTDetailsViewModel MPTDetailsViewModel { get; private set; }

		public MPTViewModel(XMPT mpt)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			MPT = mpt;
			MPTDetailsViewModel = new MPTDetailsViewModel(MPT);
			State.StateChanged -= new System.Action(OnStateChanged);
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in MPT.Devices)
			{
				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
				Devices.Add(deviceViewModel);
			}

			InitializePIMs();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => HasOnDelay);
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

		#region PIM
		void InitializePIMs()
		{
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
			{
				foreach (var pim in gkDatabase.Pims)
				{
					if (pim.MPTUID == MPT.BaseUID)
					{
						if (pim.Name.StartsWith("АО Р "))
						{
							HandAutomaticOffPim = pim;
							OnHandAutomaticOffStateChanged();
							pim.State.StateChanged -= new System.Action(OnHandAutomaticOffStateChanged);
							pim.State.StateChanged += new System.Action(OnHandAutomaticOffStateChanged);
						}
						if (pim.Name.StartsWith("АО Д "))
						{
							DoorAutomaticOffPim = pim;
							OnDoorAutomaticOffStateChanged();
							pim.State.StateChanged -= new System.Action(OnDoorAutomaticOffStateChanged);
							pim.State.StateChanged += new System.Action(OnDoorAutomaticOffStateChanged);
						}
						if (pim.Name.StartsWith("АО Н "))
						{
							FailureAutomaticOffPim = pim;
							OnFailureAutomaticOffStateChanged();
							pim.State.StateChanged -= new System.Action(OnFailureAutomaticOffStateChanged);
							pim.State.StateChanged += new System.Action(OnFailureAutomaticOffStateChanged);
						}
					}
				}
			}
		}

		XPim HandAutomaticOffPim;
		XPim DoorAutomaticOffPim;
		XPim FailureAutomaticOffPim;

		void OnHandAutomaticOffStateChanged()
		{
			IsHandAutomaticOff = HandAutomaticOffPim.State.StateClasses.Contains(XStateClass.AutoOff);
		}

		void OnDoorAutomaticOffStateChanged()
		{
			IsDoorAutomaticOff = DoorAutomaticOffPim.State.StateClasses.Contains(XStateClass.AutoOff);
		}

		void OnFailureAutomaticOffStateChanged()
		{
			IsFailureAutomaticOff = FailureAutomaticOffPim.State.StateClasses.Contains(XStateClass.AutoOff);
		}

		bool _isHandAutomaticOff;
		public bool IsHandAutomaticOff
		{
			get { return _isHandAutomaticOff; }
			set
			{
				_isHandAutomaticOff = value;
				OnPropertyChanged(() => IsHandAutomaticOff);
			}
		}

		bool _isDoorAutomaticOff;
		public bool IsDoorAutomaticOff
		{
			get { return _isDoorAutomaticOff; }
			set
			{
				_isDoorAutomaticOff = value;
				OnPropertyChanged(() => IsDoorAutomaticOff);
			}
		}

		bool _isFailureAutomaticOff;
		public bool IsFailureAutomaticOff
		{
			get { return _isFailureAutomaticOff; }
			set
			{
				_isFailureAutomaticOff = value;
				OnPropertyChanged(() => IsFailureAutomaticOff);
			}
		}
		#endregion
	}
}