using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using ControllerSDK.ViewModels;
using ChinaSKDDriver;

namespace ControllerSDK.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			MainViewModel = new MainViewModel();
			DataContext = MainViewModel;

			//Watcher.Run();
			OnConnect(this, null);
		}

		public MainViewModel MainViewModel { get; private set; }

		void OnConnect(object sender, RoutedEventArgs e)
		{
			File.Copy(@"D:\Projects\Projects\ControllerSDK\SDK_DLL\EntranceGuardDemo\Bin\EntranceGuardDemo.dll", @"D:\Projects\Projects\ControllerSDK\ControllerSDK\bin\Debug\EntranceGuardDemo.dll", true);
			MainViewModel.Wrapper = new Wrapper();
			var loginID = MainViewModel.Wrapper.Connect("172.16.6.58", 37777, "admin", "123456");
			_textBox.Text += "LoginID = " + loginID + "\n";
		}

		void OnDisconnect(object sender, RoutedEventArgs e)
		{
			MainViewModel.Wrapper.Disconnect();
		}

		void OnGetTypeAndSoft(object sender, RoutedEventArgs e)
		{
			NativeWrapper.WRAP_DevConfig_TypeAndSoftInfo_Result outResult;
			var result = NativeWrapper.WRAP_DevConfig_TypeAndSoftInfo(MainViewModel.Wrapper.LoginID, out outResult);

			DeviceSoftwareInfo deviceSoftwareInfo = null;
			if (result)
			{
				deviceSoftwareInfo = new DeviceSoftwareInfo();
				deviceSoftwareInfo.SoftwareBuildDate = new DateTime(outResult.dwSoftwareBuildDate_1, outResult.dwSoftwareBuildDate_2, outResult.dwSoftwareBuildDate_3);
				deviceSoftwareInfo.DeviceType = Wrapper.CharArrayToString(outResult.szDevType);
				deviceSoftwareInfo.SoftwareVersion = Wrapper.CharArrayToString(outResult.szSoftWareVersion);
			}
			if (result)
			{
				_textBox.Text += "SoftwareBuildDate = " + deviceSoftwareInfo.SoftwareBuildDate.ToString() + "\n";
				_textBox.Text += "DeviceType = " + deviceSoftwareInfo.DeviceType + "\n";
				_textBox.Text += "SoftwareBuildDate = " + deviceSoftwareInfo.SoftwareBuildDate + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnGetIpMask(object sender, RoutedEventArgs e)
		{
			NativeWrapper.WRAP_CFG_NETWORK_INFO_Result outResult;
			var result = NativeWrapper.WRAP_Get_DevConfig_IPMaskGate(MainViewModel.Wrapper.LoginID, out outResult);

			DeviceNetInfo deviceNetInfo = null;
			if (result)
			{
				deviceNetInfo = new DeviceNetInfo();
				deviceNetInfo.IP = Wrapper.CharArrayToString(outResult.szIP);
				deviceNetInfo.SubnetMask = Wrapper.CharArrayToString(outResult.szSubnetMask);
				deviceNetInfo.DefaultGateway = Wrapper.CharArrayToString(outResult.szDefGateway);
				deviceNetInfo.MTU = outResult.nMTU;
			}
			if (result)
			{
				_textBox.Text += "IP = " + deviceNetInfo.IP + "\n";
				_textBox.Text += "SubnetMask = " + deviceNetInfo.SubnetMask + "\n";
				_textBox.Text += "DefaultGateway = " + deviceNetInfo.DefaultGateway + "\n";
				_textBox.Text += "MTU = " + deviceNetInfo.MTU.ToString() + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnSetIpMask(object sender, RoutedEventArgs e)
		{
			return;
			var result = NativeWrapper.WRAP_Set_DevConfig_IPMaskGate(MainViewModel.Wrapper.LoginID, "172.5.2.65", "255.255.255.0", "172.5.1.1", 1000);
		}

		void OnGetMac(object sender, RoutedEventArgs e)
		{
			NativeWrapper.WRAP_DevConfig_MAC_Result outResult;
			var result = NativeWrapper.WRAP_DevConfig_MAC(MainViewModel.Wrapper.LoginID, out outResult);
			if (result)
			{
				var macAddress = Wrapper.CharArrayToString(outResult.szMAC);
				_textBox.Text += "macAddress = " + macAddress + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnGetMaxPageSize(object sender, RoutedEventArgs e)
		{
			NativeWrapper.WRAP_DevConfig_RecordFinderCaps_Result outResult;
			var result = NativeWrapper.WRAP_DevConfig_RecordFinderCaps(MainViewModel.Wrapper.LoginID, out outResult);
			if (result)
			{
				var maxPageSize = outResult.nMaxPageSize;
				_textBox.Text += "maxPageSize = " + maxPageSize + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnGetCurrentTime(object sender, RoutedEventArgs e)
		{
			NativeWrapper.NET_TIME outResult;
			var result = NativeWrapper.WRAP_DevConfig_GetCurrentTime(MainViewModel.Wrapper.LoginID, out outResult);
			if (result)
			{
				var dateTime = new DateTime(outResult.dwYear, outResult.dwMonth, outResult.dwDay, outResult.dwHour, outResult.dwMinute, outResult.dwSecond);
				_textBox.Text += "dateTime = " + dateTime.ToString() + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnSetCurrentTime(object sender, RoutedEventArgs e)
		{
			var result = NativeWrapper.WRAP_DevConfig_SetCurrentTime(MainViewModel.Wrapper.LoginID, 2014, 5, 23, 13, 14, 01);
			if (result)
			{
				_textBox.Text += "Success" + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnQueryLogList(object sender, RoutedEventArgs e)
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
				foreach (var deviceJournalItem in deviceJournalItems)
				{
					_textBox.Text += "\n";
					_textBox.Text += "DateTime = " + deviceJournalItem.DateTime.ToString() + "\n";
					_textBox.Text += "OperatorName = " + deviceJournalItem.OperatorName.ToString() + "\n";
					_textBox.Text += "Name = " + deviceJournalItem.Name.ToString() + "\n";
					_textBox.Text += "Description = " + deviceJournalItem.Description.ToString() + "\n";
				}

			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnGetAccessGeneral(object sender, RoutedEventArgs e)
		{
			NativeWrapper.CFG_ACCESS_GENERAL_INFO outResult;
			var result = NativeWrapper.WRAP_DevConfig_AccessGeneral(MainViewModel.Wrapper.LoginID, out outResult);
			DeviceGeneralInfo deviceGeneralInfo = null;
			if (result)
			{
				_textBox.Text += "Success" + "\n";
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
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnGetDevConfig_AccessControl(object sender, RoutedEventArgs e)
		{
			NativeWrapper.CFG_ACCESS_EVENT_INFO outResult;
			var result = NativeWrapper.WRAP_GetDevConfig_AccessControl(MainViewModel.Wrapper.LoginID, out outResult);
			if (result)
			{
				_textBox.Text += "Success" + "\n";


				var controllerConfig = new ControllerConfig();

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
						_textBox.Text += timeInterval.StartDateTime.ToString() + " - " + timeInterval.EndDateTime.ToString() + "\n";
					}
				}
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnSetDevConfig_AccessControl(object sender, RoutedEventArgs e)
		{
			NativeWrapper.CFG_ACCESS_EVENT_INFO info = new NativeWrapper.CFG_ACCESS_EVENT_INFO();
			info.stuDoorTimeSection = new NativeWrapper.CFG_DOOROPEN_TIMESECTION_INFO[7 * 4];
			for (int i = 0; i < info.stuDoorTimeSection.Count(); i++)
			//foreach (var timeSection in info.stuDoorTimeSection)
			{
				info.stuDoorTimeSection[i].stuTime.stuStartTime.dwHour = 1;
				info.stuDoorTimeSection[i].stuTime.stuStartTime.dwMinute = 1;
				info.stuDoorTimeSection[i].stuTime.stuStartTime.dwSecond = 1;

				info.stuDoorTimeSection[i].stuTime.stuEndTime.dwHour = 2;
				info.stuDoorTimeSection[i].stuTime.stuEndTime.dwMinute = 2;
				info.stuDoorTimeSection[i].stuTime.stuEndTime.dwSecond = 2;
			}
			var result = NativeWrapper.WRAP_SetDevConfig_AccessControl2(MainViewModel.Wrapper.LoginID, ref info);
			if (result)
			{
				_textBox.Text += "Success" + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnReBoot(object sender, RoutedEventArgs e)
		{
			var result = NativeWrapper.WRAP_DevCtrl_ReBoot(MainViewModel.Wrapper.LoginID);
			if (result)
			{
				_textBox.Text += "Success" + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnDeleteCfgFile(object sender, RoutedEventArgs e)
		{
			var result = NativeWrapper.WRAP_DevCtrl_DeleteCfgFile(MainViewModel.Wrapper.LoginID);
			if (result)
			{
				_textBox.Text += "Success" + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnGetLogsCount(object sender, RoutedEventArgs e)
		{
			NativeWrapper.QUERY_DEVICE_LOG_PARAM logParam = new NativeWrapper.QUERY_DEVICE_LOG_PARAM();
			var result = NativeWrapper.WRAP_DevCtrl_GetLogCount(MainViewModel.Wrapper.LoginID, ref logParam);
			if (result > 0)
			{
				_textBox.Text += "Success " + result.ToString() + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}
	}
}