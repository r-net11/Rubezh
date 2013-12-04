using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace GKProcessor
{
	public class PumpStationCreator2
	{
		GkDatabase GkDatabase;
		XPumpStation PumpStation;
		List<XDevice> FirePumpDevices;
		List<XDevice> NonFirePumpDevices;
		List<PumpDelay> PumpDelays;
		XPim Pim;

		public PumpStationCreator2(GkDatabase gkDatabase, XPumpStation pumpStation)
		{
			GkDatabase = gkDatabase;
			PumpStation = pumpStation;

			PumpDelays = new List<PumpDelay>();

			FirePumpDevices = new List<XDevice>();
			NonFirePumpDevices = new List<XDevice>();
			foreach (var nsDevice in pumpStation.NSDevices)
			{
				switch (nsDevice.DriverType)
				{
					case XDriverType.Pump:
					case XDriverType.RSR2_Bush:
						if (nsDevice.IntAddress <= 8)
						{
							FirePumpDevices.Add(nsDevice);
						}
						else if (nsDevice.IntAddress == 12)
						{
							NonFirePumpDevices.Add(nsDevice);
						}
						break;
				}
			}
		}

		public void Create()
		{
			CreateDelays();
			CreateDelaysLogic();
			SetFirePumpDevicesLogic();
			//SetJokeyPumpLogic();
			CreatePim();
			SetCrossReferences();
		}

		void CreateDelays()
		{
			for (int i = 0; i < FirePumpDevices.Count; i++)
			{
				var pumpDevice = FirePumpDevices[i];
				var delayTime = PumpStation.NSDeltaTime;
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

				formula.AddGetBit(XStateBit.On, PumpStation);
				formula.Add(FormulaOperationType.AND);

				formula.AddGetBit(XStateBit.Norm, pumpDelay.Delay);
				formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Задержки");
				formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpDelay.Delay);

				formula.AddGetBit(XStateBit.Off, PumpStation);
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

						formula.AddGetBit(XStateBit.On, PumpStation);
						formula.Add(FormulaOperationType.AND);

						formula.AddGetBit(XStateBit.Norm, pumpDevice);
						formula.Add(FormulaOperationType.AND);
						formula.AddPutBit(XStateBit.TurnOn_InAutomatic, pumpDevice);

						formula.AddGetBit(XStateBit.Off, PumpStation);
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
			formula.Add(FormulaOperationType.CONST, 0, (ushort)PumpStation.NSPumpsCount, "Количество основных пожарных насосов");
			formula.Add(FormulaOperationType.LT);
		}

		void SetJokeyPumpLogic()
		{
			foreach (var pumpDevice in NonFirePumpDevices)
			{
				var pumpDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == pumpDevice.UID);
				if (pumpDescriptor != null)
				{
					var formula = new FormulaBuilder();
					formula.AddGetBit(XStateBit.On, PumpStation);
					formula.Add(FormulaOperationType.COM);
					formula.AddStandardTurning(pumpDevice);
					formula.Add(FormulaOperationType.END);
					pumpDescriptor.Formula = formula;
					pumpDescriptor.FormulaBytes = formula.GetBytes();
				}
				UpdateConfigurationHelper.LinkXBases(pumpDescriptor.XBase, PumpStation);
			}
		}

		void CreatePim()
		{
			Pim = new XPim()
			{
				Name = PumpStation.PresentationName,
				DelayTime = 1,
				SetTime = 1,
				DelayRegime = DelayRegime.Off
			};
			GkDatabase.AddPim(Pim);
			var pimDescriptor = new PimDescriptor(Pim);
			GkDatabase.Descriptors.Add(pimDescriptor);

			var formula = new FormulaBuilder();

			var inputDevices = new List<XBase>();
			inputDevices.AddRange(PumpStation.InputDevices);
			foreach (var nsDevice in PumpStation.NSDevices)
			{
				if (!inputDevices.Contains(nsDevice))
					inputDevices.Add(nsDevice);
			}
			foreach (var inputDevice in inputDevices)
			{
				UpdateConfigurationHelper.LinkXBases(Pim, inputDevice);
			}
			for (int i = 0; i < inputDevices.Count; i++)
			{
				var nsDevice = inputDevices[i];
				formula.AddGetBit(XStateBit.Failure, nsDevice);
				if (i > 0)
				{
					formula.Add(FormulaOperationType.OR);
				}
			}
			formula.AddPutBit(XStateBit.Failure, Pim);

			for (int i = 0; i < inputDevices.Count; i++)
			{
				var nsDevice = inputDevices[i];
				formula.AddGetBit(XStateBit.Norm, nsDevice);
				if (i > 0)
				{
					formula.Add(FormulaOperationType.AND);
				}
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
			foreach (var nsDevice in PumpStation.NSDevices)
			{
				if (nsDevice.IntAddress <= 8 || nsDevice.IntAddress == 12)
					UpdateConfigurationHelper.LinkXBases(nsDevice, PumpStation);
			}

			foreach (var pumpDelay in PumpDelays)
			{
				UpdateConfigurationHelper.LinkXBases(pumpDelay.Delay, PumpStation);
				foreach (var pumpDevice in FirePumpDevices)
				{
					UpdateConfigurationHelper.LinkXBases(pumpDelay.Delay, pumpDevice);
				}
			}

			foreach (var nsDevice in PumpStation.NSDevices)
			{
				foreach (var pumpDelay in PumpDelays)
				{
					if (pumpDelay.Device.UID == nsDevice.UID)
					{
						UpdateConfigurationHelper.LinkXBases(nsDevice, pumpDelay.Delay);
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

			foreach (var firePumpDevice in FirePumpDevices)
			{
				foreach (var otherFirePumpDevice in FirePumpDevices)
				{
					if (firePumpDevice.UID != otherFirePumpDevice.UID)
					{
						UpdateConfigurationHelper.LinkXBases(firePumpDevice, otherFirePumpDevice);
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