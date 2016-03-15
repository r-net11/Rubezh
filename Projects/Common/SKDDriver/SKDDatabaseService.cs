using System.Data.SqlClient;
using System.Text;
using Infrastructure.Common;
using SKDDriver.DataAccess;
using SKDDriver.Translators;
using System;

namespace SKDDriver
{
	public class SKDDatabaseService : IDisposable
	{
		public SKDDataContext Context { get; private set; }

		/// <summary>
		/// Возвращает строку соединения для MS SQL Server
		/// </summary>
		/// <param name="ipAddress">IP-адрес сервера СУБД</param>
		/// <param name="ipPort">IP-порт сервера СУБД</param>
		/// <param name="instanceName">Название инстанса сервера СУБД</param>
		/// <param name="db">Имя базы данных</param>
		/// <param name="useIntegratedSecurity">Режим аутентификации на сервере СУБД</param>
		/// <param name="userID">Логин (для режима аутентификации посредством SQL Server)</param>
		/// <param name="userPwd">Пароль (для режима аутентификации посредством SQL Server)</param>
		/// <returns>Строка соединения для MS SQL Server</returns>
		public static string BuildConnectionString(string ipAddress, int ipPort, string instanceName, string db, bool useIntegratedSecurity = true, string userID = null, string userPwd = null)
		{
			var csb = new SqlConnectionStringBuilder();
			csb.DataSource = String.Format(@"{0}{1},{2}", ipAddress, String.IsNullOrEmpty(instanceName) ? String.Empty : String.Format(@"\{0}", instanceName), ipPort);
			csb.InitialCatalog = db;
			csb.IntegratedSecurity = useIntegratedSecurity;
			if (!csb.IntegratedSecurity)
			{
				csb.UserID = userID;
				csb.Password = userPwd;
			}
			return csb.ConnectionString;
		}

		/// <summary>
		/// Возвращает строку соединения к указанной базе с учетом настроенных в файле конфигурации прочих параметров соединения
		/// </summary>
		/// <param name="db">Название базы данных</param>
		/// <returns>Строка соединения с базой данных СУБД</returns>
		public static string GetConnectionString(string db)
		{
			return BuildConnectionString(
				AppServerSettingsHelper.AppServerSettings.DBServerAddress,
				AppServerSettingsHelper.AppServerSettings.DBServerPort,
				AppServerSettingsHelper.AppServerSettings.DBServerName,
				db,
				AppServerSettingsHelper.AppServerSettings.DBUseIntegratedSecurity,
				AppServerSettingsHelper.AppServerSettings.DBUserID,
				AppServerSettingsHelper.AppServerSettings.DBUserPwd);
		}

		public static string MasterConnectionString { get { return GetConnectionString("master"); } }
		public static string SkdConnectionString { get { return GetConnectionString("SKD"); } }
		public static string JournalConnectionString { get { return GetConnectionString("Journal_1"); } }
		public static string PassJournalConnectionString { get { return GetConnectionString("PassJournal_1"); } }

		public SKDDatabaseService()
		{
			Context = new SKDDataContext(SkdConnectionString);

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
			AttachmentTranslator = new AttachmentTranslator(this);
			ReportFiltersTranslator = new ReportFiltersTranslator(this);
			LicenseInfoTranslator = new LicenseInfoTranslator(this);
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

		public AttachmentTranslator AttachmentTranslator { get; private set; }

		public ReportFiltersTranslator ReportFiltersTranslator { get; private set; }

		public LicenseInfoTranslator LicenseInfoTranslator { get; private set; }

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}