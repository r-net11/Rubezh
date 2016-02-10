USE [PassJournal_0]
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