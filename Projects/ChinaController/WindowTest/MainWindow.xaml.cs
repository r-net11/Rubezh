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
using ChinaSKDDriverNativeApi;

namespace WindowTest
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		int LoginID;

		private void OnStart(object sender, RoutedEventArgs e)
		{
			fDisConnectDelegate = new NativeWrapper.fDisConnectDelegate(OnDisConnectDelegate);
			fHaveReConnectDelegate = new NativeWrapper.fHaveReConnectDelegate(OnfHaveReConnectDelegate);
			fMessCallBackDelegate = new NativeWrapper.fMessCallBackDelegate(OnfMessCallBackDelegate);

			NativeWrapper.CLIENT_Init(fDisConnectDelegate, 0);
			NativeWrapper.CLIENT_SetAutoReconnect(fHaveReConnectDelegate, 0);
			NativeWrapper.CLIENT_SetDVRMessCallBack(fMessCallBackDelegate, 0);

			NativeWrapper.NET_DEVICEINFO netDeviceInfo = new NativeWrapper.NET_DEVICEINFO();
			int error;
			LoginID = NativeWrapper.CLIENT_Login("172.16.6.53", 37777, "system", "123456", out netDeviceInfo, out error);
			bool bRet = NativeWrapper.CLIENT_StartListenEx(LoginID);
		}

		private void OnStop(object sender, RoutedEventArgs e)
		{
			bool bRet = NativeWrapper.CLIENT_StopListen(LoginID);
			NativeWrapper.CLIENT_Cleanup();
		}

		static NativeWrapper.fDisConnectDelegate fDisConnectDelegate;
		static NativeWrapper.fHaveReConnectDelegate fHaveReConnectDelegate;
		static NativeWrapper.fMessCallBackDelegate fMessCallBackDelegate;

		static void OnDisConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			;
		}

		static void OnfHaveReConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			;
		}

		static bool OnfMessCallBackDelegate(Int32 lCommand, Int32 lLoginID, IntPtr pBuf, UInt32 dwBufLen, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			return true;
		}
	}
}