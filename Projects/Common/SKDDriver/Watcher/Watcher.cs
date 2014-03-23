using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using System.Threading;
using Common;
using XFiresecAPI;
using FiresecAPI.XModels;

namespace SKDDriver
{
	public partial class Watcher
	{
		bool IsSuspending = false;
		AutoResetEvent SuspendingEvent = new AutoResetEvent(false);

		public SKDDevice Device { get; private set; }
		List<SKDDevice> AllDevices;
		bool IsStopping = false;
		AutoResetEvent StopEvent;
		Thread RunThread;
		public DateTime LastUpdateTime { get; private set; }
		SKDCallbackResult SKDCallbackResult { get; set; }
		bool IsHashFailure { get; set; }

		public Watcher(SKDDevice device)
		{
			Device = device;
			AllDevices = Device.Children.ToList();
			AllDevices.Add(Device);
		}

		public void StartThread()
		{
			IsStopping = false;
			SetDescriptorsSuspending(false);
			if (RunThread == null)
			{
				StopEvent = new AutoResetEvent(false);
				RunThread = new Thread(OnRunThread);
				RunThread.Name = "SKD Watcher " + Device.Name;
				RunThread.Start();
			}
		}

		public void StopThread()
		{
			IsStopping = true;
			if (StopEvent != null)
			{
				StopEvent.Set();
			}
			if (RunThread != null)
			{
				RunThread.Join(TimeSpan.FromSeconds(5));
			}
			RunThread = null;
			SetDescriptorsSuspending(true);
		}

		public void Suspend()
		{
			IsSuspending = true;
			if (StopEvent != null)
			{
				StopEvent.Set();
			}
			SetDescriptorsSuspending(true);
		}

		public void Resume()
		{
			IsSuspending = false;
			SuspendingEvent.Set();
			SetDescriptorsSuspending(false);
		}

		bool ReturnArterWait(int milliSeconds)
		{
			if (StopEvent != null)
			{
				StopEvent.WaitOne(TimeSpan.FromMilliseconds(milliSeconds));
			}
			WaitIfSuspending();
			return IsStopping;
		}

		void WaitIfSuspending()
		{
			if (IsSuspending)
			{
				SuspendingEvent.WaitOne(TimeSpan.FromMinutes(10));
			}
		}

		object CallbackResultLocker = new object();

		void SetDescriptorsSuspending(bool isSuspending)
		{
			lock (CallbackResultLocker)
			{
				SKDCallbackResult = new SKDCallbackResult();
				foreach (var device in AllDevices)
				{
					if (device.State != null)
					{
						device.State.IsSuspending = isSuspending;
					}
				}
				NotifyAllObjectsStateChanged();
				OnSKDCallbackResult(SKDCallbackResult);
			}
		}

		void OnRunThread()
		{
			while (true)
			{
				try
				{
					if (IsStopping)
						return;
					if (!InitializeMonitoring())
						return;
					if (IsStopping)
						return;
				}
				catch (Exception e)
				{
					AddMessage("Ошибка инициализации мониторинга", "");
					Logger.Error(e, "JournalWatcher.InitializeMonitoring");
				}

				while (true)
				{
					if (IsStopping)
						return;

					lock (CallbackResultLocker)
					{
						SKDCallbackResult = new SKDCallbackResult();
					}
					RunMonitoring();
					lock (CallbackResultLocker)
					{
						OnSKDCallbackResult(SKDCallbackResult);
					}

					if (IsStopping)
						return;

					if (IsHashFailure)
						break;

					if (StopEvent != null)
					{
						var pollInterval = 1000;
						var property = Device.Properties.FirstOrDefault(x => x.Name == "PollInterval");
						if (property != null)
						{
							pollInterval = property.Value;
						}
						if (ReturnArterWait(pollInterval))
							break;
					}

					WaitIfSuspending();
					LastUpdateTime = DateTime.Now;
				}
			}
		}

		bool InitializeMonitoring()
		{
			bool IsPingFailure = false;
			bool IsInTechnologicalRegime = false;
			bool IsGetStatesFailure = false;
			IsHashFailure = false;

			foreach (var device in AllDevices)
			{
				device.State.Clear();
			}

			while (true)
			{
				LastUpdateTime = DateTime.Now;
				SKDCallbackResult = new SKDCallbackResult();
				foreach (var device in AllDevices)
				{
					device.State.IsInitialState = true;
				}

				var deviceInfo = GetDeviceInfo();
				var result = string.IsNullOrEmpty(deviceInfo);
				if (IsPingFailure != result)
				{
					SKDCallbackResult = new SKDCallbackResult();
					IsPingFailure = result;
					if (IsPingFailure)
						AddFailureJournalItem("Нет связи с ГК", "Старт мониторинга");
					else
						AddFailureJournalItem("Связь с ГК восстановлена", "Старт_мониторинга");

					foreach (var device in AllDevices)
					{
						device.State.IsConnectionLost = IsPingFailure;
						device.State.IsInitialState = !IsPingFailure;
					}
					NotifyAllObjectsStateChanged();
					OnSKDCallbackResult(SKDCallbackResult);
				}

				if (IsPingFailure)
				{
					if (ReturnArterWait(5000))
						return false;
					continue;
				}

				result = CheckTechnologicalRegime();
				if (IsInTechnologicalRegime != result)
				{
					SKDCallbackResult = new SKDCallbackResult();
					IsInTechnologicalRegime = result;
					if (IsInTechnologicalRegime)
						AddFailureJournalItem("ГК в технологическом режиме", "Старт мониторинга");
					else
						AddFailureJournalItem("ГК в рабочем режиме", "Старт мониторинга");

					NotifyAllObjectsStateChanged();
					OnSKDCallbackResult(SKDCallbackResult);
				}

				if (IsInTechnologicalRegime)
				{
					if (ReturnArterWait(5000))
						return false;
					continue;
				}

				var hashBytes = SKDManager.CreateHash();
				var remoteHashBytes = CreateHash();
				result = !SKDManager.CompareHashes(hashBytes, remoteHashBytes);
				if (IsHashFailure != result)
				{
					SKDCallbackResult = new SKDCallbackResult();
					IsHashFailure = result;
					if (IsHashFailure)
						AddFailureJournalItem("Конфигурация прибора не соответствует конфигурации ПК", "Не совпадает хэш");
					else
						AddFailureJournalItem("Конфигурация прибора соответствует конфигурации ПК", "Совпадает хэш");

					foreach (var device in AllDevices)
					{
						device.State.IsDBMissmatch = IsHashFailure;
						device.State.IsInitialState = false;
					}
					NotifyAllObjectsStateChanged();
					OnSKDCallbackResult(SKDCallbackResult);
				}

				if (IsHashFailure)
				{
					if (ReturnArterWait(5000))
						return false;
					continue;
				}

				SKDCallbackResult = new SKDCallbackResult();
				if (!ReadMissingJournalItems())
					AddFailureJournalItem("Ошибка при синхронизации журнала");
				OnSKDCallbackResult(SKDCallbackResult);

				SKDCallbackResult = new SKDCallbackResult();
				GetAllStates();
				result = IsDBMissmatchDuringMonitoring || !IsConnected;
				if (IsGetStatesFailure != result)
				{
					IsGetStatesFailure = result;
					if (IsGetStatesFailure)
						AddFailureJournalItem("Ошибка при опросе состояний компонентов ГК");
					else
						AddFailureJournalItem("Устранена ошибка при опросе состояний компонентов ГК");
				}
				OnSKDCallbackResult(SKDCallbackResult);

				if (IsGetStatesFailure)
				{
					if (ReturnArterWait(5000))
						return false;
					continue;
				}

				SKDCallbackResult = new SKDCallbackResult();
				foreach (var device in AllDevices)
				{
					device.State.IsInitialState = false;
				}
				NotifyAllObjectsStateChanged();
				OnSKDCallbackResult(SKDCallbackResult);

				return true;
			}
		}

		void RunMonitoring()
		{
			if (CheckLicense())
			{
				//try
				//{
				//	if (!IsConnected)
				//	{
				//		return;
				//	}
				//}
				//catch (Exception e)
				//{
				//	Logger.Error(e, "Watcher.OnRunThread CheckTechnologicalRegime");
				//}

				try
				{
					CheckTasks();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Watcher.OnRunThread CheckTasks");
				}

				try
				{
					PingJournal();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Watcher.OnRunThread PingJournal");
				}
			}
		}

		void AddFailureJournalItem(string name, string description = "")
		{
			var journalItem = new SKDJournalItem()
			{
				Name = name,
				Description = description,
				StateClass = XStateClass.Unknown,
				DeviceStateClass = XStateClass.Norm,
				//GKIpAddress = Device.GetGKIpAddress()
			};
			SKDDBHelper.Add(journalItem);
			SKDCallbackResult.JournalItems.Add(journalItem);
		}

		internal void AddMessage(string name, string userName)
		{
			var journalItem = SKDDBHelper.AddMessage(name, userName);
			SKDCallbackResult.JournalItems.Add(journalItem);
		}

		void AddJournalItem(SKDJournalItem journalItem)
		{
			SKDDBHelper.Add(journalItem);
			SKDCallbackResult.JournalItems.Add(journalItem);
		}

		void AddJournalItems(List<SKDJournalItem> journalItems)
		{
			SKDDBHelper.AddMany(journalItems);
			SKDCallbackResult.JournalItems.AddRange(journalItems);
		}

		void OnSKDCallbackResult(SKDCallbackResult skdCallbackResult)
		{
			SKDProcessorManager.OnSKDCallbackResult(SKDCallbackResult);
		}

		bool IsDBMissmatchDuringMonitoring = false;
		bool CheckLicense()
		{
			return true;
		}
		void CheckTasks()
		{

		}
		
		bool CheckTechnologicalRegime()
		{
			return false;
		}
		public static List<byte> CreateHash()
		{
			return new List<byte>();
		}
		string GetDeviceInfo()
		{
			return "Device Info";
		}
		void NotifyAllObjectsStateChanged()
		{

		}
	}
}