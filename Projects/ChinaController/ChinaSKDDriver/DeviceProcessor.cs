using System.Globalization;
using System.Threading.Tasks;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infrastructure.Common;
using SKDDriver.Translators;

namespace ChinaSKDDriver
{
	public class DeviceProcessor
	{
		public Wrapper Wrapper { get; private set; }

		public SKDDevice Device { get; private set; }

		public int LoginID { get; private set; }

		public bool IsConnected { get; private set; }

		public string LoginFailureReason { get; private set; }

		private Thread Thread;
		private bool IsStopping;
		private static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public event Action<JournalItem> NewJournalItem;

		public event Action<DeviceProcessor> ConnectionAppeared;

		private bool _isOfflineLogEnabled;

		public DeviceProcessor(SKDDevice device)
		{
			Device = device;
			Wrapper = new Wrapper(Device.UID);
			Wrapper.NewJournalItem -= new Action<SKDJournalItem>(Wrapper_NewJournalItem);
			Wrapper.NewJournalItem += new Action<SKDJournalItem>(Wrapper_NewJournalItem);
		}

		private void Wrapper_NewJournalItem(SKDJournalItem skdJournalItem)
		{
			if (skdJournalItem.LoginID == LoginID)
			{
				var journalItem = new JournalItem();
				journalItem.JournalItemType = skdJournalItem.JournalItemType;
				journalItem.SystemDateTime = skdJournalItem.SystemDateTime;
				journalItem.DeviceDateTime = skdJournalItem.DeviceDateTime;
				journalItem.JournalEventNameType = skdJournalItem.JournalEventNameType;
				journalItem.DescriptionText = skdJournalItem.Description;
				journalItem.ErrorCode = (JournalErrorCode)skdJournalItem.ErrorCode;
				var cardNo = 0;
				if (Int32.TryParse(skdJournalItem.CardNo, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out cardNo))
				{
					journalItem.CardNo = cardNo;
				}

				SKDDevice device = null;

				switch (skdJournalItem.JournalEventNameType)
				{
					case JournalEventNameType.Потеря_связи:
						if (journalItem.JournalItemType == JournalItemType.Online)
							OnConnectionChanged(false);
						return;

					case JournalEventNameType.Восстановление_связи:
						if (journalItem.JournalItemType == JournalItemType.Online)
							OnConnectionChanged(true);
						return;

					case JournalEventNameType.Проход_разрешен:
					case JournalEventNameType.Проход_запрещен:
						journalItem.JournalObjectType = JournalObjectType.SKDDevice;
						device = Device.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Reader && (x.IntAddress + 1).ToString() == skdJournalItem.szReaderID);
						if (device != null)
						{
							journalItem.ObjectUID = device.UID;
							journalItem.ObjectName = device.Name;
						}
						else
						{
							journalItem.ObjectName = "Не найдено в конфигурации";
						}
						switch (skdJournalItem.emOpenMethod)
						{
							case NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_UNKNOWN:
								journalItem.JournalEventDescriptionType = JournalEventDescriptionType.Метод_открытия_Неизвестно;
								break;

							case NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY:
								journalItem.JournalEventDescriptionType = JournalEventDescriptionType.Метод_открытия_Пароль;
								break;

							case NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_CARD:
								journalItem.JournalEventDescriptionType = JournalEventDescriptionType.Метод_открытия_Карта;
								break;

							case NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST:
								journalItem.JournalEventDescriptionType = JournalEventDescriptionType.Метод_открытия_Сначала_карта;
								break;

							case NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST:
								journalItem.JournalEventDescriptionType = JournalEventDescriptionType.Метод_открытия_Сначала_пароль;
								break;

							case NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_REMOTE:
								journalItem.JournalEventDescriptionType = JournalEventDescriptionType.Метод_открытия_Удаленно;
								break;

							case NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_BUTTON:
								journalItem.JournalEventDescriptionType = JournalEventDescriptionType.Метод_открытия_Кнопка;
								var buttonDevice = Device.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Button && (x.IntAddress + 1).ToString() == skdJournalItem.szReaderID);
								if (buttonDevice != null)
								{
									journalItem.ObjectUID = buttonDevice.UID;
									journalItem.ObjectName = buttonDevice.Name;
								}
								else
								{
									journalItem.ObjectName = "Не найдено в конфигурации";
								}
								break;
						}
						journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Направление", skdJournalItem.emEventType.ToDescription()));

						if (skdJournalItem.emOpenMethod == NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_CARD)
						{
							journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Тип карты", skdJournalItem.emCardType.ToDescription()));
							journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Номер карты", skdJournalItem.CardNo));
						}
						if (skdJournalItem.emOpenMethod == NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY)
						{
							journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Пароль", skdJournalItem.szPwd));
							journalItem.CardNo = 0;
						}
						break;

					case JournalEventNameType.Дверь_не_закрыта_начало:
					case JournalEventNameType.Дверь_не_закрыта_конец:
					case JournalEventNameType.Взлом:
					case JournalEventNameType.Повторный_проход:
					case JournalEventNameType.Принуждение:
						journalItem.JournalObjectType = JournalObjectType.SKDDevice;
						device = Device.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == skdJournalItem.DoorNo);
						if (device != null)
						{
							journalItem.ObjectUID = device.UID;
							journalItem.ObjectName = device.Name;
						}
						else
						{
							journalItem.ObjectName = "Не найдено в конфигурации";
						}

						if (skdJournalItem.JournalEventNameType == JournalEventNameType.Принуждение)
						{
							journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Номер карты", skdJournalItem.CardNo));
						}
						break;

					case JournalEventNameType.Открытие_замка_двери:
					case JournalEventNameType.Закрытие_замка_двери:
						journalItem.JournalObjectType = JournalObjectType.SKDDevice;
						device = Device.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == skdJournalItem.DoorNo);
						if (device != null)
						{
							journalItem.ObjectUID = device.UID;
							journalItem.ObjectName = device.Name;
						}
						else
						{
							journalItem.ObjectName = "Не найдено в конфигурации";
						}
						break;

					case JournalEventNameType.Открытие_двери:
					case JournalEventNameType.Закрытие_двери:
					case JournalEventNameType.Неизвестный_статус_двери:
						journalItem.JournalObjectType = JournalObjectType.SKDDevice;
						device = Device.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Lock && x.IntAddress == skdJournalItem.DoorNo);
						if (device != null)
						{
							journalItem.ObjectUID = device.UID;
							journalItem.ObjectName = device.Name;

							// Меняем состояние доменной модели объектов СКД
							if (journalItem.JournalItemType == JournalItemType.Online)
							{
								device.State.StateClass = EventDescriptionAttributeHelper.ToStateClass(skdJournalItem.JournalEventNameType);
								device.State.StateClasses = new List<XStateClass>() { device.State.StateClass };
								var skdStates = new SKDStates();
								skdStates.DeviceStates.Add(device.State);
								Processor.OnStatesChanged(skdStates);
							}
						}
						else
						{
							journalItem.ObjectName = "Не найдено в конфигурации";
						}
						break;

					case JournalEventNameType.Вскрытие_контроллера_начало:
					case JournalEventNameType.Вскрытие_контроллера_конец:
						journalItem.JournalObjectType = JournalObjectType.SKDDevice;
						device = Device.Children.FirstOrDefault(x => x.DriverType == SKDDriverType.Reader && (x.IntAddress + 1).ToString() == skdJournalItem.szReaderID);
						if (device != null)
						{
							journalItem.ObjectUID = device.UID;
							journalItem.ObjectName = device.Name;
						}
						else
						{
							journalItem.ObjectName = "Не найдено в конфигурации";
						}
						break;
				}

				switch (skdJournalItem.JournalEventNameType)
				{
					case JournalEventNameType.Дверь_не_закрыта_начало:
					case JournalEventNameType.Дверь_не_закрыта_конец:
					case JournalEventNameType.Взлом:
					case JournalEventNameType.Открытие_двери:
					case JournalEventNameType.Закрытие_двери:
					case JournalEventNameType.Неизвестный_статус_двери:
						if (device != null)
						{
							// Меняем состояние доменной модели объектов СКД
							if (journalItem.JournalItemType == JournalItemType.Online)
							{
								device.State.StateClass = EventDescriptionAttributeHelper.ToStateClass(skdJournalItem.JournalEventNameType);
								device.State.StateClasses = new List<XStateClass>() { device.State.StateClass };
								var skdStates = new SKDStates();
								skdStates.DeviceStates.Add(device.State);
								Processor.OnStatesChanged(skdStates);
							}
						}
						break;
				}

				// Фиксируем GUID контроллера, событие которого регистрируем
				journalItem.ControllerUID = Device.UID;

				if (NewJournalItem != null)
					NewJournalItem(journalItem);
			}
		}

		private void OnConnectionChanged(bool isConnected, bool fireJournalItem = true)
		{
			IsConnected = isConnected;
			if (fireJournalItem)
			{
				var journalItem = new JournalItem();
				journalItem.SystemDateTime = DateTime.Now;
				journalItem.JournalEventNameType = isConnected ? JournalEventNameType.Восстановление_связи : JournalEventNameType.Потеря_связи;
				journalItem.JournalObjectType = JournalObjectType.SKDDevice;
				journalItem.ObjectUID = Device.UID;
				journalItem.ObjectName = Device.Name;
				if (NewJournalItem != null)
					NewJournalItem(journalItem);
			}

			var connectionLostSKDStates = new SKDStates();

			var allDevices = new List<SKDDevice>(Device.Children);
			allDevices.Add(Device);

			foreach (var device in allDevices)
			{
				if (isConnected)
				{
					if (device.DriverType == SKDDriverType.Lock)
					{
						var result = Wrapper.GetDoorStatus(device.IntAddress);
						switch (result)
						{
							case 0:
								device.State.StateClass = XStateClass.Unknown;
								break;

							case 1:
								device.State.StateClass = XStateClass.On;
								break;

							case 2:
								device.State.StateClass = XStateClass.Off;
								break;

							case 3:
								device.State.StateClass = XStateClass.Attention;
								break;

							default:
								device.State.StateClass = XStateClass.Unknown;
								break;
						}
						// Определяем текущую конфигурацию замка
						var doorConfiguration = Wrapper.GetDoorConfiguration(device.IntAddress);
						if (doorConfiguration != null)
						{
							// Задаем доменной модели замка текущий режим, соответствующий режиму замка на контроллере
							device.State.AccessState = (FiresecAPI.SKD.AccessState)doorConfiguration.AccessState;
						}
					}
					else
					{
						device.State.StateClass = XStateClass.Norm;
					}
				}
				else
				{
					device.State.StateClass = XStateClass.ConnectionLost;
				}
				device.State.StateClasses = new List<XStateClass>() { device.State.StateClass };
				connectionLostSKDStates.DeviceStates.Add(device.State);
			}

			Processor.OnStatesChanged(connectionLostSKDStates);

			if (IsConnected)
			{
				Logger.Info(string.Format("Контроллер '{0}'. Доступен по сети.", Device.UID));
				if (_isOfflineLogEnabled)
				{
					var getLastJournalItemTimeProducedByControllerEvent = new AutoResetEvent(false);

					Task.Factory.StartNew(() =>
					{
						Logger.Info(string.Format("Контроллер '{0}'. Запущена задача чтения оффлайн логов.", Device.UID));
						using (var journalTranslator = new JournalTranslator())
						{
							var timeOperationResult = journalTranslator.GetLastJournalItemTimeProducedByController(Device.UID);
							Logger.Info(string.Format("Контроллер '{0}'. Завершена процедура поиска в журнале событий даты последнего зарегистрированного для данного контроллера события", Device.UID));
							getLastJournalItemTimeProducedByControllerEvent.Set();
							if (!timeOperationResult.HasError)
							{
								Logger.Info(string.Format("Контроллер '{0}'. По журналу событий определена дата последнего зарегистрированного для данного контроллера события - '{1}'", Device.UID, timeOperationResult.Result));
								var offlineLogItems = Wrapper.GetOfflineLogItems(timeOperationResult.Result);
								
								Logger.Info(string.Format("Контроллер '{0}'. Передаем полученные оффлайн-события в стандартный тракт регистрации событий в системе", Device.UID));
								offlineLogItems.ForEach(Wrapper_NewJournalItem);
							}
						}
						Logger.Info(string.Format("Контроллер '{0}'. Завершилась задача чтения оффлайн логов.", Device.UID));
					});

					Logger.Info(string.Format("Контроллер '{0}'. Ожидаем завершения процедуры поиска в журнале событий даты последнего зарегистрированного для данного контроллера события", Device.UID));
					getLastJournalItemTimeProducedByControllerEvent.WaitOne();
				}

				if (ConnectionAppeared != null)
				{
					Logger.Info(string.Format("Контроллер '{0}'. Уведомляем всех подписчиков события DeviceProcessor.ConnectionAppeared", Device.UID));
					ConnectionAppeared(this);
				}
			}
		}

		/// <summary>
		/// Запускает механизм управления контроллером
		/// </summary>
		public void Start()
		{
			Logger.Info(string.Format("Контроллер '{0}'. Запускаем механизм управления.", Device.UID));

			// Загружать ли оффлайн лог в случае восстановления соединения с контроллером задается в конфигурационном файле для Сервера приложений
			_isOfflineLogEnabled = AppServerSettingsHelper.AppServerSettings.EnableOfflineLog;
			Logger.Info(string.Format("Контроллер '{0}'. Параметр конфигурации сервера 'EnableOfflineLog'='{1}'", Device.UID, _isOfflineLogEnabled));

			Device.State.StateClass = XStateClass.Unknown;
			Device.State.StateClasses = new List<XStateClass>() { Device.State.StateClass };
			foreach (var child in Device.Children)
			{
				child.State.StateClass = XStateClass.Unknown;
				child.State.StateClasses = new List<XStateClass>() { child.State.StateClass };
			}

			IsStopping = false;
			AutoResetEvent = new AutoResetEvent(false);
			Thread = new Thread(OnStart);
			Thread.Start();
		}

		/// <summary>
		/// Останавливает механизм управления контроллером
		/// </summary>
		public void Stop()
		{
			Logger.Info(string.Format("Контроллер '{0}'. Останавливаем механизм управления.", Device.UID));

			Wrapper.NewJournalItem -= new Action<SKDJournalItem>(Wrapper_NewJournalItem);
			Wrapper.Disconnect();
			IsStopping = true;
			if (AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if (Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(1));
				}
			}
		}

		private void OnStart()
		{
			var attemptCount = 0;

			while (true)
			{
				attemptCount++;
				try
				{
					Connect();
					IsConnected = LoginID > 0;
					if (IsConnected)
					{
						Thread = null;
						OnConnectionChanged(true, attemptCount > 2);
						break;
					}

					if (attemptCount == 2)
					{
						OnConnectionChanged(false);
					}

					if (IsStopping)
						return;
					if (AutoResetEvent.WaitOne(TimeSpan.FromSeconds(5)))
					{
						return;
					}
				}
				catch { }
			}
		}

		/// <summary>
		/// Перезапускает механизм управления контроллером
		/// </summary>
		public void Reconnect()
		{
			Logger.Info(string.Format("Контроллер '{0}'. Перезапускаем механизм управления.", Device.UID));
			LoginID = 0;
			Start();
		}

		public void Connect()
		{
			var address = "";
			var port = 0;
			var login = "";
			var password = "";

			var addressProperty = Device.Properties.FirstOrDefault(x => x.Name == "Address");
			if (addressProperty != null)
			{
				address = addressProperty.StringValue;
			}
			var portProperty = Device.Properties.FirstOrDefault(x => x.Name == "Port");
			if (portProperty != null)
			{
				port = portProperty.Value;
			}
			var loginProperty = Device.Properties.FirstOrDefault(x => x.Name == "Login");
			if (loginProperty != null)
			{
				login = loginProperty.StringValue;
			}
			var passwordProperty = Device.Properties.FirstOrDefault(x => x.Name == "Password");
			if (passwordProperty != null)
			{
				password = passwordProperty.StringValue;
			}
			string error;
			
			Logger.Info(string.Format("Контроллер '{0}'. Пытаемся соединиться по адресу '{1}:{2}'", Device.UID, address, port));

			LoginID = Wrapper.Connect(address, port, login, password, out error);
			LoginFailureReason = error;

			if (!string.IsNullOrEmpty(LoginFailureReason))
				Logger.Info(string.Format("Контроллер '{0}'. Попытка соединиться по адресу '{1}:{2}' не удалась по причине: {3}", Device.UID, address, port, LoginFailureReason));
		}
	}
}