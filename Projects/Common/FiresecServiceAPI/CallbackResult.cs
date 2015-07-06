using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecAPI.AutomationCallback;

namespace FiresecAPI
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
        public DbCallbackResult DbCallbackResult { get; set; }

        [DataMember]
		public GKProgressCallback GKProgressCallback { get; set; }

		[DataMember]
		public GKCallbackResult GKCallbackResult { get; set; }

		[DataMember]
		public SKDStates SKDStates { get; set; }

		[DataMember]
		public AutomationCallbackResult AutomationCallbackResult { get; set; }
	}

	public enum CallbackResultType
	{
		GKProgress,
		GKObjectStateChanged,
		SKDObjectStateChanged,
		NewEvents,
		ArchiveCompleted,
		AutomationCallbackResult,
		ConfigurationChanged,
		Disconnecting, 
        QueryDb
	}

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
        }
        
        public Guid ClientUID;
        public List<ShortEmployee> Employees;
        public List<SKDCard> Cards;
        public List<AccessTemplate> AccessTemplates;
        public List<AdditionalColumnType> AdditionalColumnTypes;
        public List<ShortDepartment> Departments;
        public List<ShortPassCardTemplate> PassCardTemplates;
        public List<ShortPosition> Positions;
        public List<DayInterval> DayIntervals;
        public List<ScheduleScheme> ScheduleSchemes;
        public List<Holiday> Holidays;
        public List<Schedule> Schedules;
        public bool IsLastPortion;
    }

    [DataContract]
    [KnownType(typeof(ShortEmployee))]
    public class DbCallbackResult<T>
    {
        public DbCallbackResult()
        {
            UID = Guid.NewGuid();
            
        }

        public Guid UID;
        public List<T> Items;
        public bool IsLastPortion;
    }

    public enum DbCallbackResultType
    {
        Employees,
        Cards
    }
    
}