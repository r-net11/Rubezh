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
            UID = Guid.NewGuid();
            Employees = new List<ShortEmployee>();
            Cards = new List<SKDCard>();
        }
        
        public Guid UID;
        public List<ShortEmployee> Employees;
        public List<SKDCard> Cards;
        public DbCallbackResultType DbCallbackResultType;
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