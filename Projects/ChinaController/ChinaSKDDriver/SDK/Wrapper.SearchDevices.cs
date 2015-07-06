using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using System;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public static event Action<SearchDevicesEventArgs> NewSearchDevice;

		private static int _searchHandle;

		static NativeWrapper.fSearchDevicesCBDelegate fSearchDevicesCbDelegate;

		private static DeviceType GetDeviceType(string deviceTypeStr)
		{
			DeviceType res;
			switch (deviceTypeStr.ToUpper())
			{
				case "BSC1221A":
					res = DeviceType.DahuaBsc1221A;
					break;
				case "BSC1201B":
					res = DeviceType.DahuaBsc1201B;
					break;
				case "BSC1202B":
					res = DeviceType.DahuaBsc1202B;
					break;
				default:
					res = DeviceType.Unknown;
					break;
			}
			return res;
		}

		static void OnSearchDevicesDelegate(ref NativeWrapper.DEVICE_NET_INFO_EX pDevNetInfo, IntPtr pUserData)
		{
			if (pDevNetInfo.szDeviceType.ToUpper() != "BSC")
				return;

			OnNewSearchDevice(new SearchDevicesEventArgs(new DeviceSearchInfo(
				GetDeviceType(pDevNetInfo.szDetailType),
				pDevNetInfo.szIP,
				pDevNetInfo.nPort == 0 ? 37777 : pDevNetInfo.nPort,
				pDevNetInfo.szSubmask,
				pDevNetInfo.szGateway,
				pDevNetInfo.szMac)));
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
		private static void OnNewSearchDevice(SearchDevicesEventArgs e)
		{
			var temp = NewSearchDevice;

			if (temp != null)
				temp(e);
		}
	}
}