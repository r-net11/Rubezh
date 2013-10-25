using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;
using System.Diagnostics;

namespace Common.GK
{
	public class PumpStationCreator
	{
		GkDatabase GkDatabase;
		XDirection Direction;
		List<XDevice> FirePumpDevices;
		List<XDevice> FailurePumpDevices;
		XDevice AM1TDevice;
		List<XDelay> Delays;
		ushort NSDeltaTime;
		ushort NSPumpsCount;

		public PumpStationCreator(GkDatabase gkDatabase, XDirection direction)
		{
			GkDatabase = gkDatabase;
			Direction = direction;
			NSDeltaTime = (ushort)direction.NSDeltaTime;
			NSPumpsCount = (ushort)direction.NSPumpsCount;

			Delays = new List<XDelay>();

			FirePumpDevices = new List<XDevice>();
			FailurePumpDevices = new List<XDevice>();
			foreach (var nsDevice in direction.NSDevices)
			{
				switch (nsDevice.Driver.DriverType)
				{
					case XDriverType.Pump:
					case XDriverType.RSR2_Bush:
						if (nsDevice.IntAddress <= 8)
						{
							FirePumpDevices.Add(nsDevice);
						}
						else
						{
							FailurePumpDevices.Add(nsDevice);
						}
						break;
					case XDriverType.AM1_T:
						AM1TDevice = nsDevice;
						break;
				}
			}
		}

		public void Create()
		{
			CreateDirectionFormulaBytes();
			CreateDelays();
			SetCrossReferences();

			foreach (var delay in Delays)
			{
				GkDatabase.AddDelay(delay);
				var deviceBinaryObject = new DelayBinaryObject(delay);
				GkDatabase.BinaryObjects.Add(deviceBinaryObject);
			}

			CreateDelaysLogic();
			SetPumpDevicesLogic();
		}

		void CreateDirectionFormulaBytes()
		{
			var failureDevices = new List<XDevice>();
			failureDevices.AddRange(FailurePumpDevices);
			if (AM1TDevice != null)
				failureDevices.Add(AM1TDevice);

			//var formula = new FormulaBuilder();
			//if (Direction.InputZones.Count > 0 || Direction.InputDevices.Count > 0)
			//{
			//    var inputObjectsCount = 0;
			//    foreach (var directionZone in Direction.DirectionZones)
			//    {
			//        formula.AddGetBitOff(directionZone.StateBit, directionZone.Zone);
			//        if (inputObjectsCount > 0)
			//        {
			//            formula.Add(FormulaOperationType.OR);
			//        }
			//        inputObjectsCount++;
			//    }
			//    foreach (var directionDevice in Direction.DirectionDevices)
			//    {
			//        formula.AddGetBitOff(directionDevice.StateBit, directionDevice.Device);
			//        if (inputObjectsCount > 0)
			//        {
			//            formula.Add(FormulaOperationType.OR);
			//        }
			//        inputObjectsCount++;
			//    }

			//    foreach (var failureDevice in failureDevices)
			//    {
			//        formula.AddGetBit(XStateBit.Failure, failureDevice);
			//        formula.Add(FormulaOperationType.COM);
			//        formula.Add(FormulaOperationType.AND);
			//    }

			//    formula.Add(FormulaOperationType.DUP);

			//    formula.AddGetBit(XStateBit.Norm, Direction);
			//    formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Направления");
			//    formula.AddPutBit(XStateBit.TurnOn_InAutomatic, Direction);

			//    formula.Add(FormulaOperationType.COM, comment: "Условие Выключения");
			//    formula.AddGetBit(XStateBit.Norm, Direction);
			//    formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Направления");
			//    formula.AddPutBit(XStateBit.TurnOff_InAutomatic, Direction);
			//}
			//formula.Add(FormulaOperationType.END);
			//var directionBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Direction != null && x.Direction.UID == Direction.UID);
			//directionBinaryObject.FormulaBytes = formula.GetBytes();

			foreach (var failureDevice in failureDevices)
			{
				Direction.InputObjects.Add(failureDevice);
				failureDevice.OutputObjects.Add(Direction);
			}
		}

		void CreateDelays()
		{
			int pumpIndex = 1;
			foreach (var pumpDevice in FirePumpDevices)
			{
				var delayTime = 0;
				if (FirePumpDevices.LastIndexOf(pumpDevice) > 0)
					delayTime = NSDeltaTime;
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

		void CreateDelaysLogic()
		{
			bool firstDelay = true;
			for (int i = 0; i < Delays.Count; i++)
			{
				var delay = Delays[i];

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
					formula.Add(FormulaOperationType.CONST, 0, NSPumpsCount, "Количество основных пожарных насосов");
					formula.Add(FormulaOperationType.LT);
					formula.AddGetBit(XStateBit.On, Direction);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, delay);

					formula.Add(FormulaOperationType.CONST, 0, NSPumpsCount, "Количество основных пожарных насосов");
					formula.Add(FormulaOperationType.GE);
					formula.AddGetBit(XStateBit.Off, Direction);
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
					formula.Add(FormulaOperationType.CONST, 0, NSPumpsCount, "Количество основных пожарных насосов");
					formula.Add(FormulaOperationType.LT);
					formula.AddGetBit(XStateBit.Norm, pumpDevice);
					formula.Add(FormulaOperationType.AND); // бит дежурный у самого насоса

					formula.AddGetBit(XStateBit.On, Direction);
					formula.Add(FormulaOperationType.AND);
					var pumpDelay = Delays.FirstOrDefault(x => x.Name == "Задержка пуска ШУН " + pumpDevice.DottedAddress);
					formula.AddGetBit(XStateBit.On, pumpDelay);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpDevice); // включить насос

					formula.AddGetBit(XStateBit.Off, Direction);
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
			var firstDelay = Delays.FirstOrDefault();
			if (firstDelay != null)
			{
				firstDelay.InputObjects.Add(Direction);
				Direction.OutputObjects.Add(firstDelay);
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