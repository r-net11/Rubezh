using System;

namespace License.Model.Entities
{
	public interface ILicenseEntity
	{
		string UID { get; set; }
		DateTime CreateDateTime { get; set; }
		int OperatorConnectionsNumber { get; set; }
		int TotalUsers { get; set; }
		bool IsUnlimitedUsers { get; set; }
		bool IsEnabledURV { get; set; }
		bool IsEnabledPhotoVerification { get; set; }
		bool IsEnabledRVI { get; set; }
		bool IsEnabledAutomation { get; set; }
		bool IsEnabledServer { get; set; }
	}
}
