using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace License.Model.Entities
{
	[Serializable]
	[DataContract]
	public class LicenseEntity : ILicenseEntity
	{
		[XmlElement("UID")]
		[DataMember]
		public string UID { get; set; }

		[XmlElement("CreateDateTime")]
		[DataMember]
		public DateTime CreateDateTime { get; set; }

		[XmlElement("OperatorConnectionsNumber")]
		[Category("License Options")]
		[DataMember]
		public int OperatorConnectionsNumber { get; set; }

		[XmlElement("TotalUsers")]
		[Category("License Options")]
		[DataMember]
		public int TotalUsers { get; set; }

		[XmlElement("IsUnlimitedUsers")]
		[Category("License Options")]
		[DataMember]
		public bool IsUnlimitedUsers { get; set; }

		[XmlElement("IsEnabledURV")]
		[Category("License Options")]
		[DataMember]
		public bool IsEnabledURV { get; set; }

		[XmlElement("IsEnabledPhotoVerification")]
		[Category("License Options")]
		[DataMember]
		public bool IsEnabledPhotoVerification { get; set; }

		[XmlElement("IsEnabledRVI")]
		[Category("License Options")]
		[DataMember]
		public bool IsEnabledRVI { get; set; }

		[XmlElement("IsEnabledAutomation")]
		[Category("License Options")]
		[DataMember]
		public bool IsEnabledAutomation { get; set; }

		[XmlElement("IsEnabledServer")]
		[Category("License Options")]
		[DataMember]
		public bool IsEnabledServer { get; set; }

		public LicenseEntity()
		{
			IsEnabledServer = true;
			CreateDateTime = DateTime.Now;
		}
	}
}
