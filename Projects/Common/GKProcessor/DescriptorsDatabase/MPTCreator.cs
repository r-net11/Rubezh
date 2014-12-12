using System.Linq;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class MPTCreator
	{
		CommonDatabase Database;
		GKMPT MPT;
		DatabaseType DatabaseType;

		public MPTCreator(CommonDatabase database, GKMPT mpt, DatabaseType dataBaseType)
		{
			DatabaseType = dataBaseType;
			Database = database;
			MPT = mpt;
		}

		public void Create()
		{
			CreateAutomaticOffBoards();
			CreateOnDevices();
			CreateBombDevices();
			CreateHandAutomaticOffPim(MPT.HandAutomaticOffPim);
			CreateDoorAutomaticOffPim(MPT.DoorAutomaticOffPim);
			CreateFailureAutomaticOffPim(MPT.FailureAutomaticOffPim);

			SetCrossReferences();
		}

		void CreateAutomaticOffBoards()
		{
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard)
				{
					var deviceDescriptor = Database.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == mptDevice.Device.UID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(GKStateBit.Norm, MPT, deviceDescriptor.DatabaseType);
					formula.AddGetBit(GKStateBit.Norm, MPT.HandAutomaticOffPim, deviceDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.AND, comment: "");
					formula.AddGetBit(GKStateBit.Norm, MPT.DoorAutomaticOffPim, deviceDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.AND, comment: "");
					formula.AddGetBit(GKStateBit.Norm, MPT.FailureAutomaticOffPim, deviceDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.AND, comment: "");

					formula.Add(FormulaOperationType.COM);
					formula.AddStandardTurning(mptDevice.Device, deviceDescriptor.DatabaseType);
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
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.ExitBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.Speaker)
				{
					var deviceDescriptor = Database.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == mptDevice.Device.UID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(GKStateBit.TurningOn, MPT, deviceDescriptor.DatabaseType);
					formula.AddGetBit(GKStateBit.On, MPT, deviceDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.OR);
					formula.AddGetBit(GKStateBit.Norm, mptDevice.Device, deviceDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный устройства");
					formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);

					formula.AddGetBit(GKStateBit.Off, MPT, deviceDescriptor.DatabaseType);
					formula.AddGetBit(GKStateBit.Norm, mptDevice.Device, deviceDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);

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
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.Bomb)
				{
					var deviceDescriptor = Database.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == mptDevice.Device.UID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(GKStateBit.On, MPT, deviceDescriptor.DatabaseType);
					formula.AddGetBit(GKStateBit.Norm, mptDevice.Device, deviceDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный устройства");
					formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);

					formula.AddGetBit(GKStateBit.Off, MPT, deviceDescriptor.DatabaseType);
					formula.AddGetBit(GKStateBit.Norm, mptDevice.Device, deviceDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);

					formula.Add(FormulaOperationType.END);
					deviceDescriptor.Formula = formula;
					deviceDescriptor.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void CreateHandAutomaticOffPim(GKPim pim)
		{
			pim.Name = "АО Р " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(pim, DatabaseType);
			Database.Descriptors.Add(pimDescriptor);
			MPT.LinkGKBases(pim);

			var formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && pimDescriptor.GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !pimDescriptor.GKBase.IsLogicOnKau))
			{
				formula.Add(FormulaOperationType.END);
				pimDescriptor.FormulaBytes = formula.GetBytes();
				return;
			}
			var hasAutomaticExpression = false;
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.HandAutomatic)
				{
					formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device, pimDescriptor.DatabaseType);
					if (hasAutomaticExpression)
						formula.Add(FormulaOperationType.OR);
					hasAutomaticExpression = true;
					pim.LinkGKBases(mptDevice.Device);
				}
			}
			if (hasAutomaticExpression)
			{
				formula.Add(FormulaOperationType.DUP);
				formula.AddGetBit(GKStateBit.Norm, pim, pimDescriptor.DatabaseType);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(GKStateBit.SetRegime_Manual, pim, pimDescriptor.DatabaseType);

				formula.AddGetBit(GKStateBit.Norm, pim, pimDescriptor.DatabaseType);
				formula.Add(FormulaOperationType.COM);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(GKStateBit.SetRegime_Automatic, pim, pimDescriptor.DatabaseType);
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void CreateDoorAutomaticOffPim(GKPim pim)
		{
			pim.Name = "АО Д " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(pim, DatabaseType);
			Database.Descriptors.Add(pimDescriptor);
			MPT.LinkGKBases(pim);

			var formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && pimDescriptor.GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !pimDescriptor.GKBase.IsLogicOnKau))
			{
				formula.Add(FormulaOperationType.END);
				pimDescriptor.FormulaBytes = formula.GetBytes();
				return;
			}

			if (MPT.UseDoorAutomatic)
			{
				var hasAutomaticExpression = false;
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == GKMPTDeviceType.Door)
					{
						formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device, pimDescriptor.DatabaseType);
						if (hasAutomaticExpression)
							formula.Add(FormulaOperationType.OR);
						hasAutomaticExpression = true;
						pim.LinkGKBases(mptDevice.Device);
					}
				}

				if (hasAutomaticExpression)
				{
					formula.Add(FormulaOperationType.DUP);
					formula.AddPutBit(GKStateBit.SetRegime_Manual, pim, pimDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.COM);
					formula.AddPutBit(GKStateBit.SetRegime_Automatic, pim, pimDescriptor.DatabaseType);
				}
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void CreateFailureAutomaticOffPim(GKPim pim)
		{
			pim.Name = "АО Н " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(pim, DatabaseType);
			Database.Descriptors.Add(pimDescriptor);
			MPT.LinkGKBases(pim);

			var formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && pimDescriptor.GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !pimDescriptor.GKBase.IsLogicOnKau))
			{
				formula.Add(FormulaOperationType.END);
				pimDescriptor.FormulaBytes = formula.GetBytes();
				return;
			}

			if (MPT.UseFailureAutomatic)
			{
				var hasAutomaticExpression = false;
				foreach (var mptDevice in MPT.MPTDevices)
				{
					formula.AddGetBit(GKStateBit.Failure, mptDevice.Device, pimDescriptor.DatabaseType);
					if (hasAutomaticExpression)
						formula.Add(FormulaOperationType.OR);
					hasAutomaticExpression = true;
					pim.LinkGKBases(mptDevice.Device);
				}

				if (hasAutomaticExpression)
				{
					formula.Add(FormulaOperationType.DUP);
					formula.AddPutBit(GKStateBit.SetRegime_Manual, pim, pimDescriptor.DatabaseType);
					formula.Add(FormulaOperationType.COM);
					formula.AddPutBit(GKStateBit.SetRegime_Automatic, pim, pimDescriptor.DatabaseType);
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
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.HandStart ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.HandStop ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.Door)
				{
					MPT.LinkGKBases(mptDevice.Device);
				}

				if (mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.ExitBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.Speaker ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.Bomb)
				{
					mptDevice.Device.LinkGKBases(MPT);
				}
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard)
				{
					mptDevice.Device.LinkGKBases(MPT.HandAutomaticOffPim);
					mptDevice.Device.LinkGKBases(MPT.DoorAutomaticOffPim);
					mptDevice.Device.LinkGKBases(MPT.FailureAutomaticOffPim);
				}
			}
		}
	}
}