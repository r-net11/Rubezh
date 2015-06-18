using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class PumpStationCreator
	{
		CommonDatabase Database;
		GKPumpStation PumpStation;
		List<GKDevice> FirePumpDevices;
		GKDevice JNPumpDevice;
		List<PumpDelay> PumpDelays;
		DatabaseType DatabaseType;

		public PumpStationCreator(CommonDatabase database, GKPumpStation pumpStation, DatabaseType dataBaseType)
		{
			Database = database;
			PumpStation = pumpStation;
			DatabaseType = dataBaseType;

			PumpDelays = new List<PumpDelay>();

			FirePumpDevices = new List<GKDevice>();
			foreach (var nsDevice in pumpStation.NSDevices)
			{
				if (nsDevice.DriverType == GKDriverType.RSR2_Bush_Fire)
					FirePumpDevices.Add(nsDevice);
				if (nsDevice.DriverType == GKDriverType.RSR2_Bush_Jokey)
					JNPumpDevice = nsDevice;
			}
		}

		public void Create()
		{
			CreateMainDelay();
			CreateDelays();
			CreateDelaysLogic();
			SetFirePumpDevicesLogic();
			//SetJokeyPumpLogic();
			CreatePim();
			SetCrossReferences();
		}

		void CreateMainDelay()
		{
			var formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk) || (DatabaseType == DatabaseType.Kau))
			{
				formula.Add(FormulaOperationType.END);
				return;
			}

			formula.Add(FormulaOperationType.DUP);
			formula.Add(FormulaOperationType.OR);
			formula.Add(FormulaOperationType.COM);
			formula.Add(FormulaOperationType.AND);
			formula.Add(FormulaOperationType.COM);

			formula.Add(FormulaOperationType.END);
		}

		void CreateDelays()
		{
			for (int i = 1; i < FirePumpDevices.Count; i++)
			{
				var pumpDevice = FirePumpDevices[i];

				var pumpDelay = new PumpDelay
				{
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
				var formula = new FormulaBuilder();

				if (i > 0)
				{
					var prevDelay = PumpDelays[i - 1];
					formula.Add(FormulaOperationType.AND);
				}
				formula.Add(FormulaOperationType.AND);

				formula.Add(FormulaOperationType.END);
			}
		}

		void SetFirePumpDevicesLogic()
		{
			for (int i = 0; i < FirePumpDevices.Count; i++)
			{
				var pumpDevice = FirePumpDevices[i];
				var pumpDescriptor = Database.Descriptors.FirstOrDefault(x => x.DescriptorType == DescriptorType.Device && x.GKBase.UID == pumpDevice.UID);
				if (pumpDescriptor != null)
				{
					var formula = new FormulaBuilder();
					AddCountFirePumpDevicesFormula(formula, pumpDescriptor.DatabaseType);
					if (i > 0)
					{
						var pumpDelay = PumpDelays.FirstOrDefault(x => x.Device.UID == pumpDevice.UID);
						formula.Add(FormulaOperationType.AND);
					}

					if (pumpDevice.NSLogic.OnClausesGroup.Clauses.Count > 0)
					{
						formula.AddClauseFormula(pumpDevice.NSLogic.OnClausesGroup, pumpDescriptor.DatabaseType);
						formula.Add(FormulaOperationType.AND);
					}

					formula.AddGetBit(GKStateBit.On, pumpDevice, pumpDescriptor.DatabaseType);
					formula.AddGetBit(GKStateBit.TurningOn, pumpDevice, pumpDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.OR);
					formula.AddGetBit(GKStateBit.Failure, pumpDevice, pumpDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.OR);
					formula.Add(FormulaOperationType.COM);
					formula.Add(FormulaOperationType.AND, comment: "Запрет на включение, если насос включен и не включается");

					formula.AddGetBit(GKStateBit.On, PumpStation, pumpDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, pumpDevice, pumpDescriptor.DatabaseType);

					formula.AddGetBit(GKStateBit.Off, PumpStation, pumpDescriptor.DatabaseType);
					formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, pumpDevice, pumpDescriptor.DatabaseType);

					formula.Add(FormulaOperationType.END);
					pumpDescriptor.Formula = formula;
					pumpDescriptor.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void AddCountFirePumpDevicesFormula(FormulaBuilder formula, DatabaseType databaseType)
		{
			var inputPumpsCount = 0;
			foreach (var firePumpDevice in FirePumpDevices)
			{
				formula.AddGetBit(GKStateBit.TurningOn, firePumpDevice, databaseType);
				formula.AddGetBit(GKStateBit.On, firePumpDevice, databaseType);
				formula.Add(FormulaOperationType.OR);
				if (inputPumpsCount > 0)
				{
					formula.Add(FormulaOperationType.ADD);
				}
				inputPumpsCount++;
			}
			formula.Add(FormulaOperationType.CONST, 0, (ushort)PumpStation.NSPumpsCount, comment: "Количество основных пожарных насосов");
			formula.Add(FormulaOperationType.LT);
		}

		void SetJokeyPumpLogic()
		{
			if (JNPumpDevice != null)
			{
				var jnDescriptor = Database.Descriptors.FirstOrDefault(x => x.DescriptorType == DescriptorType.Device && x.GKBase.UID == JNPumpDevice.UID);
				if (jnDescriptor != null)
				{
					var formula = new FormulaBuilder();
					formula.AddGetBit(GKStateBit.On, PumpStation, jnDescriptor.DatabaseType);
					//formula.AddGetBit(GKStateBit.ForbidStart_InAutomatic, JNPumpDevice);
					formula.Add(FormulaOperationType.END);
					jnDescriptor.Formula = formula;
					jnDescriptor.FormulaBytes = formula.GetBytes();
				}
				jnDescriptor.GKBase.LinkGKBases(PumpStation);
			}
		}

		void CreatePim()
		{
			Database.AddPim(PumpStation.Pim);
			var pimDescriptor = new PimDescriptor(PumpStation.Pim, DatabaseType);
			Database.Descriptors.Add(pimDescriptor);

			var formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && pimDescriptor.GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !pimDescriptor.GKBase.IsLogicOnKau))
			{
				formula.Add(FormulaOperationType.END);
				pimDescriptor.FormulaBytes = formula.GetBytes();
				return;
			}
			var inputDevices = new List<GKBase>();
			inputDevices.AddRange(PumpStation.ClauseInputDevices);
			foreach (var nsDevice in PumpStation.NSDevices)
			{
				if (!inputDevices.Contains(nsDevice))
					inputDevices.Add(nsDevice);
			}
			foreach (var inputDevice in inputDevices)
			{
				PumpStation.Pim.LinkGKBases(inputDevice);
			}
			for (int i = 0; i < inputDevices.Count; i++)
			{
				var nsDevice = inputDevices[i];
				formula.AddGetBit(GKStateBit.Failure, nsDevice, pimDescriptor.DatabaseType);
				if (i > 0)
				{
					formula.Add(FormulaOperationType.OR);
				}
			}
			formula.AddPutBit(GKStateBit.Failure, PumpStation.Pim, pimDescriptor.DatabaseType);

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void SetCrossReferences()
		{
			foreach (var nsDevice in PumpStation.NSDevices)
			{
				nsDevice.LinkGKBases(PumpStation);
			}

			foreach (var firePumpDevice in FirePumpDevices)
			{
				foreach (var otherFirePumpDevice in FirePumpDevices)
				{
					firePumpDevice.LinkGKBases(otherFirePumpDevice);
				}
			}
		}
	}

	class PumpDelay
	{
		public GKDevice Device { get; set; }
	}
}