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
			GetAccessGeneralCommand = new RelayCommand(OnGetAccessGeneral);
			GetDevConfig_AccessControlCommand = new RelayCommand(OnGetDevConfig_AccessControl);
			SetDevConfig_AccessControlCommand = new RelayCommand(OnSetDevConfig_AccessControl);
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
			return;
			var result = NativeWrapper.WRAP_Set_DevConfig_IPMaskGate(MainViewModel.Wrapper.LoginID, "172.5.2.65", "255.255.255.0", "172.5.1.1", 1000);
			if (result != null)
			{
				MessageBox.Show("Successs");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand GetMacAddressCommand { get; private set; }
		void OnGetMacAddress()
		{
			NativeWrapper.WRAP_DevConfig_MAC_Result outResult;
			var result = NativeWrapper.WRAP_DevConfig_MAC(MainViewModel.Wrapper.LoginID, out outResult);
			if (result)
			{
				var macAddress = Wrapper.CharArrayToString(outResult.szMAC);
				MessageBox.Show("macAddress = " + macAddress);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand GetMaxPageSizeCommand { get; private set; }
		void OnGetMaxPageSize()
		{
			NativeWrapper.WRAP_DevConfig_RecordFinderCaps_Result outResult;
			var result = NativeWrapper.WRAP_DevConfig_RecordFinderCaps(MainViewModel.Wrapper.LoginID, out outResult);
			if (result)
			{
				var maxPageSize = outResult.nMaxPageSize;
				MessageBox.Show("maxPageSize = " + maxPageSize);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand GetCurrentTimeCommand { get; private set; }
		void OnGetCurrentTime()
		{
			NativeWrapper.NET_TIME outResult;
			var result = NativeWrapper.WRAP_DevConfig_GetCurrentTime(MainViewModel.Wrapper.LoginID, out outResult);
			if (result)
			{
				var dateTime = new DateTime(outResult.dwYear, outResult.dwMonth, outResult.dwDay, outResult.dwHour, outResult.dwMinute, outResult.dwSecond);
				MessageBox.Show("dateTime = " + dateTime.ToString());
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand SetCurrentTimeCommand { get; private set; }
		void OnSetCurrentTime()
		{
			var result = NativeWrapper.WRAP_DevConfig_SetCurrentTime(MainViewModel.Wrapper.LoginID, 2014, 5, 23, 13, 14, 01);
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
			NativeWrapper.QUERY_DEVICE_LOG_PARAM logParam = new NativeWrapper.QUERY_DEVICE_LOG_PARAM();
			var result = NativeWrapper.WRAP_DevCtrl_GetLogCount(MainViewModel.Wrapper.LoginID, ref logParam);
			if (result > 0)
			{
				MessageBox.Show("Success " + result.ToString());
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

		public RelayCommand GetAccessGeneralCommand { get; private set; }
		void OnGetAccessGeneral()
		{
			NativeWrapper.CFG_ACCESS_GENERAL_INFO outResult;
			var result = NativeWrapper.WRAP_DevConfig_AccessGeneral(MainViewModel.Wrapper.LoginID, out outResult);
			DeviceGeneralInfo deviceGeneralInfo = null;
			if (result)
			{
				MessageBox.Show("Success");
				deviceGeneralInfo = new DeviceGeneralInfo();
				deviceGeneralInfo.OpenDoorAudioPath = Wrapper.CharArrayToString(outResult.szOpenDoorAudioPath);
				deviceGeneralInfo.CloseDoorAudioPath = Wrapper.CharArrayToString(outResult.szCloseDoorAudioPath);
				deviceGeneralInfo.InUsedAuidoPath = Wrapper.CharArrayToString(outResult.szInUsedAuidoPath);
				deviceGeneralInfo.PauseUsedAudioPath = Wrapper.CharArrayToString(outResult.szPauseUsedAudioPath);
				deviceGeneralInfo.NotClosedAudioPath = Wrapper.CharArrayToString(outResult.szNotClosedAudioPath);
				deviceGeneralInfo.WaitingAudioPath = Wrapper.CharArrayToString(outResult.szWaitingAudioPath);
				deviceGeneralInfo.UnlockReloadTime = outResult.nUnlockReloadTime;
				deviceGeneralInfo.UnlockHoldTime = outResult.nUnlockHoldTime;
				deviceGeneralInfo.IsProjectPassword = outResult.abProjectPassword;
				deviceGeneralInfo.ProjectPassword = Wrapper.CharArrayToString(outResult.szProjectPassword);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand GetDevConfig_AccessControlCommand { get; private set; }
		void OnGetDevConfig_AccessControl()
		{
			NativeWrapper.CFG_ACCESS_EVENT_INFO outResult;
			var result = NativeWrapper.WRAP_GetDevConfig_AccessControl(MainViewModel.Wrapper.LoginID, out outResult);
			if (result)
			{
				var controllerConfig = new ControllerConfig();

				var text = "";
				for (int i = 0; i < 7; i++)
				{
					var namedTimeInterval = new NamedTimeInterval();
					for (int j = 0; j < 4; j++)
					{
						var section = outResult.stuDoorTimeSection[i * 4 + j];
						var timeInterval = new TimeInterval();
						timeInterval.StartDateTime = new TimeSpan(section.stuTime.stuStartTime.dwHour, section.stuTime.stuStartTime.dwMinute, section.stuTime.stuStartTime.dwSecond);
						timeInterval.EndDateTime = new TimeSpan(section.stuTime.stuEndTime.dwHour, section.stuTime.stuEndTime.dwMinute, section.stuTime.stuEndTime.dwSecond);
						namedTimeInterval.Intervals.Add(timeInterval);
						text += timeInterval.StartDateTime.ToString() + " - " + timeInterval.EndDateTime.ToString() + " / ";
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

		public RelayCommand SetDevConfig_AccessControlCommand { get; private set; }
		void OnSetDevConfig_AccessControl()
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
			var result = NativeWrapper.WRAP_SetDevConfig_AccessControl2(MainViewModel.Wrapper.LoginID, ref info);
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
			var result = NativeWrapper.WRAP_DevCtrl_ReBoot(MainViewModel.Wrapper.LoginID);
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
			var result = NativeWrapper.WRAP_DevCtrl_DeleteCfgFile(MainViewModel.Wrapper.LoginID);
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}
	}
}