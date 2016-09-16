using License.Model.Entities;
using System.Runtime.Serialization;

namespace LicenseService.Entities
{
	[DataContract]
	public class ServiceLicenseData
	{
		[DataMember]
		public string UID { get; set; }

		[DataMember]
		public int OperatorConnectionsNumber { get; set; }

		[DataMember]
		public int TotalUsers { get; set; }

		[DataMember]
		public bool IsUnlimitedUsers { get; set; }

		[DataMember]
		public bool IsEnabledURV { get; set; }

		[DataMember]
		public bool IsEnabledPhotoVerification { get; set; }

		[DataMember]
		public bool IsEnabledRVI { get; set; }

		[DataMember]
		public bool IsEnabledAutomation { get; set; }

		public LicenseEntity ToDTO()
		{
			return new LicenseEntity
			{
				UID = UID,
				OperatorConnectionsNumber = OperatorConnectionsNumber,
				TotalUsers = TotalUsers,
				IsUnlimitedUsers = IsUnlimitedUsers,
				IsEnabledRVI = IsEnabledRVI,
				IsEnabledPhotoVerification = IsEnabledPhotoVerification,
				IsEnabledAutomation = IsEnabledAutomation,
				IsEnabledURV = IsEnabledURV
			};
		}
	}
}
