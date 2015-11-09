using System.Data.SqlClient;
using System.Text;
using Infrastructure.Common;
using SKDDriver.Translators;
using System;

namespace SKDDriver
{
	public class SKDDatabaseService : IDisposable
	{
		public DataAccess.SKDDataContext Context { get; private set; }

		public static string GetConnectionString(string db)
		{
			var csb = new SqlConnectionStringBuilder();
			csb.DataSource = String.Format(@"{0}\{1}", AppServerSettingsHelper.AppServerSettings.DBServerAddress, AppServerSettingsHelper.AppServerSettings.DBServerName);
			csb.InitialCatalog = db;
			csb.IntegratedSecurity = AppServerSettingsHelper.AppServerSettings.DBUseIntegratedSecurity;
			if (!csb.IntegratedSecurity)
			{
				csb.UserID = AppServerSettingsHelper.AppServerSettings.DBUserID;
				csb.Password = AppServerSettingsHelper.AppServerSettings.DBUserPwd;
			}
			return csb.ConnectionString;
		}

		public static string MasterConnectionString { get { return GetConnectionString("master"); } }
		public static string SkdConnectionString { get { return GetConnectionString("SKD"); } }
		public static string JournalConnectionString { get { return GetConnectionString("Journal_1"); } }
		public static string PassJournalConnectionString { get { return GetConnectionString("PassJournal_1"); } }

		public SKDDatabaseService()
		{
			Context = new DataAccess.SKDDataContext(SkdConnectionString);

			CardDoorTranslator = new CardDoorTranslator(this);
			CardTranslator = new CardTranslator(this);
			AccessTemplateTranslator = new AccessTemplateTranslator(this);
			PhotoTranslator = new PhotoTranslator(this);
			OrganisationTranslator = new OrganisationTranslator(this);
			PositionTranslator = new PositionTranslator(this);
			NightSettingsTranslator = new NightSettingsTranslator(this);
			DepartmentTranslator = new DepartmentTranslator(this);
			AdditionalColumnTypeTranslator = new AdditionalColumnTypeTranslator(this);
			AdditionalColumnTranslator = new AdditionalColumnTranslator(this);
			DayIntervalPartTranslator = new DayIntervalPartTranslator(this);
			DayIntervalTranslator = new DayIntervalTranslator(this);
			HolidayTranslator = new HolidayTranslator(this);
			ScheduleDayIntervalTranslator = new ScheduleDayIntervalTranslator(this);
			ScheduleSchemeTranslator = new ScheduleSchemeTranslator(this);
			ScheduleZoneTranslator = new ScheduleZoneTranslator(this);
			ScheduleTranslator = new ScheduleTranslator(this);
			EmployeeTranslator = new EmployeeTranslator(this);
			TimeTrackTranslator = new TimeTrackTranslator(this);
			TimeTrackDocumentTranslator = new TimeTrackDocumentTranslator(this);
			TimeTrackDocumentTypeTranslator = new TimeTrackDocumentTypeTranslator(this);
			PassCardTemplateTranslator = new PassCardTemplateTranslator(this);
			MetadataTranslator = new MetadataTranslator(this);
			if (PassJournalTranslator.ConnectionString != null)
			{
				PassJournalTranslator = new PassJournalTranslator();
			}
		}

		public NightSettingsTranslator NightSettingsTranslator { get; private set; }

		public PositionTranslator PositionTranslator { get; private set; }

		public CardTranslator CardTranslator { get; private set; }

		public CardDoorTranslator CardDoorTranslator { get; private set; }

		public AccessTemplateTranslator AccessTemplateTranslator { get; private set; }

		public OrganisationTranslator OrganisationTranslator { get; private set; }

		public EmployeeTranslator EmployeeTranslator { get; private set; }

		public DepartmentTranslator DepartmentTranslator { get; private set; }

		public AdditionalColumnTypeTranslator AdditionalColumnTypeTranslator { get; private set; }

		public AdditionalColumnTranslator AdditionalColumnTranslator { get; private set; }

		public PhotoTranslator PhotoTranslator { get; private set; }

		public DayIntervalTranslator DayIntervalTranslator { get; private set; }

		public DayIntervalPartTranslator DayIntervalPartTranslator { get; private set; }

		public HolidayTranslator HolidayTranslator { get; private set; }

		public ScheduleSchemeTranslator ScheduleSchemeTranslator { get; private set; }

		public ScheduleDayIntervalTranslator ScheduleDayIntervalTranslator { get; private set; }

		public ScheduleZoneTranslator ScheduleZoneTranslator { get; private set; }

		public ScheduleTranslator ScheduleTranslator { get; private set; }

		public TimeTrackTranslator TimeTrackTranslator { get; private set; }

		public TimeTrackDocumentTranslator TimeTrackDocumentTranslator { get; private set; }

		public TimeTrackDocumentTypeTranslator TimeTrackDocumentTypeTranslator { get; private set; }

		public PassCardTemplateTranslator PassCardTemplateTranslator { get; private set; }

		public MetadataTranslator MetadataTranslator { get; private set; }

		public PassJournalTranslator PassJournalTranslator { get; private set; }

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}