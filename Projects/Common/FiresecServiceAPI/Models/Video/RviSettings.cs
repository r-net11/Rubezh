using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class RviSettings
	{
		public RviSettings()
		{
			VideoIntegrationProvider = VideoIntegrationProvider.RviOperator;
			Ip = "localhost";
			Port = 8000;
			Login = "strazh";
			Password = "strazh12345";
		}

		[DataMember]
		public VideoIntegrationProvider VideoIntegrationProvider { get; set; }

		[DataMember]
		public string Ip { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }
	}

	/// <summary>
	/// Тип сервера интеграции с видео
	/// </summary>
	public enum VideoIntegrationProvider
	{
		/// <summary>
		/// RVi Оператор
		/// </summary>
		[Description("RVi Оператор")]
		RviOperator,

		/// <summary>
		/// RVi Интегратор
		/// </summary>
		[Description("RVi Интегратор")]
		RviIntegrator
	}
}