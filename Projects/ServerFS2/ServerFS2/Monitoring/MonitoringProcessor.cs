using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using Common;
using System.Windows.Forms;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringProcessor
	{
		public Device USBDevice { get; private set; }
		public List<MonitoringDevice> MonitoringPanelDevices { get; private set; }
		public List<Device> MonitoringNonPanelDevices { get; private set; }
		DateTime StartTime;
		object locker = new object();

		public MonitoringProcessor(Device usbDevice)
		{
			USBDevice = usbDevice;
			MonitoringPanelDevices = new List<MonitoringDevice>();
			MonitoringNonPanelDevices = new List<Device>();

			if (!usbDevice.IsParentMonitoringDisabled)
			{
				switch (usbDevice.Driver.DriverType)
				{
					case DriverType.MS_1:
					case DriverType.MS_2:
						foreach (var channelChild in usbDevice.Children)
						{
							foreach (var panelDevice in channelChild.Children)
							{
								if (!panelDevice.IsParentMonitoringDisabled)
								{
									switch (panelDevice.Driver.DriverType)
									{
										case DriverType.Rubezh_2AM:
										case DriverType.Rubezh_2OP:
										case DriverType.Rubezh_4A:
										case DriverType.BUNS:
										case DriverType.BUNS_2:
										case DriverType.BlindPanel:
											MonitoringPanelDevices.Add(new MonitoringDevice(panelDevice));
											break;
										case DriverType.IndicationBlock:
										case DriverType.PDU:
										case DriverType.PDU_PT:
										case DriverType.UOO_TL:
										case DriverType.MS_3:
										case DriverType.MS_4:
											MonitoringNonPanelDevices.Add(panelDevice);
											break;
									}
								}
							}
						}
						break;
					case DriverType.USB_Rubezh_2AM:
					case DriverType.USB_Rubezh_2OP:
					case DriverType.USB_Rubezh_4A:
					case DriverType.USB_Rubezh_P:
					case DriverType.USB_BUNS:
					case DriverType.USB_BUNS_2:
						MonitoringPanelDevices.Add(new MonitoringDevice(usbDevice));
						break;
				}
			}
			foreach (var usbProcessorInfo in USBManager.UsbProcessorInfos)
			{
				//usbProcessorInfo.UsbProcessor.NewResponse -= new Action<Response>(UsbRunner_NewResponse);
				usbProcessorInfo.UsbProcessor.NewResponse += new Action<Response>(UsbRunner_NewResponse);
			}
		}

		void OnRun()
		{
			try
			{
				if (USBManager.UsbProcessorInfos.Count == 0)
				{
					MessageBox.Show("USBManager.UsbProcessorInfos.Count == 0");
				}
				DeviceStatesManager.SetInitializingStateToAll();
				DeviceStatesManager.SetMonitoringDisabled();
				MonitoringPanelDevices.Where(x => !x.IsInitialized).ToList().ForEach(x => x.Initialize());
				DeviceStatesManager.RemoveInitializingFromAll();

					//Stopwatch stopwatch = new Stopwatch();
					//stopwatch.Start();
					//MonitoringDevices.FirstOrDefault().Panel.GetRealChildren().ForEach(x => DeviceStatesManager.GetDeviceCurrentState(x));
					//stopwatch.Stop();
					//Trace.WriteLine("GetDeviceCurrentState " + stopwatch.Elapsed.TotalSeconds);
			}
			catch (FS2StopMonitoringException)
			{
				return;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MonitoringProcessor.OnRun");
			}

			while (true)
			{
				try
				{
					if (CheckSuspending(false))
						return;

					foreach (var monitoringDevice in MonitoringPanelDevices)
					{
						if (CheckSuspending(false))
							return;
						monitoringDevice.CheckTasks();
					}

					MonitoringNonPanelDevices.ForEach(x => DeviceStatesManager.UpdatePDUPanelState(x));

					if (CheckSuspending(false))
						return;

					if (IsTimeSynchronizationNeeded)
					{
						MonitoringPanelDevices.ForEach(x => x.SynchronizeTime());
						IsTimeSynchronizationNeeded = false;
					}
				}
				catch (FS2StopMonitoringException)
				{
					return;
				}
				catch (Exception e)
				{
					Logger.Error(e, "MonitoringProcessor.LoopMonitoring");
				}
			}
		}

		void UsbRunner_NewResponse(Response response)
		{
			//Trace.WriteLine("response.Id=" + response.Id);
			lock (MonitoringDevice.Locker)
			{
				foreach (var monitoringDevice in MonitoringPanelDevices.ToList())
				{
					foreach (var request in monitoringDevice.Requests.ToList())
					{
						if (request != null && request.Id == response.Id)
						{
							monitoringDevice.OnResponceRecieved(request, response);
						}
					}
				}
			}
		}
	}
}