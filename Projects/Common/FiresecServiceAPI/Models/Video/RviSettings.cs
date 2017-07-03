﻿using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class RviSettings
	{
		public RviSettings()
		{
			VideoIntegrationProvider = VideoIntegrationProvider.RviOperator;
			Ip = "localhost";
			Port = 8091;
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
		[Description("RVi Operator 1.5")]
		RviOperator,

		/// <summary>
		/// RVi Интегратор
		/// </summary>
		[Description("RVi Operator 2")]
		RviIntegrator
	}
}