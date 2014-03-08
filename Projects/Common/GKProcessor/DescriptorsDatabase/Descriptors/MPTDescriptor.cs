using System;
using System.Collections.Generic;
using Common;
using XFiresecAPI;

namespace GKProcessor
{
	public class MPTDescriptor : BaseDescriptor
	{
		public XDelay AutomaticOffDelay { get; private set; }

		public MPTDescriptor(GkDatabase gkDatabase, XMPT mpt)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.MPT;
			MPT = mpt;
			MPT.Hold = 10;
			MPT.DelayRegime = DelayRegime.On;

			AutomaticOffDelay = new XDelay();
			AutomaticOffDelay.BaseUID = GuidHelper.CreateOn(MPT.BaseUID);
			gkDatabase.AddDelay(AutomaticOffDelay);

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

			var hasAutomaticOffExpression = false;
			if (MPT.UseDoorAutomatic)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
					{
						Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
						if (hasAutomaticOffExpression)
							Formula.Add(FormulaOperationType.OR);
						hasAutomaticOffExpression = true;
					}
				}
			}

			if (MPT.UseFailureAutomatic)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					Formula.AddGetBit(XStateBit.Failure, mptDevice.Device);
					if (hasAutomaticOffExpression)
						Formula.Add(FormulaOperationType.OR);
					hasAutomaticOffExpression = true;
				}
			}

			Formula.AddGetBit(XStateBit.On, AutomaticOffDelay);
			if (hasAutomaticOffExpression)
				Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(XStateBit.SetRegime_Manual, MPT);


			var hasAutomaticOnExpression = false;
			if (MPT.UseDoorAutomatic)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
					{
						Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
						if (hasAutomaticOnExpression)
							Formula.Add(FormulaOperationType.OR);
						hasAutomaticOnExpression = true;
					}
				}
			}
			if (hasAutomaticOnExpression)
			{
				Formula.Add(FormulaOperationType.COM);
			}
			Formula.AddGetBit(XStateBit.On, AutomaticOffDelay);
			Formula.Add(FormulaOperationType.COM);
			if (hasAutomaticOnExpression)
			{
				Formula.Add(FormulaOperationType.AND);
			}
			Formula.AddPutBit(XStateBit.SetRegime_Automatic, MPT);


			var hasOnExpression = false;
			if (MPT.StartLogic.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(MPT.StartLogic.Clauses);

				if (MPT.UseDoorStop)
				{
					foreach (var mptDevice in MPT.MPTDevices)
					{
						if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
						{
							Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
							Formula.AddGetBit(XStateBit.TurningOn, MPT);
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
					Formula.AddGetBit(XStateBit.Norm, MPT);
					Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный МПТ");
				}

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
				Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, MPT);
			}

			var hasOffExpression = false;
			if (MPT.StartLogic.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(MPT.StartLogic.Clauses);
				Formula.Add(FormulaOperationType.COM);
				hasOffExpression = true;
			}
			if (MPT.UseDoorStop)
			{
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
					{
						Formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
						Formula.AddGetBit(XStateBit.TurningOn, MPT);
						Formula.Add(FormulaOperationType.AND);
						if (hasOffExpression)
							Formula.Add(FormulaOperationType.OR);
						hasOffExpression = true;
					}
				}
			}

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
			if (hasOffExpression)
			{
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