using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using System.Windows.Threading;
using System.Threading;

namespace DevicesModule.ViewModels
{
	public static class FS2AuParametersHelper
	{
		public static event Action<string, int> Progress;
		static void OnPropgress(string value, int percentsCompleted)
		{
			if (Progress != null)
				Progress(value, percentsCompleted);
		}

		static Thread AUParametersThread;
		static AutoResetEvent StopEvent;

		static FS2AuParametersHelper()
		{
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
			{
				StopAUParametersThread();
			};
		}

		static void StopAUParametersThread()
		{
			if (StopEvent != null)
			{
				StopEvent.Set();
			}
			if (AUParametersThread != null)
			{
				AUParametersThread.Join(TimeSpan.FromSeconds(5));
			}
			AUParametersThread = null;
		}

		public static void BeginGetAuParameters(List<Device> devices)
		{
			StopAUParametersThread();
			StopEvent = new AutoResetEvent(false);
			AUParametersThread = new Thread(() => { GetAuParameters(devices); });
			AUParametersThread.Start();
		}

		static void GetAuParameters(List<Device> devices)
		{
			string errorMessage = "";
			for (int i = 0; i < devices.Count; i++)
			{
				var device = devices[i];
				OnPropgress("Чтение параметров устройства " + device.DottedPresentationNameAndAddress, (i * 100) / devices.Count);

				var result = FiresecManager.FS2ClientContract.GetConfigurationParameters(device.UID);
				if (result != null && !result.HasError && result.Result != null)
				{
					foreach (var resultProperty in result.Result)
					{
						var property = device.DeviceAUProperties.FirstOrDefault(x => x.Name == resultProperty.Name);
						if (property == null)
						{
							property = new Property()
							{
								Name = resultProperty.Name
							};
							device.DeviceAUProperties.Add(property);
						}
						property.Value = resultProperty.Value;
					}
					device.OnAUParametersChanged();
				}
				else
				{
					errorMessage += device.DottedPresentationNameAndAddress + " ,";
				}
			}
			if (errorMessage != "")
			{
				errorMessage = "Ошибка при чтении устройств " + errorMessage;
				if (errorMessage.EndsWith(" ,"))
					errorMessage = errorMessage.Remove(errorMessage.Length - 2, 2);
				OnPropgress(errorMessage, 0);
			}
			else
			{
				OnPropgress("Готово", 0);
			}
			AUParametersThread = null;
		}

		public static void BeginSetAuParameters(List<Device> devices)
		{

		}
	}
}