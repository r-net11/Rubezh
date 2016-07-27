using ChinaSKDDriverNativeApi;
using StrazhDeviceSDK.API;
using System;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		public static event Action<DeviceSearchInfo> NewSearchDevice;

		private static int _searchHandle;

		static NativeWrapper.fSearchDevicesCBDelegate fSearchDevicesCbDelegate;

		private static DeviceType GetDeviceType(string deviceTypeStr)
		{
			DeviceType res;
			switch (deviceTypeStr.ToUpper())
			{
				case "BSC1221A":
				case "ASI1201A-D":
					res = DeviceType.DahuaBsc1221A;
					break;
				case "BSC1201B":
				case "ASC1201B":
					res = DeviceType.DahuaBsc1201B;
					break;
				case "BSC1202B":
				case "ASC1202B":
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

			OnNewSearchDevice(new DeviceSearchInfo(
				GetDeviceType(pDevNetInfo.szDetailType),
				pDevNetInfo.szIP,
				pDevNetInfo.nPort == 0 ? 37777 : pDevNetInfo.nPort,
				pDevNetInfo.szSubmask,
				pDevNetInfo.szGateway,
				pDevNetInfo.szMac));
		}

		/// <summary>
		/// Начать асинхронный поиск IPC, NVS и пр. в локальной сети.
		/// Результаты поиска возвращаются в виде события NewDevice.
		/// </summary>
		public static bool StartSearchDevices()
		{
			fSearchDevicesCbDelegate = OnSearchDevicesDelegate; // Автопоиск устройств

			_searchHandle = NativeWrapper.CLIENT_StartSearchDevices(fSearchDevicesCbDelegate, IntPtr.Zero);
			return _searchHandle != 0;
		}

		/// <summary>
		/// Закончить асинхронный поиск IPC, NVS и пр. в локальной сети, начатый функцией StartSearchDevices()
		/// </summary>
		public static bool StopSearchDevices()
		{
			NativeWrapper.CLIENT_StopSearchDevices(_searchHandle);

			fSearchDevicesCbDelegate = null;
			return true;
		}

		/// <summary>
		/// Функция используется для зажигания события при обнаружении нового устройства в сети
		/// </summary>
		/// <param name="e">аргумент события</param>
		static void OnNewSearchDevice(DeviceSearchInfo info)
		{
			var temp = NewSearchDevice;

			if (temp != null)
				temp(info);
		}
	}
}