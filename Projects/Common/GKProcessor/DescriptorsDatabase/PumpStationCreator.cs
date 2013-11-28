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
		XPim Pim;
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
			CreateDelays();
			CreateDelaysLogic();
			SetFirePumpDevicesLogic();
			SetNonFirePumpDevicesLogic();
			CreatePim();
			SetCrossReferences();
		}

		void CreateDelays()
		{
			for (int i = 0; i < FirePumpDevices.Count; i++)
			{
				var pumpDevice = FirePumpDevices[i];
				var delayTime = NSDeltaTime;
				if (i == 0)
					delayTime = 0;
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

				GkDatabase.AddDelay(delay);
				var delayDescriptor = new DelayDescriptor(delay);
				GkDatabase.Descriptors.Add(delayDescriptor);
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
				if (i > 0)
				{
					var prevDelay = PumpDelays[i - 1];
					formula.AddGetBit(XStateBit.On, prevDelay.Delay);

					for (int j = 0; j < i; j++)
					{
						prevDelay = PumpDelays[j];
						formula.AddGetBit(XStateBit.Failure, prevDelay.Device);
						formula.AddGetBit(XStateBit.Norm, prevDelay.Device);
						formula.Add(FormulaOperationType.COM);
						formula.Add(FormulaOperationType.OR);
						if (j > 0)
						{
							formula.Add(FormulaOperationType.AND, comment: "Неисправности всех предидущих насососов");
						}
					}
					formula.Add(FormulaOperationType.OR);

					formula.Add(FormulaOperationType.AND);
				}

				formula.AddGetBit(XStateBit.On, Direction);
				formula.Add(FormulaOperationType.AND);

				formula.AddGetBit(XStateBit.Norm, pumpDelay.Delay);
				formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Задержки");
				formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpDelay.Delay);

				formula.AddGetBit(XStateBit.Off, Direction);
				formula.AddGetBit(XStateBit.Norm, pumpDelay.Delay);
				formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Задержки");
				formula.AddPutBit(XStateBit.TurnOff_InAutomatic, pumpDelay.Delay);

				formula.Add(FormulaOperationType.END);
				delayDescriptor.Formula = formula;
				delayDescriptor.FormulaBytes = formula.GetBytes();
			}
		}

		void SetFirePumpDevicesLogic()
		{
			for (int i = 0; i < FirePumpDevices.Count; i++ )
				{
					var pumpDevice = FirePumpDevices[i];
					var pumpDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == pumpDevice.UID);
					if (pumpDescriptor != null)
					{
						var formula = new FormulaBuilder();

						var pumpDelay = PumpDelays.FirstOrDefault(x => x.Device.UID == pumpDevice.UID);
						formula.AddGetBit(XStateBit.On, pumpDelay.Delay);

						if (i > 0)
						{
							for (int j = 0; j < i; j++)
							{
								var prevFirePumpDevice = FirePumpDevices[j];
								formula.AddGetBit(XStateBit.Failure, prevFirePumpDevice);
								formula.AddGetBit(XStateBit.Norm, prevFirePumpDevice);
								formula.Add(FormulaOperationType.COM);
								formula.Add(FormulaOperationType.OR);
								if (j > 0)
								{
									formula.Add(FormulaOperationType.AND, comment: "Неисправности всех предидущих насососов");
								}
							}
							formula.Add(FormulaOperationType.OR);
						}

						AddCountFirePumpDevicesFormula(formula);
						formula.Add(FormulaOperationType.AND);

						if (pumpDevice.NSLogic.Clauses.Count > 0)
						{
							formula.AddClauseFormula(pumpDevice.NSLogic);
							formula.Add(FormulaOperationType.AND);
						}

						formula.AddGetBit(XStateBit.On, pumpDevice);
						formula.AddGetBit(XStateBit.TurningOn, pumpDevice);
						formula.Add(FormulaOperationType.OR);
						formula.AddGetBit(XStateBit.Failure, pumpDevice);
						formula.Add(FormulaOperationType.OR);
						formula.Add(FormulaOperationType.COM);
						formula.Add(FormulaOperationType.AND, comment: "Запрет на включение, если насос включен и не включается");

						formula.AddGetBit(XStateBit.On, Direction);
						formula.Add(FormulaOperationType.AND);

						formula.AddGetBit(XStateBit.Norm, pumpDevice);
						formula.Add(FormulaOperationType.AND);
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
					UpdateConfigurationHelper.LinkXBasees(pumpDescriptor.XBase, Direction);
				}
			}
		}

		void CreatePim()
		{
			Pim = new XPim()
			{
				Name = Direction.PresentationName,
				DelayTime = 1,
				SetTime = 1,
				DelayRegime = DelayRegime.Off
			};
			GkDatabase.AddPim(Pim);
			var pimDescriptor = new PimDescriptor(Pim);
			GkDatabase.Descriptors.Add(pimDescriptor);

			var formula = new FormulaBuilder();

			var nsDevicesCount = 0;
			foreach (var nsDevice in Direction.NSDevices)
			{
				formula.AddGetBit(XStateBit.Failure, nsDevice);
				if (nsDevicesCount > 0)
				{
					formula.Add(FormulaOperationType.OR);
				}
				nsDevicesCount++;
			}
			formula.AddPutBit(XStateBit.Failure, Pim);

			nsDevicesCount = 0;
			foreach (var nsDevice in Direction.NSDevices)
			{
				formula.AddGetBit(XStateBit.Norm, nsDevice);
				if (nsDevicesCount > 0)
				{
					formula.Add(FormulaOperationType.AND);
				}
				nsDevicesCount++;
			}

			formula.Add(FormulaOperationType.DUP);
			formula.AddPutBit(XStateBit.SetRegime_Automatic, Pim);
			formula.Add(FormulaOperationType.COM);
			formula.AddPutBit(XStateBit.SetRegime_Manual, Pim);

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void SetCrossReferences()
		{
			foreach (var failureDevice in NonFirePumpDevices)
			{
				UpdateConfigurationHelper.LinkXBasees(Direction, failureDevice);
			}
			if (AM1TDevice != null)
			{
				UpdateConfigurationHelper.LinkXBasees(Direction, AM1TDevice);
			}

			foreach (var pumpDelay in PumpDelays)
			{
				UpdateConfigurationHelper.LinkXBasees(pumpDelay.Delay, Direction);
				foreach (var pumpDevice in FirePumpDevices)
				{
					UpdateConfigurationHelper.LinkXBasees(pumpDelay.Delay, pumpDevice);
				}
			}

			foreach (var nsDevice in Direction.NSDevices)
			{
				UpdateConfigurationHelper.LinkXBasees(nsDevice, Direction);
				foreach (var pumpDelay in PumpDelays)
				{
					if (pumpDelay.Device.UID == nsDevice.UID)
					{
						UpdateConfigurationHelper.LinkXBasees(nsDevice, pumpDelay.Delay);
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

			foreach (var nsDevice in Direction.NSDevices)
			{
				UpdateConfigurationHelper.LinkXBasees(Pim, nsDevice);
			}

			foreach (var firePumpDevice in FirePumpDevices)
			{
				foreach (var otherFirePumpDevice in FirePumpDevices)
				{
					if (firePumpDevice.UID != otherFirePumpDevice.UID)
					{
						UpdateConfigurationHelper.LinkXBasees(firePumpDevice, otherFirePumpDevice);
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