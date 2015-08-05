﻿using System;
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
		List<GKDevice> FirePumpDevices = new List<GKDevice>();
		GKDevice JNPumpDevice;
		List<PumpDelay> PumpDelays = new List<PumpDelay>();
		DatabaseType DatabaseType;

		public PumpStationCreator(CommonDatabase database, GKPumpStation pumpStation, DatabaseType dataBaseType)
		{
			Database = database;
			PumpStation = pumpStation;
			DatabaseType = dataBaseType;

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
			SetCrossReferences();
			CreateDelaysLogic();
			SetFirePumpDevicesLogic();
			//SetJokeyPumpLogic();
			CreatePim();
		}

		void CreateMainDelay()
		{
			var delayDescriptor = new DelayDescriptor(PumpStation.MainDelay, DatabaseType);
			Database.Descriptors.Add(delayDescriptor);
			PumpStation.LinkGKBases(PumpStation.MainDelay);
			PumpStation.MainDelay.LinkGKBases(PumpStation);

			var formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && delayDescriptor.GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !delayDescriptor.GKBase.IsLogicOnKau))
			{
				formula.Add(FormulaOperationType.END);
				delayDescriptor.FormulaBytes = formula.GetBytes();
				return;
			}

			formula.AddGetBit(GKStateBit.On, PumpStation, delayDescriptor.DatabaseType);
			formula.Add(FormulaOperationType.DUP);
			formula.AddGetBit(GKStateBit.TurningOn, PumpStation.MainDelay, delayDescriptor.DatabaseType);
			formula.AddGetBit(GKStateBit.On, PumpStation.MainDelay, delayDescriptor.DatabaseType);
			formula.Add(FormulaOperationType.OR);
			formula.Add(FormulaOperationType.COM);
			formula.Add(FormulaOperationType.AND);
			formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, PumpStation.MainDelay, delayDescriptor.DatabaseType);
			formula.Add(FormulaOperationType.COM);
			formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, PumpStation.MainDelay, delayDescriptor.DatabaseType);

			formula.Add(FormulaOperationType.END);
			delayDescriptor.Formula = formula;
			delayDescriptor.FormulaBytes = formula.GetBytes();
		}

		void CreateDelays()
		{
			for (int i = 1; i < FirePumpDevices.Count; i++)
			{
				var pumpDevice = FirePumpDevices[i];
				var delay = new GKDelay()
				{
					Name = "Задержка пуска ШУН " + pumpDevice.DottedAddress,
					DelayTime = (ushort)PumpStation.NSDeltaTime,
					Hold = 2,
					DelayRegime = DelayRegime.Off,
					IsAutoGenerated = true,
					PumpStationUID = PumpStation.UID
				};
				delay.UID = Guid.NewGuid(); //GuidHelper.CreateOn(pumpDevice.UID, 0);

				var pumpDelay = new PumpDelay
				{
					Delay = delay,
					Device = pumpDevice
				};
				PumpDelays.Add(pumpDelay);

				Database.AddDelay(delay);
				var delayDescriptor = new DelayDescriptor(delay, DatabaseType);
				Database.Descriptors.Add(delayDescriptor);
			}
		}

		void CreateDelaysLogic()
		{
			for (int i = 0; i < PumpDelays.Count; i++)
			{
				var pumpDelay = PumpDelays[i];
				var delayDescriptor = Database.Descriptors.FirstOrDefault(x => x.DescriptorType == DescriptorType.Delay && x.GKBase.UID == pumpDelay.Delay.UID);
				if (delayDescriptor == null)
					return;

				if ((DatabaseType == DatabaseType.Gk && delayDescriptor.GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !delayDescriptor.GKBase.IsLogicOnKau))
				{
					delayDescriptor.Formula.Add(FormulaOperationType.END);
					delayDescriptor.FormulaBytes = delayDescriptor.Formula.GetBytes();
					return;
				}

				var formula = new FormulaBuilder();
				AddCountFirePumpDevicesFormula(formula, delayDescriptor.DatabaseType);
				if (i > 0)
				{
					var prevDelay = PumpDelays[i - 1];
					formula.AddGetBit(GKStateBit.On, prevDelay.Delay, delayDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.AND);
				}

				formula.AddGetBit(GKStateBit.On, PumpStation, delayDescriptor.DatabaseType);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, pumpDelay.Delay, delayDescriptor.DatabaseType);

				formula.AddGetBit(GKStateBit.Off, PumpStation, delayDescriptor.DatabaseType);
				formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, pumpDelay.Delay, delayDescriptor.DatabaseType);

				formula.Add(FormulaOperationType.END);
				delayDescriptor.Formula = formula;
				delayDescriptor.FormulaBytes = formula.GetBytes();
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
						formula.AddGetBit(GKStateBit.On, pumpDelay.Delay, pumpDescriptor.DatabaseType);
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
			PumpStation.Pim.GetDataBaseParent();
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

			foreach (var pumpDelay in PumpDelays)
			{
				pumpDelay.Delay.LinkGKBases(PumpStation);
				foreach (var pumpDevice in FirePumpDevices)
				{
					pumpDelay.Delay.LinkGKBases(pumpDevice);
				}
			}

			foreach (var nsDevice in PumpStation.NSDevices)
			{
				foreach (var pumpDelay in PumpDelays)
				{
					if (pumpDelay.Device.UID == nsDevice.UID)
					{
						nsDevice.LinkGKBases(pumpDelay.Delay);
					}
				}
			}

			for (int i = 0; i < PumpDelays.Count; i++)
			{
				GKDelay prevDelay = null;
				GKDelay currentDelay = PumpDelays[i].Delay;
				GKDelay nextDelay = null;
				if (i > 0)
					prevDelay = PumpDelays[i - 1].Delay;
				if (i < PumpDelays.Count - 1)
					nextDelay = PumpDelays[i + 1].Delay;

				if (prevDelay != null)
					currentDelay.InputGKBases.Add(prevDelay);
				if (nextDelay != null)
					currentDelay.OutputGKBases.Add(nextDelay);
			}

			foreach (var firePumpDevice in FirePumpDevices)
			{
				foreach (var otherFirePumpDevice in FirePumpDevices)
				{
					firePumpDevice.LinkGKBases(otherFirePumpDevice);
				}
			}

			PumpDelays.ForEach(x => x.Delay.GetDataBaseParent());
		}
	}

	class PumpDelay
	{
		public GKDelay Delay { get; set; }
		public GKDevice Device { get; set; }
	}
}