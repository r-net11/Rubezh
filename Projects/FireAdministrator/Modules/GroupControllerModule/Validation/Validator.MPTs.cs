using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using System;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateMPTs()
		{
			ValidateCommon(GKManager.MPTs);
			ValidateMPTSameDevices();

			foreach (var mpt in GKManager.MPTs)
			{
				ValidateMPTHasNoDevices(mpt);
				ValidateMPTHasNoLogic(mpt);
				ValidateMPTSameDevices(mpt);
				ValidateMPTSameDevicesAndLogic(mpt);
				ValidateMPTDeviceParameters(mpt);
				ValidateMPTSelfLogic(mpt);
			}
		}

		/// <summary>
		/// Валидация того, что устройства контроллер Wiegand и кодонаборник не могут учавствовать в различных МПТ
		/// </summary>
		void ValidateMPTSameDevices()
		{
			var deviceUIDs = new HashSet<Guid>();
			foreach (var mpt in GKManager.DeviceConfiguration.MPTs)
			{
				foreach (var mptDevice in mpt.MPTDevices.Where(x => !x.Device.Driver.IsCardReaderOrCodeReader))
					if (!deviceUIDs.Add(mptDevice.DeviceUID))
						AddError(mpt, "Устройство " + mptDevice.Device.PresentationName + " входит в состав различных МПТ", ValidationErrorLevel.CannotWrite);
			}
		}

		/// <summary>
		/// Валидация того, что к МПТ подключены устройства
		/// </summary>
		/// <param name="mpt"></param>
		void ValidateMPTHasNoDevices(GKMPT mpt)
		{
			if (mpt.MPTDevices.Count == 0)
			{
				AddError(mpt, "К МПТ не подключены устройства", ValidationErrorLevel.CannotWrite);
			}
		}

		/// <summary>
		/// Валидация того, что у МПТ настроена логика срабатывания
		/// </summary>
		/// <param name="mpt"></param>
		void ValidateMPTHasNoLogic(GKMPT mpt)
		{
			if (mpt.MptLogic.OnClausesGroup.GetObjects().Count == 0)
				AddError(mpt, "Отсутствует логика включения", ValidationErrorLevel.CannotWrite);
		}

		/// <summary>
		/// Валидация того, что устройства МПТ совпадают
		/// Почему только контроллеры Wiegand и кодонаборники?
		/// </summary>
		/// <param name="mpt"></param>
		void ValidateMPTSameDevices(GKMPT mpt)
		{
			var devices = new HashSet<GKDevice>();
			foreach (var device in GetAllMPTDevices(mpt).Where(x => !x.Driver.IsCardReaderOrCodeReader))
			{
				if (!devices.Add(device))
					AddError(mpt, "Выходные устройства совпадают", ValidationErrorLevel.CannotWrite);
			}
		}

		/// <summary>
		/// Валидация того, что в логике срабатывания счавствуют те же устройства, что входят в состав МПТ
		/// </summary>
		/// <param name="mpt"></param>
		void ValidateMPTSameDevicesAndLogic(GKMPT mpt)
		{
			var devicesInLogic = new List<GKBase>();
			devicesInLogic.AddRange(mpt.MptLogic.OnClausesGroup.GetObjects().Where(x => x.ObjectType == GKBaseObjectType.Device));
			devicesInLogic.AddRange(mpt.MptLogic.OnClausesGroup.GetObjects().Where(x => x.ObjectType == GKBaseObjectType.Device));
			foreach (var device in GetAllMPTDevices(mpt))
			{
				if (devicesInLogic.Any(x => x.UID == device.UID))
					AddError(mpt, "Совпадают устройства условий включения или выключения с устройствами из состава МПТ", ValidationErrorLevel.CannotWrite);
			}
		}

		/// <summary>
		/// Валидация того, что устройства, входящие в МПТ, имеют правильно заданные параметры устройств
		/// </summary>
		/// <param name="mpt"></param>
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
							AddError(mpt, "Задержка МПТ для устройства " + mptDevice.Device.PresentationName + " должна быть в диапазоне от 0 до 10", ValidationErrorLevel.CannotWrite);
					}

					var holdProperty = mptDevice.Device.Properties.FirstOrDefault(x => x.Name == "Время удержания, с");
					if (holdProperty != null)
					{
						if (holdProperty.Value < 0 || holdProperty.Value > 10)
							AddError(mpt, "Удержание МПТ для устройства " + mptDevice.Device.PresentationName + " должно быть в диапазоне от 0 до 10", ValidationErrorLevel.CannotWrite);
					}
				}
			}
		}

		/// <summary>
		/// Валидация того, что в логике включения МПТ не содержит сам себя
		/// </summary>
		/// <param name="mpt"></param>
		void ValidateMPTSelfLogic(GKMPT mpt)
		{
			if (mpt.MptLogic.GetObjects().Contains(mpt))
				AddError(mpt, "МПТ зависит от самого себя", ValidationErrorLevel.CannotWrite);
		}

		/// <summary>
		/// Получение списка устройств, входящих в МПТ
		/// </summary>
		/// <param name="mpt"></param>
		/// <returns></returns>
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