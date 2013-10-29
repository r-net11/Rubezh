using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.XModels;
using FiresecClient;
using XFiresecAPI;
using System.Runtime.Serialization;

namespace Common.GK
{
	[DataContract]
	public class InternalJournalItem
	{
		[DataMember]
		public int GKNo { get; private set; }
		[DataMember]
		string GKIpAddress;
		[DataMember]
		public JournalItemType JournalItemType { get; private set; }
		[DataMember]
		public Guid ObjectUID { get; private set; }
		[DataMember]
		string EventName;
		[DataMember]
		string UserFriendlyEventName;
		[DataMember]
		JournalYesNoType EventYesNo;
		[DataMember]
		string EventDescription;
		[DataMember]
		string UserName;
		[DataMember]
		int ObjectState;
		[DataMember]
		XStateClass StateClass;

		[DataMember]
		public ushort GKObjectNo { get; private set; }
		[DataMember]
		public int UNUSED_KAUNo { get; private set; }

		[DataMember]
		DateTime DeviceDateTime;
		[DataMember]
		DateTime SystemDateTime;

		[DataMember]
		ushort UNUSED_KAUAddress { get; set; }
		[DataMember]
		JournalSourceType Source { get; set; }
		[DataMember]
		byte Code { get; set; }

		[DataMember]
		ushort UNUSED_ObjectNo;
		[DataMember]
		public ushort ObjectDeviceType { get; private set; }
		[DataMember]
		public ushort ObjectDeviceAddress { get; private set; }
		[DataMember]
		int UNUSED_ObjectFactoryNo;

		[DataMember]
		public XDevice Device { get; private set; }
		[DataMember]
		public XZone Zone { get; private set; }
		[DataMember]
		public XDirection Direction { get; private set; }

		XSubsystemType SubsytemType
		{
			get
			{
				if (JournalItemType == GK.JournalItemType.System)
					return XSubsystemType.System;
				else
					return XSubsystemType.GK;
			}
		}

		void InitializeFromObjectUID()
		{
			if (GKObjectNo != 0)
			{
				Device = XManager.Devices.FirstOrDefault(x => x.GetDatabaseNo(DatabaseType.Gk) == GKObjectNo);
				if (Device != null)
				{
					JournalItemType = JournalItemType.Device;
					ObjectUID = Device.UID;
				}
				Zone = XManager.Zones.FirstOrDefault(x => x.GetDatabaseNo(DatabaseType.Gk) == GKObjectNo);
				if (Zone != null)
				{
					JournalItemType = JournalItemType.Zone;
					ObjectUID = Zone.UID;
				}
				Direction = XManager.Directions.FirstOrDefault(x => x.GetDatabaseNo(DatabaseType.Gk) == GKObjectNo);
				if (Direction != null)
				{
					JournalItemType = JournalItemType.Direction;
					ObjectUID = Direction.UID;
				}
			}
		}

		public JournalItem ToJournalItem()
		{
			var journalItem = new JournalItem()
			{
				GKIpAddress = GKIpAddress,
				GKJournalRecordNo = GKNo,
				DeviceDateTime = DeviceDateTime,
				SystemDateTime = SystemDateTime,
				ObjectUID = ObjectUID,
				Name = EventName,
				UserFriendlyName = UserFriendlyEventName,
				YesNo = EventYesNo,
				Description = EventDescription,
				ObjectState = ObjectState,
				JournalItemType = JournalItemType,
				GKObjectNo = GKObjectNo,
				UserName = UserName,
				SubsystemType = SubsytemType,
				StateClass = StateClass,
				InternalJournalItem = this
			};

			if (Source == JournalSourceType.Object)
			{
				var stateBits = XStatesHelper.StatesFromInt(ObjectState);
				var stateClasses = XStatesHelper.StateBitsToStateClasses(stateBits, false, false, false, false, false);
				journalItem.ObjectStateClass = XStatesHelper.GetMinStateClass(stateClasses);
			}
			else
			{
				journalItem.ObjectStateClass = XStateClass.Info;
			}

			return journalItem;
		}

		void InitializeDateTime(List<byte> bytes)
		{
			var day = bytes[32 + 4];
			var month = bytes[32 + 5];
			var year = bytes[32 + 6];
			var hour = bytes[32 + 7];
			var minute = bytes[32 + 8];
			var second = bytes[32 + 9];
			try
			{
				DeviceDateTime = new DateTime(2000 + year, month, day, hour, minute, second);
			}
			catch
			{
				DeviceDateTime = DateTime.MinValue;
			}
			SystemDateTime = DateTime.Now;
		}

		public InternalJournalItem(XDevice gkDevice, List<byte> bytes)
		{
			GKIpAddress = XManager.GetIpAddress(gkDevice);
			GKNo = BytesHelper.SubstructInt(bytes, 0);
			GKObjectNo = BytesHelper.SubstructShort(bytes, 4);
			UNUSED_KAUNo = BytesHelper.SubstructInt(bytes, 32);

			JournalItemType = JournalItemType.GK;
			ObjectUID = gkDevice.UID;
			InitializeFromObjectUID();

			InitializeDateTime(bytes);

			UNUSED_KAUAddress = BytesHelper.SubstructShort(bytes, 32 + 10);
			Source = (JournalSourceType)(int)(bytes[32 + 12]);
			Code = bytes[32 + 13];

			StateClass = XStateClass.No;
			switch (Source)
			{
				case JournalSourceType.Controller:
					switch (Code)
					{
						case 0:
							EventName = "Технология";
							UserFriendlyEventName = "Перевод в технологический режим";
							break;

						case 2:
							EventName = "Установка часов";
							UserFriendlyEventName = "Синхронизация времени прибора с временем ПК";
							break;

						case 3:
							EventName = "Запись информации о блоке";
							break;

						case 4:
							EventName = "Смена ПО";
							break;

						case 5:
							EventName = "Смена БД";
							break;

						case 6:
							EventName = "Работа";
							UserFriendlyEventName = "Работа шкафа";
							break;

						case 7:
							EventName = "Вход пользователя в прибор";
							EventDescription = DescriptionsHelper.ToUser(bytes[32 + 15]);
							var bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							var bytes2 = bytes.GetRange(16, 21 - 16 + 1);
							bytes1.AddRange(bytes2);
							UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItemType = JournalItemType.User;
							break;

						case 8:
							EventName = "Выход пользователя из прибора";
							EventDescription = DescriptionsHelper.ToUser(bytes[32 + 15]);
							bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							bytes2 = bytes.GetRange(48, 53 - 48 + 1);
							bytes1.AddRange(bytes2);
							UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItemType = JournalItemType.User;
							break;

						case 9:
							EventName = "Ошибка управления";
							GKObjectNo = BytesHelper.SubstructShort(bytes, 18);
							break;

						case 10:
							EventName = "Введен новый пользователь";
							JournalItemType = JournalItemType.User;
							break;

						case 11:
							EventName = "Изменена учетная информация пользователя";
							JournalItemType = JournalItemType.User;
							break;

						case 12:
							EventName = "Произведена настройка сети";
							break;

						default:
							EventName = "Неизвестный код события контроллекра";
							EventDescription = Code.ToString();
							break;
					}
					break;

				case JournalSourceType.Device:
					var unknownType = BytesHelper.SubstructShort(bytes, 32 + 14);
					var unknownAddress = BytesHelper.SubstructShort(bytes, 32 + 16);
					var presentationAddress = (unknownAddress / 256 + 1).ToString() + "." + (unknownAddress % 256).ToString();
					var driverName = unknownType.ToString();
					var driver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == unknownType);
					if (driver != null)
					{
						driverName = driver.ShortName;
					};
					var unknownDescription = "Тип: " + driverName + " Адрес: " + presentationAddress;
					switch (Code)
					{
						case 0:
							EventName = "Неизвестный тип";
							EventDescription = unknownDescription;
							break;

						case 1:
							EventName = "Устройство с таким адресом не описано при конфигурации";
							EventDescription = unknownDescription;
							break;

						default:
							EventName = "Неизвестный код события устройства";
							EventDescription = Code.ToString();
							break;
					}
					break;

				case JournalSourceType.Object:
					UNUSED_ObjectNo = BytesHelper.SubstructShort(bytes, 32 + 18);
					ObjectDeviceType = BytesHelper.SubstructShort(bytes, 32 + 20);
					ObjectDeviceAddress = BytesHelper.SubstructShort(bytes, 32 + 22);
					UNUSED_ObjectFactoryNo = BytesHelper.SubstructInt(bytes, 32 + 24);
					ObjectState = BytesHelper.SubstructInt(bytes, 32 + 28);
					switch (Code)
					{
						case 0:
							EventName = "При конфигурации описан другой тип";
							var realType = BytesHelper.SubstructShort(bytes, 32 + 14);
							var realDriverString = "Неизвестный тип " + realType.ToString();
							var realDriver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == realType);
							if (realDriver != null)
							{
								realDriverString = realDriver.ShortName;
							}
							EventDescription = "Действительный тип: " + realDriverString;
							break;
						case 1:
							EventName = "Изменился заводской номер";
							EventDescription = "Старый заводсткой номер: " + BytesHelper.SubstructInt(bytes, 32 + 14).ToString();
							break;
						case 2:
							EventName = "Пожар-1";
							if (JournalItemType == GK.JournalItemType.Device)
								EventName = "Сработка-1";
							EventDescription = DescriptionsHelper.ToFire(bytes[32 + 15]);
							break;

						case 3:
							EventName = "Пожар-2";
							if (JournalItemType == GK.JournalItemType.Device)
								EventName = "Сработка-2";
							EventDescription = DescriptionsHelper.ToFire(bytes[32 + 15]);
							break;

						case 4:
							EventName = "Внимание";
							break;

						case 5:
							EventName = "Неисправность";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);

							if (EventYesNo == JournalYesNoType.No)
								UserFriendlyEventName = "Неисправность устранена";

							if (ObjectDeviceType == 0xE0)
								EventDescription = DescriptionsHelper.ToBUSHFailure(bytes[32 + 15]);
							else
								EventDescription = DescriptionsHelper.ToFailure(bytes[32 + 15]);
							break;

						case 6:
							UserFriendlyEventName = EventName = "Тест";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);

							switch (bytes[32 + 15])
							{
								case 1:
									EventDescription = "Кнопка";
									UserFriendlyEventName = "Тест кнопка";
									break;

								case 2:
									EventDescription = "Указка";
									UserFriendlyEventName = "Тест лазер";
									break;
							}

							if (EventYesNo == JournalYesNoType.No)
								UserFriendlyEventName += " устранено";

							break;

						case 7:
							UserFriendlyEventName = EventName = "Запыленность";
							EventYesNo = StringHelper.ToYesNo(bytes[32 + 14]);

							switch (bytes[32 + 15])
							{
								case 1:
									EventDescription = "Предварительная";
									UserFriendlyEventName = "Предварительная запыленность";
									break;

								case 2:
									EventDescription = "Критическая";
									UserFriendlyEventName = "Критическая запыленность";
									break;
							}

							if (EventYesNo == JournalYesNoType.No)
								UserFriendlyEventName += " устранена";
							break;

						case 8:
							EventName = "Информация";
							if (ObjectDeviceType == 0xE0)
								EventDescription = DescriptionsHelper.ToBUSHInformation(bytes[32 + 15]);
							else
								EventDescription = DescriptionsHelper.ToInformation(bytes[32 + 15]);
							break;

						case 9:
							EventName = "Состояние";
							switch (bytes[32 + 15])
							{
								case 2:
									EventDescription = "Включено";
									UserFriendlyEventName = "Включено";
									StateClass = XStateClass.On;
									break;

								case 3:
									EventDescription = "Выключено";
									UserFriendlyEventName = "Выключено";
									StateClass = XStateClass.Off;
									break;

								case 4:
									EventDescription = "Включается";
									UserFriendlyEventName = "Включается";
									StateClass = XStateClass.TurningOn;
									break;

								case 5:
									EventDescription = "Выключается";
									UserFriendlyEventName = "Выключается";
									StateClass = XStateClass.TurningOff;
									break;

								case 30:
									EventDescription = "Не определено";
									UserFriendlyEventName = "Состояние не определено";
									StateClass = XStateClass.Unknown;
									break;

								case 31:
									EventDescription = "Остановлено";
									UserFriendlyEventName = "Остановлено";
									StateClass = XStateClass.Info;
									break;
							}
							break;

						case 10:
							EventName = "Режим работы";
							switch (bytes[32 + 15])
							{
								case 0:
									EventDescription = "Автоматика";
									UserFriendlyEventName = "Перевод в автоматический режим";
									StateClass = XStateClass.Norm;
									break;

								case 1:
									EventDescription = "Ручное";
									UserFriendlyEventName = "Перевод в ручной режим";
									StateClass = XStateClass.AutoOff;
									break;

								case 2:
									EventDescription = "Отключение";
									UserFriendlyEventName = "Перевод в отключенный режим";
									StateClass = XStateClass.Off;
									break;

								case 3:
									EventDescription = "Не определено";
									UserFriendlyEventName = "Перевод в неопределенный режим";
									StateClass = XStateClass.Unknown;
									break;
							}
							break;

						case 13:
							EventName = "Параметры";
							UserFriendlyEventName = "Запись параметра объекта";
							EventDescription = "Номер объекта КАУ: " + Code;
							break;

						case 14:
							EventName = "Норма";
							break;

						default:
							EventName = "Неизвестный код события объекта:";
							EventDescription = Code.ToString();
							break;
					}
					break;
			}

			if (string.IsNullOrEmpty(UserFriendlyEventName))
				UserFriendlyEventName = EventName;

			if (StateClass == XStateClass.No)
				StateClass = JournalDescriptionStateHelper.GetStateClassByName(EventName);
		}
	}

	public static class StringHelper
	{
		public static JournalYesNoType ToYesNo(byte b)
		{
			if (b == 0)
				return JournalYesNoType.No;
			if (b == 1)
				return JournalYesNoType.Yes;
			return JournalYesNoType.Unknown;
		}

		
	}
}