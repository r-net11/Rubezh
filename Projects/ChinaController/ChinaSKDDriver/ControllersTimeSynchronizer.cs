using System;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI.SKD;

namespace StrazhDeviceSDK
{
	public static class ControllersTimeSynchronizer
	{
		private static Thread _thread;
		private static AutoResetEvent _stopTimeSynchronizationResetEvent = new AutoResetEvent(false);

		private static void Synchronize()
		{
			var devices = SKDManager.Devices.Where(x => x.Driver.IsController).ToList();
			foreach (var device in devices)
			{
				Synchronize(device);
			}
		}

		public static void Synchronize(SKDDevice device)
		{
			var operationResult = Processor.SynchronizeTime(device.UID);
			Logger.Info(string.Format("Контроллер '{0}'. Синхронизация времени завершилась {1}",
				device.UID,
				operationResult.HasError ? string.Format("с ошибкой : {0}", operationResult.Error) : "успешно"));
		}

		public static void Start()
		{
			if (_thread == null)
			{
				_thread = new Thread(OnRun)
				{
					Name = "ControllersTimeSynchronizer",
					IsBackground = true
				};
				_thread.Start();
			}
		}

		public static void Stop()
		{
			if (_stopTimeSynchronizationResetEvent != null)
			{
				_stopTimeSynchronizationResetEvent.Set();
				if (_thread != null)
				{
					_thread.Join(TimeSpan.FromSeconds(1));
				}
			}
		}

		private static void OnRun()
		{
			Logger.Info("Служба синхронизации времени на контроллерах запущена");
			_stopTimeSynchronizationResetEvent = new AutoResetEvent(false);
			while (true)
			{
				try
				{
					if (_stopTimeSynchronizationResetEvent.WaitOne(TimeSpan.FromHours(1)))
					{
						break;
					}

					Synchronize();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Возникло исключение при выполнении 'ControllersTimeSynchronizer.OnRun'");
				}
			}
		}
	}
}