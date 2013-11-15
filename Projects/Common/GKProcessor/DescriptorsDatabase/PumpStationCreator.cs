using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace GKProcessor
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
				switch (nsDevice.DriverType)
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
				var delayDescriptor = new DelayDescriptor(pumpDelay.Delay);
				GkDatabase.Descriptors.Add(delayDescriptor);
			}

			CreateDelaysLogic();
			SetFirePumpDevicesLogic();
			SetNonFirePumpDevicesLogic();
		}

		void CreateDirectionFormulaBytes()
		{
			foreach (var failureDevice in NonFirePumpDevices)
			{
				XManager.LinkXBasees(Direction, failureDevice);
			}
			XManager.LinkXBasees(Direction, AM1TDevice);
		}

		void CreateDelays()
		{
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
					DelayRegime = DelayRegime.Off
				};
				var pumpDelay = new PumpDelay
				{
					Delay = delay,
					Device = pumpDevice
				};

				PumpDelays.Add(pumpDelay);
			}
		}

		void CreateDelaysLogic()
		{
			for (int i = 0; i < PumpDelays.Count; i++)
			{
				var pumpDelay = PumpDelays[i];
				var delayDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Delay != null && x.Delay.UID == pumpDelay.Delay.UID);
				var formula = new FormulaBuilder();

				AddCountFirePumpDevicesFormula(formula);
				formula.AddGetBit(XStateBit.On, Direction);
				formula.Add(FormulaOperationType.AND);
				if (i > 0)
				{
					var prevDelay = PumpDelays[i - 1];
					formula.AddGetBit(XStateBit.On, prevDelay.Delay);
					formula.Add(FormulaOperationType.AND);
				}
				formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpDelay.Delay);

				formula.AddGetBit(XStateBit.Off, Direction);
				formula.AddGetBit(XStateBit.Norm, pumpDelay.Delay);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(XStateBit.TurnOff_InAutomatic, pumpDelay.Delay);

				formula.Add(FormulaOperationType.END);
				delayDescriptor.Formula = formula;
				delayDescriptor.FormulaBytes = formula.GetBytes();
			}
		}

		void AddCountFirePumpDevicesFormula(FormulaBuilder formula)
		{
			var inputPumpsCount = 0;
			foreach (var firePumpDevice in FirePumpDevices)
			{
				formula.AddGetBit(XStateBit.TurningOn, firePumpDevice);
				formula.AddGetBit(XStateBit.On, firePumpDevice);
				formula.Add(FormulaOperationType.OR);
				if (inputPumpsCount > 0)
				{
					formula.Add(FormulaOperationType.ADD);
				}
				inputPumpsCount++;
			}
			formula.Add(FormulaOperationType.CONST, 0, NSPumpsCount, "Количество основных пожарных насосов");
			formula.Add(FormulaOperationType.LT);
		}

		void SetFirePumpDevicesLogic()
		{
			foreach (var pumpDevice in FirePumpDevices)
			{
				var pumpDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == pumpDevice.UID);
				if (pumpDescriptor != null)
				{
					var formula = new FormulaBuilder();
					var inputPumpsCount = 0;
					foreach (var otherPumpDevice in FirePumpDevices)
					{
						formula.AddGetBit(XStateBit.TurningOn, otherPumpDevice);
						formula.AddGetBit(XStateBit.On, otherPumpDevice);
						formula.Add(FormulaOperationType.OR);
						if (inputPumpsCount > 0)
						{
							formula.Add(FormulaOperationType.ADD);
						}
						inputPumpsCount++;
					}
					formula.Add(FormulaOperationType.CONST, 0, NSPumpsCount, "Количество основных пожарных насосов");
					formula.Add(FormulaOperationType.LT);
					formula.AddGetBit(XStateBit.Norm, pumpDevice);
					formula.Add(FormulaOperationType.AND);

					formula.AddGetBit(XStateBit.On, pumpDevice);
					formula.AddGetBit(XStateBit.TurningOn, pumpDevice);
					formula.Add(FormulaOperationType.OR);
					formula.Add(FormulaOperationType.COM);
					formula.Add(FormulaOperationType.AND, comment:"Запрет на включение, если насос включен и не включается");


					formula.AddGetBit(XStateBit.On, Direction);
					formula.Add(FormulaOperationType.AND);
					var pumpDelay = PumpDelays.FirstOrDefault(x => x.Device.UID == pumpDevice.UID);
					formula.AddGetBit(XStateBit.On, pumpDelay.Delay);
					formula.Add(FormulaOperationType.AND);

					if (pumpDevice.NSLogic.Clauses.Count > 0)
					{
						formula.AddClauseFormula(pumpDevice.NSLogic);
						formula.Add(FormulaOperationType.AND);
					}

					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpDevice);

					formula.AddGetBit(XStateBit.Off, Direction);
					formula.AddGetBit(XStateBit.Norm, pumpDevice);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, pumpDevice);

					formula.Add(FormulaOperationType.END);

					pumpDescriptor.Formula = formula;
					pumpDescriptor.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void SetNonFirePumpDevicesLogic()
		{
			foreach (var pumpDevice in NonFirePumpDevices)
			{
				if (pumpDevice.IntAddress == 12)
				{
					var pumpDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == pumpDevice.UID);
					if (pumpDescriptor != null)
					{
						var formula = new FormulaBuilder();
						formula.AddGetBit(XStateBit.On, Direction);
						formula.Add(FormulaOperationType.COM);
						formula.AddStandardTurning(pumpDevice);
						formula.Add(FormulaOperationType.END);
						pumpDescriptor.Formula = formula;
						pumpDescriptor.FormulaBytes = formula.GetBytes();
					}
					XManager.LinkXBasees(pumpDescriptor.XBase, Direction);
				}
			}
		}

		void SetCrossReferences()
		{
			foreach (var pumpDelay in PumpDelays)
			{
				XManager.LinkXBasees(pumpDelay.Delay, Direction);
				foreach (var pumpDevice in FirePumpDevices)
				{
					XManager.LinkXBasees(pumpDelay.Delay, pumpDevice);
				}
			}

			foreach (var pumpDevice in FirePumpDevices)
			{
				XManager.LinkXBasees(pumpDevice, Direction);
				foreach (var pumpDelay in PumpDelays)
				{
					if (pumpDelay.Device.UID == pumpDevice.UID)
					{
						XManager.LinkXBasees(pumpDevice, pumpDelay.Delay);
					}
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
					currentDelay.InputXBases.Add(prevDelay);
				if (nextDelay != null)
					currentDelay.OutputXBases.Add(nextDelay);
			}
		}
	}

	class PumpDelay
	{
		public XDelay Delay { get; set; }
		public XDevice Device { get; set; }
	}
}