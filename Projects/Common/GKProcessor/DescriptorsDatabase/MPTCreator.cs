using System.Linq;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class MPTCreator
	{
		GkDatabase GkDatabase;
		GKMPT MPT;
		public GKPim HandAutomaticOffPim { get; private set; }
		public GKPim DoorAutomaticOffPim { get; private set; }
		public GKPim FailureAutomaticOffPim { get; private set; }

		public MPTCreator(GkDatabase gkDatabase, GKMPT mpt, GKPim handAutomaticOffPim, GKPim doorAutomaticOffPim, GKPim failureAutomaticOffPim)
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
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard)
				{
					var deviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == mptDevice.Device.UID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(GKStateBit.Norm, MPT);
					formula.AddGetBit(GKStateBit.Norm, HandAutomaticOffPim);
					formula.Add(FormulaOperationType.AND, comment: "");
					formula.AddGetBit(GKStateBit.Norm, DoorAutomaticOffPim);
					formula.Add(FormulaOperationType.AND, comment: "");
					formula.AddGetBit(GKStateBit.Norm, FailureAutomaticOffPim);
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
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.ExitBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.Speaker)
				{
					var deviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == mptDevice.Device.UID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(GKStateBit.TurningOn, MPT);
					formula.AddGetBit(GKStateBit.On, MPT);
					formula.Add(FormulaOperationType.OR);
					formula.AddGetBit(GKStateBit.Norm, mptDevice.Device);
					formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный устройства");
					formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, mptDevice.Device);

					formula.AddGetBit(GKStateBit.Off, MPT);
					formula.AddGetBit(GKStateBit.Norm, mptDevice.Device);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, mptDevice.Device);

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
					var deviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.Device != null && x.Device.UID == mptDevice.Device.UID);
					var formula = new FormulaBuilder();

					formula.AddGetBit(GKStateBit.On, MPT);
					formula.AddGetBit(GKStateBit.Norm, mptDevice.Device);
					formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный устройства");
					formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, mptDevice.Device);

					formula.AddGetBit(GKStateBit.Off, MPT);
					formula.AddGetBit(GKStateBit.Norm, mptDevice.Device);
					formula.Add(FormulaOperationType.AND);
					formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, mptDevice.Device);

					formula.Add(FormulaOperationType.END);
					deviceDescriptor.Formula = formula;
					deviceDescriptor.FormulaBytes = formula.GetBytes();
				}
			}
		}

		void CreateHandAutomaticOffPim(GKPim pim)
		{
			pim.Name = "АО Р " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(pim);
			GkDatabase.Descriptors.Add(pimDescriptor);
			GKDeviceConfiguration.LinkXBases(MPT, pim);

			var formula = new FormulaBuilder();

			var hasAutomaticExpression = false;
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.HandAutomatic)
				{
					formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device);
					if (hasAutomaticExpression)
						formula.Add(FormulaOperationType.OR);
					hasAutomaticExpression = true;
					GKDeviceConfiguration.LinkXBases(pim, mptDevice.Device);
				}
			}
			if (hasAutomaticExpression)
			{
				formula.Add(FormulaOperationType.DUP);
				formula.AddGetBit(GKStateBit.Norm, pim);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(GKStateBit.SetRegime_Manual, pim);

				formula.AddGetBit(GKStateBit.Norm, pim);
				formula.Add(FormulaOperationType.COM);
				formula.Add(FormulaOperationType.AND);
				formula.AddPutBit(GKStateBit.SetRegime_Automatic, pim);
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void CreateDoorAutomaticOffPim(GKPim pim)
		{
			pim.Name = "АО Д " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(pim);
			GkDatabase.Descriptors.Add(pimDescriptor);
			GKDeviceConfiguration.LinkXBases(MPT, pim);

			var formula = new FormulaBuilder();

			if (MPT.UseDoorAutomatic)
			{
				var hasAutomaticExpression = false;
				foreach (var mptDevice in MPT.MPTDevices)
				{
					if (mptDevice.MPTDeviceType == GKMPTDeviceType.Door)
					{
						formula.AddGetBit(GKStateBit.Fire1, mptDevice.Device);
						if (hasAutomaticExpression)
							formula.Add(FormulaOperationType.OR);
						hasAutomaticExpression = true;
						GKDeviceConfiguration.LinkXBases(pim, mptDevice.Device);
					}
				}

				if (hasAutomaticExpression)
				{
					formula.Add(FormulaOperationType.DUP);
					formula.AddPutBit(GKStateBit.SetRegime_Manual, pim);
					formula.Add(FormulaOperationType.COM);
					formula.AddPutBit(GKStateBit.SetRegime_Automatic, pim);
				}
			}

			formula.Add(FormulaOperationType.END);
			pimDescriptor.Formula = formula;
			pimDescriptor.FormulaBytes = formula.GetBytes();
		}

		void CreateFailureAutomaticOffPim(GKPim pim)
		{
			pim.Name = "АО Н " + MPT.PresentationName;

			var pimDescriptor = new PimDescriptor(pim);
			GkDatabase.Descriptors.Add(pimDescriptor);
			GKDeviceConfiguration.LinkXBases(MPT, pim);

			var formula = new FormulaBuilder();

			if (MPT.UseFailureAutomatic)
			{
				var hasAutomaticExpression = false;
				foreach (var mptDevice in MPT.MPTDevices)
				{
					formula.AddGetBit(GKStateBit.Failure, mptDevice.Device);
					if (hasAutomaticExpression)
						formula.Add(FormulaOperationType.OR);
					hasAutomaticExpression = true;
					GKDeviceConfiguration.LinkXBases(pim, mptDevice.Device);
				}

				if (hasAutomaticExpression)
				{
					formula.Add(FormulaOperationType.DUP);
					formula.AddPutBit(GKStateBit.SetRegime_Manual, pim);
					formula.Add(FormulaOperationType.COM);
					formula.AddPutBit(GKStateBit.SetRegime_Automatic, pim);
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
					GKDeviceConfiguration.LinkXBases(MPT, mptDevice.Device);
				}

				if (mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.ExitBoard ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.Speaker ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.Bomb)
				{
					GKDeviceConfiguration.LinkXBases(mptDevice.Device, MPT);
				}
			}

			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard)
				{
					GKDeviceConfiguration.LinkXBases(mptDevice.Device, HandAutomaticOffPim);
					GKDeviceConfiguration.LinkXBases(mptDevice.Device, DoorAutomaticOffPim);
					GKDeviceConfiguration.LinkXBases(mptDevice.Device, FailureAutomaticOffPim);
				}
			}
		}
	}
}