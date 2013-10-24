using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class PumpStationCreator
	{
		GkDatabase GkDatabase;
		XDevice PumpStationDevice;
		List<XDirection> Directions;
		List<XDevice> FirePumpDevices;
		List<XDevice> FailurePumpDevices;
		List<XDelay> Delays;
		ushort DelayTime;
		ushort PumpsCount;
		XDelay MainDelay;

		public PumpStationCreator(GkDatabase gkDatabase, XDevice pumpStationDevice)
		{
			GkDatabase = gkDatabase;
			PumpStationDevice = pumpStationDevice;
			DelayTime = pumpStationDevice.PumpStationProperty.DelayTime;
			PumpsCount = pumpStationDevice.PumpStationProperty.PumpsCount;

			Directions = new List<XDirection>();
			foreach (var directionUID in pumpStationDevice.PumpStationProperty.DirectionUIDs)
			{
				var direction = XManager.Directions.FirstOrDefault(x => x.UID == directionUID);
				if (direction != null)
				{
					Directions.Add(direction);
				}
			}

			Delays = new List<XDelay>();

			FirePumpDevices = new List<XDevice>();
			FailurePumpDevices = new List<XDevice>();
			foreach (var pumpStationPump in pumpStationDevice.PumpStationProperty.PumpStationPumps.OrderBy(x => x.PumpStationPumpType))
			{
				var pumpDevice = XManager.Devices.FirstOrDefault(x => x.UID == pumpStationPump.DeviceUID);
				if (pumpDevice != null)
				{
					if (pumpDevice.IntAddress <= 8)
					{
						FirePumpDevices.Add(pumpDevice);
					}
					else
					{
						FailurePumpDevices.Add(pumpDevice);
					}
				}
			}
		}

		public void Create()
		{
			CreateMainDelay();
			CreateDelays();
			SetCrossReferences();

			foreach (var delay in Delays)
			{
				GkDatabase.AddDelay(delay);
				var deviceBinaryObject = new DelayBinaryObject(delay);
				GkDatabase.BinaryObjects.Add(deviceBinaryObject);
			}

			CreateMainDelayLogic();
			CreateDelaysLogic();
			SetPumpDevicesLogic();
		}

		void CreateMainDelay()
		{
			MainDelay = new XDelay()
			{
				Name = "Задержка пуска НС",
				DelayTime = (ushort)(0),
				SetTime = 2,
				DelayRegime = DelayRegime.On
			};
			Delays.Add(MainDelay);
		}

		void CreateDelays()
		{
			int pumpIndex = 1;
			foreach (var pumpDevice in FirePumpDevices)
			{
				var delayTime = 0;
				if (FirePumpDevices.LastIndexOf(pumpDevice) > 0)
					delayTime = DelayTime;
				var delay = new XDelay()
				{
					Name = "Задержка пуска ШУН " + pumpDevice.DottedAddress,
					DelayTime = (ushort)delayTime,
					SetTime = 2,
					DelayRegime = DelayRegime.On
				};

				Delays.Add(delay);
				pumpIndex++;
			}
		}

		void CreateMainDelayLogic()
		{
			var mainDelayBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Delay != null && x.Delay.UID == MainDelay.UID);
			if (mainDelayBinaryObject != null)
			{
				var formula = new FormulaBuilder();
				formula.AddClauseFormula(PumpStationDevice.DeviceLogic);
				formula.AddStandardTurning(mainDelayBinaryObject.BinaryBase);

				formula.Add(FormulaOperationType.END);
				mainDelayBinaryObject.Formula = formula;
				mainDelayBinaryObject.FormulaBytes = formula.GetBytes();
			}
		}

		void Create_OLD_MainDelayLogic()
		{
			var mainDelayBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Delay != null && x.Delay.UID == MainDelay.UID);
			if (mainDelayBinaryObject != null)
			{
				var formula = new FormulaBuilder();
				var deviceBinaryObject = new DeviceBinaryObject(PumpStationDevice, DatabaseType.Gk);

				if (Directions.Count > 0)
				{
					var inputDirectionsCount = 0;
					foreach (var direction in Directions)
					{
						formula.AddGetBit(XStateBit.On, direction);
						if (inputDirectionsCount > 0)
						{
							formula.Add(FormulaOperationType.OR);
						}
						inputDirectionsCount++;
					}
					foreach (var failurePumpDevice in FailurePumpDevices)
					{
						formula.AddGetBit(XStateBit.Failure, failurePumpDevice);
						formula.Add(FormulaOperationType.COM);
						formula.Add(FormulaOperationType.AND);
					}

					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, MainDelay);
				}

				if (Directions.Count > 0)
				{
					var inputDirectionsCount = 0;
					foreach (var direction in Directions)
					{
						formula.AddGetBit(XStateBit.Off, direction);
						if (inputDirectionsCount > 0)
						{
							formula.Add(FormulaOperationType.OR);
						}
						inputDirectionsCount++;
					}
					foreach (var failurePumpDevice in FailurePumpDevices)
					{
						formula.AddGetBit(XStateBit.Failure, failurePumpDevice);
						formula.Add(FormulaOperationType.COM);
						formula.Add(FormulaOperationType.AND);
					}

					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, MainDelay);
				}

				formula.Add(FormulaOperationType.END);
				mainDelayBinaryObject.Formula = formula;
				mainDelayBinaryObject.FormulaBytes = formula.GetBytes();
			}
		}

		void CreateDelaysLogic()
		{
			bool firstDelay = true;
			for (int i = 0; i < Delays.Count; i++)
			{
				var delay = Delays[i];
				if (delay.UID == MainDelay.UID)
					continue;

				var delayBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Delay != null && x.Delay.UID == delay.UID);
				var formula = new FormulaBuilder();

				if (firstDelay)
				{
					foreach (var pumpDevice in FirePumpDevices)
					{
						formula.AddGetBit(XStateBit.TurningOn, pumpDevice);
						formula.AddGetBit(XStateBit.On, pumpDevice);
						formula.Add(FormulaOperationType.OR);
						if (FirePumpDevices.IndexOf(pumpDevice) > 0)
						{
							formula.Add(FormulaOperationType.ADD);
						}
					}
					formula.Add(FormulaOperationType.DUP);
					formula.Add(FormulaOperationType.CONST, 0, PumpsCount, "Количество основных пожарных насосов");
					formula.Add(FormulaOperationType.LT);
					formula.AddGetBit(XStateBit.On, MainDelay);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, delay);

					formula.Add(FormulaOperationType.CONST, 0, PumpsCount, "Количество основных пожарных насосов");
					formula.Add(FormulaOperationType.GE);
					formula.AddGetBit(XStateBit.Off, MainDelay);
					formula.Add(FormulaOperationType.OR);
					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, delay);
				}
				else
				{
					var prevDelay = Delays[i - 1];
					formula.AddGetBit(XStateBit.On, prevDelay);
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, delay);
					formula.AddGetBit(XStateBit.Off, prevDelay);
					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, delay);
				}
				firstDelay = false;

				formula.Add(FormulaOperationType.END);
				delayBinaryObject.Formula = formula;
				delayBinaryObject.FormulaBytes = formula.GetBytes();
			}
		}

		void SetPumpDevicesLogic()
		{
			foreach (var pumpDevice in FirePumpDevices)
			{
				var pumpBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Device != null && x.Device.UID == pumpDevice.UID);
				if (pumpBinaryObject != null)
				{
					var formula = new FormulaBuilder();
					var inputPumpsCount = 0;
					foreach (var otherPumpDevice in FirePumpDevices)
					{
						if (otherPumpDevice.UID == pumpDevice.UID)
							continue;

						formula.AddGetBit(XStateBit.TurningOn, otherPumpDevice);
						formula.AddGetBit(XStateBit.On, otherPumpDevice);
						formula.Add(FormulaOperationType.OR);
						if (inputPumpsCount > 0)
						{
							formula.Add(FormulaOperationType.ADD); // почситать количество включеных насосов
						}
						inputPumpsCount++;
					}
					formula.Add(FormulaOperationType.CONST, 0, PumpsCount, "Количество основных пожарных насосов");
					formula.Add(FormulaOperationType.LT);
					formula.AddGetBit(XStateBit.Norm, pumpDevice);
					formula.Add(FormulaOperationType.AND); // бит дежурный у самого насоса

					formula.AddGetBit(XStateBit.On, MainDelay);
					formula.Add(FormulaOperationType.AND);
					var pumpDelay = Delays.FirstOrDefault(x => x.Name == "Задержка пуска ШУН " + pumpDevice.DottedAddress);
					formula.AddGetBit(XStateBit.On, pumpDelay);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpDevice); // включить насос

					formula.AddGetBit(XStateBit.Off, MainDelay);
					formula.AddGetBit(XStateBit.Norm, pumpDevice);
					formula.Add(FormulaOperationType.AND); // бит дежурный у самого насоса
					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, pumpDevice); // выключить насос
					formula.Add(FormulaOperationType.END);

					pumpBinaryObject.Formula = formula;
					pumpBinaryObject.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void SetCrossReferences()
		{
			foreach (var direction in Directions)
			{
				MainDelay.InputObjects.Add(direction);
				direction.OutputObjects.Add(MainDelay);
			}

			foreach (var pumpDevice in FirePumpDevices)
			{
				foreach (var delay in Delays)
				{
					if (delay.Name == "Задержка пуска ШУН " + pumpDevice.DottedAddress || delay.Name == "Задержка пуска НС")
					{
						pumpDevice.InputObjects.Add(delay);
						delay.OutputObjects.Add(pumpDevice);
					}
				}
			}

			for (int i = 0; i < Delays.Count; i++)
			{
				XDelay prevDelay = null;
				XDelay currentDelay = Delays[i];
				XDelay nextDelay = null;
				if (i > 0)
					prevDelay = Delays[i - 1];
				if (i < Delays.Count - 1)
					nextDelay = Delays[i + 1];

				if (prevDelay != null)
					currentDelay.InputObjects.Add(prevDelay);
				if (nextDelay != null)
					currentDelay.OutputObjects.Add(nextDelay);

				if (i == 1)
				{
					foreach (var pumpDevice in FirePumpDevices)
					{
						currentDelay.InputObjects.Add(pumpDevice);
						pumpDevice.OutputObjects.Add(currentDelay);
					}
				}
			}
		}
	}
}