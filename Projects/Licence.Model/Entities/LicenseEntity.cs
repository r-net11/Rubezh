using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace License.Model.Entities
{
	[Serializable]
	public class LicenseEntity : ILicenseEntity
	{
		[XmlElement("UID")]
		public string UID { get; set; }

		[XmlElement("CreateDateTime")]
		public DateTime CreateDateTime { get; set; }

		[XmlElement("OperatorConnectionsNumber")]
		[Category("License Options")]
		public int OperatorConnectionsNumber { get; set; }

		[XmlElement("TotalUsers")]
		[Category("License Options")]
		public int TotalUsers { get; set; }

		[XmlElement("IsUnlimitedUsers")]
		[Category("License Options")]
		public bool IsUnlimitedUsers { get; set; }

		[XmlElement("IsEnabledURV")]
		[Category("License Options")]
		public bool IsEnabledURV { get; set; }

		[XmlElement("IsEnabledPhotoVerification")]
		[Category("License Options")]
		public bool IsEnabledPhotoVerification { get; set; }

		[XmlElement("IsEnabledRVI")]
		[Category("License Options")]
		public bool IsEnabledRVI { get; set; }

		[XmlElement("IsEnabledAutomation")]
		[Category("License Options")]
		public bool IsEnabledAutomation { get; set; }
	}
}
