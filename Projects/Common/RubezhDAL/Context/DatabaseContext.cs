using Infrastructure.Common;
using RubezhDAL.DataClasses;
using RubezhDAL.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;

namespace RubezhDAL
{
	public class DatabaseContext : DbContext
	{
		public static bool IsPostgres { get { return GlobalSettingsHelper.GlobalSettings.DbSettings.DbType == RubezhAPI.DbType.Postgres; } }
		public DatabaseContext(DbConnection connection)
			: base(connection, true)
		{
			Database.SetInitializer<DatabaseContext>(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>(true));
		}

		public void Seed()
		{
			if (!IsPostgres)
			{
				string journalClusteredIndex = GetIndexName("UID", "Journals");
				if (journalClusteredIndex != null)
					SetPkNonclustered(journalClusteredIndex, "Journals");
			}
			if (Organisations.Count() == 0)
			{
				var organisation = new Organisation
				{
					UID = Guid.NewGuid(),
					Name = "Организация",
					Users = new List<OrganisationUser>
					{  
						new OrganisationUser 
						{ 
							UID = Guid.NewGuid(), 
							UserUID = new Guid("10e591fb-e017-442d-b176-f05756d984bb") 
						}
					}
				};
				Organisations.Add(organisation);
			}
			if (GKDaySchedules.Count() == 0)
			{
				var neverDaySchedule = new GKDaySchedule
				{
					UID = Guid.NewGuid(),
					Name = "<Никогда>",
					No = 1
				};
				GKDaySchedules.Add(neverDaySchedule);

				var alwaysDaySchedule = new GKDaySchedule
				{
					UID = Guid.NewGuid(),
					Name = "<Всегда>",
					No = 2
				};
				alwaysDaySchedule.GKDayScheduleParts.Add(new GKDaySchedulePart()
					{
						StartMilliseconds = 0,
						EndMilliseconds = (int)new TimeSpan(1, 0, 0, 0, 0).TotalMilliseconds
					});
				GKDaySchedules.Add(alwaysDaySchedule);
			}
		}

		string GetIndexName(string columnName, string tableName)
		{
			if (IsPostgres)
				return null;
			string query = string.Format(
				"SELECT ind.name FROM sys.indexes ind " +
				"INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id " +
				"INNER JOIN sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id " +
				"INNER JOIN sys.tables t ON ind.object_id = t.object_id " +
				"WHERE ind.type_desc = 'CLUSTERED' and col.name = '{0}' and t.name = '{1}' ", columnName, tableName);
			return Database.SqlQuery<string>(query).FirstOrDefault();
		}

		void SetPkNonclustered(string keyName, string tableName)
		{
			if (IsPostgres || keyName == null)
				return;
			string query = string.Format("ALTER TABLE {0} DROP CONSTRAINT \"{1}\"" +
				"ALTER TABLE {0} ADD CONSTRAINT \"{1}\"" +
				"PRIMARY KEY NONCLUSTERED (\"UID\")", tableName, keyName);
			Database.ExecuteSqlCommand(query);
		}

		public DbSet<GKSchedule> GKSchedules { get; set; }
		public DbSet<GKScheduleDay> GKScheduleDays { get; set; }
		public DbSet<GKDaySchedule> GKDaySchedules { get; set; }
		public DbSet<GKDaySchedulePart> GKDayScheduleParts { get; set; }
		public DbSet<ScheduleGKDaySchedule> ScheduleGKDaySchedules { get; set; }
		public DbSet<PassJournal> PassJournals { get; set; }
		public DbSet<Journal> Journals { get; set; }
		public DbSet<EmployeeDay> EmployeeDays { get; set; }
		public DbSet<AccessTemplate> AccessTemplates { get; set; }
		public DbSet<AdditionalColumn> AdditionalColumns { get; set; }
		public DbSet<AdditionalColumnType> AdditionalColumnTypes { get; set; }
		public DbSet<Card> Cards { get; set; }
		public DbSet<CardDoor> CardDoors { get; set; }
		public DbSet<CardGKControllerUID> CardGKControllerUIDs { get; set; }
		public DbSet<CurrentConsumption> CurrentConsumptions { get; set; }
		public DbSet<DayInterval> DayIntervals { get; set; }
		public DbSet<DayIntervalPart> DayIntervalParts { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Employee> Employees { get; set; }
		public DbSet<GKCard> GKCards { get; set; }
		public DbSet<GKMetadata> GKMetadatas { get; set; }
		public DbSet<Holiday> Holidays { get; set; }
		public DbSet<NightSetting> NightSettings { get; set; }
		public DbSet<Organisation> Organisations { get; set; }
		public DbSet<OrganisationDoor> OrganisationDoors { get; set; }
		public DbSet<OrganisationUser> OrganisationUsers { get; set; }
		public DbSet<PassCardTemplate> PassCardTemplates { get; set; }
		public DbSet<Patches> Patches { get; set; }
		public DbSet<PendingCard> PendingCards { get; set; }
		public DbSet<Photo> Photos { get; set; }
		public DbSet<Position> Positions { get; set; }
		public DbSet<Schedule> Schedules { get; set; }
		public DbSet<ScheduleDay> ScheduleDays { get; set; }
		public DbSet<ScheduleScheme> ScheduleSchemes { get; set; }
		public DbSet<ScheduleZone> ScheduleZones { get; set; }
		public DbSet<TimeTrackDocument> TimeTrackDocuments { get; set; }
		public DbSet<TimeTrackDocumentType> TimeTrackDocumentTypes { get; set; }
		public DbSet<TimeTrackException> TimeTrackExceptions { get; set; }
		public DbSet<ImitatorUser> ImitatorUsers { get; set; }
		public DbSet<ImitatorUserDevice> ImitatorUserDevices { get; set; }
		public DbSet<ImitatorSchedule> ImitatorSchedules { get; set; }
		public DbSet<ImitatorSheduleInterval> ImitatorSheduleIntervals { get; set; }
		public DbSet<ImitatorJournalItem> ImitatorJournalItems { get; set; }
	}
}
