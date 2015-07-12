﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;

namespace SKDDriver.DataClasses
{
	public class DatabaseContext : DbContext
	{
		public static string ConnectionStringName;
		public DatabaseContext()
			: base(ConnectionStringName)
		{
			Database.SetInitializer<DatabaseContext>(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>(ConnectionStringName));
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
	}

	internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
		}

		protected override void Seed(DatabaseContext context)
		{
			if(context.Organisations.Count() == 0)
			{
				context.Organisations.Add(new Organisation
				{
					UID = Guid.NewGuid(),
					Name = "Огранизация",
					Users = new List<OrganisationUser>
					{  
						new OrganisationUser { UID = Guid.NewGuid(), UserUID = new Guid("10e591fb-e017-442d-b176-f05756d984bb") }
					}
				});
			}
			base.Seed(context);
		}
	}
}