using FiresecAPI;
using System;

namespace SKDDriver.DataClasses
{
	public class DbService : IDisposable
	{
		public DatabaseContext Context;

		public GKScheduleTranslator GKScheduleTranslator { get; private set; }
		public GKDayScheduleTranslator GKDayScheduleTranslator { get; private set; }
		public PassJournalTranslator PassJournalTranslator { get; private set; }
		public JournalTranslator JournalTranslator { get; private set; }
		public AccessTemplateTranslator AccessTemplateTranslator { get; private set; }
		public AdditionalColumnTypeTranslator AdditionalColumnTypeTranslator { get; private set; }
		public CardTranslator CardTranslator { get; private set; }
		public CurrentConsumptionTranslator CurrentConsumptionTranslator { get; private set; }
		public DayIntervalTranslator DayIntervalTranslator { get; private set; }
		public DepartmentTranslator DepartmentTranslator { get; private set; }
		public EmployeeTranslator EmployeeTranslator { get; private set; }
		public HolidayTranslator HolidayTranslator { get; private set; }
		public NightSettingTranslator NightSettingTranslator { get; private set; }
		public OrganisationTranslator OrganisationTranslator { get; private set; }
		public PassCardTemplateTranslator PassCardTemplateTranslator { get; private set; }
		public PositionTranslator PositionTranslator { get; private set; }
		public ScheduleTranslator ScheduleTranslator { get; private set; }
		public ScheduleSchemeTranslator ScheduleSchemeTranslator { get; private set; }
		public TimeTrackTranslator TimeTrackTranslator { get; private set; }
        public GKCardTranslator GKCardTranslator { get; private set; }
        public GKMetadataTranslator GKMetadataTranslator { get; private set; }
        public TimeTrackDocumentTypeTranslator TimeTrackDocumentTypeTranslator { get; private set; }
        public TimeTrackDocumentTranslator TimeTrackDocumentTranslator { get; private set; }
        public TestDataGenerator TestDataGenerator { get; private set; }
        public static bool IsAbort
		{
			get { return JournalTranslator.IsAbort; }
			set { JournalTranslator.IsAbort = value; }
		}

		public DbService()
		{
			Context = new DatabaseContext("SKDDbContext", DbContextType.PostgreSQL);
			GKScheduleTranslator = new GKScheduleTranslator(this);
			GKDayScheduleTranslator = new GKDayScheduleTranslator(this);
			PassJournalTranslator = new PassJournalTranslator(this);
			JournalTranslator = new JournalTranslator(this);
			AccessTemplateTranslator = new AccessTemplateTranslator(this);
			AdditionalColumnTypeTranslator = new AdditionalColumnTypeTranslator(this);
			CardTranslator = new CardTranslator(this);
			CurrentConsumptionTranslator = new CurrentConsumptionTranslator(this);
			DayIntervalTranslator = new DayIntervalTranslator(this);
			DepartmentTranslator = new DepartmentTranslator(this);
			EmployeeTranslator = new EmployeeTranslator(this);
			HolidayTranslator = new HolidayTranslator(this);
			NightSettingTranslator = new NightSettingTranslator(this);
			OrganisationTranslator = new OrganisationTranslator(this);
			PassCardTemplateTranslator = new PassCardTemplateTranslator(this);
			PositionTranslator = new PositionTranslator(this);
			ScheduleTranslator = new ScheduleTranslator(this);
			ScheduleSchemeTranslator = new ScheduleSchemeTranslator(this);
            GKCardTranslator = new GKCardTranslator(this);
            GKMetadataTranslator = new GKMetadataTranslator(this);
			TimeTrackTranslator = new TimeTrackTranslator(this);
            TimeTrackDocumentTypeTranslator = new TimeTrackDocumentTypeTranslator(this);
            TimeTrackDocumentTranslator = new TimeTrackDocumentTranslator(this);
            TestDataGenerator = new TestDataGenerator(this);
		}
        
        public void BeginGet(FiresecAPI.SKD.HRFilter filter, Action<DbCallbackResult> portionReady)
        {
            EmployeeTranslator.AsyncTranslator.BeginGet(filter.EmployeeFilter, portionReady);
            CardTranslator.AsyncTranslator.BeginGet(filter.CardFilter, portionReady);
        }

        public T GetTranslator<T>()
            where T : class
        {
            var result = EmployeeTranslator as T;
            if (result != null)
                return result;
            result = CardTranslator as T;
            if (result != null)
                return result;
            result = EmployeeTranslator.AsyncTranslator as T;
            if (result != null)
                return result;
            result = CardTranslator.AsyncTranslator as T;
            if (result != null)
                return result;
            return null;
        }

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}
