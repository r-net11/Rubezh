using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using Common;
using System.Windows.Forms;
using System.Collections;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringUSB
	{
		public Device USBDevice { get; private set; }
		public List<MonitoringPanel> MonitoringPanels { get; private set; }
		public List<Device> MonitoringNonPanels { get; private set; }
		DateTime StartTime;

		public MonitoringUSB(Device usbDevice)
		{
			USBDevice = usbDevice;
			MonitoringPanels = new List<MonitoringPanel>();
			MonitoringNonPanels = new List<Device>();

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
											MonitoringPanels.Add(new MonitoringPanel(panelDevice));
											break;
										case DriverType.IndicationBlock:
										case DriverType.PDU:
										case DriverType.PDU_PT:
										case DriverType.UOO_TL:
										case DriverType.MS_3:
										case DriverType.MS_4:
											MonitoringNonPanels.Add(panelDevice);
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
						MonitoringPanels.Add(new MonitoringPanel(usbDevice));
						break;
				}
			}
			USBManager.NewResponse += new Action<Device, Response>(UsbRunner_NewResponse);
			USBManager.UsbRemoved += new Action(monitoringPanel_ConnectionChanged);
			//foreach (var monitoringPanel in MonitoringPanels)
			//{
			//    monitoringPanel.ConnectionChanged += new Action(monitoringPanel_ConnectionChanged);
			//}
		}

		void OnRun()
		{
			try
			{
				CheckConnection();
				SetAllInitializing();
				foreach (var monitoringPanel in MonitoringPanels)
				{
					if (CheckSuspending(false))
						return;

					monitoringPanel.Initialize();
				}
				RemoveAllInitializing();
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

					foreach (var monitoringPanel in MonitoringPanels)
					{
						if (CheckSuspending(false))
							return;

						if (!monitoringPanel.IsInitialized)
							monitoringPanel.Initialize();

						if (CheckSuspending(false))
							return;

						if (monitoringPanel.IsInitialized)
						{
							monitoringPanel.ProcessMonitoring();
						}
					}

					foreach (var monitoringNonPanel in MonitoringNonPanels)
					{
						if (CheckSuspending(false))
							return;
						NonPanelStatesManager.UpdatePDUPanelState(monitoringNonPanel);
					}

					if (CheckSuspending(false))
						return;

					if (IsTimeSynchronizationNeeded)
					{
						MonitoringPanels.ForEach(x => x.SynchronizeTime());
						IsTimeSynchronizationNeeded = false;
					}

					CheckConnection();
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

		void UsbRunner_NewResponse(Device usbDevice, Response response)
		{
			try
			{
				if (USBDevice.UID == usbDevice.UID)
				{
					//Trace.WriteLine("response.Id=" + response.Id);
					foreach (var monitoringPanel in MonitoringPanels)
					{
						if (monitoringPanel.Requests.Count > 0)
							for (int i = 0; i < monitoringPanel.Requests.Count; i++)
							{
								var request = monitoringPanel.Requests[i];
								if (request != null && response.Id != 0 && request.Id == response.Id)
								{
									var idOffset = 0;
									if (response.Id > 0)
										idOffset = 4;
									for (int j = 0; j < request.RootBytes.Count; j++)
									{
										if (request.RootBytes[j] != response.Bytes[idOffset + j])
										{
											Trace.WriteLine("В пришедшем ответе совпадают ID, но не совпадают маршруты");
											return;
										}
									}
									monitoringPanel.OnResponceRecieved(request, response);
								}
							}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MonitoringUSB.UsbRunner_NewResponse");
			}
		}

		void monitoringPanel_ConnectionChanged()
		{
			CheckConnection();
		}

		void CheckConnection()
		{
			try
			{
				//if (USBDevice.Driver.DriverType == DriverType.USB_Rubezh_2AM ||
				//    USBDevice.Driver.DriverType == DriverType.USB_Rubezh_2OP ||
				//    USBDevice.Driver.DriverType == DriverType.USB_Rubezh_4A ||
				//    USBDevice.Driver.DriverType == DriverType.USB_BUNS ||
				//    USBDevice.Driver.DriverType == DriverType.USB_BUNS_2 ||
				//    USBDevice.Driver.DriverType == DriverType.USB_Rubezh_P)
				//{
				//    if (MonitoringPanels.Count == 1 && MonitoringPanels.FirstOrDefault().IsInitialized)
				//        return;
				//}

				var response = USBManager.SendShortAttempt(USBDevice, 0x01, 0x12);
				if (response.HasError)
				{
					if (USBDevice.DeviceState.IsUsbConnectionLost == false)
					{
						USBDevice.DeviceState.IsUsbConnectionLost = true;
						var deviceStatesManager = new DeviceStatesManager();
						deviceStatesManager.ForseUpdateDeviceStates(USBDevice);
						foreach (var monitoringPanel in MonitoringPanels)
						{
							monitoringPanel.OnConnectionLost();
						}
					}
					USBManager.ReInitialize(USBDevice);
					//USBManager.Initialize();
				}
				else
				{
					if (USBDevice.DeviceState.IsUsbConnectionLost == true)
					{
						USBDevice.DeviceState.IsUsbConnectionLost = false;
						var deviceStatesManager = new DeviceStatesManager();
						deviceStatesManager.ForseUpdateDeviceStates(USBDevice);
						foreach (var monitoringPanel in MonitoringPanels)
						{
							monitoringPanel.OnConnectionAppeared();
						}
					}
				}
			}
			catch (Exception e)
			{
				;
			}
		}
	}
}