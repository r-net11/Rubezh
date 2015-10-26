using System.Runtime.Serialization;

namespace FiresecLicense
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
        public int RemoteWorkplacesCount { get; set; }

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
    }
}