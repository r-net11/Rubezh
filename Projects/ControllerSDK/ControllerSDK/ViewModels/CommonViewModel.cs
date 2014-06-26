using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Windows;
using ChinaSKDDriverNativeApi;
using ChinaSKDDriver;
using ChinaSKDDriverAPI;

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
			GetLogsCountCommand = new RelayCommand(OnGetLogsCount);
			QueryLogListCommand = new RelayCommand(OnQueryLogList);
			GetProjectPasswordCommand = new RelayCommand(OnGetProjectPassword);
			SetProjectPasswordCommand = new RelayCommand(OnSetProjectPassword);
			GetDoorConfigurationCommand = new RelayCommand(OnGetDoorConfiguration);
			SetDoorConfigurationCommand = new RelayCommand(OnSetDoorConfiguration);
			ReBootCommand = new RelayCommand(OnReBoot);
			DeleteCfgFileCommand = new RelayCommand(OnDeleteCfgFile);
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
					IP = "172.5.2.65",
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

		public RelayCommand GetDoorConfigurationCommand { get; private set; }
		void OnGetDoorConfiguration()
		{
			var result = MainViewModel.Wrapper.GetDoorConfiguration();
			if (result != null)
			{
				var text = "";
				text += "ChannelName = " + result.ChannelName.ToString() + "\n";
				text += "AceessState = " + result.AceessState.ToString() + "\n";
				text += "AceessMode = " + result.AceessMode.ToString() + "\n";
				text += "EnableMode = " + result.EnableMode.ToString() + "\n";
				text += "IsSnapshotEnable = " + result.IsSnapshotEnable.ToString() + "\n";
				text += "IsDoorOpenMethod = " + result.IsDoorOpenMethod.ToString() + "\n";
				text += "IsUnlockHoldInterval = " + result.IsUnlockHoldInterval.ToString() + "\n";
				text += "IsCloseTimeout = " + result.IsCloseTimeout.ToString() + "\n";
				text += "IsOpenAlwaysTimeIndex = " + result.IsOpenAlwaysTimeIndex.ToString() + "\n";
				text += "IsHolidayTimeIndex = " + result.IsHolidayTimeIndex.ToString() + "\n";
				text += "IsBreakInAlarmEnable = " + result.IsBreakInAlarmEnable.ToString() + "\n";
				text += "IsRepeatEnterAlarmEnable = " + result.IsRepeatEnterAlarmEnable.ToString() + "\n";
				text += "IsDoorNotClosedAlarmEnable = " + result.IsDoorNotClosedAlarmEnable.ToString() + "\n";
				text += "IsDuressAlarmEnable = " + result.IsDuressAlarmEnable.ToString() + "\n";
				text += "IsDoorTimeSection = " + result.IsDoorTimeSection.ToString() + "\n";
				text += "IsSensorEnable = " + result.IsSensorEnable.ToString() + "\n";
				text += "DoorOpenMethod = " + result.DoorOpenMethod.ToString() + "\n";
				text += "UnlockHoldInterval = " + result.UnlockHoldInterval.ToString() + "\n";
				text += "CloseTimeout = " + result.CloseTimeout.ToString() + "\n";
				text += "OpenAlwaysTimeIndex = " + result.OpenAlwaysTimeIndex.ToString() + "\n";
				text += "HolidayTimeRecoNo = " + result.HolidayTimeRecoNo.ToString() + "\n";
				text += "IsBreakInAlarmEnable2 = " + result.IsBreakInAlarmEnable2.ToString() + "\n";
				text += "IsRepeatEnterAlarmEnable2 = " + result.IsRepeatEnterAlarmEnable2.ToString() + "\n";
				text += "IsDoorNotClosedAlarmEnable2 = " + result.IsDoorNotClosedAlarmEnable2.ToString() + "\n";
				text += "IsDuressAlarmEnable2 = " + result.IsDuressAlarmEnable2.ToString() + "\n";
				text += "IsSensorEnable2 = " + result.IsSensorEnable2.ToString() + "\n";

				foreach (var timeShedule in result.TimeShedules)
				{
					foreach (var timeSheduleInterval in timeShedule.TimeSheduleIntervals)
					{
						text += timeSheduleInterval.BeginHours + ":" + timeSheduleInterval.BeginMinutes + ":" + timeSheduleInterval.BeginSeconds + " - " +
						timeSheduleInterval.EndHours + ":" + timeSheduleInterval.EndMinutes + ":" + timeSheduleInterval.EndSeconds + " ||||||| ";
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
			var doorConfiguration = new DoorConfiguration();
			doorConfiguration.ChannelName = "0";
			var result = MainViewModel.Wrapper.SetDoorConfiguration(doorConfiguration);
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

		public RelayCommand GetLogsCountCommand { get; private set; }
		void OnGetLogsCount()
		{
			var result = MainViewModel.Wrapper.GetLogsCount();
			if (result > 0)
			{
				MessageBox.Show("LogsCount = " + result.ToString());
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand QueryLogListCommand { get; private set; }
		void OnQueryLogList()
		{
			NativeWrapper.WRAP_Dev_QueryLogList_Result outResult;
			var result = NativeWrapper.WRAP_QueryLogList(MainViewModel.Wrapper.LoginID, out outResult);
			List<DeviceJournalItem> deviceJournalItems = new List<DeviceJournalItem>();
			if (result)
			{
				foreach (var log in outResult.Logs)
				{
					var deviceJournalItem = new DeviceJournalItem();
					//deviceJournalItem.DateTime = new DateTime(log.stuOperateTime.year, log.stuOperateTime.month, log.stuOperateTime.day, log.stuOperateTime.hour, log.stuOperateTime.minute, log.stuOperateTime.second);
					deviceJournalItem.OperatorName = Wrapper.CharArrayToStringNoTrim(log.szOperator);
					deviceJournalItem.Name = Wrapper.CharArrayToStringNoTrim(log.szOperation);
					deviceJournalItem.Description = Wrapper.CharArrayToStringNoTrim(log.szDetailContext);
					deviceJournalItems.Add(deviceJournalItem);
				}
			}
			if (result)
			{
				var text = "";
				foreach (var deviceJournalItem in deviceJournalItems)
				{
					text += "\n";
					text += "DateTime = " + deviceJournalItem.DateTime.ToString() + "\n";
					text += "OperatorName = " + deviceJournalItem.OperatorName.ToString() + "\n";
					text += "Name = " + deviceJournalItem.Name.ToString() + "\n";
					text += "Description = " + deviceJournalItem.Description.ToString() + "\n";
				}
				MessageBox.Show(text);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}
	}
}