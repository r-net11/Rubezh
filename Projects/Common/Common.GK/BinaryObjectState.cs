using System.Collections;
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
			AdditionalStates = new List<string>();
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
							AdditionalStates.Add("Неисправность питания 1");
						if (bitArray[1])
							AdditionalStates.Add("Неисправность питания 2");
						if (bitArray[2])
							AdditionalStates.Add("Отказ АЛС 1 или 2");
						if (bitArray[3])
							AdditionalStates.Add("Отказ АЛС 3 или 4");
						if (bitArray[4])
							AdditionalStates.Add("Отказ АЛС 5 или 6");
						if (bitArray[5])
							AdditionalStates.Add("Отказ АЛС 7 или 8");
						if (bitArray[6])
							AdditionalStates.Add("Вскрытие");
						if (bitArray[8])
							AdditionalStates.Add("Короткое замыкание АЛС 1");
						if (bitArray[9])
							AdditionalStates.Add("Короткое замыкание АЛС 2");
						if (bitArray[10])
							AdditionalStates.Add("Короткое замыкание АЛС 3");
						if (bitArray[11])
							AdditionalStates.Add("Короткое замыкание АЛС 4");
						if (bitArray[12])
							AdditionalStates.Add("Короткое замыкание АЛС 5");
						if (bitArray[13])
							AdditionalStates.Add("Короткое замыкание АЛС 6");
						if (bitArray[14])
							AdditionalStates.Add("Короткое замыкание АЛС 7");
						if (bitArray[15])
							AdditionalStates.Add("Короткое замыкание АЛС 8");
						break;

					case XDriverType.GK:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AdditionalStates.Add("Неисправность питания 1");
						if (bitArray[1])
							AdditionalStates.Add("Неисправность питания 2");
						if (bitArray[2])
							AdditionalStates.Add("Неисправность ОЛС");
						if (bitArray[3])
							AdditionalStates.Add("Неисправность РЛС");
						if (bitArray[6])
							AdditionalStates.Add("Вскрытие");
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
							AdditionalStates.Add("Низкий уровень");
						if (sensorBitArray[1])
							AdditionalStates.Add("Высокий уровень");
						if (sensorBitArray[2])
							AdditionalStates.Add("Аварийный уровень");

						if (breakBitArray[0] && !kzBitArray[0])
							AdditionalStates.Add("Обрыв Низкий уровень");
						if (breakBitArray[1] && !kzBitArray[1])
							AdditionalStates.Add("Обрыв Высокий уровень");
						if (breakBitArray[2] && !kzBitArray[2])
							AdditionalStates.Add("Обрыв Аварийный уровень");

						if (kzBitArray[0])
							AdditionalStates.Add("КЗ Низкий уровень");
						if (kzBitArray[1])
							AdditionalStates.Add("КЗ Высокий уровень");
						if (kzBitArray[2])
							AdditionalStates.Add("КЗ Аварийный уровень");

						var failureBitArray = new BitArray(new int[1] { additionalShortParameters[5] / 256 });
						if (failureBitArray[0])
							AdditionalStates.Add("Вскрытие");
						if (failureBitArray[1])
							AdditionalStates.Add("Неисправность контакта");
						if (failureBitArray[2])
							AdditionalStates.Add("Авария контакта");
						if (failureBitArray[6])
							AdditionalStates.Add("Неисправность одной или обеих фаз(контроль нагрузки)");
						if (failureBitArray[7])
							AdditionalStates.Add("Несовместимость сигналов");
						break;

					case XDriverType.RM_1:
						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[1])
							AdditionalStates.Add("Напряжение запуска реле ниже нормы");
						if (bitArray[4])
							AdditionalStates.Add("Тест");
						if (bitArray[5])
							AdditionalStates.Add("КЗ выхода");
						if (bitArray[6])
							AdditionalStates.Add("Обрыв выхода");
						if (bitArray[7])
							AdditionalStates.Add("Напряжение питания устройства не в норме");
						break;

					case XDriverType.AMP_1:
						bitArray = new BitArray(new int[1] { additionalShortParameters[1] });
						if (bitArray[1])
							AdditionalStates.Add("КЗ ШС");
						if (bitArray[7])
							AdditionalStates.Add("Вскрытие корпуса");
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

		public ushort AddressOnController { get; private set; }
		public ushort PhysicalAddress { get; private set; }
		public string Description { get; private set; }
		public ushort TypeNo { get; private set; }
		public List<XStateBit> StateBits { get; private set; }
		public List<string> AdditionalStates { get; private set; }
		public List<AdditionalXStateProperty> AdditionalStateProperties { get; private set; }

		public int OnDelay { get; private set; }
		public int HoldDelay { get; private set; }
		public int OffDelay { get; private set; }
	}
}