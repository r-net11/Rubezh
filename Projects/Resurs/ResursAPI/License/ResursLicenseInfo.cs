using System.Runtime.Serialization;

namespace ResursAPI.License
{
	/// <summary>
	/// Разрешения лицензии
	/// </summary>
	[DataContract]
	public class ResursLicenseInfo
	{
		/// <summary>
		/// Количество устройств
		/// </summary>
		[DataMember]
		public int DevicesCount { get; set; }

		public override bool Equals(object obj)
		{
			var licenseInfo = obj as ResursLicenseInfo;
			return licenseInfo == null ? 
				false : 
				licenseInfo.DevicesCount == this.DevicesCount;
		}
	}
}