using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateMPTs()
		{
			ValidateMPTNameEquality();

			foreach (var mpt in XManager.DeviceConfiguration.MPTs)
			{
				if (IsManyGK())
					ValidateDifferentMPT(mpt);
				ValidateMPTHasNoDevices(mpt);
				ValidateMPTHasNoLogic(mpt);
				ValidateMPTSameDevices(mpt);
				ValidateMPTDeviceParameters(mpt);
				ValidateMPTSelfLogic(mpt);
			}
		}

		void ValidateMPTNameEquality()
		{
			var mptNos = new HashSet<int>();
			foreach (var mpt in XManager.DeviceConfiguration.MPTs)
			{
				if (!mptNos.Add(mpt.No))
					Errors.Add(new MPTValidationError(mpt, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDifferentMPT(XMPT mpt)
		{
			if (AreDevicesInSameGK(GetAllMPTDevices(mpt)))
				Errors.Add(new MPTValidationError(mpt, "МПТ содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		void ValidateMPTHasNoDevices(XMPT mpt)
		{
			var allDevices = GetAllMPTDevices(mpt);
			if (allDevices.Count == 0)
			{
				Errors.Add(new MPTValidationError(mpt, "К МПТ не подключено ни одного устройства", ValidationErrorLevel.CannotWrite));
			}
			else
			{
				if (mpt.MPTDevices.Count == 0)
				{
					Errors.Add(new MPTValidationError(mpt, "К МПТ не подключены устройства", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateMPTHasNoLogic(XMPT mpt)
		{
			//if (mpt.StartLogic.Clauses.Count + mpt.MPTDevices.Count(x => x.MPTDeviceType == MPTDeviceType.HandStart) == 0)
			//	Errors.Add(new MPTValidationError(mpt, "Отсутствует логика включения и устройства ручного пуска", ValidationErrorLevel.CannotWrite));
		}

		void ValidateMPTSameDevices(XMPT mpt)
		{
			var devices = new HashSet<XDevice>();
			foreach (var device in GetAllMPTDevices(mpt))
			{
				if (!devices.Add(device))
					Errors.Add(new MPTValidationError(mpt, "Выходные устройства совпадают", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateMPTDeviceParameters(XMPT mpt)
		{
			foreach (var mptDevice in mpt.MPTDevices)
			{
				if (mptDevice.Device != null && (mptDevice.Device.DriverType == XDriverType.RSR2_RM_1 || mptDevice.Device.DriverType == XDriverType.RSR2_MVK8))
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

		void ValidateMPTSelfLogic(XMPT mpt)
		{
			if (mpt.ClauseInputMPTs.Contains(mpt))
				Errors.Add(new MPTValidationError(mpt, "МПТ зависит от самого себя", ValidationErrorLevel.CannotWrite));
		}

		List<XDevice> GetAllMPTDevices(XMPT mpt)
		{
			var result = new List<XDevice>();
			foreach (var mptDevice in mpt.MPTDevices)
			{
				result.Add(mptDevice.Device);
			}
			return result;
		}
	}
}