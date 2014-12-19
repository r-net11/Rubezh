using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;
using System;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateMPTs()
		{
			ValidateMPTNameEquality();
			ValidateMPTSameDevices();

			foreach (var mpt in GKManager.DeviceConfiguration.MPTs)
			{
				if (IsManyGK())
					ValidateMPTDifferentGK(mpt);
				ValidateEmpty(mpt);
				ValidateMPTHasNoDevices(mpt);
				ValidateMPTHasNoLogic(mpt);
				ValidateMPTSameDevices(mpt);
				ValidateMPTSameDevicesAndLogic(mpt);
				ValidateMPTDeviceParameters(mpt);
				ValidateMPTSelfLogic(mpt);
			}
		}

		void ValidateMPTNameEquality()
		{
			var mptNos = new HashSet<int>();
			foreach (var mpt in GKManager.DeviceConfiguration.MPTs)
			{
				if (!mptNos.Add(mpt.No))
					Errors.Add(new MPTValidationError(mpt, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateMPTSameDevices()
		{
			var deviceUIDs = new HashSet<Guid>();
			foreach (var mpt in GKManager.DeviceConfiguration.MPTs)
			{
				foreach(var device in mpt.Devices)
					if (!deviceUIDs.Add(device.UID))
					Errors.Add(new MPTValidationError(mpt, "Устройство " + device.PresentationName + " входит в состав различных МПТ", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateEmpty(GKMPT mpt)
		{
			if (mpt.GetDataBaseParent() == null)
			{
				Errors.Add(new MPTValidationError(mpt, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateMPTDifferentGK(GKMPT mpt)
		{
			if (AreDevicesInSameGK(GetAllMPTDevices(mpt)))
				Errors.Add(new MPTValidationError(mpt, "МПТ содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		void ValidateMPTHasNoDevices(GKMPT mpt)
		{
			if (mpt.MPTDevices.Count == 0)
			{
				Errors.Add(new MPTValidationError(mpt, "К МПТ не подключены устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateMPTHasNoLogic(GKMPT mpt)
		{
			if (mpt.StartLogic.GetObjects().Count == 0)
				Errors.Add(new MPTValidationError(mpt, "Отсутствует логика включения", ValidationErrorLevel.CannotWrite));
		}

		void ValidateMPTSameDevices(GKMPT mpt)
		{
			var devices = new HashSet<GKDevice>();
			foreach (var device in GetAllMPTDevices(mpt))
			{
				if (!devices.Add(device))
					Errors.Add(new MPTValidationError(mpt, "Выходные устройства совпадают", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateMPTSameDevicesAndLogic(GKMPT mpt)
		{
			var devicesInLogic = new List<GKBase>();
			devicesInLogic.AddRange(mpt.StartLogic.GetObjects().Where(x => x.ObjectType == GKBaseObjectType.Deivce));
			devicesInLogic.AddRange(mpt.StartLogic.GetObjects().Where(x => x.ObjectType == GKBaseObjectType.Deivce));
			var devices = new HashSet<GKDevice>();
			foreach (var device in GetAllMPTDevices(mpt))
			{
				if (devicesInLogic.Any(x => x.UID == device.UID))
					Errors.Add(new MPTValidationError(mpt, "Совпадают устройства условий включения или выключения с устройствами из состава МПТ", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateMPTDeviceParameters(GKMPT mpt)
		{
			foreach (var mptDevice in mpt.MPTDevices)
			{
				if (mptDevice.Device != null && (mptDevice.Device.DriverType == GKDriverType.RSR2_RM_1 || mptDevice.Device.DriverType == GKDriverType.RSR2_MVK8))
				{
					var delayProperty = mptDevice.Device.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с");
					if (delayProperty != null)
					{
						if (delayProperty.Value < 0 || delayProperty.Value > 10)
							Errors.Add(new MPTValidationError(mpt, "Задержка МПТ для устройства " + mptDevice.Device.PresentationName + " должна быть в диапазоне от 0 до 10", ValidationErrorLevel.CannotWrite));
					}

					var holdProperty = mptDevice.Device.Properties.FirstOrDefault(x => x.Name == "Время удержания, с");
					if (holdProperty != null)
					{
						if (holdProperty.Value < 0 || holdProperty.Value > 10)
							Errors.Add(new MPTValidationError(mpt, "Удержание МПТ для устройства " + mptDevice.Device.PresentationName + " должно быть в диапазоне от 0 до 10", ValidationErrorLevel.CannotWrite));
					}
				}
			}
		}

		void ValidateMPTSelfLogic(GKMPT mpt)
		{
			if (mpt.ClauseInputMPTs.Contains(mpt))
				Errors.Add(new MPTValidationError(mpt, "МПТ зависит от самого себя", ValidationErrorLevel.CannotWrite));
		}

		List<GKDevice> GetAllMPTDevices(GKMPT mpt)
		{
			var result = new List<GKDevice>();
			foreach (var mptDevice in mpt.MPTDevices)
			{
				result.Add(mptDevice.Device);
			}
			return result;
		}
	}
}