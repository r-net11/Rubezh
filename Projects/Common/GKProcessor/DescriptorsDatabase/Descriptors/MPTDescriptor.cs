using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class MPTDescriptor : BaseDescriptor
	{
		GKMPT MPT { get; set; }

		public MPTDescriptor(CommonDatabase database, GKMPT mpt, DatabaseType dataBaseType)
			: base(mpt, dataBaseType)
		{
			DescriptorType = DescriptorType.MPT;
			MPT = mpt;
			MPT.Hold = 10;
			MPT.DelayRegime = DelayRegime.On;
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
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				FormulaBytes = Formula.GetBytes();
				return;
			}

			if (MPT.MptLogic.StopClausesGroup.GetObjects().Count > 0)
			{
				Formula.AddClauseFormula(MPT.MptLogic.StopClausesGroup, DatabaseType);
				if (MPT.MptLogic.OnClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.DUP);
				Formula.AddPutBit(GKStateBit.Stop_InManual, MPT, DatabaseType);
			}

			if (MPT.MptLogic.OnClausesGroup.GetObjects().Count > 0)
			{
				if (MPT.MptLogic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.COM);
				Formula.AddClauseFormula(MPT.MptLogic.OnClausesGroup, DatabaseType);
				if (MPT.MptLogic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, MPT, DatabaseType);
				if (MPT.MptLogic.UseOffCounterLogic)
				{
					Formula.AddClauseFormula(MPT.MptLogic.OnClausesGroup, DatabaseType);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, MPT, DatabaseType);
				}
			}

			if (MPT.MptLogic.OffClausesGroup.GetObjects().Count > 0)
			{
				Formula.AddClauseFormula(MPT.MptLogic.OffClausesGroup, DatabaseType);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, MPT, DatabaseType);
			}

			SetRegime(GKMPTDeviceType.HandStart, GKStateBit.TurnOn_InManual);
			SetRegime(GKMPTDeviceType.HandStop, GKStateBit.TurnOff_InManual);
			SetRegime(GKMPTDeviceType.HandAutomaticOn, GKStateBit.SetRegime_Automatic);
			SetRegime(GKMPTDeviceType.HandAutomaticOff, GKStateBit.SetRegime_Manual);

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void SetRegime(GKMPTDeviceType deviceType, GKStateBit stateBit)
		{
			var hasOR = false;
			var mptDevices = MPT.MPTDevices.FindAll(x => x.MPTDeviceType == deviceType);
			foreach (var mptDevice in mptDevices)
			{
				if (mptDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || mptDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
				{
					GKCodeReaderSettingsPart settingsPart = null;
					switch (stateBit)
					{
						case GKStateBit.TurnOn_InManual:
							settingsPart = mptDevice.CodeReaderSettings.StartSettings;
							break;

						case GKStateBit.TurnOff_InManual:
							settingsPart = mptDevice.CodeReaderSettings.StopSettings;
							break;

						case GKStateBit.SetRegime_Automatic:
							settingsPart = mptDevice.CodeReaderSettings.AutomaticOnSettings;
							break;

						case GKStateBit.SetRegime_Manual:
							settingsPart = mptDevice.CodeReaderSettings.AutomaticOffSettings;
							break;
					}
					var codeIndex = 0;
					foreach (var codeUID in settingsPart.CodeUIDs)
					{
						var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
						Formula.Add(FormulaOperationType.KOD, 0,
							DatabaseType == DatabaseType.Gk ? mptDevice.Device.GKDescriptorNo : mptDevice.Device.KAUDescriptorNo);
						Formula.Add(FormulaOperationType.CMPKOD, 1,
							DatabaseType == DatabaseType.Gk ? code.GKDescriptorNo : code.KAUDescriptorNo);
						if (codeIndex > 0)
						{
							Formula.Add(FormulaOperationType.OR);
						}
						codeIndex++;
					}
					if (hasOR)
						Formula.Add(FormulaOperationType.OR);
					if (codeIndex > 0)
						hasOR = true;
				}
				else
				{
					Formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device, DatabaseType);
					if (hasOR)
						Formula.Add(FormulaOperationType.OR);
					hasOR = true;
				}
			}
			if (hasOR)
				Formula.AddPutBit(stateBit, MPT, DatabaseType);
		}

		GKStateBit CodeReaderEnterTypeToStateBit(GKCodeReaderEnterType codeReaderEnterType)
		{
			switch (codeReaderEnterType)
			{
				case GKCodeReaderEnterType.CodeOnly:
					return GKStateBit.Attention;

				case GKCodeReaderEnterType.CodeAndOne:
					return GKStateBit.Fire1;

				case GKCodeReaderEnterType.CodeAndTwo:
					return GKStateBit.Fire2;
			}
			return GKStateBit.Fire1;
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