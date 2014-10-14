using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class DescriptorStateHelper
	{
		GKBase GKBase;
		public ushort AddressOnController { get; private set; }
		public ushort PhysicalAddress { get; private set; }
		public string Description { get; private set; }
		public ushort TypeNo { get; private set; }
		public List<GKStateBit> StateBits { get; private set; }
		public List<GKAdditionalState> AdditionalStates { get; private set; }
		List<ushort> additionalShortParameters = new List<ushort>();

		public int OnDelay { get; private set; }
		public int HoldDelay { get; private set; }
		public int OffDelay { get; private set; }

		public void Parse(List<byte> bytes, GKBase gkBase)
		{
			GKBase = gkBase;
			ushort controllerAddress = BytesHelper.SubstructShort(bytes, 2);
			AddressOnController = BytesHelper.SubstructShort(bytes, 4);
			PhysicalAddress = BytesHelper.SubstructShort(bytes, 6);
			Description = BytesHelper.BytesToStringDescription(bytes);
			int serialNo = BytesHelper.SubstructInt(bytes, 40);
			int state = BytesHelper.SubstructInt(bytes, 44);

			TypeNo = BytesHelper.SubstructShort(bytes, 0);
			StateBits = GKStatesHelper.StatesFromInt(state);
			ParseAdditionalParameters(bytes, gkBase);
			CheckConnectionLost(gkBase);
		}

		void ParseAdditionalParameters(List<byte> bytes, GKBase gkBase)
		{
			AdditionalStates = new List<GKAdditionalState>();
			for (int i = 0; i < 10; i++)
			{
				var additionalShortParameter = BytesHelper.SubstructShort(bytes, bytes.Count - 20 + i * 2);
				additionalShortParameters.Add(additionalShortParameter);
			}

			var driver = GKManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverTypeNo == TypeNo);
			if (driver != null)
			{
				var driverType = driver.DriverType;
				if (driverType == GKDriverType.GK && GKBase.GKDescriptorNo > 1)
					driverType = GKDriverType.KAU;

				switch (driverType)
				{
					case GKDriverType.KAU:
					case GKDriverType.RSR2_KAU:
						var bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Питание 1");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Питание 2");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Отказ АЛС 1 или 2");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Отказ АЛС 3 или 4");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "Отказ АЛС 5 или 6");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "Отказ АЛС 7 или 8");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Вскрытие корпуса");
						if (bitArray[8])
							AddAdditionalState(XStateClass.Failure, "Неисправность АЛС 1");
						if (bitArray[9])
							AddAdditionalState(XStateClass.Failure, "Неисправность АЛС 2");
						if (bitArray[10])
							AddAdditionalState(XStateClass.Failure, "Неисправность АЛС 3");
						if (bitArray[11])
							AddAdditionalState(XStateClass.Failure, "Неисправность АЛС 4");
						if (bitArray[12])
							AddAdditionalState(XStateClass.Failure, "Неисправность АЛС 5");
						if (bitArray[13])
							AddAdditionalState(XStateClass.Failure, "Неисправность АЛС 6");
						if (bitArray[14])
							AddAdditionalState(XStateClass.Failure, "Неисправность АЛС 7");
						if (bitArray[15])
							AddAdditionalState(XStateClass.Failure, "Неисправность АЛС 8");
						break;

					case GKDriverType.GK:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Питание 1");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Питание 2");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "ОЛС");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "РЛС");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Вскрытие корпуса");
						break;

					case GKDriverType.RSR2_Bush_Drenazh:
						var bushType = 1;
						if (GKBase is GKDevice)
						{
							GKDevice device = GKBase as GKDevice;
							var property = device.Properties.FirstOrDefault(x => x.Name == "Type");
							if (property != null)
							{
								bushType = property.Value;
							}
						}
						var sensorBitArray = new BitArray(new int[1] { additionalShortParameters[4] % 256 });
						var breakBitArray = new BitArray(new int[1] { additionalShortParameters[5] % 256 });
						var kzBitArray = new BitArray(new int[1] { additionalShortParameters[6] % 256 });

						if (sensorBitArray[0])
							AddAdditionalState(XStateClass.Info, "Низкий уровень");
						if (sensorBitArray[1])
							AddAdditionalState(XStateClass.Info, "Высокий уровень");
						if (sensorBitArray[2])
							AddAdditionalState(XStateClass.Failure, "Аварийный уровень");

						if (bushType == 1)
						{
							if (breakBitArray[0] && !kzBitArray[0])
								AddAdditionalState(XStateClass.Failure, "Обрыв Низкий уровень");
							if (breakBitArray[1] && !kzBitArray[1])
								AddAdditionalState(XStateClass.Failure, "Обрыв Высокий уровень");
							if (breakBitArray[2] && !kzBitArray[2])
								AddAdditionalState(XStateClass.Failure, "Обрыв Аварийный уровень");

							if (kzBitArray[0])
								AddAdditionalState(XStateClass.Failure, "КЗ Низкий уровень");
							if (kzBitArray[1])
								AddAdditionalState(XStateClass.Failure, "КЗ Высокий уровень");
							if (kzBitArray[2])
								AddAdditionalState(XStateClass.Failure, "КЗ Аварийный уровень");
						}
						if (bushType == 2)
						{
							if (breakBitArray[0] && !kzBitArray[0])
								AddAdditionalState(XStateClass.Failure, "Обрыв Давление низкое");
							if (breakBitArray[1] && !kzBitArray[1])
								AddAdditionalState(XStateClass.Failure, "Обрыв Давление на выходе");
							if (breakBitArray[2] && !kzBitArray[2])
								AddAdditionalState(XStateClass.Failure, "Таймаут по давлению");

							if (kzBitArray[0])
								AddAdditionalState(XStateClass.Failure, "КЗ Давление низкое");
							if (kzBitArray[1])
								AddAdditionalState(XStateClass.Failure, "КЗ Давление на выходе");
						}
						if (bushType == 3)
						{
							if (breakBitArray[0] && !kzBitArray[0])
								AddAdditionalState(XStateClass.Failure, "Обрыв ДУ ПУСК");
							if (breakBitArray[1] && !kzBitArray[1])
								AddAdditionalState(XStateClass.Failure, "Обрыв ДУ ПУСК");

							if (kzBitArray[0])
								AddAdditionalState(XStateClass.Failure, "КЗ ДУ ПУСК");
							if (kzBitArray[1])
								AddAdditionalState(XStateClass.Failure, "КЗ ДУ СТОП");
						}

						var failureBitArray = new BitArray(new int[1] { additionalShortParameters[5] / 256 });
						if (failureBitArray[0])
							AddAdditionalState(XStateClass.Failure, "Вскрытие");
						if (failureBitArray[1])
							AddAdditionalState(XStateClass.Failure, "Неисправность контакта");
						if (failureBitArray[3])
							AddAdditionalState(XStateClass.Failure, "Питание силовое");
						if (failureBitArray[6])
							AddAdditionalState(XStateClass.Failure, "Неисправность одной или обеих фаз(контроль нагрузки)");
						if (failureBitArray[7])
							AddAdditionalState(XStateClass.Failure, "Несовместимость сигналов");

						failureBitArray = new BitArray(new int[1] { additionalShortParameters[5] % 256 });
						if (failureBitArray[7])
							AddAdditionalState(XStateClass.Failure, "Питание контроллера");
						break;

					case GKDriverType.SmokeDetector:
					case GKDriverType.HeatDetector:
					case GKDriverType.CombinedDetector:
						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Оптический канал или фотоусилитель");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Температурный канал");
						break;

					case GKDriverType.RM_1:
						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Контакт не переключается");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Напряжение запуска реле ниже нормы");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "КЗ выхода");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Обрыв выхода");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Напряжение питания устройства не в норме");
						break;

					case GKDriverType.AMP_1:
					case GKDriverType.AM_1:
					case GKDriverType.AM1_T:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "КЗ ШС");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Обрыв ШС");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Вскрытие корпуса");
						break;

					case GKDriverType.MDU:
						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Блокировка пуска");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Низкое напряжение питания привода");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Обрыв кнопки НОРМА");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "КЗ кнопка НОРМА");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "Обрыв кнопка ЗАЩИТА");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "КЗ кнопки ЗАЩИТА");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Обрыв концевого выключателя ОТКРЫТО");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Обрыв концевого выключателя ЗАКРЫТО");
						if (bitArray[8 + 0])
							AddAdditionalState(XStateClass.Failure, "Обрыв цепи 2 ДВИГАТЕЛЯ");
						if (bitArray[8 + 1])
							AddAdditionalState(XStateClass.Failure, "Обрыв цепи 1 ДВИГАТЕЛЯ");
						if (bitArray[8 + 2])
							AddAdditionalState(XStateClass.Failure, "Замкнуты/разомкнуты оба концевика");
						if (bitArray[8 + 3])
							AddAdditionalState(XStateClass.Failure, "Превышение времени хода");
						break;

					case GKDriverType.MRO_2:
						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Обрыв кнопки ПУСК");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "КЗ или обрыв выходной линии");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "Напряжение питания устройства не в норме");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Обрыв кнопки СТОП");
						if (bitArray[8 + 0])
							AddAdditionalState(XStateClass.Failure, "Отсутствуют или испорчены сообщения для воспроизведения");
						if (bitArray[8 + 1])
							AddAdditionalState(XStateClass.Failure, "КЗ кнопки ПУСК");
						if (bitArray[8 + 2])
							AddAdditionalState(XStateClass.Failure, "КЗ кнопки СТОП");
						break;

					case GKDriverType.MPT:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Напряжение питания ШС ниже нормы");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Ошибка памяти");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "КЗ ШС");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Обрыв ШС");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Напряжение питания устройства не в норме");

						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "КЗ выхода 1");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "КЗ выхода 2");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "КЗ выхода 3");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "КЗ выхода 4");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "КЗ выхода 5");

						bitArray = new BitArray(new int[1] { additionalShortParameters[2] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Обрыв выхода 1");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Обрыв выхода 2");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Обрыв выхода 3");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Обрыв выхода 4");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "Обрыв выхода 5");
						break;

					case GKDriverType.FirePump:
					case GKDriverType.JockeyPump:
					case GKDriverType.DrainagePump:
						//bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						//	if (bitArray[8 + 0])
						//		AddAdditionalState(XStateClass.Failure, "Обрыв цепи питания двигателя");

						var pumpType = 0;
						if (gkBase is GKDevice)
						{
							GKDevice device = gkBase as GKDevice;
							if (device != null && device.Driver.IsPump)
							{
								var pumpTypeProperty = device.Properties.FirstOrDefault(x => x.Name == "PumpType");
								if (pumpTypeProperty != null)
								{
									pumpType = pumpTypeProperty.Value;
								}
							}
						}
						bitArray = new BitArray(new int[1] { additionalShortParameters[3] });
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, JournalStringsHelper.GetPumpFailureMessage("Обрыв входа 1", pumpType));
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, JournalStringsHelper.GetPumpFailureMessage("КЗ входа 1", pumpType));
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, JournalStringsHelper.GetPumpFailureMessage("Обрыв входа 2", pumpType));
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, JournalStringsHelper.GetPumpFailureMessage("КЗ входа 2", pumpType));
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, JournalStringsHelper.GetPumpFailureMessage("Обрыв входа 3", pumpType));
						if (bitArray[8 + 0])
							AddAdditionalState(XStateClass.Failure, JournalStringsHelper.GetPumpFailureMessage("КЗ входа 3", pumpType));
						//if (bitArray[8 + 2])
						//	AddAdditionalState(XStateClass.Failure, StringHelper.GetPumpFailureMessage("Обрыв входа 4", addressOnShleif));
						//if (bitArray[8 + 3])
						//	AddAdditionalState(XStateClass.Failure, StringHelper.GetPumpFailureMessage("КЗ входа 4", addressOnShleif));

						bitArray = new BitArray(new int[1] { additionalShortParameters[4] });
						if (!bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Вскрытие корпуса");
						if (!bitArray[8 + 0])
							AddAdditionalState(XStateClass.Failure, "Не задан тип");
						if (!bitArray[8 + 2])
							AddAdditionalState(XStateClass.Failure, "Отказ ПН");
						if (!bitArray[8 + 3])
							AddAdditionalState(XStateClass.Failure, "Отказ ШУН");
						break;

					case GKDriverType.Valve:
						//bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						//if (bitArray[7])
						//	AddAdditionalState(XStateClass.Failure, "Обрыв цепи питания двигателя");

						bitArray = new BitArray(new int[1] { additionalShortParameters[3] });
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Обрыв концевого выключателя ОТКРЫТО");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "КЗ концевого выключателя ОТКРЫТО");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "Обрыв муфтового выключателя ОТКРЫТО");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "КЗ муфтового выключателя ОТКРЫТО");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Обрыв концевого выключателя ЗАКРЫТО");
						if (bitArray[8 + 0])
							AddAdditionalState(XStateClass.Failure, "КЗ концевого выключателя ЗАКРЫТО");
						if (bitArray[8 + 2])
							AddAdditionalState(XStateClass.Failure, "Обрыв муфтового выключателя ЗАКРЫТО");
						if (bitArray[8 + 3])
							AddAdditionalState(XStateClass.Failure, "КЗ муфтового выключателя ЗАКРЫТО");
						if (bitArray[8 + 4])
							AddAdditionalState(XStateClass.Failure, "Обрыв кнопки Открыть УЗЗ/Закрыть УЗЗ");
						if (bitArray[8 + 5])
							AddAdditionalState(XStateClass.Failure, "КЗ кнопки Открыть УЗЗ/Закрыть УЗЗ");
						if (bitArray[8 + 6])
							AddAdditionalState(XStateClass.Failure, "Обрыв кнопки СТОП УЗЗ");
						if (bitArray[8 + 7])
							AddAdditionalState(XStateClass.Failure, "КЗ кнопки СТОП УЗЗ");

						bitArray = new BitArray(new int[1] { additionalShortParameters[4] });
						if (!bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Вскрытие корпуса");
						if (!bitArray[5])
							AddAdditionalState(XStateClass.Failure, "Превышение времени хода");
						if (!bitArray[6])
							AddAdditionalState(XStateClass.Failure, "КВ/МВ");
						if (!bitArray[8 + 1])
							AddAdditionalState(XStateClass.Failure, "Не задан режим");
						if (!bitArray[8 + 3])
							AddAdditionalState(XStateClass.Failure, "Отказ ШУЗ");
						break;

					case GKDriverType.Battery:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] / 256 });
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Отсутствие сетевого напряжения");

						bitArray = new BitArray(new int[1] { additionalShortParameters[0] % 256 });
						if (!bitArray[0] && bitArray[1])
							AddAdditionalState(XStateClass.Failure, "КЗ Выхода 1");
						if (bitArray[0] && !bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Перегрузка Выхода 1");
						if (bitArray[0] && bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Напряжение Выхода 1 выше нормы");
						if (!bitArray[2] && bitArray[3])
							AddAdditionalState(XStateClass.Failure, "КЗ Выхода 2");
						if (bitArray[2] && !bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Перегрузка Выхода 2");
						if (bitArray[2] && bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Напряжение Выхода 2 выше нормы");
						if (!bitArray[4] && bitArray[5])
							AddAdditionalState(XStateClass.Failure, "АКБ 1 Разряд");
						if (bitArray[4] && !bitArray[5])
							AddAdditionalState(XStateClass.Failure, "АКБ 1 Глубокий Разряд");
						if (bitArray[4] && bitArray[5])
							AddAdditionalState(XStateClass.Failure, "АКБ 1 Отсутствие");
						if (!bitArray[6] && bitArray[7])
							AddAdditionalState(XStateClass.Failure, "АКБ 2 Разряд");
						if (bitArray[6] && !bitArray[7])
							AddAdditionalState(XStateClass.Failure, "АКБ 2 Глубокий Разряд");
						if (bitArray[6] && bitArray[7])
							AddAdditionalState(XStateClass.Failure, "АКБ 2 Отсутствие");
						break;
				}

				switch (driverType)
				{
					case GKDriverType.RSR2_MVK8:
					case GKDriverType.RSR2_RM_1:
						OnDelay = additionalShortParameters[0];
						HoldDelay = additionalShortParameters[1];
						OffDelay = additionalShortParameters[2];
						break;

					case GKDriverType.RSR2_Bush_Drenazh:
					case GKDriverType.RSR2_Bush_Jokey:
					case GKDriverType.RSR2_Bush_Fire:

						switch (additionalShortParameters[1])
						{
							case 1:
								OnDelay = additionalShortParameters[0];
								break;

							case 2:
								OffDelay = additionalShortParameters[0];
								break;
						}

						var bitArray = new BitArray(new int[1] { additionalShortParameters[2] });
						switch (driverType)
						{
							case GKDriverType.RSR2_Bush_Drenazh:
								if (bitArray[0])
									AddAdditionalState(XStateClass.Failure, "Неисправность датчика низкого уровня");
								if (bitArray[1])
									AddAdditionalState(XStateClass.Failure, "Неисправность датчика высокого уровня");
								if (bitArray[2])
									AddAdditionalState(XStateClass.Failure, "Неисправность датчика аварийного уровня");
								if (bitArray[6])
									AddAdditionalState(XStateClass.Failure, "Аварийный уровень есть");
								break;
							case GKDriverType.RSR2_Bush_Jokey:
								if (bitArray[0])
									AddAdditionalState(XStateClass.Failure, "Неисправность датчика низкого давления");
								if (bitArray[1])
									AddAdditionalState(XStateClass.Failure, "Неисправность датчика нормального давления");
								if (bitArray[2])
									AddAdditionalState(XStateClass.Failure, "Неисправность датчика высокого давления");
								if (bitArray[6])
									AddAdditionalState(XStateClass.Failure, "Неисправность выхода на режим");
								break;
							case GKDriverType.RSR2_Bush_Fire:
								if (bitArray[0])
									AddAdditionalState(XStateClass.Failure, "Неисправность датчика давления а выходе");
								if (bitArray[1])
									AddAdditionalState(XStateClass.Failure, "Неисправность ДУ ПУСК");
								if (bitArray[2])
									AddAdditionalState(XStateClass.Failure, "Неисправность ДУ СТОП");
								if (bitArray[6])
									AddAdditionalState(XStateClass.Failure, "Неисправность выхода на режим");
								break;
						}
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Неисправность питания контроллера");

						bitArray = new BitArray(new int[1] { additionalShortParameters[2] / 256 });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Вскрытие");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Неисправность контактора");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "Неисправность силового питания");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Обрыв цепи ПД");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Недопустимое сочетание сигналов");
						break;
				}
			}
			else
			{
				if (TypeNo == 0x101 || TypeNo == 0x106)
				{
					OnDelay = additionalShortParameters[0];
					HoldDelay = additionalShortParameters[1];
				}
				if (TypeNo == 0x108)
				{
					OnDelay = additionalShortParameters[0];
				}
			}
		}

		void AddAdditionalState(XStateClass stateClass, string name)
		{
			if (name != null)
			{
				var additionalState = new GKAdditionalState()
				{
					StateClass = stateClass,
					Name = name
				};
				AdditionalStates.Add(additionalState);
			}
		}

		void CheckConnectionLost(GKBase gkBase)
		{
			if (gkBase is GKDevice)
			{
				var connectionLostParameter = additionalShortParameters[9];
				var connectionLostCount = connectionLostParameter / 256;

				GKDevice device = gkBase as GKDevice;
				GKDevice connectionLostParent = device.KAUParent;
				if (device.Driver.IsKauOrRSR2Kau)
				{
					connectionLostParent = device.GKParent;
					connectionLostCount = connectionLostParameter % 256;
				}
				if (connectionLostParent != null)
				{
					var property = connectionLostParent.Properties.FirstOrDefault(x => x.Name == "ConnectionLostCount");
					if (property != null)
					{
						if (connectionLostCount >= property.Value)
						{
							AdditionalStates = new List<GKAdditionalState>()
							{
								new GKAdditionalState()
								{
									StateClass = XStateClass.Failure,
									Name = "Потеря связи"
								}
							};
						}
					}
				}
			}
		}
	}
}