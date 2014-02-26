using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

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
				ValidateMPTDeviceParametersMissmatch(mpt);
			}
		}

		void ValidateMPTNameEquality()
		{
			var mptNames = new HashSet<string>();
			foreach (var mpt in XManager.DeviceConfiguration.MPTs)
			{
				if (!mptNames.Add(mpt.Name))
					Errors.Add(new MPTValidationError(mpt, "Дублируется название", ValidationErrorLevel.CannotWrite));
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
			if (mpt.StartLogic.Clauses.Count + mpt.MPTDevices.Count(x=>x.MPTDeviceType == MPTDeviceType.HandStart) == 0)
				Errors.Add(new MPTValidationError(mpt, "Отсутствует логика включения и устройства ручного пуска", ValidationErrorLevel.CannotWrite));
		}

		void ValidateMPTSameDevices(XMPT mpt)
		{
			var devices = new HashSet<XDevice>();
			foreach (var device in GetAllMPTDevices(mpt))
			{
				if(!devices.Add(device))
					Errors.Add(new MPTValidationError(mpt, "Выходные устройства совпадают", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateMPTDeviceParameters(XMPT mpt)
		{
			foreach (var mptDevice in mpt.MPTDevices)
			{
				if(mptDevice.Delay < 0 || mptDevice.Delay > 10)
					Errors.Add(new MPTValidationError(mpt, "Задержка МПТ для устройства " + mptDevice.Device.PresentationName + " должна быть в диапазоне от 0 до 10", ValidationErrorLevel.CannotWrite));

				if (mptDevice.Hold < 0 || mptDevice.Hold > 10)
					Errors.Add(new MPTValidationError(mpt, "Удержание МПТ для устройства " + mptDevice.Device.PresentationName + " должно быть в диапазоне от 0 до 10", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateMPTDeviceParametersMissmatch(XMPT mpt)
		{
			foreach (var mptDevice in mpt.MPTDevices)
			{
				var property = mptDevice.Device.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с");
				if (property != null)
				{
					if(mptDevice.Delay != property.Value)
						Errors.Add(new DeviceValidationError(mptDevice.Device, "Значение задержки не совпадает со значением, настроенным в МПТ " + mpt.PresentationName, ValidationErrorLevel.CannotWrite));
				}

				property = mptDevice.Device.Properties.FirstOrDefault(x => x.Name == "Время удержания, с");
				if (property != null)
				{
					if (mptDevice.Hold != property.Value)
						Errors.Add(new DeviceValidationError(mptDevice.Device, "Значение удержания не совпадает со значением, настроенным в МПТ " + mpt.PresentationName, ValidationErrorLevel.CannotWrite));
				}
			}
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