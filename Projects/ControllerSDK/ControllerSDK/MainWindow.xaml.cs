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

		char[] StringToCharArray(string str, int size)
		{
			var result = new char[size];
			var charArray = str.ToCharArray();
			for (int i = 0; i < Math.Min(charArray.Count(), size); i++)
			{
				result[i] = charArray[i];
			}
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
			SDKImport.CFG_ACCESS_EVENT_INFO info = new SDKImport.CFG_ACCESS_EVENT_INFO();
			info.stuDoorTimeSection = new SDKImport.CFG_DOOROPEN_TIMESECTION_INFO[7 * 4];
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
			var result = SDKImport.WRAP_SetDevConfig_AccessControl2(LoginID, ref info);
			if (result)
			{
				_textBox.Text += "Success" + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnGetTimeSchedule(object sender, RoutedEventArgs e)
		{
			SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO outResult;
			var result = SDKImport.WRAP_GetDevConfig_AccessTimeSchedule(LoginID, out outResult);
			if (result)
			{
				_textBox.Text += "Success" + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnSetTimeSchedule(object sender, RoutedEventArgs e)
		{
			SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO info = new SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO();
			var result = SDKImport.WRAP_SetDevConfig_AccessTimeSchedule(LoginID, ref info);
			if (result)
			{
				_textBox.Text += "Success" + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnInsertCard(object sender, RoutedEventArgs e)
		{
			SDKImport.NET_RECORDSET_ACCESS_CTL_CARD stuCard = new SDKImport.NET_RECORDSET_ACCESS_CTL_CARD();
			stuCard.bIsValid = true;
			stuCard.emStatus = SDKImport.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_NORMAL;
			stuCard.emType = SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_GENERAL;
			stuCard.nTimeSectionNum = 1;
			stuCard.nUserTime = 10;

			stuCard.stuCreateTime.dwYear = 2013;
			stuCard.stuCreateTime.dwMonth = 12;
			stuCard.stuCreateTime.dwDay = 12;
			stuCard.stuCreateTime.dwHour = 11;
			stuCard.stuCreateTime.dwMinute = 30;
			stuCard.stuCreateTime.dwSecond = 30;

			stuCard.stuValidStartTime.dwYear = 2013;
			stuCard.stuValidStartTime.dwMonth = 12;
			stuCard.stuValidStartTime.dwDay = 12;
			stuCard.stuValidStartTime.dwHour = 11;
			stuCard.stuValidStartTime.dwMinute = 30;
			stuCard.stuValidStartTime.dwSecond = 30;

			stuCard.stuValidEndTime.dwYear = 2014;
			stuCard.stuValidEndTime.dwMonth = 12;
			stuCard.stuValidEndTime.dwDay = 12;
			stuCard.stuValidEndTime.dwHour = 11;
			stuCard.stuValidEndTime.dwMinute = 30;
			stuCard.stuValidEndTime.dwSecond = 30;

			var strTemp = "457";
			stuCard.szCardNo = StringToCharArray(strTemp, 32);
			stuCard.nDoorNum = 2;
			stuCard.sznDoors = new int[32];
			stuCard.sznDoors[0] = 1;
			stuCard.sznDoors[1] = 2;
			stuCard.nTimeSectionNum = 2;
			stuCard.sznTimeSectionNo = new int[32];
			stuCard.sznTimeSectionNo[0] = 1;
			stuCard.sznTimeSectionNo[1] = 2;
			strTemp = "952";
			stuCard.szPsw = StringToCharArray(strTemp, 64);
			strTemp = "741";
			stuCard.szUserID = StringToCharArray(strTemp, 32);

			var result = SDKImport.WRAP_Insert_Card(LoginID, ref stuCard);
			if (result > 0)
			{
				_textBox.Text += "Success " + result.ToString() + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnInsertPassword(object sender, RoutedEventArgs e)
		{
			SDKImport.NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = new SDKImport.NET_RECORDSET_ACCESS_CTL_PWD();
			stuAccessCtlPwd.stuCreateTime.dwYear = 2013;
			stuAccessCtlPwd.stuCreateTime.dwMonth = 12;
			stuAccessCtlPwd.stuCreateTime.dwDay = 12;
			stuAccessCtlPwd.stuCreateTime.dwHour = 11;
			stuAccessCtlPwd.stuCreateTime.dwMinute = 30;
			stuAccessCtlPwd.stuCreateTime.dwSecond = 30;
			var strTemp = "456";
			stuAccessCtlPwd.szUserID = StringToCharArray(strTemp, 32);
			strTemp = "4525";
			stuAccessCtlPwd.szDoorOpenPwd = StringToCharArray(strTemp, 64);
			strTemp = "7854";
			stuAccessCtlPwd.szAlarmPwd = StringToCharArray(strTemp, 64);
			stuAccessCtlPwd.nDoorNum = 2;
			stuAccessCtlPwd.sznDoors = new int[32];
			stuAccessCtlPwd.sznDoors[0] = 1;
			stuAccessCtlPwd.sznDoors[1] = 2;

			var result = SDKImport.WRAP_Insert_Pwd(LoginID, ref stuAccessCtlPwd);
			if (result > 0)
			{
				_textBox.Text += "Success " + result + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnInsertCardRecord(object sender, RoutedEventArgs e)
		{
			SDKImport.NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = new SDKImport.NET_RECORDSET_ACCESS_CTL_CARDREC();
			var strTemp = "75236";
			stuCardRec.szCardNo = StringToCharArray(strTemp, 32);
			strTemp = "1566";
			stuCardRec.szPwd = StringToCharArray(strTemp, 64);

			stuCardRec.stuTime.dwYear = 2013;
			stuCardRec.stuTime.dwMonth = 12;
			stuCardRec.stuTime.dwDay = 12;
			stuCardRec.stuTime.dwHour = 11;
			stuCardRec.stuTime.dwMinute = 30;
			stuCardRec.stuTime.dwSecond = 30;

			stuCardRec.bStatus = true;
			stuCardRec.emMethod = SDKImport.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_CARD;
			stuCardRec.nDoor = 1;
			var result = SDKImport.WRAP_Insert_CardRec(LoginID, ref stuCardRec);
			if (result > 0)
			{
				_textBox.Text += "Success " + result.ToString() + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnInsertHoliday(object sender, RoutedEventArgs e)
		{
			SDKImport.NET_RECORDSET_HOLIDAY stuHoliday = new SDKImport.NET_RECORDSET_HOLIDAY();
			stuHoliday.nDoorNum = 2;
			stuHoliday.sznDoors = new int[32];
			stuHoliday.sznDoors[0] = 1;
			stuHoliday.sznDoors[1] = 2;

			stuHoliday.stuStartTime.dwYear = 2013;
			stuHoliday.stuStartTime.dwMonth = 9;
			stuHoliday.stuStartTime.dwDay = 1;
			stuHoliday.stuStartTime.dwHour = 0;
			stuHoliday.stuStartTime.dwMinute = 0;
			stuHoliday.stuStartTime.dwSecond = 0;

			stuHoliday.stuEndTime.dwYear = 2013;
			stuHoliday.stuEndTime.dwMonth = 9;
			stuHoliday.stuEndTime.dwDay = 2;
			stuHoliday.stuEndTime.dwHour = 0;
			stuHoliday.stuEndTime.dwMinute = 0;
			stuHoliday.stuEndTime.dwSecond = 0;
			stuHoliday.bEnable = true;
			var result = SDKImport.WRAP_Insert_Holiday(LoginID, ref stuHoliday);
			if (result > 0)
			{
				_textBox.Text += "Success " + result.ToString() + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnUpdateCard(object sender, RoutedEventArgs e)
		{
			SDKImport.NET_RECORDSET_ACCESS_CTL_CARD stuCard = new SDKImport.NET_RECORDSET_ACCESS_CTL_CARD();
			stuCard.bIsValid = true;
			stuCard.emStatus = SDKImport.NET_ACCESSCTLCARD_STATE.NET_ACCESSCTLCARD_STATE_NORMAL;
			stuCard.emType = SDKImport.NET_ACCESSCTLCARD_TYPE.NET_ACCESSCTLCARD_TYPE_GENERAL;
			stuCard.nTimeSectionNum = 1;
			stuCard.nUserTime = 10;

			stuCard.stuCreateTime.dwYear = 2013;
			stuCard.stuCreateTime.dwMonth = 12;
			stuCard.stuCreateTime.dwDay = 12;
			stuCard.stuCreateTime.dwHour = 11;
			stuCard.stuCreateTime.dwMinute = 30;
			stuCard.stuCreateTime.dwSecond = 30;

			stuCard.stuValidStartTime.dwYear = 2013;
			stuCard.stuValidStartTime.dwMonth = 12;
			stuCard.stuValidStartTime.dwDay = 12;
			stuCard.stuValidStartTime.dwHour = 11;
			stuCard.stuValidStartTime.dwMinute = 30;
			stuCard.stuValidStartTime.dwSecond = 30;

			stuCard.stuValidEndTime.dwYear = 2014;
			stuCard.stuValidEndTime.dwMonth = 12;
			stuCard.stuValidEndTime.dwDay = 12;
			stuCard.stuValidEndTime.dwHour = 11;
			stuCard.stuValidEndTime.dwMinute = 30;
			stuCard.stuValidEndTime.dwSecond = 30;

			var strTemp = "457";
			stuCard.szCardNo = StringToCharArray(strTemp, 32);
			stuCard.nDoorNum = 2;
			stuCard.sznDoors = new int[32];
			stuCard.sznDoors[0] = 1;
			stuCard.sznDoors[1] = 2;
			stuCard.nTimeSectionNum = 2;
			stuCard.sznTimeSectionNo = new int[32];
			stuCard.sznTimeSectionNo[0] = 1;
			stuCard.sznTimeSectionNo[1] = 2;
			strTemp = "952";
			stuCard.szPsw = StringToCharArray(strTemp, 64);
			strTemp = "741";
			stuCard.szUserID = StringToCharArray(strTemp, 32);

			var result = SDKImport.WRAP_Update_Card(LoginID, ref stuCard);
			if (result)
			{
				_textBox.Text += "Success " + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnUpdatePassword(object sender, RoutedEventArgs e)
		{
			SDKImport.NET_RECORDSET_ACCESS_CTL_PWD stuAccessCtlPwd = new SDKImport.NET_RECORDSET_ACCESS_CTL_PWD();
			stuAccessCtlPwd.stuCreateTime.dwYear = 2013;
			stuAccessCtlPwd.stuCreateTime.dwMonth = 12;
			stuAccessCtlPwd.stuCreateTime.dwDay = 12;
			stuAccessCtlPwd.stuCreateTime.dwHour = 11;
			stuAccessCtlPwd.stuCreateTime.dwMinute = 30;
			stuAccessCtlPwd.stuCreateTime.dwSecond = 30;
			var strTemp = "456";
			stuAccessCtlPwd.szUserID = StringToCharArray(strTemp, 32);
			strTemp = "4525";
			stuAccessCtlPwd.szDoorOpenPwd = StringToCharArray(strTemp, 64);
			strTemp = "7854";
			stuAccessCtlPwd.szAlarmPwd = StringToCharArray(strTemp, 64);
			stuAccessCtlPwd.nDoorNum = 2;
			stuAccessCtlPwd.sznDoors = new int[32];
			stuAccessCtlPwd.sznDoors[0] = 1;
			stuAccessCtlPwd.sznDoors[1] = 2;

			var result = SDKImport.WRAP_Update_Pwd(LoginID, ref stuAccessCtlPwd);
			if (result)
			{
				_textBox.Text += "Success " + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnUpdateCardRecord(object sender, RoutedEventArgs e)
		{
			SDKImport.NET_RECORDSET_ACCESS_CTL_CARDREC stuCardRec = new SDKImport.NET_RECORDSET_ACCESS_CTL_CARDREC();
			var strTemp = "75236";
			stuCardRec.szCardNo = StringToCharArray(strTemp, 32);
			strTemp = "1566";
			stuCardRec.szPwd = StringToCharArray(strTemp, 64);

			stuCardRec.stuTime.dwYear = 2013;
			stuCardRec.stuTime.dwMonth = 12;
			stuCardRec.stuTime.dwDay = 12;
			stuCardRec.stuTime.dwHour = 11;
			stuCardRec.stuTime.dwMinute = 30;
			stuCardRec.stuTime.dwSecond = 30;

			stuCardRec.bStatus = true;
			stuCardRec.emMethod = SDKImport.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_CARD;
			stuCardRec.nDoor = 1;
			var result = SDKImport.WRAP_Update_CardRec(LoginID, ref stuCardRec);
			if (result)
			{
				_textBox.Text += "Success " + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnUpdateHoliday(object sender, RoutedEventArgs e)
		{
			SDKImport.NET_RECORDSET_HOLIDAY stuHoliday = new SDKImport.NET_RECORDSET_HOLIDAY();
			stuHoliday.nDoorNum = 2;
			stuHoliday.sznDoors = new int[32];
			stuHoliday.sznDoors[0] = 1;
			stuHoliday.sznDoors[1] = 2;

			stuHoliday.stuStartTime.dwYear = 2013;
			stuHoliday.stuStartTime.dwMonth = 9;
			stuHoliday.stuStartTime.dwDay = 1;
			stuHoliday.stuStartTime.dwHour = 0;
			stuHoliday.stuStartTime.dwMinute = 0;
			stuHoliday.stuStartTime.dwSecond = 0;

			stuHoliday.stuEndTime.dwYear = 2013;
			stuHoliday.stuEndTime.dwMonth = 9;
			stuHoliday.stuEndTime.dwDay = 2;
			stuHoliday.stuEndTime.dwHour = 0;
			stuHoliday.stuEndTime.dwMinute = 0;
			stuHoliday.stuEndTime.dwSecond = 0;
			stuHoliday.bEnable = true;
			var result = SDKImport.WRAP_Update_Holiday(LoginID, ref stuHoliday);
			if (result)
			{
				_textBox.Text += "Success " + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnOpenDoor(object sender, RoutedEventArgs e)
		{
			var result = SDKImport.WRAP_DevCtrl_OpenDoor(LoginID);
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
			var result = SDKImport.WRAP_DevCtrl_ReBoot(LoginID);
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
			var result = SDKImport.WRAP_DevCtrl_DeleteCfgFile(LoginID);
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
			SDKImport.QUERY_DEVICE_LOG_PARAM logParam = new SDKImport.QUERY_DEVICE_LOG_PARAM();
			var result = SDKImport.WRAP_DevCtrl_GetLogCount(LoginID, ref logParam);
			if (result > 0)
			{
				_textBox.Text += "Success " + result.ToString() + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnRemoveRecordSet(object sender, RoutedEventArgs e)
		{
			var result = SDKImport.WRAP_DevCtrl_RemoveRecordSet(LoginID, 1, 1);
			if (result)
			{
				_textBox.Text += "Success" + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}

		void OnClearRecordSet(object sender, RoutedEventArgs e)
		{
			var result = SDKImport.WRAP_DevCtrl_ClearRecordSet(LoginID, 1);
			if (result)
			{
				_textBox.Text += "Success" + "\n";
			}
			else
			{
				_textBox.Text += "Error" + "\n";
			}
		}
	}
}