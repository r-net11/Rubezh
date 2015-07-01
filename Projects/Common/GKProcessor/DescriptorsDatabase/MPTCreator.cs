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
			SetCrossReferences();
		}

		void CreateAutomaticOffBoards()
		{
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard)
				{
					mptDevice.Device.IsLogicOnKau = MPT.IsLogicOnKau;
					var deviceDescriptor = Database.Descriptors.FirstOrDefault(x => x.DescriptorType == DescriptorType.Device && x.GKBase.UID == mptDevice.Device.UID);
					deviceDescriptor.Formula = new FormulaBuilder();

					deviceDescriptor.Formula.AddGetBit(GKStateBit.Norm, MPT, deviceDescriptor.DatabaseType);
					deviceDescriptor.Formula.Add(FormulaOperationType.DUP);
					deviceDescriptor.Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);
					deviceDescriptor.Formula.Add(FormulaOperationType.COM);
					deviceDescriptor.Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);
					deviceDescriptor.Formula.Add(FormulaOperationType.END);
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
					mptDevice.Device.IsLogicOnKau = MPT.IsLogicOnKau;
					var deviceDescriptor = Database.Descriptors.FirstOrDefault(x => x.DescriptorType == DescriptorType.Device && x.GKBase.UID == mptDevice.Device.UID);
					deviceDescriptor.Formula = new FormulaBuilder();

					deviceDescriptor.Formula.AddGetBit(GKStateBit.TurningOn, MPT, deviceDescriptor.DatabaseType);
					deviceDescriptor.Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);

					deviceDescriptor.Formula.AddGetBit(GKStateBit.Off, MPT, deviceDescriptor.DatabaseType);
					deviceDescriptor.Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);
					deviceDescriptor.Formula.Add(FormulaOperationType.END);
				}
			}
		}

		void CreateBombDevices()
		{
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.Bomb)
				{
					mptDevice.Device.IsLogicOnKau = MPT.IsLogicOnKau;
					var deviceDescriptor = Database.Descriptors.FirstOrDefault(x => x.DescriptorType == DescriptorType.Device && x.GKBase.UID == mptDevice.Device.UID);
					deviceDescriptor.Formula = new FormulaBuilder();

					deviceDescriptor.Formula.AddGetBit(GKStateBit.On, MPT, deviceDescriptor.DatabaseType);
					deviceDescriptor.Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);

					deviceDescriptor.Formula.AddGetBit(GKStateBit.Off, MPT, deviceDescriptor.DatabaseType);
					deviceDescriptor.Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, mptDevice.Device, deviceDescriptor.DatabaseType);

					//if (MPT.SuspendLogic.OnClausesGroup.GetObjects().Count > 0)
					//{
					//    deviceDescriptor.Formula.AddClauseFormula(MPT.SuspendLogic.OnClausesGroup, DatabaseType);
					//    deviceDescriptor.Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, mptDevice.Device, DatabaseType);
					//}

					deviceDescriptor.Formula.Add(FormulaOperationType.END);
				}
			}
		}

		void SetCrossReferences()
		{
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.HandStart ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.HandStop ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.HandAutomaticOn ||
					mptDevice.MPTDeviceType == GKMPTDeviceType.HandAutomaticOff)
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
		}
	}
}