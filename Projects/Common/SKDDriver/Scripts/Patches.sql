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
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'GuardZone')
BEGIN
	IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'OrganisationGuardZone')
	BEGIN
		DROP TABLE OrganisationGuardZone
	END
	IF NOT EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'GuardZone')
	BEGIN
		CREATE TABLE [dbo].[GuardZone](
			[UID] [uniqueidentifier] NOT NULL,
			[ZoneUID] [uniqueidentifier] NOT NULL,
			[ParentUID] [uniqueidentifier] NOT NULL,
			[CanSet] [bit] NOT NULL,
			[CanReset] [bit] NOT NULL,
		CONSTRAINT [PK_GuardZone] PRIMARY KEY CLUSTERED 
		(
			[UID] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]
		ALTER TABLE [dbo].[GuardZone] WITH NOCHECK ADD CONSTRAINT [FK_GuardZone_Organisation] FOREIGN KEY([ParentUid])
		REFERENCES [dbo].[Organisation] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[GuardZone] NOCHECK CONSTRAINT [FK_GuardZone_Organisation]
		ALTER TABLE [dbo].[GuardZone] WITH NOCHECK ADD CONSTRAINT [FK_GuardZone_Employee] FOREIGN KEY([ParentUid])
		REFERENCES [dbo].[Employee] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[GuardZone] NOCHECK CONSTRAINT [FK_GuardZone_Employee]
		ALTER TABLE [dbo].[GuardZone] WITH NOCHECK ADD CONSTRAINT [FK_GuardZone_AccessTemplate] FOREIGN KEY([ParentUid])
		REFERENCES [dbo].[AccessTemplate] ([Uid])
		NOT FOR REPLICATION 
		ALTER TABLE [dbo].[GuardZone] NOCHECK CONSTRAINT [FK_GuardZone_AccessTemplate]
	END
	INSERT INTO Patches (Id) VALUES ('GuardZone')	
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

		CREATE TABLE [dbo].[OrganisationDoor](
			[UID] [uniqueidentifier] NOT NULL,
			[DoorUID] [uniqueidentifier] NOT NULL,
			[OrganisationUID] [uniqueidentifier] NOT NULL,
		CONSTRAINT [PK_OrganisationZone] PRIMARY KEY CLUSTERED 
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
		[CardSeries] int NOT NULL, 
		[CardNo] int NOT NULL, 
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