using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FiresecAPI.XModels;
using FiresecClient;
using XFiresecAPI;

namespace GKProcessor
{
	public class JournalParser
	{
		public JournalItem JournalItem { get; private set; }

		public XDevice Device { get; private set; }
		public XZone Zone { get; private set; }
		public XDirection Direction { get; private set; }
		public XPumpStation PumpStation { get; private set; }
		public XDelay Delay { get; private set; }
		public XPim Pim { get; private set; }

		public JournalParser(XDevice gkDevice, List<byte> bytes)
		{
			JournalItem = new JournalItem();
			JournalItem.SubsystemType = XSubsystemType.GK;
			JournalItem.JournalItemType = JournalItemType.GK;

			JournalItem.GKIpAddress = XManager.GetIpAddress(gkDevice);
			JournalItem.GKJournalRecordNo = BytesHelper.SubstructInt(bytes, 0);
			JournalItem.GKObjectNo = BytesHelper.SubstructShort(bytes, 4);
			var UNUSED_KAUNo = BytesHelper.SubstructInt(bytes, 32);

			InitializeFromObjectUID();
			InitializeDateTime(bytes);

			JournalItem.ControllerAddress = BytesHelper.SubstructShort(bytes, 32 + 10);
			var source = (JournalSourceType)(int)(bytes[32 + 12]);
			var code = bytes[32 + 13];

			JournalItem.StateClass = XStateClass.No;
			switch (source)
			{
				case JournalSourceType.Controller:
					switch (code)
					{
						case 0:
							JournalItem.Name = "Перевод в технологический режим";
							break;

						case 2:
							JournalItem.Name = "Синхронизация времени прибора с временем ПК";
							break;

						case 4:
							JournalItem.Name = "Смена ПО";
							break;

						case 5:
							JournalItem.Name = "Смена БД";
							break;

						case 6:
							JournalItem.Name = "Перевод в рабочий режим";
							break;

						case 7:
							JournalItem.Name = "Вход пользователя в прибор";
							JournalItem.Description = JournalStringsHelper.ToUser(bytes[32 + 15]);
							var bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							var bytes2 = bytes.GetRange(16, 21 - 16 + 1);
							bytes1.AddRange(bytes2);
							JournalItem.UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItem.JournalItemType = JournalItemType.GkUser;
							break;

						case 8:
							JournalItem.Name = "Выход пользователя из прибора";
							JournalItem.Description = JournalStringsHelper.ToUser(bytes[32 + 15]);
							bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							bytes2 = bytes.GetRange(48, 53 - 48 + 1);
							bytes1.AddRange(bytes2);
							JournalItem.UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItem.JournalItemType = JournalItemType.GkUser;
							break;

						case 9:
							JournalItem.Name = "Ошибка управления";
							JournalItem.GKObjectNo = BytesHelper.SubstructShort(bytes, 18);
							break;

						case 10:
							JournalItem.Name = "Введен новый пользователь";
							JournalItem.JournalItemType = JournalItemType.GkUser;
							break;

						case 11:
							JournalItem.Name = "Изменена учетная информация пользователя";
							JournalItem.JournalItemType = JournalItemType.GkUser;
							break;

						case 12:
							JournalItem.Name = "Произведена настройка сети";
							break;

						default:
							JournalItem.Name = "Неизвестный код события контроллекра";
							JournalItem.Description = code.ToString();
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
					switch (code)
					{
						case 0:
							JournalItem.Name = "Неизвестный тип";
							JournalItem.Description = unknownDescription;
							break;

						case 1:
							JournalItem.Name = "Устройство с таким адресом не описано при конфигурации";
							JournalItem.Description = unknownDescription;
							break;

						default:
							JournalItem.Name = "Неизвестный код события устройства";
							JournalItem.Description = code.ToString();
							break;
					}
					break;

				case JournalSourceType.Object:
					var UNUSED_ObjectNo = BytesHelper.SubstructShort(bytes, 32 + 18);
					JournalItem.DescriptorType = BytesHelper.SubstructShort(bytes, 32 + 20);
					JournalItem.DescriptorAddress = BytesHelper.SubstructShort(bytes, 32 + 22);
					var UNUSED_ObjectFactoryNo = BytesHelper.SubstructInt(bytes, 32 + 24);
					JournalItem.ObjectState = BytesHelper.SubstructInt(bytes, 32 + 28);
					switch (code)
					{
						case 0:
							JournalItem.Name = "При конфигурации описан другой тип";
							var realType = BytesHelper.SubstructShort(bytes, 32 + 14);
							var realDriverString = "Неизвестный тип " + realType.ToString();
							var realDriver = XManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == realType);
							if (realDriver != null)
							{
								realDriverString = realDriver.ShortName;
							}
							JournalItem.Description = "Действительный тип: " + realDriverString;
							break;
						case 1:
							JournalItem.Name = "Изменился заводской номер";
							JournalItem.Description = "Старый заводсткой номер: " + BytesHelper.SubstructInt(bytes, 32 + 14).ToString();
							break;
						case 2:
							JournalItem.Name = "Пожар-1";
							if (JournalItem.JournalItemType == JournalItemType.Device)
								JournalItem.Name = "Сработка-1";
							JournalItem.Description = JournalStringsHelper.ToFire(bytes[32 + 15]);
							break;

						case 3:
							JournalItem.Name = "Пожар-2";
							if (JournalItem.JournalItemType == JournalItemType.Device)
								JournalItem.Name = "Сработка-2";
							JournalItem.Description = JournalStringsHelper.ToFire(bytes[32 + 15]);
							break;

						case 4:
							JournalItem.Name = "Внимание";
							break;

						case 5:
							JournalItem.Name = "Неисправность";
							if (bytes[32 + 14] == 0)
								JournalItem.Name = "Неисправность устранена";

							switch (JournalItem.DescriptorType)
							{
								case 0xD6:
									JournalItem.Description = JournalStringsHelper.ToBatteryFailure(bytes[32 + 15]);
									break;

								default:
									JournalItem.Description = JournalStringsHelper.ToFailure(bytes[32 + 15]);
									if (bytes[32 + 15] >= 241 && bytes[32 + 15] <= 254)
									{
										JournalItem.AdditionalDescription = bytes[32 + 16].ToString() + " " + bytes[32 + 17].ToString();
									}
									break;
							}
							break;

						case 6:
							JournalItem.Name = "Тест";
							if (bytes[32 + 14] == 0)
								JournalItem.Name = "Тест устранен";

							switch (bytes[32 + 15])
							{
								case 1:
									JournalItem.Description = "Кнопка";
									break;

								case 2:
									JournalItem.Description = "Указка";
									break;
							}
							break;

						case 7:
							JournalItem.Name = "Запыленность";
							if (bytes[32 + 14] == 0)
								JournalItem.Name = "Запыленность устранена";

							switch (bytes[32 + 15])
							{
								case 1:
									JournalItem.Description = "Предварительная";
									break;

								case 2:
									JournalItem.Description = "Критическая";
									break;
							}
							break;

						case 8:
							JournalItem.Name = "Информация";
                            JournalItem.Description = JournalStringsHelper.ToInformation(bytes[32 + 15]);
							break;

						case 9:
							JournalItem.Name = JournalStringsHelper.ToState(bytes[32 + 15]);
							break;

						case 10:
							JournalItem.Name = "Режим работы";
							switch (bytes[32 + 15])
							{
								case 0:
									JournalItem.Name = "Перевод в автоматический режим";
									break;

								case 1:
									JournalItem.Name = "Перевод в ручной режим";
									break;

								case 2:
									JournalItem.Name = "Перевод в отключенный режим";
									break;

								case 3:
									JournalItem.Name = "Перевод в неопределенный режим";
									break;
							}
							break;

						case 13:
							JournalItem.Name = "Запись параметра";
							break;

						case 14:
							JournalItem.Name = "Норма";
							break;

						default:
							JournalItem.Name = "Неизвестный код события объекта:";
							JournalItem.Description = code.ToString();
							break;
					}
					break;
			}

			if (Device != null && Device.DriverType == XDriverType.Pump && JournalItem.Name == "Неисправность")
			{
				JournalItem.Description = JournalStringsHelper.GetPumpFailureMessage(JournalItem.Description, Device.IntAddress);
			}

			if (JournalItem.StateClass == XStateClass.No)
			{
				JournalItem.StateClass = JournalDescriptionStateHelper.GetStateClassByName(JournalItem.Name);
			}

			if (source == JournalSourceType.Object)
			{
				var stateBits = XStatesHelper.StatesFromInt(JournalItem.ObjectState);
				var stateClasses = XStatesHelper.StateBitsToStateClasses(stateBits);
				JournalItem.ObjectStateClass = XStatesHelper.GetMinStateClass(stateClasses);
			}
			else
			{
				JournalItem.ObjectStateClass = XStateClass.Norm;
			}
		}

		void InitializeFromObjectUID()
		{
			if (JournalItem.GKObjectNo != 0)
			{
				Device = XManager.Devices.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (Device != null)
				{
					JournalItem.JournalItemType = JournalItemType.Device;
					JournalItem.ObjectUID = Device.UID;
					JournalItem.ObjectName = Device.DottedPresentationAddress + Device.ShortName;
				}
				Zone = XManager.Zones.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (Zone != null)
				{
					JournalItem.JournalItemType = JournalItemType.Zone;
					JournalItem.ObjectUID = Zone.UID;
					JournalItem.ObjectName = Zone.PresentationName;
				}
				Direction = XManager.Directions.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (Direction != null)
				{
					JournalItem.JournalItemType = JournalItemType.Direction;
					JournalItem.ObjectUID = Direction.UID;
					JournalItem.ObjectName = Direction.PresentationName;
				}
				PumpStation = XManager.PumpStations.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (PumpStation != null)
				{
					JournalItem.JournalItemType = JournalItemType.PumpStation;
					JournalItem.ObjectUID = PumpStation.UID;
					JournalItem.ObjectName = PumpStation.PresentationName;
				}

				if (DescriptorsManager.GkDatabases != null)
				{
					foreach (var gkDatabase in DescriptorsManager.GkDatabases)
					{
						Delay = gkDatabase.Delays.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
						if (Delay != null)
							break;
					}
					foreach (var gkDatabase in DescriptorsManager.GkDatabases)
					{
						Pim = gkDatabase.Pims.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
						if (Pim != null)
							break;
					}
				}
				if (Delay != null)
				{
					JournalItem.JournalItemType = JournalItemType.Delay;
					JournalItem.ObjectUID = Delay.UID;
					JournalItem.ObjectName = Delay.Name;
				}
				if (Pim != null)
				{
					JournalItem.JournalItemType = JournalItemType.Pim;
					JournalItem.ObjectUID = Pim.UID;
					JournalItem.ObjectName = Pim.Name;
				}
			}
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
				JournalItem.DeviceDateTime = new DateTime(2000 + year, month, day, hour, minute, second);
			}
			catch
			{
				JournalItem.DeviceDateTime = DateTime.MinValue;
			}
			JournalItem.SystemDateTime = DateTime.Now;
		}
	}
}