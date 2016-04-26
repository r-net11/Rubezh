using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using System.Text.RegularExpressions;
using System.Net;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		void ValidateDevices()
		{
			ValidateDevicesEquality();
			ValidateIPAddresses();

			foreach (var device in SKDManager.Devices)
			{
				if (string.IsNullOrEmpty(device.Name))
				{
					Errors.Add(new DeviceValidationError(device, "Отсутствует название устройства", ValidationErrorLevel.CannotSave));
				}
				if (device.DriverType == SKDDriverType.Reader)
				{
					if (device.IsEnabled)
					{
						if (device.Zone == null)
						{
							Errors.Add(new DeviceValidationError(device, "Считыватель не ведет в зону", ValidationErrorLevel.Warning));
						}
						if (device.Door == null)
						{
							Errors.Add(new DeviceValidationError(device, "Считыватель не участвует в точке доступа",
								ValidationErrorLevel.Warning));
						}
					}
					else
					{
						if (device.Zone != null)
						{
							Errors.Add(new DeviceValidationError(device, "Неактивный считыватель ведет в зону", ValidationErrorLevel.Warning));
						}
						if (device.Door != null)
						{
							Errors.Add(new DeviceValidationError(device, "Неактивный считыватель участвует в точке доступа",
								ValidationErrorLevel.Warning));
						}
					}
				}
			}
		}

		void ValidateDevicesEquality()
		{
			foreach (var device in SKDManager.Devices)
			{
				var deviceNames = new HashSet<string>();
				foreach (var childDevice in device.Children)
				{
					if (!deviceNames.Add(childDevice.Name))
					{
						Errors.Add(new DeviceValidationError(childDevice, "Дублируется название устройства", ValidationErrorLevel.CannotSave));
					}
				}
			}
		}

		void ValidateIPAddresses()
		{
			var ipAddresses = new HashSet<string>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.Driver.IsController)
				{
					var addressProperty = device.Properties.FirstOrDefault(x => x.Name == "Address");
					if (addressProperty == null || !SKDManager.ValidateIPAddress(addressProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не верно задан IP-адрес", ValidationErrorLevel.CannotSave));
					}
					else
					{
						if (!ipAddresses.Add(addressProperty.StringValue))
						{
							Errors.Add(new DeviceValidationError(device, "Дублируется IP-адрес устройства", ValidationErrorLevel.CannotSave));
						}
					}

					var maskProperty = device.Properties.FirstOrDefault(x => x.Name == "Mask");
					if (maskProperty == null || !SKDManager.ValidateIPAddress(maskProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не верно задана маска подсети", ValidationErrorLevel.Warning));
					}

					var gatewayProperty = device.Properties.FirstOrDefault(x => x.Name == "Gateway");
					if (gatewayProperty == null || !SKDManager.ValidateIPAddress(gatewayProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не верно задан шлюз по умолчанию", ValidationErrorLevel.Warning));
					}

					var loginProperty = device.Properties.FirstOrDefault(x => x.Name == "Login");
					if (loginProperty == null || string.IsNullOrEmpty(loginProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не задан логин", ValidationErrorLevel.CannotSave));
					}

					var passwordProperty = device.Properties.FirstOrDefault(x => x.Name == "Password");
					if (passwordProperty == null || string.IsNullOrEmpty(passwordProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не задан пароль", ValidationErrorLevel.CannotSave));
					}

					var portProperty = device.Properties.FirstOrDefault(x => x.Name == "Port");
					if (portProperty == null || portProperty.Value <= 0)
					{
						Errors.Add(new DeviceValidationError(device, "Не задан порт", ValidationErrorLevel.CannotSave));
					}
				}
			}
		}
	}
}