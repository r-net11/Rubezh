USE [PassJournal_1]
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'EmployeeDay')
BEGIN
	CREATE TABLE [dbo].[EmployeeDay](
		[UID] [uniqueidentifier] NOT NULL,
		[EmployeeUID] [uniqueidentifier] NOT NULL,
		[IsIgnoreHoliday] [bit] NOT NULL,
		[IsOnlyFirstEnter] [bit] NOT NULL,
		[AllowedLate] int NOT NULL,
		[AllowedEarlyLeave] int NOT NULL,
		[DayIntervalsString] nvarchar(max) NULL,
		[Date] [datetime] NOT NULL
	CONSTRAINT [PK_EmployeeDay] PRIMARY KEY CLUSTERED 
	(
		[UID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	INSERT INTO Patches (Id) VALUES ('EmployeeDay')	
END

GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'EmployeeDayIndexes')
BEGIN
	CREATE INDEX EmployeeDayUIDIndex ON EmployeeDay([UID])
	CREATE INDEX EmployeeDayEmployeeUIDIndex ON EmployeeDay([EmployeeUID])
	INSERT INTO Patches (Id) VALUES ('EmployeeDayIndexes')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddingIsNeedAdjustmentColumn')
BEGIN
	ALTER TABLE PassJournal ADD [IsNeedAdjustment] bit NOT NULL DEFAULT 0
	INSERT INTO Patches (Id) VALUES ('AddingIsNeedAdjustmentColumn')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddingAdjustmentDateColumn')
BEGIN
	ALTER TABLE PassJournal ADD [AdjustmentDate] datetime NULL
	INSERT INTO Patches (Id) VALUES ('AddingAdjustmentDateColumn')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddingCorrectedByUIDColumn')
BEGIN
	ALTER TABLE PassJournal ADD [CorrectedByUID] uniqueidentifier NULL
	INSERT INTO Patches (Id) VALUES ('AddingCorrectedByUIDColumn')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddingNotTakeInCalculationsColumn')
BEGIN
	ALTER TABLE PassJournal ADD [NotTakeInCalculations] bit NOT NULL DEFAULT 0
	INSERT INTO Patches (Id) VALUES ('AddingNotTakeInCalculationsColumn')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddingIsAddedManuallyColumn')
BEGIN
	ALTER TABLE PassJournal ADD [IsAddedManually] bit NOT NULL DEFAULT 0
	INSERT INTO Patches (Id) VALUES ('AddingIsAddedManuallyColumn')
END
go
if not exists (select * from Patches where Id = 'AddingEnterTimeOriginalColumn')
begin
	alter table PassJournal add [EnterTimeOriginal] datetime NULL
	insert into Patches (Id) values ('AddingEnterTimeOriginalColumn')
end
go
if not exists (select * from Patches where Id = 'AddingExitTimeOriginalColumn')
begin
	alter table PassJournal add [ExitTimeOriginal] datetime NULL
	insert into Patches (Id) values ('AddingExitTimeOriginalColumn')
end
go
if not exists (select * from Patches where Id = 'AddingIsForceClosedColumn')
begin
	alter table PassJournal add [IsForceClosed] bit NOT NULL default 0
	insert into Patches (Id) values ('AddingIsForceClosedColumn')
end
go
if not exists (select * from Patches where Id = 'AddingIsOpenColumn')
begin
	alter table PassJournal add [IsOpen] bit NOT NULL default 0
	insert into Patches (Id) values ('AddingIsOpenColumn')
end
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'CorrectedByUIDToNullable')
BEGIN
	ALTER TABLE PassJournal DROP COLUMN CorrectedByUID
	ALTER TABLE PassJournal ADD [CorrectedByUID] uniqueidentifier NULL
	INSERT INTO Patches (Id) VALUES ('CorrectedByUIDToNullable')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddingIsNeedAdjustmentOriginalColumn')
BEGIN
	ALTER TABLE PassJournal ADD [IsNeedAdjustmentOriginal] bit NOT NULL DEFAULT 0
	INSERT INTO Patches (Id) VALUES ('AddingIsNeedAdjustmentOriginalColumn')
END