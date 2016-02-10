--CREATE DATABASE "SKD"
-- SELECT pg_terminate_backend(pg_stat_activity.pid)
-- FROM pg_stat_activity
-- WHERE pg_stat_activity.datname = 'TARGET_DB'
--   AND pid <> pg_backend_pid();
--DROP DATABASE "SKD";

CREATE TABLE "Photo"(
	"UID" uuid CONSTRAINT "PK_Photo" PRIMARY KEY,
	"Data" bytea NULL
);
CREATE INDEX "PhotoUIDIndex" ON "Photo"("UID");

CREATE TABLE "Organisation"(
	"UID" uuid CONSTRAINT "PK_Organisation" PRIMARY KEY,
	"ChiefUID" uuid,
	"HRChiefUID" uuid,
	"PhotoUID" uuid,
	"Name" character varying(50),
	"Description" character varying(4000),
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp,
	"Phone" character varying(50),
	"ExternalKey" character varying(40)
);
CREATE INDEX "OrganisationUIDIndex" ON "Organisation"("UID");

CREATE TABLE "Employee"(
	"UID" uuid CONSTRAINT "PK_Employee" PRIMARY KEY,
	"PositionUID" uuid,
 	"DepartmentUID" uuid,
 	"ScheduleUID" uuid,
 	"PhotoUID" uuid, 
	"OrganisationUID" uuid,
	"EscortUID" uuid,
	"FirstName" character varying(50),
	"SecondName" character varying(50),
	"LastName" character varying(50),
	"ScheduleStartDate" timestamp NOT NULL,
	"Type" int NOT NULL,
	"TabelNo" character varying(40),
	"CredentialsStartDate" timestamp NOT NULL,
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp,
	"DocumentNumber" character varying(50),
	"BirthDate" timestamp NOT NULL,
	"BirthPlace" character varying(4000),
	"DocumentGivenDate" timestamp NOT NULL,
	"DocumentGivenBy" character varying(4000),
	"DocumentValidTo" timestamp NOT NULL,
	"Gender" int NOT NULL,
	"DocumentDepartmentCode" character varying(50),
	"Citizenship" character varying(4000),
	"DocumentType" int NOT NULL,
	"Phone" character varying(50),
	"Description" character varying(4000),
	"LastEmployeeDayUpdate" timestamp NOT NULL,
	"ExternalKey" character varying(40)
 );
CREATE INDEX "EmployeeUIDIndex" ON "Employee"("UID");
CREATE INDEX "EmployeeOrganisationUIDIndex" ON "Employee"("OrganisationUID");
CREATE INDEX "EmployeePositionUIDIndex" ON "Employee"("PositionUID");
CREATE INDEX "EmployeeDepartmentUIDIndex" ON "Employee"("DepartmentUID");
CREATE INDEX "EmployeeScheduleUIDIndex" ON "Employee"("ScheduleUID");

CREATE TABLE "AdditionalColumnType"(
	"UID" uuid CONSTRAINT "PK_AdditionalColumnType" PRIMARY KEY,
	"OrganisationUID" uuid,
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp,
	"Name" character varying(50),
	"Description" character varying(50),
	"DataType" int NOT NULL,
	"PersonType" int NOT NULL,
	"IsInGrid" boolean NOT NULL
);
CREATE INDEX "AdditionalColumnTypeUIDIndex" ON "AdditionalColumnType"("UID");
CREATE INDEX "AdditionalColumnTypeOrganisationUIDIndex" ON "AdditionalColumnType"("OrganisationUID");

CREATE TABLE "AdditionalColumn"(
	"UID" uuid CONSTRAINT "PK_AdditionalColumn" PRIMARY KEY,
	"EmployeeUID" uuid,
	"AdditionalColumnTypeUID" uuid,
	"PhotoUID" uuid,
	"TextData" text
);
CREATE INDEX "AdditionalColumnEmployeeUIDIndex" ON "AdditionalColumn"("EmployeeUID");

CREATE TABLE "ScheduleDay"(
	"UID" uuid CONSTRAINT "PK_ScheduleDay" PRIMARY KEY,
	"DayIntervalUID" uuid,
	"ScheduleSchemeUID" uuid,
	"Number" int NOT NULL
);
CREATE INDEX "ScheduleDayScheduleSchemeUIDIndex" ON "ScheduleDay"("ScheduleSchemeUID");

CREATE TABLE "DayIntervalPart"(
	"UID" uuid CONSTRAINT "PK_DayIntervalPart" PRIMARY KEY,
	"DayIntervalUID" uuid,
	"BeginTime" int NOT NULL,
	"EndTime" int NOT NULL
);
CREATE INDEX "DayIntervalPartDayIntervalUIDIndex" ON "DayIntervalPart"("DayIntervalUID");

CREATE TABLE "DayInterval"(
	"UID" uuid CONSTRAINT "PK_DayInterval" PRIMARY KEY,
	"OrganisationUID" uuid,
	"Name" character varying(50),
	"Description" character varying(4000),
	"SlideTime" int NOT NULL,
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp
);
CREATE INDEX "DayIntervalUIDIndex" ON "DayInterval"("UID");
CREATE INDEX "DayIntervalOrganisationUIDIndex" ON "DayInterval"("OrganisationUID");

CREATE TABLE "Schedule"(
	"UID" uuid CONSTRAINT "PK_Schedule" PRIMARY KEY,
	"ScheduleSchemeUID" uuid,
	"OrganisationUID" uuid,
	"Name" character varying(50),
	"Description" character varying(4000),
	"IsIgnoreHoliday" boolean NOT NULL,
	"IsOnlyFirstEnter" boolean NOT NULL,
	"AllowedLate" int NOT NULL,
	"AllowedEarlyLeave" int NOT NULL,
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp
 );
CREATE INDEX "ScheduleUIDIndex" ON "Schedule"("UID");
CREATE INDEX "ScheduleOrganisationUIDIndex" ON "Schedule"("OrganisationUID");

CREATE TABLE "ScheduleScheme"(
	"UID" uuid CONSTRAINT "PK_ScheduleScheme" PRIMARY KEY,	
	"OrganisationUID" uuid,
	"Name" character varying(50),
	"Description" character varying(4000),
	"Type" int NOT NULL,
	"DaysCount" int NOT NULL,
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp
);
CREATE INDEX "ScheduleSchemeUIDIndex" ON "ScheduleScheme"("UID");
CREATE INDEX "ScheduleSchemeOrganisationUIDIndex" ON "ScheduleScheme"("OrganisationUID");

CREATE TABLE "Position"(
	"UID" uuid CONSTRAINT "PK_Position" PRIMARY KEY,
	"OrganisationUID" uuid,
	"PhotoUID" uuid,
	"Name" character varying(50),
	"Description" character varying(4000),
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp,
	"ExternalKey" character varying(40)
);
CREATE INDEX "PositionUIDIndex" ON "Position"("UID");
CREATE INDEX "PositionOrganisationUIDIndex" ON "Position"("OrganisationUID");

CREATE TABLE "Department"(
	"UID" uuid CONSTRAINT "PK_Department" PRIMARY KEY,
	"OrganisationUID" uuid,
	"PhotoUID" uuid,
	"ParentDepartmentUID" uuid,
	"ChiefUID" uuid,
	"Name" character varying(50),
	"Description" character varying(4000),
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp,
	"Phone" character varying(50),
	"ExternalKey" character varying(40)
);
CREATE INDEX "DepartmentUIDIndex" ON "Department"("UID");
CREATE INDEX "DepartmentOrganisationUIDIndex" ON "Department"("OrganisationUID");
CREATE INDEX "DepartmentParentDepartmentUIDIndex" ON "Department"("ParentDepartmentUID");

CREATE TABLE "Holiday"(
	"UID" uuid CONSTRAINT "PK_Holiday" PRIMARY KEY,
	"OrganisationUID" uuid,
	"Name" character varying(50),
	"Description" character varying(4000),
	"Type" int NOT NULL,
	"Date" timestamp NOT NULL,
	"TransferDate" timestamp,
	"Reduction" int,
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp
);
CREATE INDEX "HolidayUIDIndex" ON "Holiday"("UID");
CREATE INDEX "HolidayOrganisationUIDIndex" ON "Holiday"("OrganisationUID");

CREATE TABLE "Card"(
	"UID" uuid CONSTRAINT "PK_Card" PRIMARY KEY,
	"EmployeeUID" uuid,
	"AccessTemplateUID" uuid,
	"PassCardTemplateUID" uuid,
	"Number" int NOT NULL,
	"CardType" int NOT NULL,
	"StartDate" timestamp NOT NULL,
	"EndDate" timestamp NOT NULL,
	"IsInStopList" boolean NOT NULL,
	"StopReason" character varying(4000),
	"Password" character varying(50),
	"DeactivationControllerUID" uuid NOT NULL,
	"UserTime" int NOT NULL,
	"GKLevel" smallint NOT NULL,
	"GKLevelSchedule" smallint NOT NULL,
	"ExternalKey" character varying(40),
	"GKCardType" int NOT NULL
);
CREATE INDEX "CardUIDIndex" ON "Card"("UID");
CREATE INDEX "CardEmployeeUIDIndex" ON "Card"("EmployeeUID");
CREATE INDEX "CardAccessTemplateUIDIndex" ON "Card"("AccessTemplateUID");
CREATE INDEX "CardPassCardTemplateUIDIndex" ON "Card"("PassCardTemplateUID");

CREATE TABLE "PendingCard"(
	"UID" uuid CONSTRAINT "PK_PendingCard" PRIMARY KEY,
	"CardUID" uuid,
	"ControllerUID" uuid NOT NULL,
	"Action" int NOT NULL
);
CREATE INDEX "PendingCardUIDIndex" ON "PendingCard"("UID");

CREATE TABLE "CardDoor"(
	"UID" uuid CONSTRAINT "PK_CardDoor" PRIMARY KEY,
	"CardUID" uuid,
	"AccessTemplateUID" uuid,
	"DoorUID" uuid NOT NULL,
	"EnterScheduleNo" int NOT NULL,
	"ExitScheduleNo" int NOT NULL
);
CREATE INDEX "CardDoorCardUIDIndex" ON "CardDoor"("CardUID");
CREATE INDEX "CardDoorAccessTemplateUIDIndex" ON "CardDoor"("AccessTemplateUID");

CREATE TABLE "AccessTemplate"(
	"UID" uuid CONSTRAINT "PK_AccessTemplate" PRIMARY KEY,
	"OrganisationUID" uuid,
	"Name" character varying(50),
	"Description" character varying(4000),
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp
);
CREATE INDEX "AccessTemplateUIDIndex" ON "AccessTemplate"("UID");
CREATE INDEX "AccessTemplateOrganisationUIDIndex" ON "AccessTemplate"("OrganisationUID");

CREATE TABLE "ScheduleZone"(
	"UID" uuid CONSTRAINT "PK_ScheduleZone" PRIMARY KEY,
	"ScheduleUID" uuid,
	"ZoneUID" uuid NOT NULL,
	"DoorUID" uuid NOT NULL
);
CREATE INDEX "ScheduleZoneScheduleUIDIndex" ON "ScheduleZone"("ScheduleUID");

CREATE TABLE "TimeTrackDocument"(
	"UID" uuid CONSTRAINT "PK_TimeTrackDocument" PRIMARY KEY,
	"EmployeeUID" uuid,
	"StartDateTime" timestamp NOT NULL,
	"EndDateTime" timestamp NOT NULL,
	"DocumentCode" int NOT NULL,
	"Comment" character varying(100),
	"FileName" character varying(100),
	"DocumentDateTime" timestamp NOT NULL,
	"DocumentNumber" int NOT NULL
);
CREATE INDEX "TimeTrackDocumentUIDIndex" ON "TimeTrackDocument"("UID");
CREATE INDEX "TimeTrackDocumentEmployeeUIDIndex" ON "TimeTrackDocument"("EmployeeUID");

CREATE TABLE "PassCardTemplate"(
	"UID" uuid CONSTRAINT "PK_PassCardTemplate" PRIMARY KEY,
	"OrganisationUID" uuid,
	"Name" character varying(50),
	"Description" character varying(4000),
	"IsDeleted" boolean NOT NULL,
	"RemovalDate" timestamp,
	"Data" bytea
);
CREATE INDEX "PassCardTemplateUIDIndex" ON "PassCardTemplate"("UID");
CREATE INDEX "PassCardTemplateOrganisationUIDIndex" ON "PassCardTemplate"("OrganisationUID");

CREATE TABLE "OrganisationUser"(
	"UID" uuid CONSTRAINT "PK_OrganisationUser" PRIMARY KEY,
	"OrganisationUID" uuid,
	"UserUID" uuid NOT NULL
);
CREATE INDEX "OrganisationUserOrganisationUIDIndex" ON "OrganisationUser"("OrganisationUID");

CREATE TABLE "OrganisationDoor"(
	"UID" uuid CONSTRAINT "PK_OrganisationDoor" PRIMARY KEY,
	"OrganisationUID" uuid,
	"DoorUID" uuid NOT NULL
);
CREATE INDEX "OrganisationDoorOrganisationUIDIndex" ON "OrganisationDoor"("OrganisationUID");

CREATE TABLE "NightSetting"(
	"UID" uuid CONSTRAINT "PK_NightSetting" PRIMARY KEY,
	"OrganisationUID" uuid,
	"NightStartTime" bigint NOT NULL,
	"NightEndTime" bigint NOT NULL
);
CREATE INDEX "NightSettingUIDIndex" ON "NightSetting"("UID");
CREATE INDEX "NightSettingOrganisationUIDIndex" ON "NightSetting"("OrganisationUID");

CREATE TABLE "TimeTrackDocumentType"(
	"UID" uuid CONSTRAINT "PK_TimeTrackDocumentType" PRIMARY KEY,
	"OrganisationUID" uuid,
	"Name" character varying(4000),
	"ShortName" character varying(10),
	"DocumentCode" int NOT NULL,
	"DocumentType" int NOT NULL
);
CREATE INDEX "TimeTrackDocumentTypeUIDIndex" ON "TimeTrackDocumentType"("UID");
CREATE INDEX "TimeTrackDocumentTypeOrganisationUIDIndex" ON "TimeTrackDocumentType"("OrganisationUID");

CREATE TABLE "TimeTrackException"(
	"UID" uuid CONSTRAINT "PK_TimeTrackException" PRIMARY KEY,
	"EmployeeUID" uuid,
	"StartDateTime" timestamp NOT NULL,
	"EndDateTime" timestamp NOT NULL,
	"DocumentType" int NOT NULL,
	"Comment" character varying(100)
);
CREATE INDEX "TimeTrackExceptionUIDIndex" ON "TimeTrackException"("UID");
CREATE INDEX "TimeTrackExceptionEmployeeUIDIndex" ON "TimeTrackException"("EmployeeUID");

CREATE TABLE "GKMetadata"(
	"UID" uuid CONSTRAINT "PK_GKMetadata" PRIMARY KEY,
	"IPAddress" character varying(50) NOT NULL,
	"SerialNo" character varying(50) NOT NULL,
	"LastJournalNo" int NOT NULL
);
CREATE INDEX "GKMetadataUIDIndex" ON "GKMetadata"("UID");

CREATE TABLE "GKCard"(
	"UID" uuid CONSTRAINT "PK_GKCard" PRIMARY KEY,
	"IPAddress" character varying(50),
	"GKNo" int NOT NULL,
	"CardNo" int NOT NULL,
	"FIO" character varying(50) NOT NULL,
	"IsActive" boolean NOT NULL,
	"UserType" smallint NOT NULL
);
CREATE INDEX "GKCardUIDIndex" ON "GKCard"("UID");

CREATE TABLE "GKSchedule"(
	"UID" uuid CONSTRAINT "PK_GKSchedule" PRIMARY KEY,
	"No" int NOT NULL,
	"Name" character varying(50),
	"Description" character varying(50),
	"Type" int NOT NULL,
	"PeriodType" int NOT NULL,
	"StartDateTime" timestamp NOT NULL,
	"HoursPeriod" int NOT NULL, 
	"HolidayScheduleNo" int NOT NULL,
	"WorkingHolidayScheduleNo" int NOT NULL,
	"Year" int NOT NULL	
);
CREATE INDEX "GKScheduleUIDIndex" ON "GKSchedule"("UID");

CREATE TABLE "GKScheduleDay"(
	"UID" uuid CONSTRAINT "PK_GKScheduleDay" PRIMARY KEY,
	"ScheduleUID" uuid,
	"DateTime" timestamp NOT NULL
);
CREATE INDEX "GKScheduleDayScheduleUIDIndex" ON "GKScheduleDay"("ScheduleUID");

CREATE TABLE "GKDaySchedule"(
	"UID" uuid CONSTRAINT "PK_GKDaySchedule" PRIMARY KEY,
	"No" int NOT NULL,
	"Name" character varying(50),
	"Description" character varying(50)
);
CREATE INDEX "GKDayScheduleUIDIndex" ON "GKDaySchedule"("UID");

CREATE TABLE "ScheduleGKDaySchedule"(
	"UID" uuid  CONSTRAINT "PK_ScheduleGKDaySchedule" PRIMARY KEY,
	"ScheduleUID" uuid,
	"DayScheduleUID" uuid
);
CREATE INDEX "ScheduleGKDayScheduleScheduleUIDIndex" ON "ScheduleGKDaySchedule"("ScheduleUID");

CREATE TABLE "GKDaySchedulePart"(
	"UID" uuid CONSTRAINT "PK_GKDaySchedulePart" PRIMARY KEY,
	"DayScheduleUID" uuid NOT NULL,
	"No" int NOT NULL,
	"Name" character varying(50),
	"Description" character varying(50),
	"StartMilliseconds" int NOT NULL,
	"EndMilliseconds" int NOT NULL
);
CREATE INDEX "GKDaySchedulePartDayScheduleUIDIndex" ON "GKDaySchedulePart"("DayScheduleUID");


CREATE TABLE "CurrentConsumption"(
	"UID" uuid CONSTRAINT "PK_CurrentConsumption" PRIMARY KEY,
	"AlsUID" uuid NOT NULL,
	"Current" int NOT NULL,
	"DateTime" timestamp NOT NULL
);
CREATE INDEX "CurrentConsumptionAlsUIDIndex" ON "CurrentConsumption"("AlsUID");

CREATE TABLE "CardGKControllerUID"(
	"UID" uuid CONSTRAINT "PK_CardGKControllerUID" PRIMARY KEY,
	"CardUID" uuid,
	"GKControllerUID" uuid NOT NULL
);
CREATE INDEX "CardGKControllerUIDCardUIDIndex" ON "CardGKControllerUID"("CardUID");

CREATE TABLE "Journal"(
	"UID" uuid CONSTRAINT "PK_Journal" PRIMARY KEY,
	"EmployeeUID" uuid,
	"SystemDate" timestamp NOT NULL,
	"DeviceDate" timestamp,
	"Subsystem" int NOT NULL,
	"Name" int NOT NULL,
	"Description" int NOT NULL,
	"DescriptionText" character varying(4000),
	"ObjectType" int NOT NULL,
	"ObjectName" character varying(100),
	"ObjectUID" uuid NOT NULL,
	"Detalisation" character varying(4000),
	"UserName" character varying(50),
	"VideoUID" uuid,
	"CameraUID" uuid,
	"CardNo" int NOT NULL
);
CREATE INDEX "JournalUIDIndex" ON "Journal"("UID");
CREATE INDEX "JournalDeviceDateIndex" ON "Journal"("DeviceDate");
CREATE INDEX "JournalSystemDateIndex" ON "Journal"("SystemDate");
CREATE INDEX "JournalNameIndex" ON "Journal"("Name");
CREATE INDEX "JournalDescriptionIndex" ON "Journal"("Description");
CREATE INDEX "JournalObjectUIDIndex" ON "Journal"("ObjectUID");

CREATE TABLE "PassJournal"(
	"UID" uuid CONSTRAINT "PK_PassJournal" PRIMARY KEY,
	"EmployeeUID" uuid,
	"ZoneUID" uuid NOT NULL,
	"EnterTime" timestamp NOT NULL,
	"ExitTime" timestamp NULL
);
CREATE INDEX "PassJournalUIDIndex" ON "PassJournal"("UID");

CREATE TABLE "EmployeeDay"(
	"UID" uuid CONSTRAINT "PK_EmployeeDay" PRIMARY KEY,
	"EmployeeUID" uuid,
	"IsIgnoreHoliday" boolean NOT NULL,
	"IsOnlyFirstEnter" boolean NOT NULL,
	"AllowedLate" int NOT NULL,
	"AllowedEarlyLeave" int NOT NULL,
	"DayIntervalsString" character varying(4000),
	"Date" timestamp NOT NULL
);
CREATE INDEX "EmployeeDayUIDIndex" ON "EmployeeDay"("UID");
CREATE INDEX "EmployeeDayEmployeeUIDIndex" ON "EmployeeDay"("EmployeeUID");

CREATE TABLE "Patches" (
	"Id" character varying(100) CONSTRAINT "PK_Patches" PRIMARY KEY
);

ALTER TABLE "Organisation" ADD CONSTRAINT "FK_Organisation_PhotoUID" FOREIGN KEY ("PhotoUID") REFERENCES "Photo" ("UID") ON DELETE CASCADE;
ALTER TABLE "Organisation" ADD CONSTRAINT "FK_Organisation_ChiefUID" FOREIGN KEY ("ChiefUID") REFERENCES "Employee" ("UID") ON DELETE SET NULL;
ALTER TABLE "Organisation" ADD CONSTRAINT "FK_Organisation_HRChiefUID" FOREIGN KEY ("HRChiefUID") REFERENCES "Employee" ("UID") ON DELETE SET NULL;

ALTER TABLE "Employee" ADD CONSTRAINT "FK_Employee_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;
ALTER TABLE "Employee" ADD CONSTRAINT "FK_Employee_PhotoUID" FOREIGN KEY ("PhotoUID") REFERENCES "Photo" ("UID") ON DELETE SET NULL;
ALTER TABLE "Employee" ADD CONSTRAINT "FK_Employee_EscortUID" FOREIGN KEY ("EscortUID") REFERENCES "Employee" ("UID") ON DELETE SET NULL;
ALTER TABLE "Employee" ADD CONSTRAINT "FK_Employee_ScheduleUID" FOREIGN KEY ("ScheduleUID") REFERENCES "Schedule" ("UID");
ALTER TABLE "Employee" ADD CONSTRAINT "FK_Employee_PositionUID" FOREIGN KEY ("PositionUID") REFERENCES "Position" ("UID") ON DELETE SET NULL;
ALTER TABLE "Employee" ADD CONSTRAINT "FK_Employee_DepartmentUID" FOREIGN KEY ("DepartmentUID") REFERENCES "Department" ("UID") ON DELETE SET NULL;

ALTER TABLE "AdditionalColumnType" ADD CONSTRAINT "FK_AdditionalColumnType_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;

ALTER TABLE "AdditionalColumn" ADD CONSTRAINT "FK_AdditionalColumn_PhotoUID" FOREIGN KEY ("PhotoUID") REFERENCES "Photo" ("UID") ON DELETE CASCADE;
ALTER TABLE "AdditionalColumn" ADD CONSTRAINT "FK_AdditionalColumn_EmployeeUID" FOREIGN KEY ("EmployeeUID") REFERENCES "Employee" ("UID") ON DELETE CASCADE;
ALTER TABLE "AdditionalColumn" ADD CONSTRAINT "FK_AdditionalColumn_AdditionalColumnTypeUID" FOREIGN KEY ("AdditionalColumnTypeUID") REFERENCES "AdditionalColumnType" ("UID") ON DELETE CASCADE;

ALTER TABLE "ScheduleDay" ADD CONSTRAINT "FK_ScheduleDay_DayIntervalUID" FOREIGN KEY ("DayIntervalUID") REFERENCES "DayInterval" ("UID") ON DELETE SET NULL;
ALTER TABLE "ScheduleDay" ADD CONSTRAINT "FK_ScheduleDay_ScheduleSchemeUID" FOREIGN KEY ("ScheduleSchemeUID") REFERENCES "ScheduleScheme" ("UID") ON DELETE CASCADE;

ALTER TABLE "DayIntervalPart" ADD CONSTRAINT "FK_DayIntervalPart_DayIntervalUID" FOREIGN KEY ("DayIntervalUID") REFERENCES "DayInterval" ("UID") ON DELETE CASCADE;
ALTER TABLE "DayInterval" ADD CONSTRAINT "FK_DayInterval_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;

ALTER TABLE "ScheduleScheme" ADD CONSTRAINT "FK_ScheduleScheme_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;

ALTER TABLE "Schedule" ADD CONSTRAINT "FK_Schedule_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;
ALTER TABLE "Schedule" ADD CONSTRAINT "FK_Schedule_ScheduleSchemeUID" FOREIGN KEY ("ScheduleSchemeUID") REFERENCES "ScheduleScheme" ("UID") ON DELETE SET NULL;

ALTER TABLE "Position" ADD CONSTRAINT "FK_Position_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;
ALTER TABLE "Position" ADD CONSTRAINT "FK_Position_PhotoUID" FOREIGN KEY ("PhotoUID") REFERENCES "Photo" ("UID") ON DELETE SET NULL;

ALTER TABLE "Department" ADD CONSTRAINT "FK_Department_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;
ALTER TABLE "Department" ADD CONSTRAINT "FK_Department_PhotoUID" FOREIGN KEY ("PhotoUID") REFERENCES "Photo" ("UID") ON DELETE SET NULL;
ALTER TABLE "Department" ADD CONSTRAINT "FK_Department_ParentDepartmentUID" FOREIGN KEY ("ParentDepartmentUID") REFERENCES "Department" ("UID") ON DELETE SET NULL;
ALTER TABLE "Department" ADD CONSTRAINT "FK_Department_ChiefUID" FOREIGN KEY ("ChiefUID") REFERENCES "Employee" ("UID") ON DELETE SET NULL;

ALTER TABLE "Holiday" ADD CONSTRAINT "FK_Holiday_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;

ALTER TABLE "Card" ADD CONSTRAINT "FK_Card_EmployeeUID" FOREIGN KEY ("EmployeeUID") REFERENCES "Employee" ("UID") ON DELETE SET NULL;
ALTER TABLE "Card" ADD CONSTRAINT "FK_Card_AccessTemplateUID" FOREIGN KEY ("AccessTemplateUID") REFERENCES "AccessTemplate" ("UID") ON DELETE SET NULL;
ALTER TABLE "Card" ADD CONSTRAINT "FK_Card_PassCardTemplateUID" FOREIGN KEY ("PassCardTemplateUID") REFERENCES "PassCardTemplate" ("UID");

ALTER TABLE "PendingCard" ADD CONSTRAINT "FK_PendingCard_CardUID" FOREIGN KEY ("CardUID") REFERENCES "Card" ("UID") ON DELETE CASCADE;

ALTER TABLE "CardDoor" ADD CONSTRAINT "FK_CardDoor_CardUID" FOREIGN KEY ("CardUID") REFERENCES "Card" ("UID") ON DELETE CASCADE;
ALTER TABLE "CardDoor" ADD CONSTRAINT "FK_CardDoor_AccessTemplateUID" FOREIGN KEY ("AccessTemplateUID") REFERENCES "AccessTemplate" ("UID") ON DELETE CASCADE;

ALTER TABLE "AccessTemplate" ADD CONSTRAINT "FK_AccessTemplate_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;
ALTER TABLE "ScheduleZone" ADD CONSTRAINT "FK_ScheduleZone_ScheduleUID" FOREIGN KEY ("ScheduleUID") REFERENCES "Schedule" ("UID") ON DELETE CASCADE;
ALTER TABLE "TimeTrackDocument" ADD CONSTRAINT "FK_TimeTrackDocument_EmployeeUID" FOREIGN KEY ("EmployeeUID") REFERENCES "Employee" ("UID") ON DELETE SET NULL;
ALTER TABLE "PassCardTemplate" ADD CONSTRAINT "FK_PassCardTemplate_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;
ALTER TABLE "OrganisationUser" ADD CONSTRAINT "FK_OrganisationUser_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE CASCADE;
ALTER TABLE "OrganisationDoor" ADD CONSTRAINT "FK_OrganisationDoor_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE CASCADE;
ALTER TABLE "NightSetting" ADD CONSTRAINT "FK_NightSetting_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;
ALTER TABLE "TimeTrackDocumentType" ADD CONSTRAINT "FK_TimeTrackDocumentType_OrganisationUID" FOREIGN KEY ("OrganisationUID") REFERENCES "Organisation" ("UID") ON DELETE SET NULL;
ALTER TABLE "TimeTrackException" ADD CONSTRAINT "FK_TimeTrackException_EmployeeUID" FOREIGN KEY ("EmployeeUID") REFERENCES "Employee" ("UID") ON DELETE SET NULL;
ALTER TABLE "GKScheduleDay" ADD CONSTRAINT "FK_GKScheduleDay_ScheduleUID" FOREIGN KEY ("ScheduleUID") REFERENCES "GKSchedule" ("UID") ON DELETE CASCADE;

ALTER TABLE "ScheduleGKDaySchedule" ADD CONSTRAINT "FK_ScheduleGKDaySchedule_ScheduleUID" FOREIGN KEY ("ScheduleUID") REFERENCES "GKSchedule" ("UID") ON DELETE CASCADE;
ALTER TABLE "ScheduleGKDaySchedule" ADD CONSTRAINT "FK_ScheduleGKDaySchedule_DayScheduleUID" FOREIGN KEY ("DayScheduleUID") REFERENCES "GKDaySchedule" ("UID") ON DELETE SET NULL;

ALTER TABLE "GKDaySchedulePart" ADD CONSTRAINT "FK_GKDaySchedulePart_DayScheduleUID" FOREIGN KEY ("DayScheduleUID") REFERENCES "GKDaySchedule" ("UID") ON DELETE CASCADE;
ALTER TABLE "CardGKControllerUID" ADD CONSTRAINT "FK_CardGKControllerUID_CardUID" FOREIGN KEY ("CardUID") REFERENCES "Card" ("UID") ON DELETE CASCADE;

ALTER TABLE "Journal" ADD CONSTRAINT "FK_Journal_EmployeeUID" FOREIGN KEY ("EmployeeUID") REFERENCES "Employee" ("UID") ON DELETE SET NULL;
ALTER TABLE "PassJournal" ADD CONSTRAINT "FK_PassJournal_EmployeeUID" FOREIGN KEY ("EmployeeUID") REFERENCES "Employee" ("UID") ON DELETE SET NULL;

CREATE EXTENSION "uuid-ossp";
CREATE FUNCTION CreateOrganisation() RETURNS void AS $$
DECLARE
	OrgUID uuid;
BEGIN
	OrgUID := uuid_generate_v4();
	INSERT INTO "Organisation" ("UID", "Name" ,"IsDeleted") VALUES (OrgUid,'Организация','0');
	INSERT INTO "OrganisationUser" ("UID","UserUID","OrganisationUID") VALUES (uuid_generate_v4(),'10e591fb-e017-442d-b176-f05756d984bb',OrgUid);
END;
$$ LANGUAGE plpgsql;
select * from CreateOrganisation();











