using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace DevicesModule.Validation
{
	partial class Validator
	{
		private const string ValidChars = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщыьъэюя- .1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`~!@#$;%^:&?*()-_=+|[]'<>,\"\\/{}&#xD;№";
		private FiresecConfiguration _firesecConfiguration;
		public List<IValidationError> _errors;
		private List<Guid> _validateDevicesWithSerialNumber;

		public Validator(FiresecConfiguration firesecConfiguration)
		{
			_firesecConfiguration = firesecConfiguration;
		}

		public IEnumerable<IValidationError> Validate()
		{
			_errors = new List<IValidationError>();
			_firesecConfiguration.DeviceConfiguration.UpdateGuardConfiguration();
			_firesecConfiguration.InvalidateConfiguration();
			ValidateDevices();
			ValidateZones();
			ValidateDirections();
			ValidateGuardUsers();
			return _errors;
		}
	}
}