using System.Data.Entity;

namespace SKDDriver.DataClasses
{
	public class SKDDbContext : DbContext
	{
		public ContextType ContextType {get; private set;}

		public SKDDbContext(string connectionStringName, ContextType contextType)
			: base(connectionStringName)
		{
			ContextType = contextType;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			string schemaStr;
			switch (ContextType)
			{
				case ContextType.MSSQL:
					schemaStr = "dbo";
					break;
				case ContextType.PostgreSQL:
					schemaStr = "public";
					break;
				default:
					schemaStr = "";
					break;
			}
			modelBuilder.Entity<GKSchedule>().ToTable("GKSchedule", schemaStr);
			modelBuilder.Entity<GKScheduleDay>().ToTable("GKScheduleDay", schemaStr);
			modelBuilder.Entity<GKDaySchedule>().ToTable("GKDaySchedule", schemaStr);
			modelBuilder.Entity<GKDaySchedulePart>().ToTable("GKDaySchedulePart", schemaStr);
			modelBuilder.Entity<ScheduleGKDaySchedule>().ToTable("ScheduleGKDaySchedule", schemaStr);
			modelBuilder.Entity<PassJournal>().ToTable("PassJournal", schemaStr);
			modelBuilder.Entity<Journal>().ToTable("Journal", schemaStr);
			modelBuilder.Entity<EmployeeDay>().ToTable("EmployeeDay", schemaStr);
			modelBuilder.Entity<DayInterval>().ToTable("DayInterval", schemaStr);
			modelBuilder.Entity<DayIntervalPart>().ToTable("DayIntervalPart", schemaStr);
			modelBuilder.Entity<Schedule>().ToTable("Schedule", schemaStr);
			modelBuilder.Entity<ScheduleScheme>().ToTable("ScheduleScheme", schemaStr);
			modelBuilder.Entity<ScheduleDay>().ToTable("ScheduleDay", schemaStr);
			modelBuilder.Entity<ScheduleZone>().ToTable("ScheduleZone", schemaStr);
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
		public DbSet<TimeTrackDocumnetType> TimeTrackDocumnetTypes { get; set; }
		public DbSet<TimeTrackException> TimeTrackExceptions { get; set; }
	}
}
