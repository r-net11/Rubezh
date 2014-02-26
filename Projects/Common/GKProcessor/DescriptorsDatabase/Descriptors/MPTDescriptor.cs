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

			bool hasExpression = false;
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandAutomatic)
				{
					Formula.AddGetBit(XStateBit.On, mptDevice.Device);
					if (hasExpression)
						Formula.Add(FormulaOperationType.OR);
					hasExpression = true;
				}
			}
			if (MPT.UseDoorAutomatic)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
					{
						Formula.AddGetBit(XStateBit.On, mptDevice.Device);
						if (hasExpression)
							Formula.Add(FormulaOperationType.OR);
						hasExpression = true;
					}
				}
			}
			if (MPT.UseFailureAutomatic)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					Formula.AddGetBit(XStateBit.Failure, mptDevice.Device);
					if (hasExpression)
						Formula.Add(FormulaOperationType.OR);
					hasExpression = true;
				}
			}
			if (hasExpression)
			{
				Formula.AddPutBit(XStateBit.SetRegime_Manual, MPT);
			}

			hasExpression = false;
			if (MPT.StartLogic.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(MPT.StartLogic.Clauses);
				hasExpression = true;
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandStart)
				{
					Formula.AddGetBit(XStateBit.On, mptDevice.Device);
					if (hasExpression)
						Formula.Add(FormulaOperationType.OR);
					hasExpression = true;
				}
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandStop)
				{
					Formula.AddGetBit(XStateBit.On, mptDevice.Device);
					Formula.Add(FormulaOperationType.COM);
					if (hasExpression)
						Formula.Add(FormulaOperationType.AND);
					hasExpression = true;
				}
			}

			if (hasExpression)
			{
				Formula.AddGetBit(XStateBit.Norm, MPT);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный МПТ");
				Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, MPT);
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