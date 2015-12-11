using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI
{
	[DataContract]
	public class PollResult
	{
		/// <summary>
		/// Индекс сообщения
		/// </summary>
		[DataMember]
		public int CallbackIndex { get; set; }

		/// <summary>
		/// Клиент отключается
		/// </summary>
		[DataMember]
		public bool IsDisconnecting { get; set; }

		/// <summary>
		/// Требуется перезапуск клиента
		/// </summary>
		[DataMember]
		public bool IsReconnectionRequired { get; set; }

		/// <summary>
		/// Изменена конфигурация
		/// </summary>
		//[DataMember]
		//public bool IsConfigurationChanged { get; set; }

		/// <summary>
		/// Набор изменений для отправки клиенту
		/// </summary>
		[DataMember]
		public List<CallbackResult> CallbackResults { get; set; }

		public PollResult()
		{
			CallbackResults = new List<CallbackResult>();
		}

	}
}