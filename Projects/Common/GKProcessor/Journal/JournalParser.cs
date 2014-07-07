using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI.Events;

namespace GKProcessor
{
	public class JournalParser
	{
		public JournalItem JournalItem { get; private set; }

		public XDevice Device { get; private set; }
		public XZone Zone { get; private set; }
		public XDirection Direction { get; private set; }
		public XPumpStation PumpStation { get; private set; }
		public XMPT MPT { get; private set; }
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
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Перевод_в_технологический_режим;
							break;

						case 2:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Синхронизация_времени_прибора_с_временем_ПК;
							break;

						case 4:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Смена_ПО;
							break;

						case 5:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Смена_БД;
							break;

						case 6:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Перевод_в_рабочий_режим;
							break;

						case 7:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Вход_пользователя_в_прибор;
							JournalItem.Description = JournalStringsHelper.ToUser(bytes[32 + 15]);
							var bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							var bytes2 = bytes.GetRange(16, 21 - 16 + 1);
							bytes1.AddRange(bytes2);
							JournalItem.UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItem.JournalItemType = JournalItemType.GkUser;
							break;

						case 8:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Выход_пользователя_из_прибора;
							JournalItem.Description = JournalStringsHelper.ToUser(bytes[32 + 15]);
							bytes1 = bytes.GetRange(6, 31 - 6 + 1);
							bytes2 = bytes.GetRange(48, 53 - 48 + 1);
							bytes1.AddRange(bytes2);
							JournalItem.UserName = Encoding.Default.GetString(bytes1.ToArray(), 0, bytes1.Count);
							JournalItem.JournalItemType = JournalItemType.GkUser;
							break;

						case 9:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Ошибка_управления;
							JournalItem.GKObjectNo = BytesHelper.SubstructShort(bytes, 18);
							break;

						case 10:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Введен_новый_пользователь;
							JournalItem.JournalItemType = JournalItemType.GkUser;
							break;

						case 11:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Изменена_учетная_информация_пользователя;
							JournalItem.JournalItemType = JournalItemType.GkUser;
							break;

						case 12:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Произведена_настройка_сети;
							break;

						default:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Неизвестный_код_события_контроллекра;
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
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Неизвестный_тип;
							JournalItem.Description = unknownDescription;
							break;

						case 1:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Устройство_с_таким_адресом_не_описано_при_конфигурации;
							JournalItem.Description = unknownDescription;
							break;

						default:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Неизвестный_код_события_устройства;
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
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.При_конфигурации_описан_другой_тип;
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
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Изменился_заводской_номер;
							JournalItem.Description = "Старый заводской номер: " + BytesHelper.SubstructInt(bytes, 32 + 14).ToString();
							break;
						case 2:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Пожар_1;
							if (JournalItem.JournalItemType == JournalItemType.Device)
							{
								JournalItem.GlobalEventNameType = GlobalEventNameEnum.Сработка_1;
							}
							JournalItem.Description = JournalStringsHelper.ToFire(bytes[32 + 15]);
							break;

						case 3:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Пожар_2;
							if (JournalItem.JournalItemType == JournalItemType.Device)
							{
								JournalItem.GlobalEventNameType = GlobalEventNameEnum.Сработка_2;
							}
							JournalItem.Description = JournalStringsHelper.ToFire(bytes[32 + 15]);
							break;

						case 4:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Внимание;
							break;

						case 5:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Неисправность;
							if (bytes[32 + 14] == 0)
							{
								JournalItem.GlobalEventNameType = GlobalEventNameEnum.Неисправность_устранена;
							}

							switch (JournalItem.DescriptorType)
							{
								case 0xD6:
									JournalItem.Description = JournalStringsHelper.ToBatteryFailure(bytes[32 + 15]);
									break;

								default:
									JournalItem.Description = JournalStringsHelper.ToFailure(bytes[32 + 15]);
									if (bytes[32 + 15] >= 241 && bytes[32 + 15] <= 254)
									{
										var firstAdditionalDescription = bytes[32 + 16];
										var secondAdditionalDescription = bytes[32 + 17];
										if (firstAdditionalDescription != 0 || secondAdditionalDescription != 0)
										{
											JournalItem.AdditionalDescription = firstAdditionalDescription.ToString() + " " + secondAdditionalDescription.ToString();
										}
									}
									break;
							}
							break;

						case 6:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Тест;
							if (bytes[32 + 14] == 0)
							{
								JournalItem.GlobalEventNameType = GlobalEventNameEnum.Тест_устранен;
							}

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
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Запыленность;
							if (bytes[32 + 14] == 0)
							{
								JournalItem.GlobalEventNameType = GlobalEventNameEnum.Запыленность_устранена;
							}

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
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Информация;
							JournalItem.Description = JournalStringsHelper.ToInformation(bytes[32 + 15]);
							break;

						case 9:
							JournalItem.GlobalEventNameType = JournalStringsHelper.ToState(bytes[32 + 15]);
							break;

						case 10:
							switch (bytes[32 + 15])
							{
								case 0:
									JournalItem.GlobalEventNameType = GlobalEventNameEnum.Перевод_в_автоматический_режим;
									break;

								case 1:
									JournalItem.GlobalEventNameType = GlobalEventNameEnum.Перевод_в_ручной_режим;
									break;

								case 2:
									JournalItem.GlobalEventNameType = GlobalEventNameEnum.Перевод_в_отключенный_режим;
									break;

								case 3:
									JournalItem.GlobalEventNameType = GlobalEventNameEnum.Перевод_в_неопределенный_режим;
									break;

								default:
									JournalItem.GlobalEventNameType = GlobalEventNameEnum.Неизвестный_код_события_объекта;
									break;
							}
							break;

						case 13:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Запись_параметра;
							break;

						case 14:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Норма;
							break;

						default:
							JournalItem.GlobalEventNameType = GlobalEventNameEnum.Неизвестный_код_события_объекта;
							JournalItem.Description = code.ToString();
							break;
					}
					break;
			}

			JournalItem.StateClass = EventDescriptionAttributeHelper.ToStateClass(JournalItem.GlobalEventNameType);
			JournalItem.Name = EventDescriptionAttributeHelper.ToName(JournalItem.GlobalEventNameType);

			//if (Device != null && Device.DriverType == XDriverType.Pump && JournalItem.Name == "Неисправность")
			//{
			//	var pumpTypeProperty = Device.Properties.FirstOrDefault(x => x.Name == "PumpType");
			//	if (pumpTypeProperty != null)
			//	{
			//		JournalItem.Description = JournalStringsHelper.GetPumpFailureMessage(JournalItem.Description, pumpTypeProperty.Value);
			//	}
			//}

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
					JournalItem.ObjectUID = Device.BaseUID;
					JournalItem.ObjectName = Device.DottedPresentationAddress + Device.ShortName;
				}
				Zone = XManager.Zones.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (Zone != null)
				{
					JournalItem.JournalItemType = JournalItemType.Zone;
					JournalItem.ObjectUID = Zone.BaseUID;
					JournalItem.ObjectName = Zone.PresentationName;
				}
				Direction = XManager.Directions.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (Direction != null)
				{
					JournalItem.JournalItemType = JournalItemType.Direction;
					JournalItem.ObjectUID = Direction.BaseUID;
					JournalItem.ObjectName = Direction.PresentationName;
				}
				PumpStation = XManager.PumpStations.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (PumpStation != null)
				{
					JournalItem.JournalItemType = JournalItemType.PumpStation;
					JournalItem.ObjectUID = PumpStation.BaseUID;
					JournalItem.ObjectName = PumpStation.PresentationName;
				}
				MPT = XManager.MPTs.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (MPT != null)
				{
					JournalItem.JournalItemType = JournalItemType.MPT;
					JournalItem.ObjectUID = MPT.BaseUID;
					JournalItem.ObjectName = MPT.PresentationName;
				}
				Delay = XManager.Delays.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (Delay != null)
				{
					JournalItem.JournalItemType = JournalItemType.Delay;
					JournalItem.ObjectUID = Delay.BaseUID;
					JournalItem.ObjectName = Delay.PresentationName;
				}
				else
				{
					Delay = XManager.AutoGeneratedDelays.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
					if (Delay != null)
					{
						JournalItem.JournalItemType = JournalItemType.Delay;
						JournalItem.ObjectUID = Delay.BaseUID;
						JournalItem.ObjectName = Delay.PresentationName;
					}
				}
				Pim = XManager.AutoGeneratedPims.FirstOrDefault(x => x.GKDescriptorNo == JournalItem.GKObjectNo);
				if (Pim != null)
				{
					JournalItem.JournalItemType = JournalItemType.Pim;
					JournalItem.ObjectUID = Pim.BaseUID;
					JournalItem.ObjectName = Pim.PresentationName;
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