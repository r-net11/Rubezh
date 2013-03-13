﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.XModels;
using FiresecClient;
using XFiresecAPI;
using System.Diagnostics;

namespace Common.GK
{
	public class BinaryObjectState
	{
		public ushort AddressOnController { get; private set; }
		public ushort PhysicalAddress { get; private set; }
		public string Description { get; private set; }
		public ushort TypeNo { get; private set; }
		public List<XStateBit> StateBits { get; private set; }
		public List<XAdditionalState> AdditionalStates { get; private set; }
		public List<AdditionalXStateProperty> AdditionalStateProperties { get; private set; }

		public int OnDelay { get; private set; }
		public int HoldDelay { get; private set; }
		public int OffDelay { get; private set; }

		public BinaryObjectState(List<byte> bytes)
		{
			ushort controllerAddress = BytesHelper.SubstructShort(bytes, 2);
			AddressOnController = BytesHelper.SubstructShort(bytes, 4);
			PhysicalAddress = BytesHelper.SubstructShort(bytes, 6);
			Description = BytesHelper.BytesToStringDescription(bytes.Skip(8).Take(32).ToList()).TrimEnd(' ');
			int serialNo = BytesHelper.SubstructInt(bytes, 40);
			int state = BytesHelper.SubstructInt(bytes, 44);

			TypeNo = BytesHelper.SubstructShort(bytes, 0);

			StateBits = XStatesHelper.StatesFromInt(state);
			SetAdditionalParameters(bytes);
		}

		void SetAdditionalParameters(List<byte> bytes)
		{
			AdditionalStateProperties = new List<AdditionalXStateProperty>();
			AdditionalStates = new List<XAdditionalState>();
			var additionalShortParameters = new List<ushort>();
			for (int i = 0; i < 10; i++)
			{
				var additionalShortParameter = BytesHelper.SubstructShort(bytes, 48 + i * 2);
				additionalShortParameters.Add(additionalShortParameter);
			}

			var driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverTypeNo == TypeNo);
			if (driver != null)
			{
				switch (driver.DriverType)
				{
					case XDriverType.KAU:
					case XDriverType.RSR2_KAU:
						var bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Неисправность питания 1");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Неисправность питания 2");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Отказ АЛС 1 или 2");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Отказ АЛС 3 или 4");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "Отказ АЛС 5 или 6");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "Отказ АЛС 7 или 8");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Вскрытие");
						if (bitArray[8])
							AddAdditionalState(XStateClass.Failure, "Короткое замыкание АЛС 1");
						if (bitArray[9])
							AddAdditionalState(XStateClass.Failure, "Короткое замыкание АЛС 2");
						if (bitArray[10])
							AddAdditionalState(XStateClass.Failure, "Короткое замыкание АЛС 3");
						if (bitArray[11])
							AddAdditionalState(XStateClass.Failure, "Короткое замыкание АЛС 4");
						if (bitArray[12])
							AddAdditionalState(XStateClass.Failure, "Короткое замыкание АЛС 5");
						if (bitArray[13])
							AddAdditionalState(XStateClass.Failure, "Короткое замыкание АЛС 6");
						if (bitArray[14])
							AddAdditionalState(XStateClass.Failure, "Короткое замыкание АЛС 7");
						if (bitArray[15])
							AddAdditionalState(XStateClass.Failure, "Короткое замыкание АЛС 8");
						break;

					case XDriverType.GK:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Неисправность питания 1");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Неисправность питания 2");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Неисправность ОЛС");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Неисправность РЛС");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Вскрытие");
						break;

					case XDriverType.RSR2_MVK8:
					case XDriverType.RSR2_RM_1:
						OnDelay = additionalShortParameters[0];
						HoldDelay = additionalShortParameters[1];
						OffDelay = additionalShortParameters[2];
						break;

					case XDriverType.RSR2_Bush:
						var sensorBitArray = new BitArray(new int[1] { additionalShortParameters[4] % 256 });
						var breakBitArray = new BitArray(new int[1] { additionalShortParameters[5] % 256 });
						var kzBitArray = new BitArray(new int[1] { additionalShortParameters[6] % 256 });

						if (sensorBitArray[0])
							AddAdditionalState(XStateClass.Failure, "Низкий уровень");
						if (sensorBitArray[1])
							AddAdditionalState(XStateClass.Failure, "Высокий уровень");
						if (sensorBitArray[2])
							AddAdditionalState(XStateClass.Failure, "Аварийный уровень");

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

						var failureBitArray = new BitArray(new int[1] { additionalShortParameters[5] / 256 });
						if (failureBitArray[0])
							AddAdditionalState(XStateClass.Failure, "Вскрытие");
						if (failureBitArray[1])
							AddAdditionalState(XStateClass.Failure, "Неисправность контакта");
						if (failureBitArray[2])
							AddAdditionalState(XStateClass.Failure, "Авария контакта");
						if (failureBitArray[6])
							AddAdditionalState(XStateClass.Failure, "Неисправность одной или обеих фаз(контроль нагрузки)");
						if (failureBitArray[7])
							AddAdditionalState(XStateClass.Failure, "Несовместимость сигналов");
						break;

					case XDriverType.RM_1:
						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Напряжение запуска реле ниже нормы");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Test, "Тест");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "КЗ выхода");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Обрыв выхода");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Напряжение питания устройства не в норме");
						break;

					case XDriverType.AMP_1:
					case XDriverType.AM_1:
					case XDriverType.AM1_T:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "КЗ ШС");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Обрыв ШС");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Вскрытие корпуса");
						break;

					case XDriverType.MDU:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[3])
							AddAdditionalState(XStateClass.Info, "Заслонка закрывается");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Info, "Заслонка открывается");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Test, "Тест кнопка");
						if (bitArray[8 + 6])
							AddAdditionalState(XStateClass.Info, "Заслонка закрыта");
						if (bitArray[8 + 7])
							AddAdditionalState(XStateClass.Info, "Заслонка открыта");

						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Дистанционная команда «НОРМА» поступила когда был замкнут включатель «ЗАЩИТА");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Низкое напряжение питания привода (ниже 20V)");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Обрыв в цепи включателя «НОРМА»");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Замыкание в цепи включателя «НОРМА»");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "Обрыв в цепи включателя «ЗАЩИТА");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "Замыкание в цепи включателя «ЗАЩИТА»");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Обрыв в цепи конечного выключателя «ОТКРЫТО»");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Обрыв в цепи концевого выключателя «ЗАКРЫТО»");
						if (bitArray[8 + 0])
							AddAdditionalState(XStateClass.Failure, "Через датчик тока, включенного последовательно обмоткой прямого вращения двигателя, не проходит ток");
						if (bitArray[8 + 1])
							AddAdditionalState(XStateClass.Failure, "Через датчик тока, включенного последовательно встречной обмоткой прямого вращения двигателя, не проходит ток");
						if (bitArray[8 + 2])
							AddAdditionalState(XStateClass.Failure, "Запрещенное состояние концевых выключателей (Замкнуты/разомкнуты оба концевика)");
						if (bitArray[8 + 3])
							AddAdditionalState(XStateClass.Failure, "Лимит времени достижения конечного выключателя");
						if (bitArray[8 + 4])
							AddAdditionalState(XStateClass.Failure, "Напряжение запуска реле ниже нормы");
						if (bitArray[8 + 5])
							AddAdditionalState(XStateClass.Failure, "Реле не реагирует на команды");
						break;

					case XDriverType.MRO_2:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Info, "Воспроизведение включено");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Info, "Воспроизведение сигнала аналогового входа");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Info, "Замена списка сообщений новым сообщением");

						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Сопротивление больше 1,5 кОм");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Сопротивление в диапазоне 100-340 Ом");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Сопротивление выше на 0,5 Ом");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "КЗ илиОбрыв выходной линии");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Test, "Тест кнопка");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "Напряжение питания ниже нормы");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Напряжение питания ниже нормы");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Info, "Сигнал кнопки Стоп");
						if (bitArray[8 + 0])
							AddAdditionalState(XStateClass.Failure, "Отсутствуют или испорчены сообщения для воспроизведения");
						if (bitArray[8 + 1])
							AddAdditionalState(XStateClass.Info, "КЗ кнопки пуск");
						if (bitArray[8 + 2])
							AddAdditionalState(XStateClass.Info, "КЗ кнопки стоп");
						break;

					case XDriverType.MPT:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[2])
							AddAdditionalState(XStateClass.Info, "Изменение автоматики по неисправности");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Info, "Изменение автоматики по кнопке «Стоп»");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Info, "Запуск АУП по RSR");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Info, "Изменение автоматики по датчику «Двери-окна»");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Info, "Автоматика включена");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Info, "Изменение автоматики по ТМ");

						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "Напряжение питания ШС ниже нормы");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Ошибка памяти");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Test, "Кнопка «Тест»");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "КЗ ШС");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Failure, "Обрыв ШС");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Failure, "Напряжение питания прибора ниже нормы");

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
						if (bitArray[5])
							AddAdditionalState(XStateClass.Info, "Отложенный пуск АУП по датчику «Двери-окна");
						if (bitArray[6])
							AddAdditionalState(XStateClass.Info, "Пуск АУП завершен");
						if (bitArray[7])
							AddAdditionalState(XStateClass.Info, "Останов тушения по кнопке «Стоп»");

						bitArray = new BitArray(new int[1] { additionalShortParameters[3] });
						if (bitArray[1])
							AddAdditionalState(XStateClass.Info, "Программирование мастер-ключа ТМ");
						break;
				}
			}
			else
			{
				if (TypeNo == 0x106)
				{
					var property1 = new AdditionalXStateProperty()
					{
						Name = "Задержка",
						Value = additionalShortParameters[0]
					};
					AdditionalStateProperties.Add(property1);
					var property2 = new AdditionalXStateProperty()
					{
						Name = "Удержание",
						Value = additionalShortParameters[1]
					};
					AdditionalStateProperties.Add(property2);

					OnDelay = additionalShortParameters[0];
					HoldDelay = additionalShortParameters[1];
				}
			}
		}

		void AddAdditionalState(XStateClass stateClass, string name)
		{
			var additionalState = new XAdditionalState()
			{
				StateClass = stateClass,
				Name = name
			};
			AdditionalStates.Add(additionalState);
		}
	}
}