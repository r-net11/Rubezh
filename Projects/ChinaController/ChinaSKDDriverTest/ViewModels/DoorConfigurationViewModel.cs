using System.Windows;
using StrazhDeviceSDK.API;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace ControllerSDK.ViewModels
{
	public class DoorConfigurationViewModel : BaseViewModel
	{
		public DoorConfigurationViewModel()
		{
			GetDoorConfigurationCommand = new RelayCommand(OnGetDoorConfiguration);
			SetDoorConfigurationCommand = new RelayCommand(OnSetDoorConfiguration);

			InitAvailableAccessStates();
			InitAvailableAccessModes();
			InitAvailableDoorOpenMethods();
		}

		DoorConfiguration _doorConfiguration;
		public DoorConfiguration DoorConfiguration
		{
			get { return _doorConfiguration; }
			set
			{
				_doorConfiguration = value;
				OnPropertyChanged(() => DoorConfiguration);
			}
		}

		public RelayCommand GetDoorConfigurationCommand { get; private set; }
		void OnGetDoorConfiguration()
		{
			const string msg = "Получение конфигурации двери {0} - {1}";
			DoorConfiguration = MainViewModel.Wrapper.GetDoorConfiguration(DoorNo);
			if (DoorConfiguration != null)
			{
				MessageBox.Show(string.Format(msg, DoorNo, "Успех"));
			}
			else
			{
				MessageBox.Show(string.Format(msg, DoorNo, "Ошибка"));
			}
		}

		public RelayCommand SetDoorConfigurationCommand { get; private set; }
		void OnSetDoorConfiguration()
		{
			const string msg = "Запись конфигурации двери {0} - {1}";
			DoorConfiguration.ChannelName = DoorNo.ToString();
			var result = MainViewModel.Wrapper.SetDoorConfiguration(DoorConfiguration, DoorNo);
			if (result)
			{
				MessageBox.Show(string.Format(msg, DoorNo, "Успех"));
			}
			else
			{
				MessageBox.Show(string.Format(msg, DoorNo, "Ошибка"));
			}
		}

		int _doorNo;
		public int DoorNo
		{
			get { return _doorNo; }
			set
			{
				_doorNo = value;
				OnPropertyChanged(() => DoorNo);
			}
		}

		public ObservableCollection<AccessState> AvailableAccessStates { get; private set; }
		void InitAvailableAccessStates()
		{
			AvailableAccessStates = new ObservableCollection<AccessState>();
			AvailableAccessStates.Add(AccessState.ACCESS_STATE_NORMAL);
			AvailableAccessStates.Add(AccessState.ACCESS_STATE_CLOSEALWAYS);
			AvailableAccessStates.Add(AccessState.ACCESS_STATE_OPENALWAYS);
		}

		public ObservableCollection<AccessMode> AvailableAccessModes { get; private set; }
		void InitAvailableAccessModes()
		{
			AvailableAccessModes = new ObservableCollection<AccessMode>();
			AvailableAccessModes.Add(AccessMode.ACCESS_MODE_HANDPROTECTED);
			AvailableAccessModes.Add(AccessMode.ACCESS_MODE_SAFEROOM);
			AvailableAccessModes.Add(AccessMode.ACCESS_MODE_OTHER);
		}

		public ObservableCollection<DoorOpenMethod> AvailableDoorOpenMethods { get; private set; }
		void InitAvailableDoorOpenMethods()
		{
			AvailableDoorOpenMethods = new ObservableCollection<DoorOpenMethod>
			{
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_UNKNOWN,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_ONLY,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_OR_CARD,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD_FIRST,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_FIRST,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_SECTION,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_FINGERPRINTONLY,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_OR_CARD_OR_FINGERPRINT,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD_AND_FINGERPRINT,
				DoorOpenMethod.CFG_DOOR_OPEN_METHOD_MULTI_PERSON
			};
		}
	}
}