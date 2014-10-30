using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public class JournalParser
	{
		public JournalItem JournalItem { get; private set; }

		public GKDevice Device { get; private set; }
		public GKZone Zone { get; private set; }
		public GKDirection Direction { get; private set; }
		public GKPumpStation PumpStation { get; private set; }
		public GKMPT MPT { get; private set; }
		public GKDelay Delay { get; private set; }
		public GKPim Pim { get; private set; }
		public GKGuardZone GuardZone { get; private set; }
		public int GKJournalRecordNo { get; private set; }
		public ushort GKObjectNo { get; private set; }
		public int ObjectState { get; private set; }

		public JournalParser(GKDevice gkControllerDevice, List<byte> bytes)
		{
			JournalItem = new JournalItem();
			JournalItem.JournalSubsystemType = JournalSubsystemType.GK;
			JournalItem.JournalObjectType = JournalObjectType.GKDevice;

			var gkIpAddress = GKManager.GetIpAddress(gkControllerDevice);
			if (!string.IsNullOrEmpty(gkIpAddress))
				JournalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("IP-адрес ГК", gkIpAddress.ToString()));

			GKJournalRecordNo = BytesHelper.SubstructInt(bytes, 0);
			if (GKJournalRecordNo > 0)
				JournalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Запись ГК", GKJournalRecordNo.ToString()));

			GKObjectNo = BytesHelper.SubstructShort(bytes, 4);
			var UNUSED_KAUNo = BytesHelper.SubstructInt(bytes, 32);

			InitializeFromObjectUID();
			InitializeDateTime(bytes);

			var ControllerAddress = BytesHelper.SubstructShort(bytes, 32 + 10);
			var source = (JournalSourceType)(int)(bytes[32 + 12]);
			var code = bytes[32 + 13];

			switch (source)
			{
				case JournalSourceType.Controller:
					switch (code)
					{
						case 0:
							JournalItem.JournalEventNameType = JournalEventNameType.Перевод_в_технологический_режим;
							break;

						case 2:
							JournalItem.JournalEventNameType = JournalEventNameType.Синхронизация_времени_прибора_с_временем_ПК;
							break;

						case 4:
							JournalItem.JournalEventNameType = JournalEventNameType.Смена_ПО;
							break;

						case 5:
							JournalItem.JournalEventNameType = JournalEventNameType.Смена_БД;
							break;

						case 6:
							JournalItem.JournalEventNameType = JournalEventNameType.Перевод_в_рабочий_режим;
							break;

						case 7:
							JournalItem.JournalEventNameType = JournalEventNameType.Вход_пользователя_в_прибор;
							JournalItem.JournalEventDescriptionType = JournalStringsHelper.ToUser(bytes[32 + 15]);
							var bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							var bytes2 = bytes.GetRange(16, 21 - 16 + 1);
							bytes1.AddRange(bytes2);
							JournalItem.UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItem.JournalObjectType = JournalObjectType.GKUser;
							break;

						case 8:
							JournalItem.JournalEventNameType = JournalEventNameType.Выход_пользователя_из_прибора;
							JournalItem.JournalEventDescriptionType = JournalStringsHelper.ToUser(bytes[32 + 15]);
							bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							bytes2 = bytes.GetRange(48, 53 - 48 + 1);
							bytes1.AddRange(bytes2);
							JournalItem.UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItem.JournalObjectType = JournalObjectType.GKUser;
							break;

						case 9:
							JournalItem.JournalEventNameType = JournalEventNameType.Ошибка_управления;
							GKObjectNo = BytesHelper.SubstructShort(bytes, 18);
							break;

						case 10:
							JournalItem.JournalEventNameType = JournalEventNameType.Введен_новый_пользователь;
							JournalItem.JournalObjectType = JournalObjectType.GKUser;
							break;

						case 11:
							JournalItem.JournalEventNameType = JournalEventNameType.Изменена_учетная_информация_пользователя;
							JournalItem.JournalObjectType = JournalObjectType.GKUser;
							break;

						case 12:
							JournalItem.JournalEventNameType = JournalEventNameType.Произведена_настройка_сети;
							break;

						default:
							JournalItem.JournalEventNameType = JournalEventNameType.Неизвестный_код_события_контроллекра;
							JournalItem.DescriptionText = code.ToString();
							break;
					}
					JournalItem.ObjectUID = gkControllerDevice.UID;
					break;

				case JournalSourceType.Device:
					var unknownType = BytesHelper.SubstructShort(bytes, 32 + 14);
					var unknownAddress = BytesHelper.SubstructShort(bytes, 32 + 16);
					var presentationAddress = (unknownAddress / 256 + 1).ToString() + "." + (unknownAddress % 256).ToString();
					var driverName = unknownType.ToString();
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == unknownType);
					if (driver != null)
					{
						driverName = driver.ShortName;
					};
					var unknownDescription = "Тип: " + driverName + " Адрес: " + presentationAddress;
					switch (code)
					{
						case 0:
							JournalItem.JournalEventNameType = JournalEventNameType.Неизвестный_тип;
							JournalItem.DescriptionText = unknownDescription;
							break;

						case 1:
							JournalItem.JournalEventNameType = JournalEventNameType.Устройство_с_таким_адресом_не_описано_при_конфигурации;
							JournalItem.DescriptionText = unknownDescription;
							break;

						default:
							JournalItem.JournalEventNameType = JournalEventNameType.Неизвестный_код_события_устройства;
							JournalItem.DescriptionText = code.ToString();
							break;
					}
					break;

				case JournalSourceType.Object:
					var UNUSED_ObjectNo = BytesHelper.SubstructShort(bytes, 32 + 18);
					var descriptorType = BytesHelper.SubstructShort(bytes, 32 + 20);
					JournalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Тип дескриптора", descriptorType.ToString()));
					var descriptorAddress = BytesHelper.SubstructShort(bytes, 32 + 22);
					JournalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Адрес дескриптора", descriptorAddress.ToString()));
					var objectFactoryNo = (uint)BytesHelper.SubstructInt(bytes, 32 + 24);
					if (objectFactoryNo > 0)
						JournalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Заводской номер", objectFactoryNo.ToString()));
					ObjectState = BytesHelper.SubstructInt(bytes, 32 + 28);
					switch (code)
					{
						case 0:
							JournalItem.JournalEventNameType = JournalEventNameType.При_конфигурации_описан_другой_тип;
							var realType = BytesHelper.SubstructShort(bytes, 32 + 14);
							var realDriverString = "Неизвестный тип " + realType.ToString();
							var realDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == realType);
							if (realDriver != null)
							{
								realDriverString = realDriver.ShortName;
							}
							JournalItem.DescriptionText = "Действительный тип: " + realDriverString;
							break;
						case 1:
							JournalItem.JournalEventNameType = JournalEventNameType.Изменился_заводской_номер;
							JournalItem.DescriptionText = "Старый заводской номер: " + BytesHelper.SubstructInt(bytes, 32 + 14).ToString();
							break;
						case 2:
							JournalItem.JournalEventNameType = JournalEventNameType.Пожар_1;
							if (JournalItem.JournalObjectType == JournalObjectType.GKDevice)
							{
								JournalItem.JournalEventNameType = JournalEventNameType.Сработка_1;
							}
							if (JournalItem.JournalObjectType == JournalObjectType.GKGuardZone)
							{
								JournalItem.JournalEventNameType = JournalEventNameType.Сработка_Охранной_Зоны;
							}
							JournalItem.JournalEventDescriptionType = JournalStringsHelper.ToFire(bytes[32 + 15]);
							break;

						case 3:
							JournalItem.JournalEventNameType = JournalEventNameType.Пожар_2;
							if (JournalItem.JournalObjectType == JournalObjectType.GKDevice)
							{
								JournalItem.JournalEventNameType = JournalEventNameType.Сработка_2;
							}
							JournalItem.JournalEventDescriptionType = JournalStringsHelper.ToFire(bytes[32 + 15]);
							break;

						case 4:
							JournalItem.JournalEventNameType = JournalEventNameType.Внимание;
							break;

						case 5:
							JournalItem.JournalEventNameType = JournalEventNameType.Неисправность;
							if (bytes[32 + 14] == 0)
							{
								JournalItem.JournalEventNameType = JournalEventNameType.Неисправность_устранена;
							}

							switch (descriptorType)
							{
								case 0xD6:
									JournalItem.JournalEventDescriptionType = JournalStringsHelper.ToBatteryFailure(bytes[32 + 15]);
									break;

								default:
									JournalItem.JournalEventDescriptionType = JournalStringsHelper.ToFailure(bytes[32 + 15]);
									if (bytes[32 + 15] >= 241 && bytes[32 + 15] <= 254)
									{
										var firstAdditionalDescription = bytes[32 + 16];
										var secondAdditionalDescription = bytes[32 + 17];
										if (firstAdditionalDescription != 0 || secondAdditionalDescription != 0)
										{
											JournalItem.DescriptionText = firstAdditionalDescription.ToString() + " " + secondAdditionalDescription.ToString();
										}
									}
									if (Device != null && Device.DriverType == GKDriverType.Valve)
									{
										JournalItem.DescriptionText = "Код уточнения " + bytes[32 + 15].ToString();
									}
									break;
							}
							break;

						case 6:
							JournalItem.JournalEventNameType = JournalEventNameType.Тест;
							if (bytes[32 + 14] == 0)
							{
								JournalItem.JournalEventNameType = JournalEventNameType.Тест_устранен;
							}

							switch (bytes[32 + 15])
							{
								case 1:
									JournalItem.JournalEventDescriptionType = JournalEventDescriptionType.Кнопка;
									break;

								case 2:
									JournalItem.JournalEventDescriptionType = JournalEventDescriptionType.Указка;
									break;
							}
							break;

						case 7:
							JournalItem.JournalEventNameType = JournalEventNameType.Запыленность;
							if (bytes[32 + 14] == 0)
							{
								JournalItem.JournalEventNameType = JournalEventNameType.Запыленность_устранена;
							}

							switch (bytes[32 + 15])
							{
								case 1:
									JournalItem.JournalEventDescriptionType = JournalEventDescriptionType.Предварительная;
									break;

								case 2:
									JournalItem.JournalEventDescriptionType = JournalEventDescriptionType.Критическая;
									break;
							}
							break;

						case 8:
							JournalItem.JournalEventNameType = JournalEventNameType.Информация;
							JournalItem.JournalEventDescriptionType = JournalStringsHelper.ToInformation(bytes[32 + 15]);
							if (Device != null && Device.DriverType == GKDriverType.Valve)
							{
								JournalItem.JournalEventDescriptionType = JournalStringsHelper.ToValveInformation(bytes[32 + 15]);
								if (JournalItem.JournalEventDescriptionType == JournalEventDescriptionType.NULL)
								{
									JournalItem.DescriptionText = "Код уточнения " + bytes[32 + 15].ToString();
								}
							}
							break;

						case 9:
							JournalItem.JournalEventNameType = JournalStringsHelper.ToState(bytes[32 + 15]);
							if (Device != null && Device.DriverType == GKDriverType.Valve)
							{
								JournalItem.JournalEventNameType = JournalStringsHelper.ToValveState(bytes[32 + 15]);
							}
							break;

						case 10:
							switch (bytes[32 + 15])
							{
								case 0:
									JournalItem.JournalEventNameType = JournalEventNameType.Перевод_в_автоматический_режим;
									break;

								case 1:
									JournalItem.JournalEventNameType = JournalEventNameType.Перевод_в_ручной_режим;
									break;

								case 2:
									JournalItem.JournalEventNameType = JournalEventNameType.Перевод_в_отключенный_режим;
									break;

								case 3:
									JournalItem.JournalEventNameType = JournalEventNameType.Перевод_в_неопределенный_режим;
									break;

								default:
									JournalItem.JournalEventNameType = JournalEventNameType.Неизвестный_код_события_объекта;
									break;
							}
							break;

						case 13:
							JournalItem.JournalEventNameType = JournalEventNameType.Запись_параметра;
							break;

						case 14:
							JournalItem.JournalEventNameType = JournalEventNameType.Норма;
							break;

						default:
							JournalItem.JournalEventNameType = JournalEventNameType.Неизвестный_код_события_объекта;
							JournalItem.DescriptionText = code.ToString();
							break;
					}
					break;
			}

			InitializeMAMessage();
		}

		void InitializeFromObjectUID()
		{
			if (GKObjectNo != 0)
			{
				JournalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Компонент ГК", GKObjectNo.ToString()));

				Device = GKManager.Devices.FirstOrDefault(x => x.GKDescriptorNo == GKObjectNo);
				if (Device != null)
				{
					JournalItem.JournalObjectType = JournalObjectType.GKDevice;
					JournalItem.ObjectUID = Device.UID;
					JournalItem.ObjectName = Device.DottedPresentationAddress + Device.ShortName;
				}
				Zone = GKManager.Zones.FirstOrDefault(x => x.GKDescriptorNo == GKObjectNo);
				if (Zone != null)
				{
					JournalItem.JournalObjectType = JournalObjectType.GKZone;
					JournalItem.ObjectUID = Zone.UID;
					JournalItem.ObjectName = Zone.PresentationName;
				}
				Direction = GKManager.Directions.FirstOrDefault(x => x.GKDescriptorNo == GKObjectNo);
				if (Direction != null)
				{
					JournalItem.JournalObjectType = JournalObjectType.GKDirection;
					JournalItem.ObjectUID = Direction.UID;
					JournalItem.ObjectName = Direction.PresentationName;
				}
				PumpStation = GKManager.PumpStations.FirstOrDefault(x => x.GKDescriptorNo == GKObjectNo);
				if (PumpStation != null)
				{
					JournalItem.JournalObjectType = JournalObjectType.GKPumpStation;
					JournalItem.ObjectUID = PumpStation.UID;
					JournalItem.ObjectName = PumpStation.PresentationName;
				}
				MPT = GKManager.MPTs.FirstOrDefault(x => x.GKDescriptorNo == GKObjectNo);
				if (MPT != null)
				{
					JournalItem.JournalObjectType = JournalObjectType.GKMPT;
					JournalItem.ObjectUID = MPT.UID;
					JournalItem.ObjectName = MPT.PresentationName;
				}
				Delay = GKManager.Delays.FirstOrDefault(x => x.GKDescriptorNo == GKObjectNo);
				if (Delay != null)
				{
					JournalItem.JournalObjectType = JournalObjectType.GKDelay;
					JournalItem.ObjectUID = Delay.UID;
					JournalItem.ObjectName = Delay.PresentationName;
				}
				else
				{
					Delay = GKManager.AutoGeneratedDelays.FirstOrDefault(x => x.GKDescriptorNo == GKObjectNo);
					if (Delay != null)
					{
						JournalItem.JournalObjectType = JournalObjectType.GKDelay;
						JournalItem.ObjectUID = Delay.UID;
						JournalItem.ObjectName = Delay.PresentationName;
					}
				}
				Pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.GKDescriptorNo == GKObjectNo);
				if (Pim != null)
				{
					JournalItem.JournalObjectType = JournalObjectType.GKPim;
					JournalItem.ObjectUID = Pim.UID;
					JournalItem.ObjectName = Pim.PresentationName;
				}
				GuardZone = GKManager.GuardZones.FirstOrDefault(x => x.GKDescriptorNo == GKObjectNo);
				if (GuardZone != null)
				{
					JournalItem.JournalObjectType = JournalObjectType.GKGuardZone;
					JournalItem.ObjectUID = GuardZone.UID;
					JournalItem.ObjectName = GuardZone.PresentationName;
				}
			}
		}

		void InitializeMAMessage()
		{
			if (Device != null && Device.DriverType == GKDriverType.RSR2_AM_1)
			{
				if (JournalItem.JournalEventNameType == JournalEventNameType.Норма)
				{
					var property = Device.Properties.FirstOrDefault(x => x.Name == "Сообщение для нормы");
					if (property != null)
					{
						JournalItem.DescriptionText = property.StringValue;
					}
				}
				if (JournalItem.JournalEventNameType == JournalEventNameType.Сработка_1)
				{
					var property = Device.Properties.FirstOrDefault(x => x.Name == "Сообщение для сработки 1");
					if (property != null)
					{
						JournalItem.DescriptionText = property.StringValue;
					}
				}
				if (JournalItem.JournalEventNameType == JournalEventNameType.Сработка_2)
				{
					var property = Device.Properties.FirstOrDefault(x => x.Name == "Сообщение для сработки 2");
					if (property != null)
					{
						JournalItem.DescriptionText = property.StringValue;
					}
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