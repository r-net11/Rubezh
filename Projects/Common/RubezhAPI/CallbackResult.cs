using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;

namespace RubezhAPI
{
	/// <summary>
	/// Порция изменений, которые сервер должен передать клиенту
	/// </summary>
	[DataContract]
	public class CallbackResult
	{
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
		GKPropertyChanged,
		NewEvents,
		AutomationCallbackResult,
		ConfigurationChanged,
		Disconnecting,
		OperationResult
	}

    [DataContract]
    public class DbCallbackResult
    {
        public DbCallbackResult()
        {
            ClientUID = Guid.NewGuid();
            Employees = new List<ShortEmployee>();
            Cards = new List<SKDCard>();
            AccessTemplates = new List<AccessTemplate>();
            AdditionalColumnTypes = new List<AdditionalColumnType>();
            Departments = new List<ShortDepartment>();
            PassCardTemplates = new List<ShortPassCardTemplate>();
            Positions = new List<ShortPosition>();
            DayIntervals = new List<DayInterval>();
            ScheduleSchemes = new List<ScheduleScheme>();
            Schedules = new List<Schedule>();
            Positions = new List<ShortPosition>();
			Holidays = new List<Holiday>();
        }

        [DataMember]
        public Guid ClientUID { get; set; }
        [DataMember]
        public List<ShortEmployee> Employees { get; set; }
        [DataMember]
        public List<SKDCard> Cards { get; set; }
        [DataMember]
        public List<AccessTemplate> AccessTemplates { get; set; }
        [DataMember]
        public List<AdditionalColumnType> AdditionalColumnTypes { get; set; }
        [DataMember]
        public List<ShortDepartment> Departments { get; set; }
        [DataMember]
        public List<ShortPassCardTemplate> PassCardTemplates { get; set; }
        [DataMember]
        public List<ShortPosition> Positions { get; set; }
        [DataMember]
        public List<DayInterval> DayIntervals { get; set; }
        [DataMember]
        public List<ScheduleScheme> ScheduleSchemes { get; set; }
        [DataMember]
        public List<Holiday> Holidays { get; set; }
        [DataMember]
        public List<Schedule> Schedules { get; set; }
        [DataMember]
        public bool IsLastPortion { get; set; }
    }
}