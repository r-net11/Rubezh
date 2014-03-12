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
			CreateHandAutomaticOffPim();
			CreateDoorAutomaticOffPim();
			CreateFailureAutomaticOffPim();
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
					formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный МПТ");
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

		void CreateHandAutomaticOffPim()
		{
			HandAutomaticOffPim.Name = "АО " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(HandAutomaticOffPim);
			GkDatabase.Descriptors.Add(pimDescriptor);
			UpdateConfigurationHelper.LinkXBases(MPT, HandAutomaticOffPim);

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
					UpdateConfigurationHelper.LinkXBases(HandAutomaticOffPim, mptDevice.Device);
				}
			}
			if (hasAutomaticExpression)
			{
				formula.Add(FormulaOperationType.DUP);
				formula.AddGetBit(XStateBit.On, HandAutomaticOffPim);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(XStateBit.TurnOff_InAutomatic, HandAutomaticOffPim);

				formula.AddGetBit(XStateBit.On, HandAutomaticOffPim);
				formula.Add(FormulaOperationType.COM);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(XStateBit.TurnOn_InAutomatic, HandAutomaticOffPim);
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void CreateDoorAutomaticOffPim()
		{
			DoorAutomaticOffPim.Name = "АО " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(DoorAutomaticOffPim);
			GkDatabase.Descriptors.Add(pimDescriptor);
			UpdateConfigurationHelper.LinkXBases(MPT, DoorAutomaticOffPim);

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
					}
				}

				if (hasAutomaticExpression)
				{
					formula.Add(FormulaOperationType.DUP);
					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, DoorAutomaticOffPim);
					formula.Add(FormulaOperationType.COM);
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, DoorAutomaticOffPim);
				}
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void CreateFailureAutomaticOffPim()
		{
			FailureAutomaticOffPim.Name = "АО " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(FailureAutomaticOffPim);
			GkDatabase.Descriptors.Add(pimDescriptor);
			UpdateConfigurationHelper.LinkXBases(MPT, FailureAutomaticOffPim);

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
				}

				if (hasAutomaticExpression)
				{
					formula.Add(FormulaOperationType.DUP);
					formula.AddPutBit(XStateBit.TurnOff_InAutomatic, FailureAutomaticOffPim);
					formula.Add(FormulaOperationType.COM);
					formula.AddPutBit(XStateBit.TurnOn_InAutomatic, FailureAutomaticOffPim);
				}
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void SetCrossReferences()
		{
			//UpdateConfigurationHelper.LinkXBases(MPT, HandAutomaticOffPim);
			//UpdateConfigurationHelper.LinkXBases(MPT, DoorAutomaticOffPim);
			//UpdateConfigurationHelper.LinkXBases(MPT, FailureAutomaticOffPim);

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

			foreach (var mptDevice in MPT.MPTDevices)
			{
				UpdateConfigurationHelper.LinkXBases(FailureAutomaticOffPim, mptDevice.Device);
			}
		}
	}
}