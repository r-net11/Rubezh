using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI;

namespace GKProcessor
{
	public class MPTDescriptor : BaseDescriptor
	{
		GKMPT MPT { get; set; }
		GKPim GlobalPim { get; set; }

		public MPTDescriptor(GKPim globalPim, GKMPT mpt)
			: base(mpt)
		{
			GlobalPim = globalPim;
			DescriptorType = DescriptorType.MPT;
			MPT = mpt;
			MPT.DelayRegime = DelayRegime.On;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x106);
			SetAddress(0);
			SetPropertiesBytes();
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}

			MPT.LinkToDescriptor(GlobalPim);
			Formula.AddGetBit(GKStateBit.On, GlobalPim);
			Formula.Add(FormulaOperationType.BR, 2, 1);
			Formula.Add(FormulaOperationType.EXIT);

			var mirrorParents = MPT.GetMirrorParents();
			Formula.AddMirrorLogic(MPT, mirrorParents);

			if (MPT.MptLogic.StopClausesGroup.GetObjects().Count > 0)
			{
				Formula.AddClauseFormula(MPT.MptLogic.StopClausesGroup);
				if (MPT.MptLogic.OnClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.DUP);
				Formula.AddPutBit(GKStateBit.Stop_InManual, MPT);
			}

			if (MPT.MptLogic.OnClausesGroup.GetObjects().Count > 0)
			{
				if (MPT.MptLogic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.COM);
				Formula.AddClauseFormula(MPT.MptLogic.OnClausesGroup);
				if (MPT.MptLogic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, MPT);
				if (MPT.MptLogic.UseOffCounterLogic)
				{
					Formula.AddClauseFormula(MPT.MptLogic.OnClausesGroup);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, MPT);
				}
			}

			if (!MPT.MptLogic.UseOffCounterLogic && MPT.MptLogic.OffClausesGroup.GetObjects().Count > 0)
			{
				Formula.AddClauseFormula(MPT.MptLogic.OffClausesGroup);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, MPT);
			}

			SetRegime(GKMPTDeviceType.HandStart, GKStateBit.TurnOn_InManual);
			SetRegime(GKMPTDeviceType.HandStop, GKStateBit.TurnOff_InManual);
			SetRegime(GKMPTDeviceType.HandAutomaticOn, GKStateBit.SetRegime_Automatic);
			SetRegime(GKMPTDeviceType.HandAutomaticOff, GKStateBit.SetRegime_Manual);

			Formula.Add(FormulaOperationType.END);
		}

		void SetRegime(GKMPTDeviceType deviceType, GKStateBit stateBit)
		{
			var count = 0;
			foreach (var mptDevice in MPT.MPTDevices.Where(x => x.MPTDeviceType == deviceType && x.Device != null))
			{
				if (mptDevice.Device.Driver.IsCardReaderOrCodeReader)
				{
					if (mptDevice.CodeReaderSettings.MPTSettings.CanBeUsed)
					{
						FormulaHelper.AddCodeReaderLogic(Formula, mptDevice.CodeReaderSettings.MPTSettings, mptDevice.Device);
						if (count > 0)
							Formula.Add(FormulaOperationType.OR);
						count++;
					}
				}
				else
				{
					Formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device);
					if (count > 0)
						Formula.Add(FormulaOperationType.OR);
					count++;
				}
			}
			if (count > 0)
			{
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(stateBit, MPT);
				Formula.Add(FormulaOperationType.EXIT);
			}
		}

		void SetPropertiesBytes()
		{
			Parameters = new List<byte>();
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