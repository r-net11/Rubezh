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
		public XPim HandAutomaticOffPim { get; private set; }
		public XPim DoorAutomaticOffPim { get; private set; }
		public XPim FailureAutomaticOffPim { get; private set; }

		public MPTCreator(GkDatabase gkDatabase, XMPT mpt, XPim handAutomaticOffPim, XPim doorAutomaticOffPim, XPim failureAutomaticOffPim)
		{
			GkDatabase = gkDatabase;
			MPT = mpt;
			HandAutomaticOffPim = handAutomaticOffPim;
			DoorAutomaticOffPim = doorAutomaticOffPim;
			FailureAutomaticOffPim = failureAutomaticOffPim;
		}

		public void Create()
		{
			CreateAutomaticOffBoards();
			CreateOnDevices();
			CreateBombDevices();
			CreateHandAutomaticOffPim(HandAutomaticOffPim);
			CreateDoorAutomaticOffPim(DoorAutomaticOffPim);
			CreateFailureAutomaticOffPim(FailureAutomaticOffPim);

			SetCrossReferences();
		}

		void CreateAutomaticOffBoards()
		{
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.AutomaticOffBoard)
				{
					var deviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.BaseUID == mptDevice.Device.BaseUID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(XStateBit.Norm, MPT);
					formula.AddGetBit(XStateBit.Norm, HandAutomaticOffPim);
					formula.Add(FormulaOperationType.AND, comment: "");
					formula.AddGetBit(XStateBit.Norm, DoorAutomaticOffPim);
					formula.Add(FormulaOperationType.AND, comment: "");
					formula.AddGetBit(XStateBit.Norm, FailureAutomaticOffPim);
					formula.Add(FormulaOperationType.AND, comment: "");

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
					var deviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.BaseUID == mptDevice.Device.BaseUID);
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
					var deviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.BaseUID == mptDevice.Device.BaseUID);
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

		void CreateHandAutomaticOffPim(XPim pim)
		{
			pim.Name = "АО Р " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(pim);
			GkDatabase.Descriptors.Add(pimDescriptor);
			UpdateConfigurationHelper.LinkXBases(MPT, pim);

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
					UpdateConfigurationHelper.LinkXBases(pim, mptDevice.Device);
				}
			}
			if (hasAutomaticExpression)
			{
				formula.Add(FormulaOperationType.DUP);
				formula.AddGetBit(XStateBit.Norm, pim);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(XStateBit.SetRegime_Manual, pim);

				formula.AddGetBit(XStateBit.Norm, pim);
				formula.Add(FormulaOperationType.COM);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(XStateBit.SetRegime_Automatic, pim);
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void CreateDoorAutomaticOffPim(XPim pim)
		{
			pim.Name = "АО Д " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(pim);
			GkDatabase.Descriptors.Add(pimDescriptor);
			UpdateConfigurationHelper.LinkXBases(MPT, pim);

			var formula = new FormulaBuilder();

			if (MPT.UseDoorAutomatic)
			{
				var hasAutomaticExpression = false;
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == MPTDeviceType.Door)
					{
						formula.AddGetBit(XStateBit.Fire1, mptDevice.Device);
						if (hasAutomaticExpression)
							formula.Add(FormulaOperationType.OR);
						hasAutomaticExpression = true;
						UpdateConfigurationHelper.LinkXBases(pim, mptDevice.Device);
					}
				}

				if (hasAutomaticExpression)
				{
					formula.Add(FormulaOperationType.DUP);
					formula.AddPutBit(XStateBit.SetRegime_Manual, pim);
					formula.Add(FormulaOperationType.COM);
					formula.AddPutBit(XStateBit.SetRegime_Automatic, pim);
				}
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void CreateFailureAutomaticOffPim(XPim pim)
		{
			pim.Name = "АО Н " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(pim);
			GkDatabase.Descriptors.Add(pimDescriptor);
			UpdateConfigurationHelper.LinkXBases(MPT, pim);

			var formula = new FormulaBuilder();

			if (MPT.UseFailureAutomatic)
			{
				var hasAutomaticExpression = false;
				foreach (var mptDevice in MPT.MPTDevices)
				{
					formula.AddGetBit(XStateBit.Failure, mptDevice.Device);
					if (hasAutomaticExpression)
						formula.Add(FormulaOperationType.OR);
					hasAutomaticExpression = true;
					UpdateConfigurationHelper.LinkXBases(pim, mptDevice.Device);
				}

				if (hasAutomaticExpression)
				{
					formula.Add(FormulaOperationType.DUP);
					formula.AddPutBit(XStateBit.SetRegime_Manual, pim);
					formula.Add(FormulaOperationType.COM);
					formula.AddPutBit(XStateBit.SetRegime_Automatic, pim);
				}
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void SetCrossReferences()
		{
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandStart ||
					mptDevice.MPTDeviceType == MPTDeviceType.HandStop ||
					mptDevice.MPTDeviceType == MPTDeviceType.Door)
				{
					UpdateConfigurationHelper.LinkXBases(MPT, mptDevice.Device);
				}

				if (mptDevice.MPTDeviceType == MPTDeviceType.AutomaticOffBoard ||
					mptDevice.MPTDeviceType == MPTDeviceType.DoNotEnterBoard ||
					mptDevice.MPTDeviceType == MPTDeviceType.ExitBoard ||
					mptDevice.MPTDeviceType == MPTDeviceType.Speaker ||
					mptDevice.MPTDeviceType == MPTDeviceType.Bomb)
				{
					UpdateConfigurationHelper.LinkXBases(mptDevice.Device, MPT);
				}
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.AutomaticOffBoard)
				{
					UpdateConfigurationHelper.LinkXBases(mptDevice.Device, HandAutomaticOffPim);
					UpdateConfigurationHelper.LinkXBases(mptDevice.Device, DoorAutomaticOffPim);
					UpdateConfigurationHelper.LinkXBases(mptDevice.Device, FailureAutomaticOffPim);
				}
			}
		}
	}
}