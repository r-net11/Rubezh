using System;
using System.Collections.Generic;
using Common;
using XFiresecAPI;

namespace GKProcessor
{
	public class MPTDescriptor : BaseDescriptor
	{
		public MPTDescriptor(GkDatabase gkDatabase, XMPT mpt)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.MPT;
			MPT = mpt;
			MPT.Hold = 10;
			MPT.DelayRegime = DelayRegime.Off;
			Build();
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

			bool hasOnExpression = false;
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandAutomatic)
				{
					Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
					if (hasOnExpression)
						Formula.Add(FormulaOperationType.OR);
					hasOnExpression = true;
				}
			}
			if (MPT.UseDoorAutomatic)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
					{
						Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
						if (hasOnExpression)
							Formula.Add(FormulaOperationType.OR);
						hasOnExpression = true;
					}
				}
			}
			if (MPT.UseFailureAutomatic)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					Formula.AddGetBit(XStateBit.Failure, mptDevice.Device);
					if (hasOnExpression)
						Formula.Add(FormulaOperationType.OR);
					hasOnExpression = true;
				}
			}

			Formula.AddGetBit(XStateBit.On, MPT);
			if (hasOnExpression)
				Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(XStateBit.SetRegime_Manual, MPT);

			hasOnExpression = false;
			if (MPT.StartLogic.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(MPT.StartLogic.Clauses);
				Formula.AddGetBit(XStateBit.Norm, MPT);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный МПТ");
				hasOnExpression = true;
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandStart)
				{
					Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
					if (hasOnExpression)
						Formula.Add(FormulaOperationType.OR);
					hasOnExpression = true;
				}
			}

			if (hasOnExpression)
			{
				Formula.Add(FormulaOperationType.DUP);
				Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, MPT);
				Formula.Add(FormulaOperationType.COM);
			}

			var hasOffExpression = false;

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandStop)
				{
					Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
					if (hasOffExpression)
						Formula.Add(FormulaOperationType.OR);
					hasOffExpression = true;
				}
			}
			if (MPT.UseDoorStop)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
					{
						Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
						if (hasOffExpression)
							Formula.Add(FormulaOperationType.OR);
						hasOffExpression = true;
					}
				}
			}
			if (hasOffExpression)
			{
				if (hasOnExpression)
				{
					Formula.Add(FormulaOperationType.OR);
				}
				Formula.AddPutBit(XStateBit.TurnOff_InAutomatic, MPT);
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