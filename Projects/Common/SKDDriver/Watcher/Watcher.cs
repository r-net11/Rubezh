using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using JournalItem = FiresecAPI.SKD.JournalItem;

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
					AddMessage(EventNameEnum.Ошибка_инициализации_мониторинга, "");
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
			bool IsIntervalsSynchroniztionFailure = false;
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
						AddFailureJournalItem(EventNameEnum.Нет_связи_с_ГК, EventDescription.Старт_мониторинга);
					else
						AddFailureJournalItem(EventNameEnum.Связь_с_ГК_восстановлена, EventDescription.Старт_мониторинга);

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
						AddFailureJournalItem(EventNameEnum.ГК_в_технологическом_режиме, EventDescription.Старт_мониторинга);
					else
						AddFailureJournalItem(EventNameEnum.ГК_в_рабочем_режиме, EventDescription.Старт_мониторинга);

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
						AddFailureJournalItem(EventNameEnum.Конфигурация_прибора_не_соответствует_конфигурации_ПК, EventDescription.Не_совпадает_хэш);
					else
						AddFailureJournalItem(EventNameEnum.Конфигурация_прибора_соответствует_конфигурации_ПК, EventDescription.Совпадает_хэш);

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
					AddFailureJournalItem(EventNameEnum.Ошибка_при_синхронизации_журнала);
				OnSKDCallbackResult(SKDCallbackResult);

				SKDCallbackResult = new SKDCallbackResult();
				GetAllStates();
				result = IsDBMissmatchDuringMonitoring || !IsConnected;
				if (IsGetStatesFailure != result)
				{
					IsGetStatesFailure = result;
					if (IsGetStatesFailure)
						AddFailureJournalItem(EventNameEnum.Ошибка_при_опросе_состояний_компонентов_ГК);
					else
						AddFailureJournalItem(EventNameEnum.Устранена_ошибка_при_опросе_состояний_компонентов_ГК);
				}
				OnSKDCallbackResult(SKDCallbackResult);

				if (IsGetStatesFailure)
				{
					if (ReturnArterWait(5000))
						return false;
					continue;
				}

				SKDCallbackResult = new SKDCallbackResult();
				result = !SyncronyzeIntervals();
				if (IsIntervalsSynchroniztionFailure != result)
				{
					IsIntervalsSynchroniztionFailure = result;
					if (IsIntervalsSynchroniztionFailure)
						AddFailureJournalItem(EventNameEnum.Ошибка_при_синхронизации_временных_интервалов);
					else
						AddFailureJournalItem(EventNameEnum.Устранена_ошибка_при_синхронизации_временных_интервалов);
				}
				OnSKDCallbackResult(SKDCallbackResult);

				if (IsIntervalsSynchroniztionFailure)
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

		bool SyncronyzeIntervals()
		{
			var bytes = new List<byte>();
			bytes.Add(11);
			var result = SKDDeviceProcessor.SendBytes(Device, bytes);
			if (result.HasError)
				return false;
			if(result.Bytes.Count < 16)
				return false;

			var localHash = SKDSynchronyzation.GetTimeIntervalsHash(SKDManager.SKDConfiguration.TimeIntervalsConfiguration);
			var remoteHash = result.Bytes;
			//if (!localHash.SequenceEqual(remoteHash))
			if (!CompreHashes(localHash, remoteHash))
			{
				return SKDSynchronyzation.WriteIntervals(Device);
			}
			return true;
		}

		bool CompreHashes(List<byte> hash1, List<byte> hash2)
		{
			for (int i = 0; i < 16; i++)
			{
				if (hash1[i] != hash2[i])
					return false;
			}
			return true;
		}

		void RunMonitoring()
		{
			if (CheckLicense())
			{
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

		void AddFailureJournalItem(EventNameEnum name, EventDescription description = EventDescription.Нет)
		{
			var journalItem = new JournalItem()
			{
				Name = name,
				Description = description,
				State = XStateClass.Unknown,
				//GKIpAddress = Device.GetGKIpAddress()
			};
			SKDDBHelper.Add(journalItem);
			SKDCallbackResult.JournalItems.Add(journalItem);
		}

		internal void AddMessage(EventNameEnum name, string userName)
		{
			var journalItem = SKDDBHelper.AddMessage(name, userName);
			SKDCallbackResult.JournalItems.Add(journalItem);
		}

		void AddJournalItem(JournalItem journalItem)
		{
			SKDDBHelper.Add(journalItem);
			SKDCallbackResult.JournalItems.Add(journalItem);
		}

		void AddJournalItems(List<JournalItem> journalItems)
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