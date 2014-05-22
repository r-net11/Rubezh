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

		Int32 loginID = 1;

		void OnConnect(object sender, RoutedEventArgs e)
		{
			SDKImport.fDisConnectDelegate dCbFunc = new SDKImport.fDisConnectDelegate(OnfDisConnect);
			var result = SDKImport.CLIENT_Init(dCbFunc, (UInt32)0);

			ControllerSDK.SDKImport.NET_DEVICEINFO deviceInfo;
			int error = 0;
			Int32 loginID = SDKImport.CLIENT_Login("172.16.2.55", (UInt16)37777, "admin", "admin", out deviceInfo, out error);
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
			var result = SDKImport.WRAP_DevConfig_TypeAndSoftInfo(loginID, out outResult);
		}

		void OnGetIpMask(object sender, RoutedEventArgs e)
		{
			SDKImport.CFG_NETWORK_INFO outResult;
			var result = SDKImport.WRAP_Get_DevConfig_IPMaskGate(loginID, out outResult);
		}

		void OnSetIpMask(object sender, RoutedEventArgs e)
		{
			var result = SDKImport.WRAP_Set_DevConfig_IPMaskGate(loginID, "172.5.2.65", "255.255.255.0", "172.5.1.1", 1000);
		}

		void OnGetMac(object sender, RoutedEventArgs e)
		{
			SDKImport.WRAP_DevConfig_MAC_Result outResult;
			var result = SDKImport.WRAP_DevConfig_MAC(loginID, out outResult);
		}

		
	}
}