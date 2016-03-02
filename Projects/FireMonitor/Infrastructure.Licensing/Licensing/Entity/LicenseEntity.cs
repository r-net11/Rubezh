using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Licensing.Entity
{
	public class LicenseEntity
	{
		public string UID { get; set; }

		public int OperatorConnectionsNumber { get; set; }

		public int TotalUsers { get; set; }

		public bool IsUnlimitedUsers { get; set; }

		public bool IsEnabledURV { get; set; }

		public bool IsEnabledPhotoVerification { get; set; }

		public bool IsEnabledRVI { get; set; }

		public bool IsEnabledAutomation { get; set; }

		public DateTime CreateDateTime { get; set; }
	}
}
