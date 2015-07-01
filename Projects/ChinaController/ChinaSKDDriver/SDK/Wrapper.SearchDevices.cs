using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public static event Action<SearchDevicesEventArgs> NewSearchDevice;
		private static int _searchHandle;

		static NativeWrapper.fSearchDevicesCBDelegate fSearchDevicesCbDelegate;
		
		static void OnSearchDevicesDelegate(ref NativeWrapper.DEVICE_NET_INFO_EX pDevNetInfo, IntPtr pUserData)
		{
			if (pDevNetInfo.szDeviceType == "BSC")
				OnNewSearchDevice(new SearchDevicesEventArgs(new DeviceSearchInfo(
					pDevNetInfo.szIP,
					pDevNetInfo.nPort == 0 ? 37777 : pDevNetInfo.nPort,
					pDevNetInfo.szSubmask,
					pDevNetInfo.szGateway,
					pDevNetInfo.szMac
				)));
		}

		/// <summary>
		/// Начать асинхронный поиск IPC, NVS и пр. в локальной сети.
		/// Результаты поиска возвращаются в виде события NewDevice.
		/// </summary>
		public void StartSearchDevices()
		{
			fSearchDevicesCbDelegate = OnSearchDevicesDelegate; // Автопоиск устройств
			
			_searchHandle = NativeWrapper.CLIENT_StartSearchDevices(fSearchDevicesCbDelegate, IntPtr.Zero);
		}

		/// <summary>
		/// Закончить асинхронный поиск IPC, NVS и пр. в локальной сети, начатый функцией StartSearchDevices()
		/// </summary>
		public void StopSearchDevices()
		{
			NativeWrapper.CLIENT_StopSearchDevices(_searchHandle);
			
			fSearchDevicesCbDelegate = null;
		}

		/// <summary>
		/// Функция используется для зажигания события при обнаружении нового устройства в сети
		/// </summary>
		/// <param name="e">аргумент события</param>
		static void OnNewSearchDevice(SearchDevicesEventArgs e)
		{
			var temp = NewSearchDevice;

			if (temp != null)
				temp(e);
		}
	}
}