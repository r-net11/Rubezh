USE master
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'PassJournal_0')
BEGIN
	SET NOEXEC ON
END
GO
CREATE DATABASE PassJournal_0
GO
USE PassJournal_0

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

-- ============================================================================
-- Пользовательские типы данных
-- ============================================================================
CREATE TYPE SqlTime FROM DATETIME

CREATE TYPE SqlDate FROM DATETIME
GO
CREATE RULE TimeOnlyRule AS
	datediff(dd, 0, @DateTime) = 0
GO
CREATE RULE DateOnlyRule AS
	dateAdd(dd, datediff(dd,0,@DateTime), 0) = @DateTime
GO
EXEC sp_bindrule 'TimeOnlyRule', 'SqlTime'
EXEC sp_bindrule 'DateOnlyRule', 'SqlDate'
GO

-- ============================================================================
-- Хранимые процедуры
-- ============================================================================
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
	EXEC('ALTER TABLE '+@tableName+' DROP CONSTRAINT ' + @ConstraintName)
END
GO

-- ============================================================================
-- Таблицы
-- ============================================================================
CREATE TABLE [dbo].[PassJournal](
	[UID] [uniqueidentifier] NOT NULL,
	[EmployeeUID] [uniqueidentifier] NOT NULL,
	[ZoneUID] [uniqueidentifier] NOT NULL,
	[EnterTime] [datetime] NOT NULL,
	[ExitTime] [datetime] NULL,
	[IsNeedAdjustment] [bit] NOT NULL,
	[AdjustmentDate] [datetime] NULL,
	[CorrectedByUID] [uniqueidentifier] NULL,
	[NotTakeInCalculations] [bit] NOT NULL,
	[IsAddedManually] [bit] NOT NULL,
	[IsForceClosed] [bit] NOT NULL,
	[IsOpen] [bit] NOT NULL,
	[IsNeedAdjustmentOriginal] [bit] NOT NULL
 CONSTRAINT [PK_PassJournal] PRIMARY KEY CLUSTERED
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE INDEX PassJournalUIDIndex ON PassJournal([UID])

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

CREATE INDEX EmployeeDayUIDIndex ON EmployeeDay([UID])
CREATE INDEX EmployeeDayEmployeeUIDIndex ON EmployeeDay([EmployeeUID])

CREATE TABLE Patches (Id nvarchar(100) not null)
INSERT INTO Patches (Id) VALUES
	('EmployeeDay')
INSERT INTO Patches (Id) VALUES
	('EmployeeDayIndexes')
INSERT INTO Patches (Id) VALUES
('AddingIsNeedAdjustmentColumn')
INSERT INTO Patches (Id) VALUES
('AddingCorrectedByUIDColumn')
INSERT INTO Patches (Id) VALUES
('AddingAdjustmentDateColumn')
INSERT INTO Patches (Id) VALUES
('AddingNotTakeInCalculationsColumn')
INSERT INTO Patches (Id) VALUES
('AddingIsAddedManuallyColumn')
INSERT INTO Patches (Id) VALUES
('AddingEnterTimeOriginalColumn')
INSERT INTO Patches (Id) VALUES
('AddingExitTimeOriginalColumn')
INSERT INTO Patches (Id) VALUES
('AddingIsForceClosedColumn')
INSERT INTO Patches (Id) VALUES
('AddingIsOpenColumn')
INSERT INTO Patches (Id) VALUES
('CorrectedByUIDToNullable')
INSERT INTO Patches (Id) VALUES
('AddingIsNeedAdjustmentOriginalColumn')
INSERT INTO Patches (Id) VALUES
('AddingNotTakeInCalculationsOriginalColumn')
INSERT INTO Patches (Id) VALUES
('DropColumnDefaultConstraint')
INSERT INTO Patches (Id) VALUES
('RemoveEnterTimeOriginalColumn')
INSERT INTO Patches (Id) VALUES
('RemoveExitTimeOriginalColumn')
INSERT INTO Patches (Id) VALUES
('RemoveNotTakeInCalculationsOriginalColumn')

SET NOEXEC OFF