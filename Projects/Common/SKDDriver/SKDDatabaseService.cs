﻿using System;
using Infrastructure.Common;
using SKDDriver.Translators;

namespace SKDDriver
{
	public class SKDDatabaseService : IDisposable
	{
		public DataAccess.SKDDataContext Context { get; private set; }

		public static string ConnectionString
		{
			get
			{
				var serverName = GlobalSettingsHelper.GlobalSettings.DBServerName;
				var connectionString = @"Data Source=.\" + serverName + ";Initial Catalog=SKD;Integrated Security=True;Language='English'";
				return connectionString;
			}
		}

		public SKDDatabaseService()
		{
			Context = new DataAccess.SKDDataContext(ConnectionString);
			Context.CommandTimeout = 600;

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
			GKMetadataTranslator = new GKMetadataTranslator(this);
			GKCardTranslator = new GKCardTranslator(this);
			GKScheduleTranslator = new GKScheduleTranslator(this);
			CurrentConsumptionTranslator = new CurrentConsumptionTranslator(this);
			if (PassJournalTranslator.ConnectionString != null)
			{
				PassJournalTranslator = new PassJournalTranslator();
				
			}
		}

		public static void Tst()
		{
			EFTest.EFTest.Test();
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
		public GKMetadataTranslator GKMetadataTranslator { get; private set; }
		public GKCardTranslator GKCardTranslator { get; private set; }
		public PassJournalTranslator PassJournalTranslator { get; private set; }
		public GKScheduleTranslator GKScheduleTranslator { get; private set; }
		public CurrentConsumptionTranslator CurrentConsumptionTranslator { get; private set; } 

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}