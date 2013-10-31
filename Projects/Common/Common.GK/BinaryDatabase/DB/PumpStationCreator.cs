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
		List<XDevice> NonFirePumpDevices;
		XDevice AM1TDevice;
		List<PumpDelay> PumpDelays;
		ushort NSDeltaTime;
		ushort NSPumpsCount;

		public PumpStationCreator(GkDatabase gkDatabase, XDirection direction)
		{
			GkDatabase = gkDatabase;
			Direction = direction;
			NSDeltaTime = (ushort)direction.NSDeltaTime;
			NSPumpsCount = (ushort)direction.NSPumpsCount;

			PumpDelays = new List<PumpDelay>();

			FirePumpDevices = new List<XDevice>();
			NonFirePumpDevices = new List<XDevice>();
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
						else if (nsDevice.IntAddress == 12 || nsDevice.IntAddress == 14)
						{
							NonFirePumpDevices.Add(nsDevice);
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

			foreach (var pumpDelay in PumpDelays)
			{
				GkDatabase.AddDelay(pumpDelay.Delay);
				var delayBinaryObject = new DelayBinaryObject(pumpDelay.Delay);
				GkDatabase.BinaryObjects.Add(delayBinaryObject);
			}

			CreateDelaysLogic();
			SetFirePumpDevicesLogic();
			SetNonFirePumpDevicesLogic();
		}

		void CreateDirectionFormulaBytes()
		{
			var failureDevices = new List<XDevice>();
			failureDevices.AddRange(NonFirePumpDevices);
			if (AM1TDevice != null)
				failureDevices.Add(AM1TDevice);

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
				var pumpDelay = new PumpDelay
				{
					Delay = delay,
					Device = pumpDevice
				};

				PumpDelays.Add(pumpDelay);
				pumpIndex++;
			}
		}

		void CreateDelaysLogic()
		{
			bool firstDelay = true;
			for (int i = 0; i < PumpDelays.Count; i++)
			{
				var pumpDelay = PumpDelays[i];

				var delayBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Delay != null && x.Delay.UID == pumpDelay.Delay.UID);
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
                    formula.Add(FormulaOperationType.CONST, 0, NSPumpsCount, "Количество основных пожарных насосов");
                    formula.Add(FormulaOperationType.LT);

                    formula.AddClauseFormula(pumpDelay.Device.NSLogic);

                    formula.Add(FormulaOperationType.DUP);
                    formula.AddGetBit(XStateBit.On, Direction);
                    formula.Add(FormulaOperationType.AND);
                    formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpDelay.Delay);

                    formula.Add(FormulaOperationType.COM);
                    formula.AddGetBit(XStateBit.Off, Direction);
                    formula.Add(FormulaOperationType.OR);
                    formula.AddPutBit(XStateBit.TurnOff_InAutomatic, pumpDelay.Delay);
				}
				else
				{
					var prevDelay = PumpDelays[i - 1];
					formula.AddGetBit(XStateBit.On, prevDelay.Delay);
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpDelay.Delay);
					formula.AddGetBit(XStateBit.Off, prevDelay.Delay);
					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, pumpDelay.Delay);
				}
				firstDelay = false;

				formula.Add(FormulaOperationType.END);
				delayBinaryObject.Formula = formula;
				delayBinaryObject.FormulaBytes = formula.GetBytes();
			}
		}

		void SetFirePumpDevicesLogic()
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
					var pumpDelay = PumpDelays.FirstOrDefault(x => x.Device.UID == pumpDevice.UID);
					formula.AddGetBit(XStateBit.On, pumpDelay.Delay);
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

		void SetNonFirePumpDevicesLogic()
		{
			foreach (var pumpDevice in NonFirePumpDevices)
			{
				if (pumpDevice.IntAddress == 12)
				{
					var pumpBinaryObject = GkDatabase.BinaryObjects.FirstOrDefault(x => x.Device != null && x.Device.UID == pumpDevice.UID);
					if (pumpBinaryObject != null)
					{
						var formula = new FormulaBuilder();
						formula.AddGetBit(XStateBit.On, Direction);
						formula.Add(FormulaOperationType.DUP, 0, 0);
						formula.AddPutBit(XStateBit.TurnOff_InAutomatic, pumpBinaryObject.BinaryBase);
						formula.Add(FormulaOperationType.COM);
						formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpBinaryObject.BinaryBase);
						formula.Add(FormulaOperationType.END);
						pumpBinaryObject.Formula = formula;
						pumpBinaryObject.FormulaBytes = formula.GetBytes();
					}
					XManager.LinkBinaryObjects(pumpBinaryObject.BinaryBase, Direction);
				}
			}
		}

		void SetCrossReferences()
		{
			var firstDelay = PumpDelays.FirstOrDefault();
			if (firstDelay != null)
			{
				XManager.LinkBinaryObjects(firstDelay.Delay, Direction);
			}

			foreach (var pumpDevice in FirePumpDevices)
			{
				foreach (var pumpDelay in PumpDelays)
				{
					if (pumpDelay.Device.UID == pumpDevice.UID)
					{
						XManager.LinkBinaryObjects(pumpDevice, pumpDelay.Delay);
					}
					XManager.LinkBinaryObjects(pumpDevice, Direction);
				}
			}

			for (int i = 0; i < PumpDelays.Count; i++)
			{
				XDelay prevDelay = null;
				XDelay currentDelay = PumpDelays[i].Delay;
				XDelay nextDelay = null;
				if (i > 0)
					prevDelay = PumpDelays[i - 1].Delay;
				if (i < PumpDelays.Count - 1)
					nextDelay = PumpDelays[i + 1].Delay;

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

	class PumpDelay
	{
		public XDelay Delay { get; set; }
		public XDevice Device { get; set; }
	}
}