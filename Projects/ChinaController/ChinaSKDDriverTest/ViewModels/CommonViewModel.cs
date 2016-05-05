using System;
using System.Runtime.InteropServices;
using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using StrazhDeviceSDK.API;
using StrazhDeviceSDK.NativeAPI;

namespace ControllerSDK.ViewModels
{
	public class CommonViewModel : BaseViewModel
	{
		public CommonViewModel()
		{
			GetDeviceSoftwareInfoCommand = new RelayCommand(OnGetDeviceSoftwareInfo);
			GetDeviceNetInfoCommand = new RelayCommand(OnGetDeviceNetInfo);
			SetDeviceNetInfoCommand = new RelayCommand(OnSetDeviceNetInfo);
			GetMacAddressCommand = new RelayCommand(OnGetMacAddress);
			GetMaxPageSizeCommand = new RelayCommand(OnGetMaxPageSize);
			GetCurrentTimeCommand = new RelayCommand(OnGetCurrentTime);
			SetCurrentTimeCommand = new RelayCommand(OnSetCurrentTime);
			GetControllerDirectionTypeCommand = new RelayCommand(OnGetControllerDirectionType);
			SetControllerDirectionTypeCommand = new RelayCommand(OnSetControllerDirectionType);
			GetTimeSettingsCommand = new RelayCommand(OnGetTimeSettings);
			SetTimeSettingsCommand = new RelayCommand(OnSetTimeSettings);
			GetDoorConfigurationCommand = new RelayCommand(OnGetDoorConfiguration);
			SetDoorConfigurationCommand = new RelayCommand(OnSetDoorConfiguration);
			ReBootCommand = new RelayCommand(OnReBoot);
			UpdateFirmwareCommand = new RelayCommand(OnUpdateFirmware);
			DeleteCfgFileCommand = new RelayCommand(OnDeleteCfgFile);
			SetPasswordCommand = new RelayCommand(OnSetPassword);
			TestCommand = new RelayCommand(OnTest);

			AvailableControllerDirectionTypes = new ObservableCollection<DoorType>();
			AvailableControllerDirectionTypes.Add(DoorType.OneWay);
			AvailableControllerDirectionTypes.Add(DoorType.TwoWay);

			Login = "system";
			OldPassword = "123456";
			NewPassword = "123456";
		}

		public RelayCommand GetDeviceSoftwareInfoCommand { get; private set; }
		void OnGetDeviceSoftwareInfo()
		{
			var deviceSoftwareInfo = MainViewModel.Wrapper.GetDeviceSoftwareInfo();
			if (deviceSoftwareInfo != null)
			{
				var text = "SoftwareBuildDate = " + deviceSoftwareInfo.SoftwareBuildDate.ToString() + "\n";
				text += "DeviceType = " + deviceSoftwareInfo.DeviceType + "\n";
				text += "SoftwareVersion = " + deviceSoftwareInfo.SoftwareVersion + "\n";
				MessageBox.Show(text);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand GetDeviceNetInfoCommand { get; private set; }
		void OnGetDeviceNetInfo()
		{
			var deviceNetInfo = MainViewModel.Wrapper.GetDeviceNetInfo();
			if (deviceNetInfo != null)
			{
				var text = "IP = " + deviceNetInfo.Address + "\n";
				text += "SubnetMask = " + deviceNetInfo.Mask + "\n";
				text += "DefaultGateway = " + deviceNetInfo.DefaultGateway + "\n";
				MessageBox.Show(text);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand SetDeviceNetInfoCommand { get; private set; }
		void OnSetDeviceNetInfo()
		{
			if (MessageBox.Show("Вы уверены, что хотите перезаписать сетевые настройки?") == MessageBoxResult.OK)
			{
				var deviceNetInfo = new SKDControllerNetworkSettings()
				{
					Address = "172.16.6.40",
					Mask = "255.255.252.0",
					DefaultGateway = "172.16.6.219"
				};
				MainViewModel.Wrapper.SetDeviceNetInfo(deviceNetInfo);
			}
		}

		public RelayCommand GetMacAddressCommand { get; private set; }
		void OnGetMacAddress()
		{
			var result = MainViewModel.Wrapper.GetDeviceMacAddress();
			if (result != null)
			{
				MessageBox.Show("macAddress = " + result);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand GetMaxPageSizeCommand { get; private set; }
		void OnGetMaxPageSize()
		{
			var result = MainViewModel.Wrapper.GetMaxPageSize();
			if (result > 0)
			{
				MessageBox.Show("maxPageSize = " + result);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand GetCurrentTimeCommand { get; private set; }
		void OnGetCurrentTime()
		{
			var result = MainViewModel.Wrapper.GetDateTime();
			if (result > DateTime.MinValue)
			{
				MessageBox.Show("dateTime = " + result.ToString());
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand SetCurrentTimeCommand { get; private set; }
		void OnSetCurrentTime()
		{
			var result = MainViewModel.Wrapper.SetDateTime(DateTime.Now);
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand GetControllerDirectionTypeCommand { get; private set; }
		void OnGetControllerDirectionType()
		{
			SelectedControllerDirectionType = MainViewModel.Wrapper.GetControllerDoorType();
		}

		public RelayCommand SetControllerDirectionTypeCommand { get; private set; }
		void OnSetControllerDirectionType()
		{
			var result = MainViewModel.Wrapper.SetControllerDoorType(SelectedControllerDirectionType);
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public ObservableCollection<DoorType> AvailableControllerDirectionTypes { get; private set; }

		DoorType _selectedControllerDirectionType;
		public DoorType SelectedControllerDirectionType
		{
			get { return _selectedControllerDirectionType; }
			set
			{
				_selectedControllerDirectionType = value;
				OnPropertyChanged(() => SelectedControllerDirectionType);
			}
		}

		public RelayCommand GetTimeSettingsCommand { get; private set; }
		void OnGetTimeSettings()
		{
			var controllerTimeSettings = MainViewModel.Wrapper.GetControllerTimeSettings();
		}

		public RelayCommand SetTimeSettingsCommand { get; private set; }
		void OnSetTimeSettings()
		{
			var controllerTimeSettings = new SKDControllerTimeSettings();
			controllerTimeSettings.Name = "___Name___";
			var result = MainViewModel.Wrapper.SetControllerTimeSettings(controllerTimeSettings);
		}

		DoorConfiguration DoorConfiguration;

		public RelayCommand GetDoorConfigurationCommand { get; private set; }
		void OnGetDoorConfiguration()
		{
			DoorConfiguration = MainViewModel.Wrapper.GetDoorConfiguration(0);
			if (DoorConfiguration != null)
			{
				var text = "";
				text += "ChannelName = " + DoorConfiguration.ChannelName + "\n";
				text += "AceessState = " + DoorConfiguration.AccessState.ToString() + "\n";
				text += "AceessMode = " + DoorConfiguration.AccessMode.ToString() + "\n";
				text += "EnableMode = " + DoorConfiguration.EnableMode.ToString() + "\n";
				text += "IsSnapshotEnable = " + DoorConfiguration.IsSnapshotEnable.ToString() + "\n";
				text += "\n";
				text += "UseDoorOpenMethod = " + DoorConfiguration.UseDoorOpenMethod.ToString() + "\n";
				text += "UseUnlockHoldInterval = " + DoorConfiguration.UseUnlockHoldInterval.ToString() + "\n";
				text += "UseCloseTimeout = " + DoorConfiguration.UseCloseTimeout.ToString() + "\n";
				text += "UseOpenAlwaysTimeIndex = " + DoorConfiguration.UseOpenAlwaysTimeIndex.ToString() + "\n";
				text += "UseHolidayTimeIndex = " + DoorConfiguration.UseHolidayTimeIndex.ToString() + "\n";
				text += "UseBreakInAlarmEnable = " + DoorConfiguration.UseBreakInAlarmEnable.ToString() + "\n";
				text += "UseRepeatEnterAlarmEnable = " + DoorConfiguration.UseRepeatEnterAlarmEnable.ToString() + "\n";
				text += "UseDoorNotClosedAlarmEnable = " + DoorConfiguration.UseDoorNotClosedAlarmEnable.ToString() + "\n";
				text += "UseDuressAlarmEnable = " + DoorConfiguration.UseDuressAlarmEnable.ToString() + "\n";
				text += "UseDoorTimeSection = " + DoorConfiguration.UseDoorTimeSection.ToString() + "\n";
				text += "UseSensorEnable = " + DoorConfiguration.UseSensorEnable.ToString() + "\n";
				text += "\n";
				text += "DoorOpenMethod = " + DoorConfiguration.DoorOpenMethod.ToString() + "\n";
				text += "UnlockHoldInterval = " + DoorConfiguration.UnlockHoldInterval.ToString() + "\n";
				text += "CloseTimeout = " + DoorConfiguration.CloseTimeout.ToString() + "\n";
				text += "OpenAlwaysTimeIndex = " + DoorConfiguration.OpenAlwaysTimeIndex.ToString() + "\n";
				text += "HolidayTimeRecoNo = " + DoorConfiguration.HolidayTimeRecoNo.ToString() + "\n";
				text += "IsBreakInAlarmEnable = " + DoorConfiguration.IsBreakInAlarmEnable.ToString() + "\n";
				text += "IsRepeatEnterAlarmEnable = " + DoorConfiguration.IsRepeatEnterAlarmEnable.ToString() + "\n";
				text += "IsDoorNotClosedAlarmEnable = " + DoorConfiguration.IsDoorNotClosedAlarmEnable.ToString() + "\n";
				text += "IsDuressAlarmEnable = " + DoorConfiguration.IsDuressAlarmEnable.ToString() + "\n";
				text += "IsSensorEnable = " + DoorConfiguration.IsSensorEnable.ToString() + "\n";

				foreach (var doorDayInterval in DoorConfiguration.DoorDayIntervalsCollection.DoorDayIntervals)
				{
					foreach (var doorDayIntervalPart in doorDayInterval.DoorDayIntervalParts)
					{
						text += doorDayIntervalPart.StartHour + ":" + doorDayIntervalPart.StartMinute + " - " +
						doorDayIntervalPart.EndHour + ":" + doorDayIntervalPart.EndMinute + " ||||||| ";
					}
					text += "\n";
				}

				MessageBox.Show(text);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand SetDoorConfigurationCommand { get; private set; }
		void OnSetDoorConfiguration()
		{
			DoorConfiguration.ChannelName = "0";
			DoorConfiguration.IsDoorNotClosedAlarmEnable = true;

			DoorConfiguration.UnlockHoldInterval = 444;
			DoorConfiguration.HolidayTimeRecoNo = 44;
			var result = MainViewModel.Wrapper.SetDoorConfiguration(DoorConfiguration, 0);
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand ReBootCommand { get; private set; }
		void OnReBoot()
		{
			var result = MainViewModel.Wrapper.Reboot();
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand DeleteCfgFileCommand { get; private set; }
		void OnDeleteCfgFile()
		{
			var result = MainViewModel.Wrapper.Reset();
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged(() => Login);
			}
		}

		string _oldPassword;
		public string OldPassword
		{
			get { return _oldPassword; }
			set
			{
				_oldPassword = value;
				OnPropertyChanged(() => OldPassword);
			}
		}

		string _newPassword;
		public string NewPassword
		{
			get { return _newPassword; }
			set
			{
				_newPassword = value;
				OnPropertyChanged(() => NewPassword);
			}
		}

		public RelayCommand SetPasswordCommand { get; private set; }
		void OnSetPassword()
		{
			var result = MainViewModel.Wrapper.SetControllerPassword(Login, OldPassword, NewPassword);
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand UpdateFirmwareCommand { get; private set; }
		void OnUpdateFirmware()
		{
			return;

			var result = MainViewModel.Wrapper.UpdateFirmware("C:/firmware.wav");
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}


		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			NativeWrapper.CFG_ACCESS_EVENT_INFO testStruct = new NativeWrapper.CFG_ACCESS_EVENT_INFO();
			testStruct.nCloseTimeout = 123;

			IntPtr pValue = new IntPtr();
			var size = Marshal.SizeOf(typeof(NativeWrapper.CFG_ACCESS_EVENT_INFO));
			pValue = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(testStruct, pValue, true);

			NativeWrapper.TestStruct(pValue);
		}
	}
}