using Infrastructure.Common.Windows;
using ResursAPI;
using ResursNetwork.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Resurs.Processor
{
	public class DeviceProcessor
	{
		AutoResetEvent StopEvent;
		Thread RunThread;

		public void Start()
		{
			if (RunThread == null)
			{
				StopEvent = new AutoResetEvent(false);
				RunThread = new Thread(OnRunThread);
				RunThread.Name = "Monitoring";
				RunThread.Start();
			}
		}

		public void Stop()
		{
			if (StopEvent != null)
			{
				StopEvent.Set();
			}
			if (RunThread != null)
			{
				RunThread.Join(TimeSpan.FromSeconds(5));
			}
			RunThread = null;
		}

		void OnRunThread()
		{
			while (true)
			{
				RunMonitoring();

				if (StopEvent != null)
				{
					if (StopEvent != null)
					{
						StopEvent.WaitOne(TimeSpan.FromMinutes(1));
					}
				}
			}
		}

		void RunMonitoring()
		{

		}

		public static bool AddToMonitoring(Device device)
		{
			try
			{
				if (device.CanMonitor)
				{
					if (device.DeviceType == DeviceType.Network)
						NetworksManager.Instance.AddNetwork(device);
					else if (device.DeviceType == DeviceType.Counter)
						NetworksManager.Instance.AddDevice(device);
				}
				return true;
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return false;
			}
		}

		public static bool DeleteFromMonitoring(Device device)
		{
			try
			{
				if (device.CanMonitor)
				{
					if (device.DeviceType == DeviceType.Network)
						NetworksManager.Instance.RemoveNetwork(device.UID);
					else if (device.DeviceType == DeviceType.Counter)
						NetworksManager.Instance.RemoveDevice(device.UID);
				}
				return true;
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return false;
			}
		}

		public static bool SetStatus(Device device)
		{
			try
			{
				NetworksManager.Instance.SetSatus(device.UID, device.IsActive);
				return true;
			}
			catch(Exception e)
			{
				MessageBoxService.Show(e.Message);
				return false;
			}
		}

		public static bool WriteParameters(Device device)
		{
			return true;
		}

		public static bool WriteParameter(Guid deviceUID, string parameterName, ValueType parameterValue) 
		{ 
			return true; 
		}

		public static bool SendCommand(Guid guid, string p)
		{
			return true;
		}
	}
}