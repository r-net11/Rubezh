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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var xxx = SDKImport.DevConfig_TypeAndSoftInfo2(1);

			ControllerSDK.SDKImport.DHDEV_VERSION_INFO versionInfo = new ControllerSDK.SDKImport.DHDEV_VERSION_INFO();
			int pRetLen = 0;
			//var result0 = SDKImport.CLIENT_QueryDevState(1, 1, versionInfo, sizeof(versionInfo), ref pRetLen, 3000);

			string n1 = "192.16.0.1";
			char[] pchDVRIP = new char[n1.Length];
			pchDVRIP = n1.ToCharArray();

			string n2 = "admin";
			char[] pchUserName = new char[n2.Length];
			pchUserName = n1.ToCharArray();

			string n3 = "admin";
			char[] pchPassword = new char[n3.Length];
			pchPassword = n1.ToCharArray();

			ControllerSDK.SDKImport.NET_DEVICEINFO deviceInfo = new ControllerSDK.SDKImport.NET_DEVICEINFO();
			deviceInfo.sSerialNumber = new byte[48];

			int error = 0;

			var result1 = SDKImport.CLIENT_Login(pchDVRIP, (ushort)37777, pchUserName, pchPassword, deviceInfo, ref error);
			var result2 = SDKImport.CLIENT_Cleanup();
			//var result = SDKImport.DevConfig_TypeAndSoftInfo(1);
		}
	}

	public class SDKImport
	{
		[DllImport(@"EntranceGuardDemo.exe", EntryPoint = "DevConfig_TypeAndSoftInfo2")]
		unsafe public static extern bool DevConfig_TypeAndSoftInfo2(long loginID);


		public struct NET_DEVICEINFO
		{
			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
			public byte[] sSerialNumber;
			public byte byAlarmInPortNum;
			public byte byAlarmOutPortNum;
			public byte byDiskNum;
			public byte byDVRType;
			public byte byChanNum;
		}

		[DllImport(@"dhnetsdk.dll", CharSet=CharSet.Auto)]
		unsafe public static extern long CLIENT_Login(char[] pchDVRIP, ushort wDVRPort, char[] pchUserName, char[] pchPassword, NET_DEVICEINFO lpDeviceInfo, ref int error);



		public struct DHDEV_VERSION_INFO
		{
			char[] szDevSerialNo;
			char byDevType;
			char[] szDevType;
			int nProtocalVer;
			char[] szSoftWareVersion;
			ulong dwSoftwareBuildDate;
			char[] szPeripheralSoftwareVersion;
			ulong dwPeripheralSoftwareBuildDate;
			char[] szGeographySoftwareVersion;
			ulong dwGeographySoftwareBuildDate;
			char[] szHardwareVersion;
			ulong dwHardwareDate;
			char[] szWebVersion;
			ulong dwWebBuildDate;
			char[] reserved;
		}

		[DllImport(@"dhnetsdk.dll")]
		unsafe public static extern bool CLIENT_QueryDevState(long lLoginID, int nType, char* pBuf, int nBufLen, ref int pRetLen, int waittime);

		[DllImport(@"dhnetsdk.dll")]
		unsafe public static extern bool CLIENT_Cleanup();
	}
}