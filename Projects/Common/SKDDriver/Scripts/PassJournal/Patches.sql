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
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'AddingNotTakeInCalculationsOriginalColumn')
	BEGIN
		ALTER TABLE PassJournal ADD [NotTakeInCalculationsOriginal] bit NOT NULL DEFAULT 0
		INSERT INTO Patches (Id) VALUES ('AddingNotTakeInCalculationsOriginalColumn')
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

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Drop_PassJournal_NotTakeInCalculationsOriginal_Default')
BEGIN
	EXECUTE [dbo].[DropColumnDefaultConstraint] [PassJournal], [NotTakeInCalculationsOriginal]
	INSERT INTO Patches (Id) VALUES ('Drop_PassJournal_NotTakeInCalculationsOriginal_Default')
END
GO

---------------------------------------------Release 1.0.3------------------------------------------------
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveEnterTimeOriginalColumn')
BEGIN
	ALTER TABLE [PassJournal] DROP COLUMN [EnterTimeOriginal]
	INSERT INTO Patches (Id) VALUES('RemoveEnterTimeOriginalColumn')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveExitTimeOriginalColumn')
BEGIN
	ALTER TABLE [PassJournal] DROP COLUMN [ExitTimeOriginal]
	INSERT INTO Patches (Id) VALUES('RemoveExitTimeOriginalColumn')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveIsNeedAdjustmentOriginalColumn')
BEGIN
	ALTER TABLE [PassJournal] DROP COLUMN [IsNeedAdjustmentOriginal]
	INSERT INTO Patches (Id) VALUES('RemoveIsNeedAdjustmentOriginalColumn')
END
GO

IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'RemoveNotTakeInCalculationsOriginalColumn')
BEGIN
	ALTER TABLE [PassJournal] DROP COLUMN [NotTakeInCalculationsOriginal]
	INSERT INTO Patches (Id) VALUES('RemoveNotTakeInCalculationsOriginalColumn')
END
GO