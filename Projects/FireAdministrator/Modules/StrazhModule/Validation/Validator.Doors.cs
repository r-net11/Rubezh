using System.Collections.Generic;
using System.Linq;
using Localization.Strazh.Errors;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;
using System;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		void ValidateDoors()
		{
			ValidateDoorsNoEquality();
			ValidateDoorNamesEquality();
			ValidateDoorsReadersUnique();

			foreach (var door in SKDManager.Doors)
			{
				if (string.IsNullOrEmpty(door.Name))
				{
                    Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_EmptyNameError, ValidationErrorLevel.CannotSave));
				}

				var inDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == door.InDeviceUID);
				if (inDevice == null)
					Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_InDeviceError, ValidationErrorLevel.CannotSave));

				var outDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == door.OutDeviceUID);
				if (outDevice == null)
					Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_OutDeviceEmptyError, ValidationErrorLevel.CannotSave));

				if (inDevice != null && inDevice.DriverType != SKDDriverType.Reader)
					Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_OutReaderError, ValidationErrorLevel.CannotWrite));

				if (inDevice != null && outDevice != null)
				{
					if (door.DoorType == DoorType.OneWay)
					{
						if (outDevice.DriverType != SKDDriverType.Button && outDevice.IntAddress != inDevice.IntAddress)
							Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_OutDeviceError, ValidationErrorLevel.CannotSave));
					}
					if (door.DoorType != DoorType.OneWay)
					{
						if (outDevice.DriverType != SKDDriverType.Reader && outDevice.IntAddress != inDevice.IntAddress + 1)
							Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_OutDeviceError, ValidationErrorLevel.CannotSave));
					}
				}

				ValidateDoorReaderDoorType(door);
			}
		}

		void ValidateDoorsNoEquality()
		{
			var doorNos = new HashSet<int>();
			foreach (var door in SKDManager.Doors)
			{
				if (!doorNos.Add(door.No))
					Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_DublicateNumberError, ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDoorNamesEquality()
		{
			var doorNames = new HashSet<string>();
			foreach (var door in SKDManager.Doors)
			{
				if (!doorNames.Add(door.Name))
				{
					Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_DublicateNameError, ValidationErrorLevel.CannotSave));
				}
			}
		}

		void ValidateDoorsReadersUnique()
		{
			var devicesUID = new HashSet<Guid>();
			foreach (var door in SKDManager.Doors)
			{
				if (door.InDeviceUID != Guid.Empty)
				{
					if (!devicesUID.Add(door.InDeviceUID))
					{
						Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_SameDeviceError, ValidationErrorLevel.CannotSave));
					}
				}
				if (door.OutDeviceUID != Guid.Empty && door.OutDevice != null && door.OutDevice.DriverType == SKDDriverType.Reader)
				{
					if (!devicesUID.Add(door.OutDeviceUID))
					{
						Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_SameDeviceError, ValidationErrorLevel.CannotSave));
					}
				}
			}
		}

		void ValidateDoorReaderDoorType(SKDDoor door)
		{
			if (door.InDevice != null && door.InDevice.Parent != null && door.InDevice.Parent.DoorType != door.DoorType)
			{
				Errors.Add(new DoorValidationError(door, CommonErrors.ValidateDoors_DeviceTypeError, ValidationErrorLevel.CannotSave));
			}
		}
	}
}