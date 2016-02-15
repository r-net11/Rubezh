using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI
{
	/// <summary>
	/// Порция изменений, которые сервер должен передать клиенту
	/// </summary>
	[DataContract]
	public class CallbackResult
	{
		/// <summary>
		/// Индекс сообщения
		/// </summary>
		[DataMember]
		public int Index { get; set; }

		/// <summary>
		/// Тип результата изменения
		/// </summary>
		[DataMember]
		public CallbackResultType CallbackResultType { get; set; }

		/// <summary>
		/// Список событий
		/// </summary>
		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		/// <summary>
		/// Прогресс операций
		/// </summary>
		[DataMember]
		public GKProgressCallback GKProgressCallback { get; set; }

		/// <summary>
		/// Изменения, связанные с ГК
		/// </summary>
		[DataMember]
		public GKCallbackResult GKCallbackResult { get; set; }
		/// <summary>
		/// Изменения, связанные с Rvi
		/// </summary>
		[DataMember]
		public RviCallbackResult RviCallbackResult { get; set; }
		/// <summary>
		/// Изменения параметров устройств ГК
		/// </summary>
		[DataMember]
		public GKPropertyChangedCallback GKPropertyChangedCallback { get; set; }

		[DataMember]
		public AutomationCallbackResult AutomationCallbackResult { get; set; }

		[DataMember]
		public CallbackOperationResult CallbackOperationResult { get; set; }
	}

	public enum CallbackResultType
	{
		GKProgress,
		GKObjectStateChanged,
		RviObjectStateChanged,
		GKPropertyChanged,
		NewEvents,
		UpdateEvents,
		AutomationCallbackResult,
		OperationResult,
		ConfigurationChanged
	}
}