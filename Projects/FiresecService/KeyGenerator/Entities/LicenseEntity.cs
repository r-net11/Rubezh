using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using License.Model.Entities;

namespace KeyGenerator.Entities
{
	public class LicenseEntity
	{
		public string UID { get; private set; }
		public DateTime CreateDateTime { get; private set; }
		public int OperatorConnectionsNumber { get; private set; }
		public int TotalUsers { get; private set; }
		public bool IsUnlimitedUsers { get; private set; }
		public bool IsEnabledURV { get; private set; }
		public bool IsEnabledPhotoVerification { get; private set; }
		public bool IsEnabledRVI { get; private set; }
		public bool IsEnabledAutomation { get; private set; }

		public LicenseEntity(ILicenseEntity entity)
		{
			if (entity == null) return;

			UID = entity.UID;
			CreateDateTime = entity.CreateDateTime;
			OperatorConnectionsNumber = entity.OperatorConnectionsNumber;
			TotalUsers = entity.TotalUsers;
			IsUnlimitedUsers = entity.IsUnlimitedUsers;
			IsEnabledURV = entity.IsEnabledURV;
			IsEnabledPhotoVerification = entity.IsEnabledPhotoVerification;
			IsEnabledRVI = entity.IsEnabledRVI;
			IsEnabledAutomation = entity.IsEnabledAutomation;
		}
	}
}
