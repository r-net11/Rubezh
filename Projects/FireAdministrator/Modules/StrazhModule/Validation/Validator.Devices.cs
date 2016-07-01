using System.Collections.Generic;
using System.Linq;
using Localization.Strazh.Errors;
using StrazhAPI.SKD;
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
                    Errors.Add(new DeviceValidationError(device, CommonErrors.ValidateDevices_EmptyNameError, ValidationErrorLevel.CannotSave));
				}
				if (device.DriverType == SKDDriverType.Reader)
				{
					if (device.IsEnabled)
					{
						if (device.Zone == null)
						{
                            Errors.Add(new DeviceValidationError(device, CommonErrors.ValidateDevices_ZoneNullError, ValidationErrorLevel.Warning));
						}
						if (device.Door == null)
						{
                            Errors.Add(new DeviceValidationError(device, CommonErrors.ValidateDevices_DoorNullError,
								ValidationErrorLevel.Warning));
						}
					}
					else
					{
						if (device.Zone != null)
						{
                            Errors.Add(new DeviceValidationError(device, CommonErrors.ValidateDevices_ZoneError, ValidationErrorLevel.Warning));
						}
						if (device.Door != null)
						{
                            Errors.Add(new DeviceValidationError(device, CommonErrors.ValidateDevices_DoorError,
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
                        Errors.Add(new DeviceValidationError(childDevice, CommonErrors.ValidateDevicesEqualityicate_DublicateError, ValidationErrorLevel.CannotSave));
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
                        Errors.Add(new DeviceValidationError(device, CommonErrors.IPAddressError, ValidationErrorLevel.CannotSave));
					}
					else
					{
						if (!ipAddresses.Add(addressProperty.StringValue))
						{
                            Errors.Add(new DeviceValidationError(device, CommonErrors.ValidateIPAddresses_DublicateError, ValidationErrorLevel.CannotSave));
						}
					}

					var maskProperty = device.Properties.FirstOrDefault(x => x.Name == "Mask");
					if (maskProperty == null || !SKDManager.ValidateIPAddress(maskProperty.StringValue))
					{
                        Errors.Add(new DeviceValidationError(device, CommonErrors.MaskError, ValidationErrorLevel.Warning));
					}

					var gatewayProperty = device.Properties.FirstOrDefault(x => x.Name == "Gateway");
					if (gatewayProperty == null || !SKDManager.ValidateIPAddress(gatewayProperty.StringValue))
					{
                        Errors.Add(new DeviceValidationError(device, CommonErrors.GatewayError, ValidationErrorLevel.Warning));
					}

					var loginProperty = device.Properties.FirstOrDefault(x => x.Name == "Login");
					if (loginProperty == null || string.IsNullOrEmpty(loginProperty.StringValue))
					{
                        Errors.Add(new DeviceValidationError(device, CommonErrors.ValidateIPAddresses_LoginError, ValidationErrorLevel.CannotSave));
					}

					var passwordProperty = device.Properties.FirstOrDefault(x => x.Name == "Password");
					if (passwordProperty == null || string.IsNullOrEmpty(passwordProperty.StringValue))
					{
                        Errors.Add(new DeviceValidationError(device, CommonErrors.ValidateIPAddresses_PasswordError, ValidationErrorLevel.CannotSave));
					}

					var portProperty = device.Properties.FirstOrDefault(x => x.Name == "Port");
					if (portProperty == null || portProperty.Value <= 0)
					{
						Errors.Add(new DeviceValidationError(device, CommonErrors.ValidateIPAddresses_PortError, ValidationErrorLevel.CannotSave));
					}
				}
			}
		}
	}
}