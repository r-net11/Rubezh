using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecClient;
using XFiresecAPI;

namespace GKProcessor
{
	public class MPTCreator
	{
		GkDatabase GkDatabase;
		XMPT MPT;
		public XDelay AutomaticOffDelay { get; private set; }

		public MPTCreator(GkDatabase gkDatabase, XMPT mpt, XDelay automaticOffDelay)
		{
			GkDatabase = gkDatabase;
			MPT = mpt;
			AutomaticOffDelay = automaticOffDelay;
		}

		public void Create()
		{
			CreateAutomaticBoards();
			CreateOnDevices();
			CreateBombDevices();
			SetCrossReferences();
		}

		void CreateAutomaticBoards()
		{
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.AutomaticOffBoard)
				{
					var deviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == mptDevice.Device.UID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(XStateBit.Norm, MPT);
					formula.Add(FormulaOperationType.COM);
					formula.AddStandardTurning(mptDevice.Device);
					formula.Add(FormulaOperationType.END);
					deviceDescriptor.Formula = formula;
					deviceDescriptor.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void CreateOnDevices()
		{
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.DoNotEnterBoard ||
					mptDevice.MPTDeviceType == MPTDeviceType.ExitBoard ||
					mptDevice.MPTDeviceType == MPTDeviceType.Speaker)
				{
					var deviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == mptDevice.Device.UID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(XStateBit.TurningOn, MPT);
					formula.AddGetBit(XStateBit.On, MPT);
					formula.Add(FormulaOperationType.OR);
					formula.AddGetBit(XStateBit.Norm, mptDevice.Device);
					formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный устройства");
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, mptDevice.Device);

					formula.AddGetBit(XStateBit.Off, MPT);
					formula.AddGetBit(XStateBit.Norm, mptDevice.Device);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, mptDevice.Device);

					formula.Add(FormulaOperationType.END);
					deviceDescriptor.Formula = formula;
					deviceDescriptor.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void CreateBombDevices()
		{
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.Bomb)
				{
					var deviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == mptDevice.Device.UID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(XStateBit.On, MPT);
					formula.AddGetBit(XStateBit.Norm, mptDevice.Device);
					formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный устройства");
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, mptDevice.Device);

					formula.AddGetBit(XStateBit.Off, MPT);
					formula.AddGetBit(XStateBit.Norm, mptDevice.Device);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, mptDevice.Device);

					formula.Add(FormulaOperationType.END);
					deviceDescriptor.Formula = formula;
					deviceDescriptor.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void CreateAutomaticOffDelay()
		{
			AutomaticOffDelay.Name = "АО " + MPT.PresentationName;
			AutomaticOffDelay.DelayTime = 0;
			AutomaticOffDelay.SetTime = 10;
			AutomaticOffDelay.DelayRegime = DelayRegime.On;

			var delayDescriptor = new DelayDescriptor(AutomaticOffDelay);
			GkDatabase.Descriptors.Add(delayDescriptor);
			UpdateConfigurationHelper.LinkXBases(MPT, AutomaticOffDelay);
			UpdateConfigurationHelper.LinkXBases(AutomaticOffDelay, MPT);

			var formula = new FormulaBuilder();

			var hasAutomaticExpression = false;
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandAutomatic)
				{
					formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
					if (hasAutomaticExpression)
						formula.Add(FormulaOperationType.OR);
					hasAutomaticExpression = true;
				}
			}
			if (hasAutomaticExpression)
			{
				formula.Add(FormulaOperationType.DUP);
				formula.AddGetBit(XStateBit.Norm, MPT);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(XStateBit.SetRegime_Manual, AutomaticOffDelay);

				formula.AddGetBit(XStateBit.Norm, MPT);
				formula.Add(FormulaOperationType.COM);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(XStateBit.SetRegime_Automatic, AutomaticOffDelay);
			}

			formula.Add(FormulaOperationType.END);
			delayDescriptor.Formula = formula;
			delayDescriptor.FormulaBytes = formula.GetBytes();
		}

		void SetCrossReferences()
		{
			//UpdateConfigurationHelper.LinkXBases(MPT, MPT);
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.AutomaticOffBoard ||
					mptDevice.MPTDeviceType == MPTDeviceType.DoNotEnterBoard ||
					mptDevice.MPTDeviceType == MPTDeviceType.ExitBoard ||
					mptDevice.MPTDeviceType == MPTDeviceType.Speaker ||
					mptDevice.MPTDeviceType == MPTDeviceType.Bomb)
				{
					UpdateConfigurationHelper.LinkXBases(mptDevice.Device, MPT);
					UpdateConfigurationHelper.LinkXBases(MPT, mptDevice.Device);
				}

				if (mptDevice.MPTDeviceType == MPTDeviceType.HandAutomatic ||
						mptDevice.MPTDeviceType == MPTDeviceType.HandStart ||
						mptDevice.MPTDeviceType == MPTDeviceType.HandStop ||
						mptDevice.MPTDeviceType == MPTDeviceType.Door)
				{
					UpdateConfigurationHelper.LinkXBases(MPT, mptDevice.Device);
				}
			}
		}
	}
}