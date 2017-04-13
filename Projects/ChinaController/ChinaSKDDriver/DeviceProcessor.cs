using Common;
using Infrastructure.Common;
using Localization.StrazhDeviceSDK.Common;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using StrazhDAL;
using StrazhDeviceSDK.API;
using StrazhDeviceSDK.NativeAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrazhDeviceSDK
{
	public class DeviceProcessor
	{
		int _lastJournalItemNo { get; set; }
		int _lastAlarmJournalItemNo { get; set; }

		bool _isOfflineReadOutStart { get; set; }

		List<SKDJournalItem> _onlineJournalItems { get; set; }
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
			Wrapper = new Wrapper();
			Wrapper.NewJournalItem += Wrapper_NewJournalItem;
			_onlineJournalItems = new List<SKDJournalItem>();
		}

		void OnNewJournalItem(SKDJournalItem skdJournalItem)
		{
			if (skdJournalItem.LoginID != LoginID)
				return;

			skdJournalItem.No = skdJournalItem.IsAlarm ? ++_lastAlarmJournalItemNo : skdJournalItem.IsPass ? ++_lastJournalItemNo : 0; // костыль, потому что с контроллера при онлайн проходе не приходит номер события

			var journalItem = new JournalItem
			{
				JournalItemType = skdJournalItem.JournalItemType,
				SystemDateTime = skdJournalItem.SystemDateTime,
				DeviceDateTime = skdJournalItem.DeviceDateTime,
				JournalEventNameType = skdJournalItem.JournalEventNameType,
				DescriptionText = skdJournalItem.Description,
				ErrorCode = (JournalErrorCode)skdJournalItem.ErrorCode,
				No = skdJournalItem.No
			};
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
						journalItem.ObjectName = CommonResources.NotFoundInConfig;
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
								journalItem.ObjectName = CommonResources.NotFoundInConfig;
							}
							break;
					}
					journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem(CommonResources.Direction, skdJournalItem.emEventType.ToDescription()));

					if (skdJournalItem.emOpenMethod == NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_CARD)
					{
						journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem(CommonResources.PasscardType, skdJournalItem.emCardType.ToDescription()));
						journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem(CommonResources.PasscardNumber, skdJournalItem.CardNo));
					}
					if (skdJournalItem.emOpenMethod == NativeWrapper.NET_ACCESS_DOOROPEN_METHOD.NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY)
					{
						journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem(CommonResources.Password, skdJournalItem.szPwd));
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
						journalItem.ObjectName = CommonResources.NotFoundInConfig;
					}

					if (skdJournalItem.JournalEventNameType == JournalEventNameType.Принуждение)
					{
						journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem(CommonResources.PasscardNumber, skdJournalItem.CardNo));
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
						journalItem.ObjectName = CommonResources.NotFoundInConfig;
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
						journalItem.ObjectName = CommonResources.NotFoundInConfig;
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
						journalItem.ObjectName = CommonResources.NotFoundInConfig;
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

		void Wrapper_NewJournalItem(SKDJournalItem skdJournalItem)
		{
			if (_isOfflineReadOutStart)
			{
				lock (_onlineJournalItems)
					_onlineJournalItems.Add(skdJournalItem);
			}
			else
				OnNewJournalItem(skdJournalItem);
		}

		private void OnConnectionChanged(bool isConnected, bool fireJournalItemEvent = true)
		{
			IsConnected = isConnected;
			if (fireJournalItemEvent)
			{
				var journalItem = new JournalItem
				{
					SystemDateTime = DateTime.Now,
					JournalEventNameType = isConnected ? JournalEventNameType.Восстановление_связи : JournalEventNameType.Потеря_связи,
					JournalObjectType = JournalObjectType.SKDDevice,
					ObjectUID = Device.UID,
					ObjectName = Device.Name
				};
				if (NewJournalItem != null)
					NewJournalItem(journalItem);
			}

			var connectionLostSKDStates = new SKDStates();

			foreach (var device in Device.ChildrenAndSelf)
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
							device.State.AccessState = (StrazhAPI.SKD.AccessState)doorConfiguration.AccessState;
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
				Logger.Info(String.Format("Контроллер '{0}'. Доступен по сети. Синхронизируем время.", Device.UID));
				ControllersTimeSynchronizer.Synchronize(Device);
				var deviceInfo = Wrapper.GetDeviceSoftwareInfo();
				var isCorrectFirmwareVersion = deviceInfo.SoftwareBuildDate >= SKDDeviceInfo.LastSoftwareBuildDate;

				if (isCorrectFirmwareVersion)
				{
					if (_isOfflineLogEnabled)
					{
						Task.Factory.StartNew(() =>
						{
							_isOfflineReadOutStart = true;
							Logger.Info(String.Format("Контроллер '{0}'. Запускаем задачу чтения оффлайн логов.", Device.UID));

							var offlineAccessSKDJournalItems = Wrapper.GetOfflineSKDJournalItems(_lastJournalItemNo);
							var offlineAlarmSKDJournalItems = Wrapper.GetOfflineAlarmSKDJournalItems(_lastAlarmJournalItemNo);
							(offlineAccessSKDJournalItems.Concat(offlineAlarmSKDJournalItems)).OrderBy(x => x.DeviceDateTime).ForEach(OnNewJournalItem);

							lock (_onlineJournalItems)
							{
								_onlineJournalItems.ForEach(OnNewJournalItem);
								_onlineJournalItems.Clear();
							}

							_isOfflineReadOutStart = false;
							Logger.Info(String.Format("Контроллер '{0}'. Задача чтения оффлайн логов завершилась.", Device.UID));
						});
					}
					if (ConnectionAppeared != null)
						ConnectionAppeared(this);
				}
				else
				{
					var errorSKDStates = new SKDStates();
					foreach (var device in Device.ChildrenAndSelf)
					{
						device.State.StateClass = XStateClass.Failure;
						device.State.StateClasses = new List<XStateClass> { device.State.StateClass };
						errorSKDStates.DeviceStates.Add(device.State);
						Processor.OnStatesChanged(errorSKDStates);
					}
				}
			}
		}

		public void Start()
		{
			// Загружать ли оффлайн лог в случае восстановления соединения с контроллером задается в конфигурационном файле для Сервера приложений
			_isOfflineLogEnabled = AppServerSettingsHelper.AppServerSettings.EnableOfflineLog;

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

		public void Stop()
		{
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

		public bool UpdateFirmware(string fileName)
		{
			return NativeWrapper.WRAP_Upgrade(LoginID, fileName);
		}

		private void OnStart()
		{
			using (var journalTranslator = new JournalTranslator())
			{
				var lastJournalItemNo = journalTranslator.GetLastJournalItemNoByController(Device.UID);
				var lastAlarmJournalItemNo = journalTranslator.GetLastAlarmJournalItemNoByController(Device.UID);
				while (lastAlarmJournalItemNo.HasError || lastAlarmJournalItemNo.HasError)
				{
					Logger.Error("Контроллер '{0}'. Процесс получения событий с контроллера не запущен. Не удалось получить номер последнего события и БД.", Device.Name);
					Thread.Sleep(1000);
					lastJournalItemNo = journalTranslator.GetLastJournalItemNoByController(Device.UID);
					lastAlarmJournalItemNo = journalTranslator.GetLastAlarmJournalItemNoByController(Device.UID);
				}
				_lastJournalItemNo = lastJournalItemNo.Result;
				_lastAlarmJournalItemNo = lastAlarmJournalItemNo.Result;
			}

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

		public void Reconnect()
		{
			LoginID = 0;
			Start();
		}

		void Connect()
		{
			var addresss = "";
			var port = 0;
			var login = "";
			var password = "";

			var addressProperty = Device.Properties.FirstOrDefault(x => x.Name == "Address");
			if (addressProperty != null)
			{
				addresss = addressProperty.StringValue;
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
			LoginID = Wrapper.Connect(addresss, port, login, password, out error);
			LoginFailureReason = error;
		}
	}
}