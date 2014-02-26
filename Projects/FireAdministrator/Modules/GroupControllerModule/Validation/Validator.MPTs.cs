using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public static partial class Validator
	{
		static void ValidateMPTs()
		{
			ValidateMPTNameEquality();

			foreach (var mpt in XManager.DeviceConfiguration.MPTs)
			{
				if (IsManyGK())
					ValidateDifferentMPT(mpt);
				ValidateMPTHasNoDevices(mpt);
				ValidateMPTHasNoLogic(mpt);
				ValidateMPTSameDevices(mpt);
			}
		}

		static void ValidateMPTNameEquality()
		{
			var mptNames = new HashSet<string>();
			foreach (var mpt in XManager.DeviceConfiguration.MPTs)
			{
				if (!mptNames.Add(mpt.Name))
					Errors.Add(new MPTValidationError(mpt, "Дублируется название", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidateDifferentMPT(XMPT mpt)
		{
			if (AreDevicesInSameGK(GetAllMPTDevices(mpt)))
				Errors.Add(new MPTValidationError(mpt, "МПТ содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		static void ValidateMPTHasNoDevices(XMPT mpt)
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

		static void ValidateMPTHasNoLogic(XMPT mpt)
		{
			if (mpt.StartLogic.Clauses.Count == 0)
				Errors.Add(new MPTValidationError(mpt, "Отсутствует логика включения", ValidationErrorLevel.Warning));
		}

		static void ValidateMPTSameDevices(XMPT mpt)
		{
			var devices = new HashSet<XDevice>();
			foreach (var device in GetAllMPTDevices(mpt))
			{
				if(!devices.Add(device))
					Errors.Add(new MPTValidationError(mpt, "Выходные устройства совпадают", ValidationErrorLevel.Warning));
			}
		}

		static List<XDevice> GetAllMPTDevices(XMPT mpt)
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