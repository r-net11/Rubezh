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
				text += "SoftwareBuildDate = " + deviceSoftwareInfo.SoftwareBuildDate + "\n";
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
			NativeWrapper.CFG_ACCESS_EVENT_INFO outResult;
			var result = NativeWrapper.WRAP_GetDoorConfiguration(MainViewModel.Wrapper.LoginID, 0, out outResult);
			if (result)
			{
				var controllerConfig = new ControllerConfig();

				var text = "";
				text += "abBreakInAlarmEnable = " + outResult.abBreakInAlarmEnable.ToString() + "\n";
				text += "szChannelName = " + Wrapper.CharArrayToString(outResult.szChannelName) + "\n";
				text += "emState = " + outResult.emState.ToString() + "\n";
				text += "emMode = " + outResult.emMode.ToString() + "\n";
				text += "nEnableMode = " + outResult.nEnableMode.ToString() + "\n";
				text += "bSnapshotEnable = " + outResult.bSnapshotEnable.ToString() + "\n";
				text += "abDoorOpenMethod = " + outResult.abDoorOpenMethod.ToString() + "\n";
				text += "abUnlockHoldInterval = " + outResult.abUnlockHoldInterval.ToString() + "\n";
				text += "abCloseTimeout = " + outResult.abCloseTimeout.ToString() + "\n";
				text += "abOpenAlwaysTimeIndex = " + outResult.abOpenAlwaysTimeIndex.ToString() + "\n";
				text += "abHolidayTimeIndex = " + outResult.abHolidayTimeIndex.ToString() + "\n";
				text += "abBreakInAlarmEnable = " + outResult.abBreakInAlarmEnable.ToString() + "\n";
				text += "abRepeatEnterAlarmEnable = " + outResult.abRepeatEnterAlarmEnable.ToString() + "\n";
				text += "abDoorNotClosedAlarmEnable = " + outResult.abDoorNotClosedAlarmEnable.ToString() + "\n";
				text += "abDuressAlarmEnable = " + outResult.abDuressAlarmEnable.ToString() + "\n";
				text += "abDoorTimeSection = " + outResult.abDoorTimeSection.ToString() + "\n";
				text += "abSensorEnable = " + outResult.abSensorEnable.ToString() + "\n";
				text += "byReserved = " + outResult.byReserved.ToString() + "\n";
				text += "emDoorOpenMethod = " + outResult.emDoorOpenMethod.ToString() + "\n";
				text += "nUnlockHoldInterval = " + outResult.nUnlockHoldInterval.ToString() + "\n";
				text += "nCloseTimeout = " + outResult.nCloseTimeout.ToString() + "\n";
				text += "nOpenAlwaysTimeIndex = " + outResult.nOpenAlwaysTimeIndex.ToString() + "\n";
				text += "nHolidayTimeRecoNo = " + outResult.nHolidayTimeRecoNo.ToString() + "\n";
				text += "bBreakInAlarmEnable = " + outResult.bBreakInAlarmEnable.ToString() + "\n";
				text += "bRepeatEnterAlarm = " + outResult.bRepeatEnterAlarm.ToString() + "\n";
				text += "bDoorNotClosedAlarmEnable = " + outResult.bDoorNotClosedAlarmEnable.ToString() + "\n";
				text += "bDuressAlarmEnable = " + outResult.bDuressAlarmEnable.ToString() + "\n";
				text += "bSensorEnable = " + outResult.bSensorEnable.ToString() + "\n";


				var timeSheduleIntervals = new List<TimeSheduleInterval>();
				for (int i = 0; i < outResult.stuDoorTimeSection.Count(); i++)
				{
					var cfg_DOOROPEN_TIMESECTION_INFO = outResult.stuDoorTimeSection[i];
					var timeSheduleInterval = new TimeSheduleInterval();
					timeSheduleInterval.BeginHours = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuStartTime.dwHour;
					timeSheduleInterval.BeginMinutes = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuStartTime.dwMinute;
					timeSheduleInterval.BeginSeconds = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuStartTime.dwSecond;
					timeSheduleInterval.EndHours = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuEndTime.dwHour;
					timeSheduleInterval.EndMinutes = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuEndTime.dwMinute;
					timeSheduleInterval.EndSeconds = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuEndTime.dwSecond;
					timeSheduleIntervals.Add(timeSheduleInterval);
				}

				var timeShedules = new List<TimeShedule>();
				for (int i = 0; i < 7; i++)
				{
					var timeShedule = new TimeShedule();
					for (int j = 0; j < 4; j++)
					{
						var timeSheduleInterval = timeSheduleIntervals[i * 4 + j];
						timeShedule.TimeSheduleIntervals.Add(timeSheduleInterval);
					}
					timeShedules.Add(timeShedule);
				}

				foreach (var timeShedule in timeShedules)
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
			NativeWrapper.CFG_ACCESS_EVENT_INFO info = new NativeWrapper.CFG_ACCESS_EVENT_INFO();
			info.stuDoorTimeSection = new NativeWrapper.CFG_DOOROPEN_TIMESECTION_INFO[7 * 4];
			for (int i = 0; i < info.stuDoorTimeSection.Count(); i++)
			{
				info.stuDoorTimeSection[i].stuTime.stuStartTime.dwHour = 1;
				info.stuDoorTimeSection[i].stuTime.stuStartTime.dwMinute = i;
				info.stuDoorTimeSection[i].stuTime.stuStartTime.dwSecond = i;

				info.stuDoorTimeSection[i].stuTime.stuEndTime.dwHour = 2;
				info.stuDoorTimeSection[i].stuTime.stuEndTime.dwMinute = i * 2;
				info.stuDoorTimeSection[i].stuTime.stuEndTime.dwSecond = i * 2;
			}
			var result = NativeWrapper.WRAP_SetDoorConfiguration(MainViewModel.Wrapper.LoginID, 0, ref info);
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
			var result = MainViewModel.Wrapper.DeleteAll();
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
			var result = NativeWrapper.WRAP_Dev_QueryLogList(MainViewModel.Wrapper.LoginID, out outResult);
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