using System.Configuration;
using SKDDriver.Translators;

namespace SKDDriver
{
	public static class SKDDatabaseService
	{
		static DataAccess.SKDDataContext Context;
		static string ConnectionString
		{
			get	{ return ConfigurationManager.ConnectionStrings["SKDDriver.Properties.Settings.SKDConnectionString"].ConnectionString; }
		}

		static SKDDatabaseService()
		{
			Context = new DataAccess.SKDDataContext(ConnectionString);
			CardDoorTranslator = new CardDoorTranslator(Context);
			CardTranslator = new CardTranslator(Context, CardDoorTranslator);
			AccessTemplateTranslator = new AccessTemplateTranslator(Context, CardDoorTranslator);
			JournalItemTranslator = new JournalItemTranslator(Context);
			PhotoTranslator = new PhotoTranslator(Context);
			OrganisationTranslator = new OrganisationTranslator(Context, PhotoTranslator);
			PositionTranslator = new PositionTranslator(Context, PhotoTranslator);
			NightSettingsTranslator = new NightSettingsTranslator(Context);
			DepartmentTranslator = new DepartmentTranslator(Context, PhotoTranslator);
			AdditionalColumnTypeTranslator = new AdditionalColumnTypeTranslator(Context);
			AdditionalColumnTranslator = new AdditionalColumnTranslator(Context, PhotoTranslator, AdditionalColumnTypeTranslator);
			DayIntervalPartTranslator = new DayIntervalPartTranslator(Context);
			DayIntervalTranslator = new DayIntervalTranslator(Context, DayIntervalPartTranslator);
			HolidayTranslator = new HolidayTranslator(Context);
			ScheduleDayIntervalTranslator = new ScheduleDayIntervalTranslator(Context);
			ScheduleSchemeTranslator = new ScheduleSchemeTranslator(Context, ScheduleDayIntervalTranslator);
			ScheduleZoneTranslator = new ScheduleZoneTranslator(Context);
			ScheduleTranslator = new ScheduleTranslator(Context, ScheduleZoneTranslator);
			EmployeeTranslator = new EmployeeTranslator(Context, PositionTranslator, DepartmentTranslator, AdditionalColumnTranslator, CardTranslator, PhotoTranslator, ScheduleTranslator);
			TimeTrackTranslator = new TimeTrackTranslator(Context);
			TimeTrackDocumentTranslator = new TimeTrackDocumentTranslator(Context);
			AdditionalColumnTypeTranslator.AdditionalColumnTranslator = AdditionalColumnTranslator;
		}

		public static NightSettingsTranslator NightSettingsTranslator { get; private set; }
		public static PositionTranslator PositionTranslator { get; private set; }
		public static CardTranslator CardTranslator { get; private set; }
		public static CardDoorTranslator CardDoorTranslator { get; private set; }
		public static AccessTemplateTranslator AccessTemplateTranslator { get; private set; }
		public static OrganisationTranslator OrganisationTranslator { get; private set; }
		public static JournalItemTranslator JournalItemTranslator { get; private set; }
		public static EmployeeTranslator EmployeeTranslator { get; private set; }
		public static DepartmentTranslator DepartmentTranslator { get; private set; }
		public static AdditionalColumnTypeTranslator AdditionalColumnTypeTranslator { get; private set; }
		public static AdditionalColumnTranslator AdditionalColumnTranslator { get; private set; }
		public static PhotoTranslator PhotoTranslator { get; private set; }
		public static DayIntervalTranslator DayIntervalTranslator { get; private set; }
		public static DayIntervalPartTranslator DayIntervalPartTranslator { get; private set; }
		public static HolidayTranslator HolidayTranslator { get; private set; }
		public static ScheduleSchemeTranslator ScheduleSchemeTranslator { get; private set; }
		public static ScheduleDayIntervalTranslator ScheduleDayIntervalTranslator { get; private set; }
		public static ScheduleZoneTranslator ScheduleZoneTranslator { get; private set; }
		public static ScheduleTranslator ScheduleTranslator { get; private set; }
		public static TimeTrackTranslator TimeTrackTranslator { get; private set; }
		public static TimeTrackDocumentTranslator TimeTrackDocumentTranslator { get; private set; }
	}
}