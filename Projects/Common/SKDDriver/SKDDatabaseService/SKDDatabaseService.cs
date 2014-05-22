using SKDDriver.Translators;

namespace SKDDriver
{
	public static class SKDDatabaseService
	{
		static DataAccess.SKDDataContext Context;

		static SKDDatabaseService()
		{
			Context = new DataAccess.SKDDataContext();
			DocumentTranslator = new DocumentTranslator(Context);
			CardZoneTranslator = new CardZoneTranslator(Context);
			CardTranslator = new CardTranslator(Context, CardZoneTranslator);
			AccessTemplateTranslator = new AccessTemplateTranslator(Context, CardZoneTranslator);
			JournalItemTranslator = new JournalItemTranslator(Context);
			PhotoTranslator = new PhotoTranslator(Context);
			OrganisationTranslator = new OrganisationTranslator(Context, PhotoTranslator);
			PositionTranslator = new PositionTranslator(Context, PhotoTranslator);
			DepartmentTranslator = new DepartmentTranslator(Context, PhotoTranslator);
			AdditionalColumnTypeTranslator = new AdditionalColumnTypeTranslator(Context);
			AdditionalColumnTranslator = new AdditionalColumnTranslator(Context, PhotoTranslator, AdditionalColumnTypeTranslator);
			EmployeeReplacementTranslator = new EmployeeReplacementTranslator(Context);
			TimeIntervalTranslator = new TimeIntervalTranslator(Context);
			NamedIntervalTranslator = new NamedIntervalTranslator(Context, TimeIntervalTranslator);
			HolidayTranslator = new HolidayTranslator(Context);
			DayIntervalTranslator = new DayIntervalTranslator(Context);
			ScheduleSchemeTranslator = new ScheduleSchemeTranslator(Context, DayIntervalTranslator);
			ScheduleZoneTranslator = new ScheduleZoneTranslator(Context);
			ScheduleTranslator = new ScheduleTranslator(Context, ScheduleZoneTranslator);
			EmployeeTranslator = new EmployeeTranslator(Context, EmployeeReplacementTranslator, PositionTranslator, DepartmentTranslator, AdditionalColumnTranslator, CardTranslator, PhotoTranslator, ScheduleTranslator);
		}

		public static DocumentTranslator DocumentTranslator { get; private set; }
		public static PositionTranslator PositionTranslator { get; private set; }
		public static CardTranslator CardTranslator { get; private set; }
		public static CardZoneTranslator CardZoneTranslator { get; private set; }
		public static AccessTemplateTranslator AccessTemplateTranslator { get; private set; }
		public static OrganisationTranslator OrganisationTranslator { get; private set; }
		public static JournalItemTranslator JournalItemTranslator { get; private set; }
		public static EmployeeTranslator EmployeeTranslator { get; private set; }
		public static DepartmentTranslator DepartmentTranslator { get; private set; }
		public static AdditionalColumnTypeTranslator AdditionalColumnTypeTranslator { get; private set; }
		public static AdditionalColumnTranslator AdditionalColumnTranslator { get; private set; }
		public static PhotoTranslator PhotoTranslator { get; private set; }
		public static EmployeeReplacementTranslator EmployeeReplacementTranslator { get; private set; }
		public static NamedIntervalTranslator NamedIntervalTranslator { get; private set; }
		public static TimeIntervalTranslator TimeIntervalTranslator { get; private set; }
		public static HolidayTranslator HolidayTranslator { get; private set; }
		public static ScheduleSchemeTranslator ScheduleSchemeTranslator { get; private set; }
		public static DayIntervalTranslator DayIntervalTranslator { get; private set; }
		public static ScheduleZoneTranslator ScheduleZoneTranslator { get; private set; }
		public static ScheduleTranslator ScheduleTranslator { get; private set; }
	}
}