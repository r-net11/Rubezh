using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace ControllerSDK
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		Int32 LoginID = 1;

		string CharArrayToString(char[] charArray)
		{
			var result = new string(charArray);
			int i = result.IndexOf('\0');
			if (i >= 0)
				result = result.Substring(0, i);
			return result;
		}

		string CharArrayToStringNoTrim(char[] charArray)
		{
			var result = new string(charArray);
			//int i = result.IndexOf('\0');
			//if (i >= 0)
			//    result = result.Substring(0, i);
			return result;
		}

		void OnConnect(object sender, RoutedEventArgs e)
		{
			SDKImport.fDisConnectDelegate dCbFunc = new SDKImport.fDisConnectDelegate(OnfDisConnect);
			var result = SDKImport.CLIENT_Init(dCbFunc, (UInt32)0);

			ControllerSDK.SDKImport.NET_DEVICEINFO deviceInfo;
			int error = 0;
			LoginID = SDKImport.CLIENT_Login("172.16.2.56", (UInt16)37777, "admin", "123456", out deviceInfo, out error);

			_textBox.Text += "LoginID = " + LoginID + "\n";
		}

		void OnfDisConnect(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			if (dwUser == 0)
			{
				return;
			}
		}

		void OnDisconnect(object sender, RoutedEventArgs e)
		{
			var result = SDKImport.CLIENT_Cleanup();
		}

		void OnGetTypeAndSoft(object sender, RoutedEventArgs e)
		{
			SDKImport.WRAP_DevConfig_TypeAndSoftInfo_Result outResult;
			var result = SDKImport.WRAP_DevConfig_TypeAndSoftInfo(LoginID, out outResult);

			DeviceSoftwareInfo deviceSoftwareInfo = null;
			if (result)
			{
				deviceSoftwareInfo = new DeviceSoftwareInfo();
				deviceSoftwareInfo.SoftwareBuildDate = new DateTime(outResult.dwSoftwareBuildDate_1, outResult.dwSoftwareBuildDate_2, outResult.dwSoftwareBuildDate_3);
				deviceSoftwareInfo.DeviceType = CharArrayToString(outResult.szDevType);
				deviceSoftwareInfo.SoftwareVersion = CharArrayToString(outResult.szSoftWareVersion);
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
			SDKImport.WRAP_CFG_NETWORK_INFO_Result outResult;
			var result = SDKImport.WRAP_Get_DevConfig_IPMaskGate(LoginID, out outResult);

			DeviceNetInfo deviceNetInfo = null;
			if (result)
			{
				deviceNetInfo = new DeviceNetInfo();
				deviceNetInfo.IP = CharArrayToString(outResult.szIP);
				deviceNetInfo.SubnetMask = CharArrayToString(outResult.szSubnetMask);
				deviceNetInfo.DefaultGateway = CharArrayToString(outResult.szDefGateway);
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
			var result = SDKImport.WRAP_Set_DevConfig_IPMaskGate(LoginID, "172.5.2.65", "255.255.255.0", "172.5.1.1", 1000);
		}

		void OnGetMac(object sender, RoutedEventArgs e)
		{
			SDKImport.WRAP_DevConfig_MAC_Result outResult;
			var result = SDKImport.WRAP_DevConfig_MAC(LoginID, out outResult);
			if (result)
			{
				var macAddress = CharArrayToString(outResult.szMAC);
				_textBox.Text += "macAddress = " + macAddress + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnGetMaxPageSize(object sender, RoutedEventArgs e)
		{
			SDKImport.WRAP_DevConfig_RecordFinderCaps_Result outResult;
			var result = SDKImport.WRAP_DevConfig_RecordFinderCaps(LoginID, out outResult);
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
			SDKImport.NET_TIME outResult;
			var result = SDKImport.WRAP_DevConfig_GetCurrentTime(LoginID, out outResult);
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
			var result = SDKImport.WRAP_DevConfig_SetCurrentTime(LoginID, 2014, 5, 23, 13, 14, 01);
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
			SDKImport.WRAP_Dev_QueryLogList_Result outResult;
			var result = SDKImport.WRAP_Dev_QueryLogList(LoginID, out outResult);
			List<DeviceJournalItem> deviceJournalItems = new List<DeviceJournalItem>();
			if (result)
			{
				foreach (var log in outResult.Logs)
				{
					var deviceJournalItem = new DeviceJournalItem();
					//deviceJournalItem.DateTime = new DateTime(log.stuOperateTime.year, log.stuOperateTime.month, log.stuOperateTime.day, log.stuOperateTime.hour, log.stuOperateTime.minute, log.stuOperateTime.second);
					deviceJournalItem.OperatorName = CharArrayToStringNoTrim(log.szOperator);
					deviceJournalItem.Name = CharArrayToStringNoTrim(log.szOperation);
					deviceJournalItem.Description = CharArrayToStringNoTrim(log.szDetailContext);
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
			SDKImport.CFG_ACCESS_GENERAL_INFO outResult;
			var result = SDKImport.WRAP_DevConfig_AccessGeneral(LoginID, out outResult);
			DeviceGeneralInfo deviceGeneralInfo = null;
			if (result)
			{
				_textBox.Text += "Success" + "\n";
				deviceGeneralInfo = new DeviceGeneralInfo();
				deviceGeneralInfo.OpenDoorAudioPath = CharArrayToString(outResult.szOpenDoorAudioPath);
				deviceGeneralInfo.CloseDoorAudioPath = CharArrayToString(outResult.szCloseDoorAudioPath);
				deviceGeneralInfo.InUsedAuidoPath = CharArrayToString(outResult.szInUsedAuidoPath);
				deviceGeneralInfo.PauseUsedAudioPath = CharArrayToString(outResult.szPauseUsedAudioPath);
				deviceGeneralInfo.NotClosedAudioPath = CharArrayToString(outResult.szNotClosedAudioPath);
				deviceGeneralInfo.WaitingAudioPath = CharArrayToString(outResult.szWaitingAudioPath);
				deviceGeneralInfo.UnlockReloadTime = outResult.nUnlockReloadTime;
				deviceGeneralInfo.UnlockHoldTime = outResult.nUnlockHoldTime;
				deviceGeneralInfo.IsProjectPassword = outResult.abProjectPassword;
				deviceGeneralInfo.ProjectPassword = CharArrayToString(outResult.szProjectPassword);
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnGetDevConfig_AccessControl(object sender, RoutedEventArgs e)
		{
			SDKImport.CFG_ACCESS_EVENT_INFO outResult;
			var result = SDKImport.WRAP_GetDevConfig_AccessControl(LoginID, out outResult);
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
						_textBox.Text += timeInterval.StartDateTime.ToString() + " " + timeInterval.StartDateTime.ToString() + "\n";
					}
				}
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}
	}
}