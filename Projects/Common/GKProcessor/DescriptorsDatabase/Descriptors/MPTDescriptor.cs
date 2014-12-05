﻿using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class MPTDescriptor : BaseDescriptor
	{
		public GKPim HandAutomaticOffPim { get; private set; }
		public GKPim DoorAutomaticOffPim { get; private set; }
		public GKPim FailureAutomaticOffPim { get; private set; }

		public MPTDescriptor(GkDatabase gkDatabase, GKMPT mpt)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.MPT;
			MPT = mpt;
			MPT.Hold = 10;
			MPT.DelayRegime = DelayRegime.On;

			HandAutomaticOffPim = new GKPim();
			HandAutomaticOffPim.IsAutoGenerated = true;
			HandAutomaticOffPim.MPTUID = MPT.UID;
			HandAutomaticOffPim.UID = GuidHelper.CreateOn(MPT.UID, 0);
			gkDatabase.AddPim(HandAutomaticOffPim);

			DoorAutomaticOffPim = new GKPim();
			DoorAutomaticOffPim.IsAutoGenerated = true;
			DoorAutomaticOffPim.MPTUID = MPT.UID;
			DoorAutomaticOffPim.UID = GuidHelper.CreateOn(MPT.UID, 1);
			gkDatabase.AddPim(DoorAutomaticOffPim);

			FailureAutomaticOffPim = new GKPim();
			FailureAutomaticOffPim.IsAutoGenerated = true;
			FailureAutomaticOffPim.MPTUID = MPT.UID;
			FailureAutomaticOffPim.UID = GuidHelper.CreateOn(MPT.UID, 2);
			gkDatabase.AddPim(FailureAutomaticOffPim);
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x106);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();

			var hasOnExpression = false;
			if (MPT.StartLogic.OnClausesGroup.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(MPT.StartLogic.OnClausesGroup, DatabaseType);
				hasOnExpression = true;
			}

			if (MPT.UseDoorStop)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == GKMPTDeviceType.Door)
					{
						Formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device, DatabaseType);
						Formula.AddGetBit(GKStateBit.TurningOn, MPT, DatabaseType);
						Formula.Add(FormulaOperationType.AND);
						Formula.Add(FormulaOperationType.COM);
						if (hasOnExpression)
							Formula.Add(FormulaOperationType.AND);
						hasOnExpression = true;
					}
				}
			}
			if (hasOnExpression)
			{
				Formula.AddGetBit(GKStateBit.Norm, MPT, DatabaseType);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный МПТ");

				Formula.AddGetBit(GKStateBit.Norm, HandAutomaticOffPim, DatabaseType);
				Formula.Add(FormulaOperationType.AND, comment: "");

				Formula.AddGetBit(GKStateBit.Norm, DoorAutomaticOffPim, DatabaseType);
				Formula.Add(FormulaOperationType.AND, comment: "");

				Formula.AddGetBit(GKStateBit.Norm, FailureAutomaticOffPim, DatabaseType);
				Formula.Add(FormulaOperationType.AND, comment: "");
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.HandStart)
				{
					Formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device, DatabaseType);
					if (hasOnExpression)
						Formula.Add(FormulaOperationType.OR);
					hasOnExpression = true;
				}
			}

			if (hasOnExpression)
			{
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, MPT, DatabaseType);
			}

			var hasOffExpression = false;
			if (MPT.UseDoorStop)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == GKMPTDeviceType.Door)
					{
						Formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device, DatabaseType);
						Formula.AddGetBit(GKStateBit.TurningOn, MPT, DatabaseType);
						Formula.Add(FormulaOperationType.AND);
						if (hasOffExpression)
							Formula.Add(FormulaOperationType.OR);
						hasOffExpression = true;
					}
				}
			}

			if (hasOffExpression)
			{
				Formula.AddGetBit(GKStateBit.Norm, MPT, DatabaseType);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный МПТ");

				Formula.AddGetBit(GKStateBit.Norm, HandAutomaticOffPim, DatabaseType);
				Formula.Add(FormulaOperationType.AND, comment: "");

				//Formula.AddGetBit(GKStateBit.Norm, DoorAutomaticOffPim);
				//Formula.Add(FormulaOperationType.AND, comment: "");

				Formula.AddGetBit(GKStateBit.Norm, FailureAutomaticOffPim, DatabaseType);
				Formula.Add(FormulaOperationType.AND, comment: "");
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.HandStop)
				{
					Formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device, DatabaseType);
					if (hasOffExpression)
						Formula.Add(FormulaOperationType.OR);
					hasOffExpression = true;
				}
			}
			if (hasOffExpression)
			{
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, MPT, DatabaseType);
			}

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void SetPropertiesBytes()
		{
			var binProperties = new List<BinProperty>();
			binProperties.Add(new BinProperty()
			{
				No = 0,
				Value = (ushort)MPT.Delay
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = (ushort)MPT.Hold
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)MPT.DelayRegime
			});

			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}
	}
}