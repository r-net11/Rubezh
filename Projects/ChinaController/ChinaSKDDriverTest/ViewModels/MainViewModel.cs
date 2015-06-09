﻿using ChinaSKDDriver;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace ControllerSDK.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public static Wrapper Wrapper { get; set; }

		public CardsViewModel CardsViewModel { get; private set; }
		public PasswordsViewModel PasswordsViewModel { get; private set; }
		public HolidaysViewModel HolidaysViewModel { get; private set; }
		public TimeShedulesViewModel TimeShedulesViewModel { get; private set; }
		public ControlViewModel ControlViewModel { get; private set; }
		public JournalViewModel JournalViewModel { get; private set; }
		public NativeJournalViewModel NativeJournalViewModel { get; private set; }
		public CommonViewModel CommonViewModel { get; private set; }
		public DoorConfigurationViewModel DoorConfigurationViewModel { get; private set; }
		public AntiPassBackViewModel AntiPassBackViewModel { get; private set; }
		public InterlockViewModel InterlockViewModel { get; private set; }
		public LogItemsViewModel LogItemsViewModel { get; private set; }
		public AccessesViewModel AccessesViewModel { get; private set; }

		public MainViewModel()
		{
			Wrapper = new Wrapper();

			CardsViewModel = new CardsViewModel();
			PasswordsViewModel = new PasswordsViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			TimeShedulesViewModel = new TimeShedulesViewModel();
			ControlViewModel = new ControlViewModel();
			JournalViewModel = new JournalViewModel();
			NativeJournalViewModel = new NativeJournalViewModel();
			CommonViewModel = new CommonViewModel();
			DoorConfigurationViewModel = new DoorConfigurationViewModel();
			AntiPassBackViewModel = new AntiPassBackViewModel();
			InterlockViewModel = new InterlockViewModel();
			LogItemsViewModel = new LogItemsViewModel();
			AccessesViewModel = new AccessesViewModel();

			ConnectCommand = new RelayCommand(OnConnect);
			DisconnectCommand = new RelayCommand(OnDisconnect);
			OnConnect();
		}

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			Wrapper.Initialize();
			string error;
			var connectionSettings = ConnectionSettingsHelper.Get();
			MainViewModel.Wrapper.Connect(connectionSettings.Address, connectionSettings.Port, connectionSettings.Login, connectionSettings.Password, out error);
		}

		public RelayCommand DisconnectCommand { get; private set; }
		void OnDisconnect()
		{
			MainViewModel.Wrapper.Disconnect();
		}
	}
}