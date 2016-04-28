USE [SKD]
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'OrganisationUser')
BEGIN
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationUser')
BEGIN
CREATE TABLE OrganisationUser(
[UID] [uniqueidentifier] NOT NULL,
[UserUID] [uniqueidentifier] NOT NULL,
[OrganisationUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_OrganisationUser] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[OrganisationUser] WITH NOCHECK ADD CONSTRAINT [FK_OrganisationUser_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[OrganisationUser] NOCHECK CONSTRAINT [FK_OrganisationUser_Organisation]
END
INSERT INTO Patches (Id) VALUES ('OrganisationUser')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AlterPatches')
BEGIN
ALTER TABLE Patches ALTER COLUMN Id nvarchar(100) not null
INSERT INTO Patches (Id) VALUES ('AlterPatches')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Doors')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationZone')
BEGIN
DROP TABLE OrganisationZone
END
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardZone')
BEGIN
DROP TABLE CardZone
END
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardDoor')
BEGIN
CREATE TABLE [dbo].[CardDoor](
[UID] [uniqueidentifier] NOT NULL,
[DoorUID] [uniqueidentifier] NOT NULL,
[ParentUID] [uniqueidentifier] NULL,
[ParentType] [int] NULL,
[IsWithEscort] [bit] NOT NULL,
[IsAntipass] [bit] NOT NULL,
[IntervalUID] [uniqueidentifier] NULL,
[IntervalType] [int] NULL,
[IsDeleted] [bit] NOT NULL ,
[RemovalDate] [datetime] NOT NULL ,
CONSTRAINT [PK_CardDoor] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[CardDoor]  WITH NOCHECK ADD  CONSTRAINT [FK_CardDoor_Card] FOREIGN KEY([ParentUid])
REFERENCES [dbo].[Card] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[CardDoor] NOCHECK CONSTRAINT [FK_CardDoor_Card]

ALTER TABLE [dbo].[CardDoor] WITH NOCHECK ADD  CONSTRAINT [FK_CardDoor_AccessTemplate] FOREIGN KEY([ParentUid])
REFERENCES [dbo].[AccessTemplate] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[CardDoor] NOCHECK CONSTRAINT [FK_CardDoor_AccessTemplate]
END
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationDoor')
BEGIN
CREATE TABLE [dbo].[OrganisationDoor](
[UID] [uniqueidentifier] NOT NULL,
[DoorUID] [uniqueidentifier] NOT NULL,
[OrganisationUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_OrganisationDoor] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[OrganisationDoor] WITH NOCHECK ADD CONSTRAINT [FK_OrganisationDoor_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[OrganisationDoor] NOCHECK CONSTRAINT [FK_OrganisationDoor_Organisation]
END
INSERT INTO Patches (Id) VALUES ('Doors')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DoorsEnterExit')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardDoor')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'IntervalUID' and table_name = 'CardDoor')
BEGIN
ALTER TABLE CardDoor DROP COLUMN IntervalUID
ALTER TABLE CardDoor DROP COLUMN IntervalType
ALTER TABLE CardDoor ADD [EnterIntervalUID] uniqueidentifier NULL
ALTER TABLE CardDoor ADD [EnterIntervalType] int NULL
ALTER TABLE CardDoor ADD [ExitIntervalUID] uniqueidentifier NULL
ALTER TABLE CardDoor ADD [ExitIntervalType] int NULL
END
END
INSERT INTO Patches (Id) VALUES ('DoorsEnterExit')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'OrganisationZone')
BEGIN
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationZone')
BEGIN
CREATE TABLE [dbo].[OrganisationZone](
[UID] [uniqueidentifier] NOT NULL,
[ZoneUID] [uniqueidentifier] NOT NULL,
[OrganisationUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_OrganisationZone] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[OrganisationZone] WITH NOCHECK ADD CONSTRAINT [FK_OrganisationZone_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[OrganisationZone] NOCHECK CONSTRAINT [FK_OrganisationZone_Organisation]
END
INSERT INTO Patches (Id) VALUES ('OrganisationZone')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DoorsEnterExit')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardDoor')
BEGIN
ALTER TABLE CardDoor DROP COLUMN IntervalUID
ALTER TABLE CardDoor DROP COLUMN IntervalType
ALTER TABLE CardDoor ADD [EnterIntervalUID] uniqueidentifier NULL
ALTER TABLE CardDoor ADD [EnterIntervalType] int NULL
ALTER TABLE CardDoor ADD [ExitIntervalUID] uniqueidentifier NULL
ALTER TABLE CardDoor ADD [ExitIntervalType] int NULL
END
INSERT INTO Patches (Id) VALUES ('DoorsEnterExit')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'PendingCard')
BEGIN
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'PendingCard')
BEGIN
CREATE TABLE [dbo].[PendingCard](
[UID] [uniqueidentifier] NOT NULL,
[CardUID] [uniqueidentifier] NOT NULL,
[Action] [int] NOT NULL,
CONSTRAINT [PK_PendingCard] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[PendingCard]  WITH NOCHECK ADD  CONSTRAINT [FK_PendingCard_Card] FOREIGN KEY([CardUid])
REFERENCES [dbo].[Card] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[PendingCard] NOCHECK CONSTRAINT [FK_PendingCard_Card]
END
INSERT INTO Patches (Id) VALUES ('PendingCard')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'CommonJournal')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Journal')
BEGIN
DROP TABLE Journal
END
CREATE TABLE [dbo].[Journal](
[UID] [uniqueidentifier] NOT NULL,
[SystemDate] [datetime] NOT NULL,
[DeviceDate] [datetime] NOT NULL,
[Subsystem] [int] NOT NULL,
[Name] [int] NOT NULL,
[Description] [int] NOT NULL,
[NameText] [nvarchar](50) NULL,
[DescriptionText] [nvarchar](max) NULL,
[State] [int] NOT NULL,
[ObjectType] [int] NOT NULL,
[ObjectName] [nvarchar](50) NULL,
[ObjectUID] [uniqueidentifier] NOT NULL,
[UserName] [nvarchar](50) NULL,
[CardSeries] [int] NOT NULL,
[CardNo] [int] NOT NULL,
CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[Journal]  WITH NOCHECK ADD  CONSTRAINT [FK_Journal_Card] FOREIGN KEY([ObjectUID])
REFERENCES [dbo].[Card] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Journal] NOCHECK CONSTRAINT [FK_Journal_Card]
INSERT INTO Patches (Id) VALUES ('CommonJournal')
END

GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'PassJournal')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'PassJournal')
BEGIN
DROP TABLE PassJournal
END
CREATE TABLE [dbo].[PassJournal](
[UID] [uniqueidentifier] NOT NULL,
[EmployeeUID] [uniqueidentifier] NOT NULL,
[ZoneUID] [uniqueidentifier] NOT NULL,
[EnterTime] [datetime] NOT NULL,
[ExitTime] [datetime] NOT NULL,
CONSTRAINT [PK_PassJournal] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[PassJournal]  WITH NOCHECK ADD  CONSTRAINT [FK_PassJournal_Employee] FOREIGN KEY([EmployeeUID])
REFERENCES [dbo].[Employee] ([UID])
ON UPDATE CASCADE
ON DELETE CASCADE
NOT FOR REPLICATION
ALTER TABLE [dbo].[PassJournal] CHECK CONSTRAINT [FK_PassJournal_Employee]
INSERT INTO Patches (Id) VALUES ('PassJournal')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'PendingCardControllerUID')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'PendingCard')
BEGIN
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'ControllerUID' and table_name = 'PendingCard')
BEGIN
ALTER TABLE PendingCard ADD [ControllerUID] [uniqueidentifier] NOT NULL
END
END
INSERT INTO Patches (Id) VALUES ('PendingCardControllerUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'OrganisationCardTemplate')
BEGIN
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationCardTemplate')
BEGIN
CREATE TABLE [dbo].[OrganisationCardTemplate](
[UID] [uniqueidentifier] NOT NULL,
[CardTemplateUID] [uniqueidentifier] NOT NULL,
[OrganisationUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_OrganisationCardTemplate] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[OrganisationCardTemplate] WITH NOCHECK ADD CONSTRAINT [FK_OrganisationCardTemplate_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[OrganisationCardTemplate] NOCHECK CONSTRAINT [FK_OrganisationCardTemplate_Organisation]
END
INSERT INTO Patches (Id) VALUES ('OrganisationCardTemplate')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveCardSeries')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Card')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'Series' and table_name = 'Card')
BEGIN
ALTER TABLE [Card] DROP COLUMN [Series]
END
END
INSERT INTO Patches (Id) VALUES ('RemoveCardSeries')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveJournalCardSeries')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Journal')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'CardSeries' and table_name = 'Journal')
BEGIN
ALTER TABLE Journal DROP COLUMN [CardSeries]
END
END
INSERT INTO Patches (Id) VALUES ('RemoveJournalCardSeries')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'EventNamesDescription')
BEGIN
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'EventNames')
BEGIN
CREATE TABLE EventNames (EventName int not null)
END
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'EventDescriptions')
BEGIN
CREATE TABLE EventDescriptions (EventDescription int not null)
END
INSERT INTO Patches (Id) VALUES ('EventNamesDescription')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DoorsEnterExitUIDtoID')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardDoor')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'EnterIntervalUID' and table_name = 'CardDoor')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'ExitIntervalUID' and table_name = 'CardDoor')
BEGIN
ALTER TABLE CardDoor DROP COLUMN [EnterIntervalUID]
ALTER TABLE CardDoor DROP COLUMN [ExitIntervalUID]
ALTER TABLE CardDoor ADD [EnterIntervalID] int NOT NULL
ALTER TABLE CardDoor ADD [ExitIntervalID] int NOT NULL
END
END
END
INSERT INTO Patches (Id) VALUES ('DoorsEnterExitUIDtoID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DoorsExitUIDtoID')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardDoor')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'ExitIntervalID' and table_name = 'CardDoor')
BEGIN
ALTER TABLE CardDoor DROP COLUMN [ExitIntervalID]
ALTER TABLE CardDoor ADD [ExitIntervalID] int NOT NULL
END
END
INSERT INTO Patches (Id) VALUES ('DoorsExitUIDtoID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddJournalEmployeeUID')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Journal')
BEGIN
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'EmployeeUID' and table_name = 'Journal')
BEGIN
ALTER TABLE Journal ADD [EmployeeUID] [uniqueidentifier] NULL
END
END
INSERT INTO Patches (Id) VALUES ('AddJournalEmployeeUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveDocument')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Document')
BEGIN
DROP TABLE Document
END
INSERT INTO Patches (Id) VALUES ('RemoveDocument')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveDocumentUIDIndex')
BEGIN
IF EXISTS (SELECT Name FROM sysindexes WHERE Name = 'DocumentUIDIndex')
BEGIN
DROP Index DocumentUIDIndex ON Document
END
INSERT INTO Patches (Id) VALUES ('RemoveDocumentUIDIndex')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveEmployeeReplacement')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = '[EmployeeReplacement')
BEGIN
DROP TABLE EmployeeReplacement
END
INSERT INTO Patches (Id) VALUES ('RemoveEmployeeReplacement')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveEmployeeReplacementUIDIndex')
BEGIN
IF EXISTS (SELECT Name FROM sysindexes WHERE Name = 'EmployeeReplacementUIDIndex')
BEGIN
DROP Index EmployeeReplacementUIDIndex ON EmployeeReplacement
END
INSERT INTO Patches (Id) VALUES ('RemoveEmployeeReplacementUIDIndex')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveEmployeeReplacementUIDIndex')
BEGIN
IF EXISTS (SELECT Name FROM sysindexes WHERE Name = 'EmployeeReplacementUIDIndex')
BEGIN
DROP Index EmployeeReplacementUIDIndex ON EmployeeReplacement
END
INSERT INTO Patches (Id) VALUES ('RemoveEmployeeReplacementUIDIndex')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DoorsDropAntipassEscort')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardDoor')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'IsAntipass' and table_name = 'CardDoor')
BEGIN
ALTER TABLE CardDoor DROP COLUMN IsAntipass
END
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'IsWithEscort' and table_name = 'CardDoor')
BEGIN
ALTER TABLE CardDoor DROP COLUMN IsWithEscort
END
END
INSERT INTO Patches (Id) VALUES ('DoorsDropAntipassEscort')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'CardPassCardTemplateUID')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Card')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'CardTemplateUID' and table_name = 'Card')
BEGIN
EXEC sp_rename '[Card].[CardTemplateUID]', 'PassCardTemplateUID', 'Column'
END
END
INSERT INTO Patches (Id) VALUES ('CardPassCardTemplateUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'JournalDropCardNo')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Journal')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'CardNo' and table_name = 'Journal')
BEGIN
ALTER TABLE Journal DROP COLUMN CardNo
END
END
INSERT INTO Patches (Id) VALUES ('JournalDropCardNo')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveCardDoorParentType')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'CardDoor' and table_name = 'CardDoor')
ALTER TABLE CardDoor DROP COLUMN ParentType
INSERT INTO Patches (Id) VALUES ('RemoveCardDoorParentType')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveCardDoorParentUID')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'ParentUID' and table_name = 'CardDoor')
ALTER TABLE CardDoor DROP COLUMN ParentUID
INSERT INTO Patches (Id) VALUES ('RemoveCardDoorParentUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'FK_CardDoor_Card_DROP')
BEGIN
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CardDoor_Card]') AND parent_object_id = OBJECT_ID(N'[dbo].[CardDoor]'))
ALTER TABLE [dbo].[CardDoor] DROP CONSTRAINT [FK_CardDoor_Card]
INSERT INTO Patches (Id) VALUES ('FK_CardDoor_Card_DROP')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'FK_CardDoor_AccessTemplate_DROP')
BEGIN
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CardDoor_AccessTemplate]') AND parent_object_id = OBJECT_ID(N'[dbo].[CardDoor]'))
ALTER TABLE [dbo].[CardDoor] DROP CONSTRAINT [FK_CardDoor_AccessTemplate]
INSERT INTO Patches (Id) VALUES ('FK_CardDoor_AccessTemplate_DROP')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddCardDoorCardUID')
BEGIN
IF EXISTS (select table_name from INFORMATION_SCHEMA.columns where table_name = 'CardDoor')
ALTER TABLE CardDoor ADD [CardUID] [uniqueidentifier] NULL
INSERT INTO Patches (Id) VALUES ('AddCardDoorCardUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'PassJournalNullable')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Journal')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'EnterTime' and table_name = 'PassJournal')
BEGIN
ALTER TABLE PassJournal ALTER COLUMN EnterTime datetime null
END
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'ExitTime' and table_name = 'PassJournal')
BEGIN
ALTER TABLE PassJournal ALTER COLUMN ExitTime datetime null
END
END
INSERT INTO Patches (Id) VALUES ('PassJournalNullable')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'SKDCardPasswordDeactivation')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Card')
BEGIN
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'DeactivationControllerUID' and table_name = 'Card')
BEGIN
ALTER TABLE [Card] ADD DeactivationControllerUID [uniqueidentifier] NULL
END
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'Password' and table_name = 'Card')
BEGIN
ALTER TABLE [Card] ADD [Password] [nvarchar](50) NULL
END
END
INSERT INTO Patches (Id) VALUES ('SKDCardPasswordDeactivation')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddAccessTemplateUID')
BEGIN
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'AccessTemplateUID' and table_name = 'CardDoor')
ALTER TABLE CardDoor ADD [AccessTemplateUID] [uniqueidentifier] NULL
INSERT INTO Patches (Id) VALUES ('AddAccessTemplateUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'FK_CardDoor_Card_CascadeDelete')
BEGIN
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CardDoor_Card]') AND parent_object_id = OBJECT_ID(N'[dbo].[CardDoor]'))
ALTER TABLE [dbo].[CardDoor] DROP CONSTRAINT [FK_CardDoor_Card]

ALTER TABLE [dbo].[CardDoor]  WITH NOCHECK ADD  CONSTRAINT [FK_CardDoor_Card] FOREIGN KEY([CardUID])
REFERENCES [dbo].[Card] ([Uid])
ON DELETE CASCADE
NOT FOR REPLICATION

INSERT INTO Patches (Id) VALUES ('FK_CardDoor_Card_CascadeDelete')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'FK_CardDoor_AccessTemplate_CascadeDelete')
BEGIN
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CardDoor_AccessTemplate]') AND parent_object_id = OBJECT_ID(N'[dbo].[CardDoor]'))
ALTER TABLE [dbo].[CardDoor] DROP CONSTRAINT [FK_CardDoor_AccessTemplate]

ALTER TABLE [dbo].[CardDoor] WITH NOCHECK ADD  CONSTRAINT [FK_CardDoor_AccessTemplate] FOREIGN KEY([AccessTemplateUID])
REFERENCES [dbo].[AccessTemplate] ([Uid])
ON DELETE CASCADE
NOT FOR REPLICATION

INSERT INTO Patches (Id) VALUES ('FK_CardDoor_AccessTemplate_CascadeDelete')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_FK_Journal_Card')
BEGIN
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Journal_Card]') AND parent_object_id = OBJECT_ID(N'[dbo].[Journal]'))
ALTER TABLE [dbo].[Journal] DROP CONSTRAINT [FK_Journal_Card]
INSERT INTO Patches (Id) VALUES ('Drop_FK_Journal_Card')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Journal_Detalisation')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Journal')
BEGIN
ALTER TABLE Journal ADD [Detalisation] [text] NULL
END
INSERT INTO Patches (Id) VALUES ('Journal_Detalisation')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Journal_NameText_nvarchar(max)')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Journal')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'NameText' and table_name = 'Journal')
BEGIN
ALTER TABLE Journal ALTER COLUMN NameText nvarchar(max) NULL
END
END
INSERT INTO Patches (Id) VALUES ('Journal_NameText_nvarchar(max)')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Card_UserTime')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Card')
BEGIN
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'UserTime' and table_name = 'Card')
ALTER TABLE Card ADD UserTime int default 0 NOT NULL
END
INSERT INTO Patches (Id) VALUES ('Card_UserTime')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'TimeTrackExceptions')
BEGIN
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'TimeTrackException')
BEGIN
CREATE TABLE [dbo].[TimeTrackException](
[UID] [uniqueidentifier] NOT NULL,
[EmployeeUID] [uniqueidentifier] NOT NULL,
[StartDateTime] [datetime] NOT NULL,
[EndDateTime] [datetime] NOT NULL,
[DocumentType] [int] NOT NULL,
[Comment] [nvarchar](100) NULL,
CONSTRAINT [PK_TimeTrackException] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[TimeTrackException]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeTrackException_Employee] FOREIGN KEY([EmployeeUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
END
INSERT INTO Patches (Id) VALUES ('TimeTrackExceptions')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Schedule_AllowedLateAndEarlyLeave')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Schedule')
BEGIN
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'AllowedLate' and table_name = 'Schedule')
ALTER TABLE Schedule ADD AllowedLate int default 0 NOT NULL
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'AllowedEarlyLeave' and table_name = 'Schedule')
ALTER TABLE Schedule ADD AllowedEarlyLeave int default 0 NOT NULL
END
INSERT INTO Patches (Id) VALUES ('Schedule_AllowedLateAndEarlyLeave')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'PassJournal_EnterTime_NotNull')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'PassJournal')
BEGIN
ALTER TABLE PassJournal ALTER COLUMN EnterTime datetime NOT NULL
END
INSERT INTO Patches (Id) VALUES ('PassJournal_EnterTime_NotNull')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DropIndex_PhoneUIDIndex')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Phone')
DROP INDEX PhoneUIDIndex ON Phone
INSERT INTO Patches (Id) VALUES ('DropIndex_PhoneUIDIndex')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DropTable_Phone')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Phone')
BEGIN
DROP TABLE Phone
END
INSERT INTO Patches (Id) VALUES ('DropTable_Phone')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Employee_DropColumn_Dismissed')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'Dismissed' and table_name = 'Employee')
ALTER TABLE Employee DROP COLUMN Dismissed
INSERT INTO Patches (Id) VALUES ('Employee_DropColumn_Dismissed')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'CreateTable_HolidaySettings')
BEGIN
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'HolidaySettings')
BEGIN
CREATE TABLE HolidaySettings(
[UID] [uniqueidentifier] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
NightStartTime [bigint] NOT NULL,
NightEndTime [bigint] NOT NULL,
EveningStartTime [bigint] NOT NULL,
EveningEndTime [bigint] NOT NULL,
CONSTRAINT [PK_HolidaySettings] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].HolidaySettings WITH NOCHECK ADD CONSTRAINT [FK_HolidaySettings_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].HolidaySettings NOCHECK CONSTRAINT [FK_HolidaySettings_Organisation]
END
INSERT INTO Patches (Id) VALUES ('CreateTable_HolidaySettings')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'CreateIndex_HolidaySettingsUIDIndex')
BEGIN
IF NOT EXISTS (SELECT Name FROM sysindexes WHERE Name = 'HolidaySettingsUIDIndex')
BEGIN
CREATE INDEX HolidaySettingsUIDIndex ON HolidaySettings([UID])
END
INSERT INTO Patches (Id) VALUES ('CreateIndex_HolidaySettingsUIDIndex')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RenameDay')
BEGIN
BEGIN
EXEC sp_rename 'Day', 'ScheduleDay'
END
INSERT INTO Patches (Id) VALUES ('RenameDay')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RenameDayNamedIntervalUID')
BEGIN
BEGIN
EXEC sp_rename 'ScheduleDay.NamedIntervalUID', 'DayIntervalUID', 'COLUMN'
END
INSERT INTO Patches (Id) VALUES ('RenameDayNamedIntervalUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RenameInterval')
BEGIN
BEGIN
EXEC sp_rename 'Interval', 'DayIntervalPart'
END
INSERT INTO Patches (Id) VALUES ('RenameInterval')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RenameIntervalNamedIntervalUID')
BEGIN
BEGIN
EXEC sp_rename 'DayIntervalPart.NamedIntervalUID', 'DayIntervalUID', 'COLUMN'
END
INSERT INTO Patches (Id) VALUES ('RenameIntervalNamedIntervalUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RenameNamedInterval')
BEGIN
BEGIN
EXEC sp_rename 'NamedInterval', 'DayInterval'
END
INSERT INTO Patches (Id) VALUES ('RenameNamedInterval')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RenameTimeTrackException')
BEGIN
BEGIN
EXEC sp_rename 'TimeTrackException', 'TimeTrackDocument'
END
INSERT INTO Patches (Id) VALUES ('RenameTimeTrackException')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RenameTimeTrackExceptionDocumentType')
BEGIN
BEGIN
EXEC sp_rename 'TimeTrackDocument.DocumentType', 'DocumentCode', 'COLUMN'
END
INSERT INTO Patches (Id) VALUES ('RenameTimeTrackExceptionDocumentType')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'ScheduleSchemeDaysCount')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'ScheduleScheme')
BEGIN
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'DaysCount' and table_name = 'ScheduleScheme')
ALTER TABLE ScheduleScheme ADD [DaysCount] int NOT NULL CONSTRAINT "ScheduleScheme_DaysCount_Default" DEFAULT 0
END
INSERT INTO Patches (Id) VALUES ('ScheduleSchemeDaysCount')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'TimeTrackDocumentDateTimeAndNumber')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'TimeTrackDocument')
BEGIN
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'DocumentDateTime' and table_name = 'TimeTrackDocument')
ALTER TABLE TimeTrackDocument ADD [DocumentDateTime] [datetime] NOT NULL CONSTRAINT "TimeTrackDocument_DocumentDateTime_Default" DEFAULT CURRENT_TIMESTAMP
IF NOT EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'DocumentNumber' and table_name = 'TimeTrackDocument')
ALTER TABLE TimeTrackDocument ADD [DocumentNumber] [int] NOT NULL CONSTRAINT "TimeTrackDocument_DocumentNumber_Default" DEFAULT 1
END
INSERT INTO Patches (Id) VALUES ('TimeTrackDocumentDateTimeAndNumber')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RenameHolidaySettings')
BEGIN
BEGIN
EXEC sp_rename 'HolidaySettings', 'NightSettings'
END
INSERT INTO Patches (Id) VALUES ('RenameHolidaySettings')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'TimeTrackDocumentType')
BEGIN
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'TimeTrackDocumentType')
BEGIN
CREATE TABLE [dbo].[TimeTrackDocumentType](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](max) NOT NULL,
[ShortName] [nvarchar](10) NOT NULL,
[DocumentCode] [int] NOT NULL,
[DocumentType] [int] NOT NULL,
[OrganisationUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_TimeTrackDocumentType] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE INDEX TimeTrackDocumentTypeUIDIndex ON TimeTrackDocumentType([UID])

ALTER TABLE [dbo].[TimeTrackDocumentType] WITH NOCHECK ADD CONSTRAINT [FK_TimeTrackDocumentType_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[TimeTrackDocumentType] NOCHECK CONSTRAINT [FK_TimeTrackDocumentType_Organisation]
END
INSERT INTO Patches (Id) VALUES ('TimeTrackDocumentType')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'PassCardTemplate')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationCardTemplate')
BEGIN
DROP TABLE OrganisationCardTemplate
END
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'PassCardTemplate')
BEGIN
CREATE TABLE [dbo].[PassCardTemplate](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NULL,
[Description] [nvarchar](max) NULL,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
[Data] [varbinary](max) NULL,
CONSTRAINT [PK_PassCardTemplate] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[PassCardTemplate] WITH NOCHECK ADD CONSTRAINT [FK_PassCardTemplate_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[PassCardTemplate] NOCHECK CONSTRAINT [FK_PassCardTemplate_Organisation]
END
INSERT INTO Patches (Id) VALUES ('PassCardTemplate')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'PassCardTemplate')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationCardTemplate')
BEGIN
DROP TABLE OrganisationCardTemplate
END
IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'PassCardTemplate')
BEGIN
CREATE TABLE [dbo].[PassCardTemplate](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NULL,
[Description] [nvarchar](max) NULL,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
[Data] [varbinary](max) NULL,
CONSTRAINT [PK_PassCardTemplate] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
ALTER TABLE [dbo].[PassCardTemplate] WITH NOCHECK ADD CONSTRAINT [FK_PassCardTemplate_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[PassCardTemplate] NOCHECK CONSTRAINT [FK_PassCardTemplate_Organisation]
END
INSERT INTO Patches (Id) VALUES ('PassCardTemplate')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DepartmentChiefUID')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Department')
BEGIN
ALTER TABLE Department ADD [ChiefUID] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000'
ALTER TABLE [dbo].Department WITH NOCHECK ADD CONSTRAINT [FK_Department_Employee] FOREIGN KEY([ChiefUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
END
INSERT INTO Patches (Id) VALUES ('DepartmentChiefUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'OrganisationChiefUID')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Organisation')
BEGIN
ALTER TABLE Organisation ADD [ChiefUID] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000'
ALTER TABLE [dbo].Organisation WITH NOCHECK ADD CONSTRAINT [FK_Organisation_Employee] FOREIGN KEY([ChiefUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
END
INSERT INTO Patches (Id) VALUES ('OrganisationChiefUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DepartmentChiefUID_NOCHECK')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Department')
BEGIN
ALTER TABLE [dbo].Department NOCHECK CONSTRAINT [FK_Department_Employee]
END
INSERT INTO Patches (Id) VALUES ('DepartmentChiefUID_NOCHECK')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'OrganisationChiefUID_NOCHECK')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Organisation')
BEGIN
ALTER TABLE [dbo].Organisation NOCHECK CONSTRAINT [FK_Organisation_Employee]
END
INSERT INTO Patches (Id) VALUES ('OrganisationChiefUID_NOCHECK')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'HRChiefUID')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Department')
BEGIN
ALTER TABLE Department ADD [HRChiefUID] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000'
ALTER TABLE [dbo].Department WITH NOCHECK ADD CONSTRAINT [FK_Department_Employee3] FOREIGN KEY([HRChiefUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].Department NOCHECK CONSTRAINT [FK_Department_Employee3]
END
INSERT INTO Patches (Id) VALUES ('HRChiefUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DepartmentPhone')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Department')
BEGIN
ALTER TABLE Department ADD [Phone] [nvarchar](50) NULL
END
INSERT INTO Patches (Id) VALUES ('DepartmentPhone')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'OrganisationPhone')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Organisation')
BEGIN
ALTER TABLE Organisation ADD [Phone] [nvarchar](50) NULL
END
INSERT INTO Patches (Id) VALUES ('OrganisationPhone')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'EmployeePhone')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Employee')
BEGIN
ALTER TABLE Employee ADD [Phone] [nvarchar](50) NULL
END
INSERT INTO Patches (Id) VALUES ('EmployeePhone')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'ScheduleZone_Drop_IsDeleted')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'ScheduleZone')
BEGIN
ALTER TABLE ScheduleZone DROP COLUMN IsDeleted
ALTER TABLE ScheduleZone DROP COLUMN RemovalDate
END
INSERT INTO Patches (Id) VALUES ('ScheduleZone_Drop_IsDeleted')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'CardDoor_Drop_IsDeleted')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'CardDoor')
BEGIN
ALTER TABLE CardDoor DROP COLUMN IsDeleted
ALTER TABLE CardDoor DROP COLUMN RemovalDate
END
INSERT INTO Patches (Id) VALUES ('CardDoor_Drop_IsDeleted')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DayIntervalPart_Drop_IsDeleted')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'DayIntervalPart')
BEGIN
ALTER TABLE DayIntervalPart DROP COLUMN IsDeleted
ALTER TABLE DayIntervalPart DROP COLUMN RemovalDate
END
INSERT INTO Patches (Id) VALUES ('DayIntervalPart_Drop_IsDeleted')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'ScheduleDay_Drop_IsDeleted')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'ScheduleDay')
BEGIN
ALTER TABLE ScheduleDay DROP COLUMN IsDeleted
ALTER TABLE ScheduleDay DROP COLUMN RemovalDate
END
INSERT INTO Patches (Id) VALUES ('ScheduleDay_Drop_IsDeleted')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Add_Indexes')
BEGIN
CREATE INDEX AccessTemplateUIDIndex ON AccessTemplate([UID])
CREATE INDEX AdditionalColumnTypeOrgUIDIndex ON AdditionalColumnType([OrganisationUID])
CREATE INDEX DepartmentOrgUIDIndex ON Department([OrganisationUID])
CREATE INDEX EmployeeOrgUIDIndex ON Employee([OrganisationUID])
CREATE INDEX HolidayOrgUIDIndex ON Holiday([OrganisationUID])
CREATE INDEX DayIntervalOrgUIDIndex ON DayInterval([OrganisationUID])
CREATE INDEX PositionOrgUIDIndex ON Position([OrganisationUID])
CREATE INDEX ScheduleOrgUIDIndex ON Schedule([OrganisationUID])
CREATE INDEX ScheduleSchemeOrgUIDIndex ON ScheduleScheme([OrganisationUID])
CREATE INDEX CardOrgUIDIndex ON Card([EmployeeUID])
CREATE INDEX AccessTemplateOrgUIDIndex ON AccessTemplate([OrganisationUID])
CREATE INDEX PassCardTemplateOrgUIDIndex ON PassCardTemplate([OrganisationUID])
CREATE INDEX EmployeeDeptUIDIndex ON Employee([DepartmentUID])
CREATE INDEX EmployeePosUIDIndex ON Employee([PositionUID])
INSERT INTO Patches (Id) VALUES ('Add_Indexes')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Organisation_HRChief')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Department')
BEGIN
DECLARE @Command nvarchar(1000)
SELECT @Command = 'ALTER TABLE DEPARTMENT DROP ' + d.name
FROM sys.tables t
JOIN sys.default_constraints d
ON d.parent_object_id = t.object_id
JOIN sys.columns c
ON c.object_id = t.object_id
AND c.column_id = d.parent_column_id
WHERE t.name = N'Department'
AND c.name = N'HRChiefUID'
EXECUTE (@Command)
ALTER TABLE DEPARTMENT DROP CONSTRAINT FK_Department_Employee3
ALTER TABLE Department DROP COLUMN HRChiefUID
END
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Organisation')
BEGIN
ALTER TABLE Organisation ADD [HRChiefUID] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000'
ALTER TABLE [dbo].Organisation WITH NOCHECK ADD CONSTRAINT [FK_Organisation_Employee3] FOREIGN KEY([HRChiefUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].Organisation NOCHECK CONSTRAINT [FK_Organisation_Employee3]
END
INSERT INTO Patches (Id) VALUES ('Organisation_HRChief')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Journal_Indexes')
BEGIN
ALTER TABLE Journal ALTER COLUMN DeviceDate datetime NULL
CREATE INDEX JournalDeviceDateIndex ON Journal([DeviceDate])
CREATE INDEX JournalSystemDateIndex ON Journal([SystemDate])
CREATE INDEX JournalNameIndex ON Journal([Name])
CREATE INDEX JournalDescriptionIndex ON Journal([Description])
CREATE INDEX JournalObjectUIDIndex ON Journal([ObjectUID])
INSERT INTO Patches (Id) VALUES ('Journal_Indexes')
END

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RenameCardDoorIntervals')
BEGIN
EXEC sp_rename 'CardDoor.EnterIntervalID', 'EnterScheduleNo', 'COLUMN'
EXEC sp_rename 'CardDoor.ExitIntervalID', 'ExitScheduleNo', 'COLUMN'
INSERT INTO Patches (Id) VALUES ('RenameCardDoorIntervals')
END

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'CardNoString')
BEGIN
ALTER TABLE Card ALTER COLUMN Number nvarchar(50) NOT NULL
INSERT INTO Patches (Id) VALUES ('CardNoString')
END

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveEventNamesAndDescriptions')
BEGIN
DROP TABLE EventNames
DROP TABLE EventDescriptions
INSERT INTO Patches (Id) VALUES ('RemoveEventNamesAndDescriptions')
END

GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_Employee_Appointed')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Employee')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'Appointed' and table_name = 'Employee')
BEGIN
ALTER TABLE Employee DROP COLUMN Appointed
END
END
INSERT INTO Patches (Id) VALUES ('Drop_Employee_Appointed')
END

GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_Journal_And_PassJournal')
BEGIN
DROP TABLE Journal
DROP TABLE PassJournal
INSERT INTO Patches (Id) VALUES ('Drop_Journal_And_PassJournal')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'EmployeeDescription')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Employee')
BEGIN
ALTER TABLE Employee ADD [Description] [nvarchar](max) NULL
END
INSERT INTO Patches (Id) VALUES ('EmployeeDescription')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Create_Journal_And_PassJournal_Metadata')
BEGIN
CREATE TABLE [dbo].[JournalMetadata](
[UID] [uniqueidentifier] NOT NULL,
[No] [int] NOT NULL,
[StartDateTime] [datetime] NOT NULL,
[EndDateTime] [datetime] NOT NULL,
CONSTRAINT [PK_JournalMetadata] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[PassJournalMetadata](
[UID] [uniqueidentifier] NOT NULL,
[No] [int] NOT NULL,
[StartDateTime] [datetime] NOT NULL,
[EndDateTime] [datetime] NOT NULL,
CONSTRAINT [PK_PassJournalMetadata] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
INSERT INTO Patches (Id) VALUES ('Create_Journal_And_PassJournal_Metadata')
END

GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_CardSubsystemType')
BEGIN
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'CardSubsystemType' and table_name = 'Card')
BEGIN
ALTER TABLE Card DROP CONSTRAINT Card_CardSubsystemType_Default
ALTER TABLE Card DROP COLUMN CardSubsystemType
END
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'CardSubsystemType' and table_name = 'CardDoor')
BEGIN
ALTER TABLE CardDoor DROP CONSTRAINT CardDoor_CardSubsystemType_Default
ALTER TABLE CardDoor DROP COLUMN CardSubsystemType
END
IF EXISTS (select column_name from INFORMATION_SCHEMA.columns where column_name = 'CardSubsystemType' and table_name = 'OrganisationDoor')
BEGIN
ALTER TABLE OrganisationDoor DROP CONSTRAINT OrganisationDoor_CardSubsystemType_Default
ALTER TABLE OrganisationDoor DROP COLUMN CardSubsystemType
END
INSERT INTO Patches (Id) VALUES ('Drop_CardSubsystemType')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Department_Chief_Default_Name')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Department')
BEGIN
DECLARE @Command nvarchar(1000)
SELECT @Command = 'ALTER TABLE DEPARTMENT DROP ' + d.name
FROM sys.tables t
JOIN sys.default_constraints d
ON d.parent_object_id = t.object_id
JOIN sys.columns c
ON c.object_id = t.object_id
AND c.column_id = d.parent_column_id
WHERE t.name = N'Department'
AND c.name = N'ChiefUID'
EXECUTE (@Command)
ALTER TABLE [Department] ADD CONSTRAINT Department_ChiefUID_Default DEFAULT '00000000-0000-0000-0000-000000000000' FOR ChiefUID
END
INSERT INTO Patches (Id) VALUES ('Department_Chief_Default_Name')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Organisation_Chief_Default_Name')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Department')
BEGIN
DECLARE @Command nvarchar(1000)
SELECT @Command = 'ALTER TABLE Organisation DROP ' + d.name
FROM sys.tables t
JOIN sys.default_constraints d
ON d.parent_object_id = t.object_id
JOIN sys.columns c
ON c.object_id = t.object_id
AND c.column_id = d.parent_column_id
WHERE t.name = N'Organisation'
AND c.name = N'ChiefUID'
EXECUTE (@Command)
ALTER TABLE [Organisation] ADD CONSTRAINT Organisation_ChiefUID_Default DEFAULT '00000000-0000-0000-0000-000000000000' FOR ChiefUID
SELECT @Command = 'ALTER TABLE Organisation DROP ' + d.name
FROM sys.tables t
JOIN sys.default_constraints d
ON d.parent_object_id = t.object_id
JOIN sys.columns c
ON c.object_id = t.object_id
AND c.column_id = d.parent_column_id
WHERE t.name = N'Organisation'
AND c.name = N'HRChiefUID'
EXECUTE (@Command)
ALTER TABLE [Organisation] ADD CONSTRAINT Organisation_HRChiefUID_Default DEFAULT '00000000-0000-0000-0000-000000000000' FOR HRChiefUID
END
INSERT INTO Patches (Id) VALUES ('Organisation_Chief_Default_Name')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_table_OrganisationZone')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationZone')
BEGIN
DROP TABLE OrganisationZone
END
INSERT INTO Patches (Id) VALUES ('Drop_table_OrganisationZone')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'ScheduleZone_DoorUID')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'ScheduleZone')
BEGIN
ALTER TABLE [ScheduleZone] ADD [DoorUID] [uniqueidentifier] NOT NULL CONSTRAINT "ScheduleZone_DoorUID_Default" DEFAULT '00000000-0000-0000-0000-000000000000'
ALTER TABLE [ScheduleZone] ADD CONSTRAINT "ScheduleZone_ZoneUID_Default" DEFAULT '00000000-0000-0000-0000-000000000000' FOR ZoneUID
END
INSERT INTO Patches (Id) VALUES ('ScheduleZone_DoorUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Card_drop_IsDeleted')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Card')
BEGIN
ALTER TABLE [Card] DROP COLUMN IsDeleted
ALTER TABLE [Card] DROP COLUMN RemovalDate
END
INSERT INTO Patches (Id) VALUES ('Card_drop_IsDeleted')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Employee_LastEmployeeDayUpdate')
BEGIN
ALTER TABLE Employee ADD LastEmployeeDayUpdate datetime NOT NULL CONSTRAINT "Schedule_LastEmployeeDayUpdate_Default" DEFAULT '1900-01-01 00:00:00.000'
INSERT INTO Patches (Id) VALUES ('Employee_LastEmployeeDayUpdate')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Card_Number_Int')
BEGIN
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Card')
BEGIN
DELETE FROM Card
ALTER TABLE Card
ALTER COLUMN Number int NOT NULL
END

INSERT INTO Patches (Id) VALUES ('Card_Number_Int')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'ExternalKeys')
BEGIN
ALTER TABLE Employee ADD ExternalKey nvarchar(40) NOT NULL DEFAULT '-1'
ALTER TABLE Position ADD ExternalKey nvarchar(40) NOT NULL DEFAULT '-1'
ALTER TABLE Department ADD ExternalKey nvarchar(40) NOT NULL DEFAULT '-1'
ALTER TABLE Card ADD ExternalKey nvarchar(40) NOT NULL DEFAULT '-1'
ALTER TABLE Organisation ADD ExternalKey nvarchar(40) NOT NULL DEFAULT '-1'

INSERT INTO Patches (Id) VALUES ('ExternalKeys')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DropEveningSettings')
BEGIN
ALTER TABLE NightSettings DROP COLUMN EveningStartTime
ALTER TABLE NightSettings DROP COLUMN EveningEndTime
INSERT INTO Patches (Id) VALUES ('DropEveningSettings')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Employee_TabelNo_String')
BEGIN
ALTER TABLE Employee ALTER COLUMN TabelNo nvarchar(50) NULL
INSERT INTO Patches (Id) VALUES ('Employee_TabelNo_String')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'TimeTrackDocument_FileName')
BEGIN
ALTER TABLE TimeTrackDocument ADD [FileName] [nvarchar](100) NULL
INSERT INTO Patches (Id) VALUES ('TimeTrackDocument_FileName')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Employee_TabelNo_String_40')
BEGIN
ALTER TABLE Employee ALTER COLUMN TabelNo nvarchar(40) NULL
INSERT INTO Patches (Id) VALUES ('Employee_TabelNo_String_40')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'NullableGender')
BEGIN
ALTER TABLE Employee ALTER COLUMN Gender int NULL
INSERT INTO Patches (Id) VALUES ('NullableGender')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'NullableDocumentType')
BEGIN
ALTER TABLE Employee ALTER COLUMN DocumentType int NULL
INSERT INTO Patches (Id) VALUES ('NullableDocumentType')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'NullableBirthDate')
BEGIN
ALTER TABLE Employee ALTER COLUMN BirthDate datetime NULL
INSERT INTO Patches (Id) VALUES ('NullableBirthDate')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'NullableDocumentGivenDate')
BEGIN
ALTER TABLE Employee ALTER COLUMN DocumentGivenDate datetime NULL
INSERT INTO Patches (Id) VALUES ('NullableDocumentGivenDate')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'NullableDocumentValidTo')
BEGIN
ALTER TABLE Employee ALTER COLUMN DocumentValidTo datetime NULL
INSERT INTO Patches (Id) VALUES ('NullableDocumentValidTo')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveExitScheduleNoFromCardDoor')
BEGIN
ALTER TABLE [CardDoor] DROP COLUMN [ExitScheduleNo]
INSERT INTO Patches (Id) VALUES ('RemoveExitScheduleNoFromCardDoor')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'DropColumnDefaultConstraint')
BEGIN
	exec [dbo].[sp_executesql] @statement = N'
	CREATE PROCEDURE [dbo].[DropColumnDefaultConstraint]
(
	@tableName VARCHAR(MAX),
	@columnName VARCHAR(MAX)
)
AS
BEGIN
DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name
FROM SYS.DEFAULT_CONSTRAINTS
WHERE PARENT_OBJECT_ID = OBJECT_ID(@tableName)
AND PARENT_COLUMN_ID = (
    SELECT column_id FROM sys.columns
    WHERE NAME = @columnName AND object_id = OBJECT_ID(@tableName))
IF @ConstraintName IS NOT NULL
    EXEC(''ALTER TABLE ''+@tableName+'' DROP CONSTRAINT '' + @ConstraintName)
END'
	INSERT INTO Patches (Id) VALUES ('DropColumnDefaultConstraint')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddIsHandicappedCardColumn')
BEGIN
ALTER TABLE [Card] ADD [IsHandicappedCard] bit NOT NULL DEFAULT 0
INSERT INTO Patches (Id) VALUES ('AddIsHandicappedCardColumn')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddAllowedAbsentLowThan')
BEGIN
ALTER TABLE [Schedule] ADD [AllowedAbsentLowThan] int NOT NULL DEFAULT 0
INSERT INTO Patches (Id) VALUES ('AddAllowedAbsentLowThan')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddNotAllowOvertimeLowerThan')
BEGIN
ALTER TABLE [Schedule] ADD [NotAllowOvertimeLowerThan] int NOT NULL DEFAULT 0
INSERT INTO Patches (Id) VALUES ('AddNotAllowOvertimeLowerThan')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddIsEnabledAllowLate')
BEGIN
	ALTER TABLE [Schedule] ADD [IsEnabledAllowLate] bit NOT NULL DEFAULT 0
	INSERT INTO Patches (Id) VALUES ('AddIsEnabledAllowLate')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddIsEnabledAllowEarlyLeave')
BEGIN
	ALTER TABLE [Schedule] ADD [IsEnabledAllowEarlyLeave] bit NOT NULL DEFAULT 0
	INSERT INTO Patches (Id) VALUES ('AddIsEnabledAllowEarlyLeave')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddIsAllowAbsent')
BEGIN
	ALTER TABLE [Schedule] ADD [IsAllowAbsent] bit NOT NULL DEFAULT 0
	INSERT INTO Patches (Id) VALUES ('AddIsAllowAbsent')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddIsEnabledOvertime')
BEGIN
	ALTER TABLE [Schedule] ADD [IsEnabledOvertime] bit NOT NULL DEFAULT 0
	INSERT INTO Patches (Id) VALUES ('AddIsEnabledOvertime')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddIsOutsideColumn')
BEGIN
	ALTER TABLE [TimeTrackDocument] ADD [IsOutside] bit NOT NULL DEFAULT 0
	INSERT INTO Patches (Id) VALUES ('AddIsOutsideColumn')
END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='CreateGKMetadata')
	BEGIN
		IF EXISTS(SELECT *
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = 'dbo'
                 AND  TABLE_NAME = 'GKMetadata')
			DROP TABLE GKMetadata
		DELETE FROM Patches WHERE Id='CreateGKMetadata'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='Recreate_GKMetadata')
	BEGIN
		IF EXISTS(SELECT *
					 FROM INFORMATION_SCHEMA.TABLES
					 WHERE TABLE_SCHEMA = 'dbo'
					 AND  TABLE_NAME = 'GKMetadata')
			DROP TABLE GKMetadata
		DELETE FROM Patches WHERE Id='Recreate_GKMetadata'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='Recreate_GKMetadata2')
	BEGIN
		IF EXISTS(SELECT *
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = 'dbo'
                 AND  TABLE_NAME = 'GKMetadata')
			DROP TABLE GKMetadata
		DELETE FROM Patches WHERE Id='Recreate_GKMetadata2'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='GKCards')
	BEGIN
		IF EXISTS(SELECT *
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = 'dbo'
                 AND  TABLE_NAME = 'GKCards')
			DROP TABLE GKCards
		DELETE FROM Patches WHERE Id='GKCards'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='GKCards_Table')
	BEGIN
		IF EXISTS(SELECT *
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = 'dbo'
                 AND  TABLE_NAME = 'GKCards')
			DROP TABLE GKCards
		DELETE FROM Patches WHERE Id='GKCards_Table'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='GKCardType')
	BEGIN
		IF EXISTS(SELECT *
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = 'dbo'
                 AND  TABLE_NAME = 'GKCards')
			DROP TABLE GKCards
		DELETE FROM Patches WHERE Id='GKCardType'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='RemoveGKCardTypeColumn')
	BEGIN
		IF EXISTS(SELECT *
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE TABLE_SCHEMA = 'dbo'
                 AND  TABLE_NAME = 'GKCards')
			DROP TABLE GKCards
		DELETE FROM Patches WHERE Id='RemoveGKCardTypeColumn'
	END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='GKScheduleDay')
	BEGIN
		DROP TABLE GKScheduleDay
	END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='GKDaySchedulePart')
	BEGIN
		DROP TABLE GKDaySchedulePart
	END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='GKSchedule')
	BEGIN
		IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE OBJECT_NAME(referenced_object_id) = 'GKSchedule')
			BEGIN
				DECLARE @stmt VARCHAR(300);

				-- Cursor to generate ALTER TABLE DROP CONSTRAINT statements
				DECLARE cur CURSOR FOR
				SELECT 'ALTER TABLE ' + OBJECT_SCHEMA_NAME(parent_object_id) + '.' + OBJECT_NAME(parent_object_id) +
				' DROP CONSTRAINT ' + name
				FROM sys.foreign_keys
				WHERE	OBJECT_NAME(referenced_object_id) = 'GKSchedule';

				OPEN cur;
				FETCH cur INTO @stmt;

				-- Drop each found foreign key constraint
				WHILE @@FETCH_STATUS = 0
					BEGIN
					EXEC (@stmt);
					FETCH cur INTO @stmt;
					END

					CLOSE cur;
					DEALLOCATE cur;
				END
			END
	GO
	IF object_id('SKD..#GKSchedule') is not null
		BEGIN
			DROP TABLE GKSchedule
		END
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='ScheduleGKDaySchedule')
	BEGIN
		DROP TABLE ScheduleGKDaySchedule
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='GKSchedule')
	BEGIN
		DELETE FROM Patches WHERE Id='GKSchedule'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='RemoveGKLevelScheduleColumn')
	BEGIN
		DELETE FROM Patches WHERE Id='RemoveGKLevelScheduleColumn'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='RemoveGKLevelColumn')
	BEGIN
		DELETE FROM Patches WHERE Id='RemoveGKLevelColumn'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='RemoveOrganisationGKDoor')
	BEGIN
		DELETE FROM Patches WHERE Id='RemoveOrganisationGKDoor'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='GKSchedule2')
BEGIN
	DELETE FROM Patches WHERE Id='GKSchedule2'
END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='AddTimeTrackSystemDocumentTypes')
	BEGIN
		DELETE FROM Patches WHERE Id='AddTimeTrackSystemDocumentTypes'
	END
GO
IF EXISTS(SELECT * FROM Patches WHERE Id='AddTimeTrackSystemDocumentTypesData')
	BEGIN
		DELETE FROM Patches WHERE Id='AddTimeTrackSystemDocumentTypesData'
	END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='TimeTrackSystemDocumentTypes')
	BEGIN
		DROP TABLE TimeTrackSystemDocumentTypes
	END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddIsSystemColumnInTimeTrackDocumentType')
	BEGIN
		ALTER TABLE [TimeTrackDocumentType] ADD [IsSystem] bit NOT NULL DEFAULT 0
		INSERT INTO Patches (Id) VALUES ('AddIsSystemColumnInTimeTrackDocumentType')
	END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddIsNightSettingsEnabledColumn')
	BEGIN
		ALTER TABLE [NightSettings] ADD [IsNightSettingsEnabled] bit NOT NULL DEFAULT 0
		INSERT INTO Patches (Id) VALUES ('AddIsNightSettingsEnabledColumn')
	END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Attachment')
BEGIN
	IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Attachment')
	BEGIN
		CREATE TABLE [dbo].[Attachment](
			[UID] [uniqueidentifier] NOT NULL,
			[Name] [varchar](255) NOT NULL,
		CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED
		(
			[UID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
		INSERT INTO Patches (Id) VALUES ('Attachment')
	END
END
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='GKDaySchedule')
BEGIN
	DROP TABLE [GKDaySchedule]
END
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='GKSchedule')
BEGIN
	DROP TABLE [GKSchedule]
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_NightSettings_IsNightSettingsEnabled_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [NightSettings], [IsNightSettingsEnabled]
	INSERT INTO Patches (Id) VALUES ('Drop_NightSettings_IsNightSettingsEnabled_Default')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_Schedule_AllowedAbsentLowThan_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [Schedule], [AllowedAbsentLowThan]
	INSERT INTO Patches (Id) VALUES ('Drop_NightSettings_AllowedAbsentLowThan_Default')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_Schedule_NotAllowOvertimeLowerThan_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [Schedule], [NotAllowOvertimeLowerThan]
	INSERT INTO Patches (Id) VALUES ('Drop_Schedule_NotAllowOvertimeLowerThan_Default')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_Schedule_IsEnabledAllowLate_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [Schedule], [IsEnabledAllowLate]
	INSERT INTO Patches (Id) VALUES ('Drop_Schedule_IsEnabledAllowLate_Default')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_Schedule_IsEnabledAllowEarlyLeave_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [Schedule], [IsEnabledAllowEarlyLeave]
	INSERT INTO Patches (Id) VALUES ('Drop_Schedule_IsEnabledAllowEarlyLeave_Default')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_Schedule_IsAllowAbsent_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [Schedule], [IsAllowAbsent]
	INSERT INTO Patches (Id) VALUES ('Drop_Schedule_IsAllowAbsent_Default')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_Schedule_IsEnabledOvertime_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [Schedule], [IsEnabledOvertime]
	INSERT INTO Patches (Id) VALUES ('Drop_Schedule_IsEnabledOvertime_Default')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_TimeTrackDocument_IsOutside_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [TimeTrackDocument], [IsOutside]
	INSERT INTO Patches (Id) VALUES ('Drop_TimeTrackDocument_IsOutside_Default')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_TimeTrackDocumentType_IsSystem_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [TimeTrackDocumentType], [IsSystem]
	INSERT INTO Patches (Id) VALUES ('Drop_TimeTrackDocumentType_IsSystem_Default')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'SP_CreateOrganisationTimeTrackDocumentTypes_Added')
BEGIN
	exec [dbo].[sp_executesql] @statement = N'CREATE PROCEDURE [dbo].[CreateOrganisationTimeTrackDocumentTypes]
(
	@organisationUID uniqueidentifier
)
AS
BEGIN
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Продолжительность работы в дневное время'', N''Я'', 1, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Продолжительность работы в ночное время'', N''Н'', 2, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Продолжительность работы в выходные и нерабочие праздничные дни'', N''РВ'', 3, 0, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Продолжительность сверхурочной работы'', N''С'', 4, 0, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Продолжительность работы вахтовым методом'', N''ВМ'', 5, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Служебная командировка'', N''К'', 6, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Повышение квалификации с отрывом от работы'', N''ПК'', 7, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Повышение квалификации с отрывом от работы в другой местности'', N''ПМ'', 8, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Ежегодный основной оплачиваемый отпуск'', N''ОТ'', 9, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Ежегодный дополнительный оплачиваемый отпуск'', N''ОД'', 10, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Дополнительный отпуск в связи с обучением с сохранением среднего заработка работникам, совмещающим работу с обучением'', N''У'', 11, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Сокращенная продолжительность рабочего времени для обучающихся без отрыва от производства с частичным сохранением заработной платы'', N''УВ'', 12, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Дополнительный отпуск в связи с обучением без сохранения заработной платы'', N''УД'', 13, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Отпуск по беременности и родам (отпуск в связи с усыновлением новорожденного ребенка)'', N''Р'', 14, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Отпуск по уходу за ребенком до достижения им возраста трех лет'', N''ОЖ'', 15, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Отпуск без сохранения заработной платы, предоставляемый работнику по разрешению работодателя'', N''ДО'', 16, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Отпуск без сохранения заработной платы при условиях, предусмотренных действующим законодательством Российской Федерации'', N''ОЗ'', 17, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Ежегодный дополнительный отпуск без сохранения заработной платы'', N''ДБ'', 18, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Временная нетрудоспособность (кроме случаев, предусмотренных кодом "Т") с назначением пособия согласно законодательству'', N''Б'', 19, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Временная нетрудоспособность без назначения пособия в случаях, предусмотренных законодательством'', N''Т'', 20, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Сокращенная продолжительность рабочего времени против нормальной продолжительности рабочего дня в случаях, предусмотренных законодательством'', N''ЛЧ'', 21, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Время вынужденного прогула в случае признания увольнения, перевода на другую работу или отстранения от работы незаконным и с восстановлением на прежней работе'', N''ПВ'', 22, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Невыходы на время исполнения государственных или общественных обязанностей согласно законодательству'', N''Г'', 23, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Прогулы (отсутствие на рабочем месте без уважительных причин в течение времени, установленного законодательством)'', N''ПР'', 24, 2, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Продолжительность работы в режиме неполного рабочего времени по инициативе работодателя в случаях, предусмотренных законодательством'', N''НС'', 25, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Выходные дни (еженедельный отпуск) и нерабочие праздничные дни'', N''В'', 26, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Дополнительные выходные дни (оплачиваемые)'', N''ОВ'', 27, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Дополнительные выходные дни (без сохранения заработной платы)'', N''НВ'', 28, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Забастовка (при условиях и в порядке, предусмотренных законом)'', N''ЗБ'', 29, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Неявки по невыясненным причинам (до выяснения обстоятельств)'', N''НН'', 30, 2, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Время простоя по вине работодателя'', N''РП'', 31, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Время простоя по причинам, не зависящим от работодателя и работника'', N''НП'', 32, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Время простоя по вине работника'', N''ВП'', 33, 2, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Отстранение от работы (недопущение к работе) с оплатой (пособием) в соответствии с законодательством'', N''НО'', 34, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Отстранение от работы (недопущение к работе) по причинам, предусмотренным законодательством, без начисления заработной платы'', N''НБ'', 35, 2, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), N''Время приостановки работы в случае задержки выплаты заработной платы'', N''НЗ'', 36, 3, @organisationUID, 1)
END'
	INSERT INTO Patches (Id) VALUES ('SP_CreateOrganisationTimeTrackDocumentTypes_Added')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'SP_CreateOrganisationsTimeTrackDocumentTypes_Added')
BEGIN
	exec [dbo].[sp_executesql] @statement = N'CREATE PROCEDURE [dbo].[CreateOrganisationsTimeTrackDocumentTypes]
AS
BEGIN
	DECLARE @OrganisationUID uniqueidentifier
	DECLARE @OrganisationCursor CURSOR
	SET @OrganisationCursor = CURSOR SCROLL FOR SELECT [UID] FROM Organisation

	OPEN @OrganisationCursor
	FETCH NEXT FROM @OrganisationCursor INTO @OrganisationUID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF NOT EXISTS (SELECT * FROM TimeTrackDocumentType WHERE [OrganisationUID] = @OrganisationUID AND [IsSystem] = 1)
		BEGIN
			EXECUTE [dbo].[CreateOrganisationTimeTrackDocumentTypes] @OrganisationUID
		END
		FETCH NEXT FROM @OrganisationCursor INTO @OrganisationUID
	END
	CLOSE @OrganisationCursor
END'
	INSERT INTO Patches (Id) VALUES ('SP_CreateOrganisationsTimeTrackDocumentTypes_Added')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'OrganisationsTimeTrackDocumentTypes_Created')
BEGIN
	EXEC [dbo].[CreateOrganisationsTimeTrackDocumentTypes]
	INSERT INTO Patches (Id) VALUES ('OrganisationsTimeTrackDocumentTypes_Created')
END
GO

--****************************************************************************************Release 1.0.2*********************************************************************
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'FiltersTableAdded')
BEGIN
	IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Filters')
	CREATE TABLE [dbo].[Filters]
	(
			[UID] UNIQUEIDENTIFIER NOT NULL,
			[UserID] UNIQUEIDENTIFIER NOT NULL,
			[Name] NVARCHAR(50) NOT NULL,
			[Type] int NOT NULL,
			[XMLContent] NVARCHAR(MAX) NULL,
		CONSTRAINT [PK_Filters] PRIMARY KEY CLUSTERED
		(
			[UID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	INSERT INTO Patches (Id) VALUES ('FiltersTableAdded')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveCredentialsStartDateField')
BEGIN
	ALTER TABLE [Employee] DROP COLUMN [CredentialsStartDate]
	INSERT INTO Patches (Id) VALUES ('RemoveCredentialsStartDateField')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Table_AccessTemplateDeactivatingReader_Added')
BEGIN
	CREATE TABLE [dbo].[AccessTemplateDeactivatingReader](
		[UID] [uniqueidentifier] NOT NULL,
		[AccessTemplateUID] [uniqueidentifier] NOT NULL,
		[DeactivatingReaderUID] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_AccessTemplateDeactivatingReader] PRIMARY KEY CLUSTERED 
	(
		[UID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[AccessTemplateDeactivatingReader]  WITH NOCHECK ADD  CONSTRAINT [FK_AccessTemplateDeactivatingReader_AccessTemplate] FOREIGN KEY([AccessTemplateUID])
	REFERENCES [dbo].[AccessTemplate] ([UID])
	NOT FOR REPLICATION 
	
	ALTER TABLE [dbo].[AccessTemplateDeactivatingReader] NOCHECK CONSTRAINT [FK_AccessTemplateDeactivatingReader_AccessTemplate]

	INSERT INTO Patches (Id) VALUES ('Table_AccessTemplateDeactivatingReader_Added')
END
GO
