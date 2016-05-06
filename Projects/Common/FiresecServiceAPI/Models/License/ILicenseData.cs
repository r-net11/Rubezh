﻿namespace StrazhAPI.Models
{
	public interface ILicenseData
	{
		bool IsEnabledURV { get; set; }

		bool IsEnabledPhotoVerification { get; set; }

		bool IsEnabledRVI { get; set; }

		bool IsEnabledAutomation { get; set; }

		bool IsUnlimitedUsers { get; set; }
	}
}