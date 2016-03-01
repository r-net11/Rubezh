using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI;
using System.Diagnostics;

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
		public int RunningTime { get; private set; }

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
				//if (driverType == GKDriverType.GK && GKBase.GKDescriptorNo > 1)
				//	driverType = GKDriverType.KAU;

				switch (driverType)
				{
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
							AddAdditionalState(XStateClass.Failure, "Обрыв цепи питания двигателя");
						if (failureBitArray[7])
							AddAdditionalState(XStateClass.Failure, "Несовместимость сигналов");

						failureBitArray = new BitArray(new int[1] { additionalShortParameters[5] % 256 });
						if (failureBitArray[7])
							AddAdditionalState(XStateClass.Failure, "Питание контроллера");
						break;
				}

				switch (driverType)
				{
					case GKDriverType.RSR2_MDU:
					case GKDriverType.RSR2_MDU24:
					case GKDriverType.RSR2_Buz_KV:
					case GKDriverType.RSR2_Buz_KVMV:
					case GKDriverType.RSR2_Buz_KVDU:
						if (additionalShortParameters[1] == 1)
						{
							OnDelay = additionalShortParameters[0];
						}
						if (additionalShortParameters[1] == 2)
						{
							RunningTime = additionalShortParameters[0];
						}
						if (additionalShortParameters[1] == 3)
						{
							HoldDelay = additionalShortParameters[0];
						}
						break;
					case GKDriverType.RSR2_MVK8:
					case GKDriverType.RSR2_RM_1:
					case GKDriverType.RSR2_OPS:
					case GKDriverType.RSR2_OPZ:
					case GKDriverType.RSR2_OPK:
						OnDelay = additionalShortParameters[0];
						HoldDelay = additionalShortParameters[1];
						OffDelay = additionalShortParameters[2];
						break;

					case GKDriverType.RSR2_Bush_Drenazh:
					case GKDriverType.RSR2_Bush_Jokey:
					case GKDriverType.RSR2_Bush_Fire:
					case GKDriverType.RSR2_Bush_Shuv:
					case GKDriverType.RSR2_Valve_DU:
					case GKDriverType.RSR2_Valve_KV:
					case GKDriverType.RSR2_Valve_KVMV:

						switch (additionalShortParameters[1])
						{
							case 1:
								OnDelay = additionalShortParameters[0];
								break;

							case 2:
								OffDelay = additionalShortParameters[0];
								break;
						}

						var bitArray = new BitArray(new int[1] { additionalShortParameters[6] });
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
									AddAdditionalState(XStateClass.Failure, "Неисправность датчика давления на выходе");
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

						bitArray = new BitArray(new int[1] { additionalShortParameters[6] / 256 });
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

					case GKDriverType.RSR2_ABPC:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Питание 1");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Питание 2");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "КЗ выхода");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Обрыв выхода");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "Измерения");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "Вскрытие корпуса");
						break;

					case GKDriverType.RSR2_ABShS:
					case GKDriverType.RSR2_ABTK:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Питание 1");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Питание 2");
						if (bitArray[2])
							AddAdditionalState(XStateClass.Failure, "КЗ ШС");
						if (bitArray[3])
							AddAdditionalState(XStateClass.Failure, "Обрыв ШС");
						if (bitArray[4])
							AddAdditionalState(XStateClass.Failure, "Измерения");
						if (bitArray[5])
							AddAdditionalState(XStateClass.Failure, "Вскрытие корпуса");
						break;

					case GKDriverType.RSR2_HandDetectorEridan:
					case GKDriverType.RSR2_HeatDetectorEridan:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Токопотребление");
						break;

					case GKDriverType.RK_HandDetector:
					case GKDriverType.RK_SmokeDetector:
					case GKDriverType.RK_HeatDetector:
					case GKDriverType.RK_RM:
					case GKDriverType.RK_AM:
					case GKDriverType.RK_OPK:
					case GKDriverType.RK_OPZ:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AddAdditionalState(XStateClass.Failure, "Питание 1");
						if (bitArray[1])
							AddAdditionalState(XStateClass.Failure, "Питание 2");
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
					OffDelay = additionalShortParameters[1];
					HoldDelay = additionalShortParameters[2];
				}
				if (TypeNo == 0x104)
				{
					OffDelay = additionalShortParameters[0];
					HoldDelay = additionalShortParameters[1];
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
				if (device.Driver.IsKau)
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