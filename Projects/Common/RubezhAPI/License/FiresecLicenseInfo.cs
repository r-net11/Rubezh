using System.Runtime.Serialization;

namespace RubezhAPI.License
{
	/// <summary>
	/// Разрешения лицензии
	/// </summary>
	[DataContract]
	public class FiresecLicenseInfo
	{
		/// <summary>
		/// Тип лицензии
		/// </summary>
		[DataMember]
		public LicenseMode LicenseMode { get; set; }

		/// <summary>
		/// Количество удаленных рабочих мест
		/// </summary>
		[DataMember]
		public int RemoteClientsCount { get; set; }

		/// <summary>
		/// Разрешение на пожаротушение
		/// </summary>
		[DataMember]
		public bool HasFirefighting { get; set; }

		/// <summary>
		/// Разрешение на охранные зоны
		/// </summary>
		[DataMember]
		public bool HasGuard { get; set; }

		/// <summary>
		/// Разрешение на СКД
		/// </summary>
		[DataMember]
		public bool HasSKD { get; set; }

		/// <summary>
		/// Разрешение на видео
		/// </summary>
		[DataMember]
		public bool HasVideo { get; set; }

		/// <summary>
		/// Разрешение на OPC-сервер
		/// </summary>
		[DataMember]
		public bool HasOpcServer { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is FiresecLicenseInfo)
			{
				var licenseInfo = obj as FiresecLicenseInfo;
				return licenseInfo.RemoteClientsCount == this.RemoteClientsCount
					&& licenseInfo.HasFirefighting == this.HasFirefighting
					&& licenseInfo.HasGuard == this.HasGuard
					&& licenseInfo.HasSKD == this.HasSKD
					&& licenseInfo.HasVideo == this.HasVideo
					&& licenseInfo.HasOpcServer == this.HasOpcServer;
			}
			else
				return false;
		}
	}
}