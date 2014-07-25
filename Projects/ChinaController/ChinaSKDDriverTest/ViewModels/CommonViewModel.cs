using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

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
			GetProjectPasswordCommand = new RelayCommand(OnGetProjectPassword);
			SetProjectPasswordCommand = new RelayCommand(OnSetProjectPassword);
			GetDoorConfigurationCommand = new RelayCommand(OnGetDoorConfiguration);
			SetDoorConfigurationCommand = new RelayCommand(OnSetDoorConfiguration);
			ReBootCommand = new RelayCommand(OnReBoot);
			UpdateFirmwareCommand = new RelayCommand(OnUpdateFirmware);
			DeleteCfgFileCommand = new RelayCommand(OnDeleteCfgFile);
			TestCommand = new RelayCommand(OnTest);
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
				var text = "IP = " + deviceNetInfo.IP + "\n";
				text += "SubnetMask = " + deviceNetInfo.SubnetMask + "\n";
				text += "DefaultGateway = " + deviceNetInfo.DefaultGateway + "\n";
				text += "MTU = " + deviceNetInfo.MTU.ToString() + "\n";
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
				var deviceNetInfo = new DeviceNetInfo()
				{
					IP = "172.16.6.53",
					SubnetMask = "255.255.255.0",
					DefaultGateway = "172.5.1.1",
					MTU = 1000
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

		public RelayCommand GetProjectPasswordCommand { get; private set; }
		void OnGetProjectPassword()
		{
			var projectPassword = MainViewModel.Wrapper.GetProjectPassword();
			if (!string.IsNullOrEmpty(projectPassword))
			{
				MessageBox.Show(projectPassword);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand SetProjectPasswordCommand { get; private set; }
		void OnSetProjectPassword()
		{
			var projectPassword = "99999999";
			var result = MainViewModel.Wrapper.SetProjectPassword(projectPassword);
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
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