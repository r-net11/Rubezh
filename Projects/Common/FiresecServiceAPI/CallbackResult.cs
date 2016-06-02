using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.AutomationCallback;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using StrazhAPI.SKD.Device;

namespace StrazhAPI
{
	[DataContract]
	public class CallbackResult
	{
		[DataMember]
		public Guid ArchivePortionUID { get; set; }

		[DataMember]
		public CallbackResultType CallbackResultType { get; set; }

		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public SKDProgressCallback SKDProgressCallback { get; set; }

		[DataMember]
		public SKDCallbackResult SKDCallbackResult { get; set; }

		[DataMember]
		public SKDStates SKDStates { get; set; }

		[DataMember]
		public AutomationCallbackResult AutomationCallbackResult { get; set; }

		[DataMember]
		public List<SKDDeviceSearchInfo> SearchDevices { get; set; }

		[DataMember]
		public SKDCard Card { get; set; }
	}

	public enum CallbackResultType
	{
		SKDProgress,
		SKDObjectStateChanged,
		NewEvents,
		ArchiveCompleted,
		AutomationCallbackResult,
		ConfigurationChanged,
		Disconnecting,
		NewSearchDevices,

		/// <summary>
		/// Команда со стороны Сервера на закрытие соединения Клиентом
		/// </summary>
		DisconnectClientCommand,

		/// <summary>
		/// Уведовление о том, что на Сервере сменилась лицензия
		/// </summary>
		LicenseChanged,

		/// <summary>
		/// Уведомление о том, что был осуществлен проход по "Гостевой" карте
		/// </summary>
		GuestCardPassed,

		/// <summary>
		/// Уведомление о деактивации карты
		/// </summary>
		CardDeactivated,

		/// <summary>
		/// Уведомление о изменении лога загрузки ядра сервера
		/// </summary>
		CoreLoadingLogChanged
	}
}