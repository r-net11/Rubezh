using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FiresecAPI
{
	/// <summary>
	/// Класс объекта состояния служб Сервера приложений
	/// </summary>
	[DataContract]
	public class AppServerHealthInfo
	{
		/// <summary>
		/// Состояние службы по работе с СУБД
		/// </summary>
		[DataMember]
		public ServiceHealthStatus DatabaseServiceHealthStatus { get; set; }

		/// <summary>
		/// Состояние службы по генерации отчетов
		/// </summary>
		[DataMember]
		public ServiceHealthStatus ReportServiceHealthStatus { get; set; }

		/// <summary>
		/// Состояние службы автоматизации
		/// </summary>
		[DataMember]
		public ServiceHealthStatus AutomationServiceHealthStatus { get; set; }
	}

	public enum ServiceHealthStatus
	{
		/// <summary>
		/// Служба работает и готова принимать клиентские запросы
		/// </summary>
		Alive = 0,
		/// <summary>
		/// Служба не работает
		/// </summary>
		Dead = 1
	}
}
