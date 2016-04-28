USE master
GO
IF EXISTS(SELECT name FROM sys.databases WHERE name = 'SKD')
BEGIN
SET NOEXEC ON
END
GO
CREATE DATABASE SKD
GO
USE SKD

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

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

CREATE PROCEDURE [dbo].[CreateOrganisationTimeTrackDocumentTypes]
(
	@organisationUID uniqueidentifier
)
AS
BEGIN
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Продолжительность работы в дневное время', 'Я', 1, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Продолжительность работы в ночное время', 'Н', 2, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Продолжительность работы в выходные и нерабочие праздничные дни', 'РВ', 3, 0, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Продолжительность сверхурочной работы', 'С', 4, 0, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Продолжительность работы вахтовым методом', 'ВМ', 5, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Служебная командировка', 'К', 6, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Повышение квалификации с отрывом от работы', 'ПК', 7, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Повышение квалификации с отрывом от работы в другой местности', 'ПМ', 8, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Ежегодный основной оплачиваемый отпуск', 'ОТ', 9, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Ежегодный дополнительный оплачиваемый отпуск', 'ОД', 10, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Дополнительный отпуск в связи с обучением с сохранением среднего заработка работникам, совмещающим работу с обучением', 'У', 11, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Сокращенная продолжительность рабочего времени для обучающихся без отрыва от производства с частичным сохранением заработной платы', 'УВ', 12, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Дополнительный отпуск в связи с обучением без сохранения заработной платы', 'УД', 13, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Отпуск по беременности и родам (отпуск в связи с усыновлением новорожденного ребенка)', 'Р', 14, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Отпуск по уходу за ребенком до достижения им возраста трех лет', 'ОЖ', 15, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Отпуск без сохранения заработной платы, предоставляемый работнику по разрешению работодателя', 'ДО', 16, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Отпуск без сохранения заработной платы при условиях, предусмотренных действующим законодательством Российской Федерации', 'ОЗ', 17, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Ежегодный дополнительный отпуск без сохранения заработной платы', 'ДБ', 18, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Временная нетрудоспособность (кроме случаев, предусмотренных кодом "Т") с назначением пособия согласно законодательству', 'Б', 19, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Временная нетрудоспособность без назначения пособия в случаях, предусмотренных законодательством', 'Т', 20, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Сокращенная продолжительность рабочего времени против нормальной продолжительности рабочего дня в случаях, предусмотренных законодательством', 'ЛЧ', 21, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Время вынужденного прогула в случае признания увольнения, перевода на другую работу или отстранения от работы незаконным и с восстановлением на прежней работе', 'ПВ', 22, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Невыходы на время исполнения государственных или общественных обязанностей согласно законодательству', 'Г', 23, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Прогулы (отсутствие на рабочем месте без уважительных причин в течение времени, установленного законодательством)', 'ПР', 24, 2, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Продолжительность работы в режиме неполного рабочего времени по инициативе работодателя в случаях, предусмотренных законодательством', 'НС', 25, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Выходные дни (еженедельный отпуск) и нерабочие праздничные дни', 'В', 26, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Дополнительные выходные дни (оплачиваемые)', 'ОВ', 27, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Дополнительные выходные дни (без сохранения заработной платы)', 'НВ', 28, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Забастовка (при условиях и в порядке, предусмотренных законом)', 'ЗБ', 29, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Неявки по невыясненным причинам (до выяснения обстоятельств)', 'НН', 30, 2, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Время простоя по вине работодателя', 'РП', 31, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Время простоя по причинам, не зависящим от работодателя и работника', 'НП', 32, 1, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Время простоя по вине работника', 'ВП', 33, 2, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Отстранение от работы (недопущение к работе) с оплатой (пособием) в соответствии с законодательством', 'НО', 34, 3, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Отстранение от работы (недопущение к работе) по причинам, предусмотренным законодательством, без начисления заработной платы', 'НБ', 35, 2, @organisationUID, 1)
	INSERT INTO TimeTrackDocumentType ([UID],[Name],[ShortName],[DocumentCode],[DocumentType],[OrganisationUID],[IsSystem]) VALUES(NEWID(), 'Время приостановки работы в случае задержки выплаты заработной платы', 'НЗ', 36, 3, @organisationUID, 1)
END
GO

CREATE PROCEDURE [dbo].[CreateOrganisationsTimeTrackDocumentTypes]
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
END
GO

CREATE TABLE [dbo].[Attachment](
	[UID] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Photo](
[UID] [uniqueidentifier] NOT NULL,
[Data] [varbinary](MAX) NULL,
CONSTRAINT [PK_Photo] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[AdditionalColumnType](
[UID] [uniqueidentifier] NOT NULL,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[Name] [nvarchar](50) NULL,
[Description] [nvarchar](max) NULL,
[DataType] [int] NULL,
[PersonType] [int],
[OrganisationUID] [uniqueidentifier] NULL,
[IsInGrid] [bit] NOT NULL,
CONSTRAINT [PK_AdditionalColumnType] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[AdditionalColumn](
[UID] [uniqueidentifier] NOT NULL,
[EmployeeUID] [uniqueidentifier] NULL,
[AdditionalColumnTypeUID] [uniqueidentifier] NULL,
[TextData] [text] NULL,
[PhotoUID] [uniqueidentifier] NULL,
CONSTRAINT [PK_AdditionalColumn] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[ScheduleDay](
[UID] [uniqueidentifier] NOT NULL,
[DayIntervalUID] [uniqueidentifier] NULL,
[ScheduleSchemeUID] [uniqueidentifier] NOT NULL,
[Number] [int] NOT NULL,
CONSTRAINT [PK_ScheduleDay] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Department](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NULL,
[Description] [nvarchar](max) NULL,
[PhotoUID] [uniqueidentifier] NULL,
[ParentDepartmentUID] [uniqueidentifier] NULL,
[ContactEmployeeUID] [uniqueidentifier] NULL,
[AttendantUID] [uniqueidentifier] NULL,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL ,
[OrganisationUID] [uniqueidentifier] NULL,
[ChiefUID] [uniqueidentifier] NOT NULL CONSTRAINT "Department_Chief_Default_Name" DEFAULT '00000000-0000-0000-0000-000000000000',
Phone nvarchar(50) NULL,
ExternalKey nvarchar(40) NOT NULL DEFAULT '-1'
CONSTRAINT [PK_Department_1] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Employee](
[UID] [uniqueidentifier] NOT NULL,
FirstName [nvarchar](50) NULL,
SecondName [nvarchar](50) NULL,
LastName [nvarchar](50) NULL,
PhotoUID [uniqueidentifier] NULL,
PositionUID [uniqueidentifier] NULL,
DepartmentUID [uniqueidentifier] NULL,
ScheduleUID [uniqueidentifier] NULL,
ScheduleStartDate [datetime] NOT NULL,
[Type] [int] NULL,
TabelNo [nvarchar](40) NULL,
EscortUID [uniqueidentifier] NULL,
IsDeleted [bit] NOT NULL,
RemovalDate [datetime] NOT NULL,
OrganisationUID [uniqueidentifier] NULL,
DocumentNumber [nvarchar](50),
BirthDate [datetime] NULL,
BirthPlace [nvarchar](MAX),
DocumentGivenDate [datetime] NULL,
DocumentGivenBy [nvarchar](MAX),
DocumentValidTo [datetime] NULL,
Gender [int] NULL,
DocumentDepartmentCode nvarchar(50),
Citizenship [nvarchar](MAX),
DocumentType [int] NULL,
Phone [nvarchar](50) NULL,
[Description] [nvarchar](max) NULL,
LastEmployeeDayUpdate datetime NOT NULL CONSTRAINT "Schedule_LastEmployeeDayUpdate_Default" DEFAULT '1900-01-01 00:00:00.000',
ExternalKey nvarchar(40) NOT NULL DEFAULT '-1'
CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Holiday](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NOT NULL,
[Type] [int] NOT NULL,
[Date] [datetime] NOT NULL,
[TransferDate] [datetime] NULL,
[Reduction] [int] NOT NULL,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
CONSTRAINT [PK_Holiday] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].DayIntervalPart(
[BeginTime] [int] NOT NULL,
[EndTime] [int] NOT NULL,
[UID] [uniqueidentifier] NOT NULL,
[DayIntervalUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_DayIntervalPart] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[DayInterval](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NOT NULL,
[Description] [nvarchar](max) NULL,
[SlideTime] [int] NOT NULL,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
CONSTRAINT [PK_DayInterval] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Position](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NULL,
[Description] [nvarchar](max) NULL,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
[PhotoUID] [uniqueidentifier] NULL,
ExternalKey nvarchar(40) NOT NULL DEFAULT -1
CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Schedule](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NOT NULL,
[ScheduleSchemeUID] [uniqueidentifier] NULL,
[IsIgnoreHoliday] [bit] NOT NULL CONSTRAINT "Schedule_IsIgnoreHoliday_Default" DEFAULT 0,
[IsOnlyFirstEnter] [bit] NOT NULL CONSTRAINT "Schedule_IsOnlyFirstEnter_Default" DEFAULT 0,
[AllowedLate] [int] NOT NULL CONSTRAINT "Schedule_AllowedLate_Default" DEFAULT 0,
[AllowedEarlyLeave] [int] NOT NULL CONSTRAINT "Schedule_AllowedEarlyLeave_Default" DEFAULT 0,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
[AllowedAbsentLowThan] [int] NOT NULL,
[NotAllowOvertimeLowerThan] [int] NOT NULL,
[IsEnabledAllowLate] [bit] NOT NULL,
[IsEnabledAllowEarlyLeave] [bit] NOT NULL,
[IsAllowAbsent] [bit] NOT NULL,
[IsEnabledOvertime] [bit] NOT NULL,
CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ScheduleScheme](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NOT NULL,
[Type] [int] NOT NULL,
[Description] [nvarchar](max) NULL,
[DaysCount] [int] NOT NULL CONSTRAINT "ScheduleScheme_DaysCount_Default" DEFAULT 0,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
CONSTRAINT [PK_ScheduleScheme] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Card](
[UID] [uniqueidentifier] NOT NULL,
[Number] [int] NOT NULL,
[EmployeeUID] [uniqueidentifier] NULL,
[AccessTemplateUID] [uniqueidentifier] NULL,
[CardType] [int] NULL,
[StartDate] [datetime] NOT NULL,
[EndDate] [datetime] NOT NULL,
[IsInStopList] [bit] NOT NULL,
[StopReason] [text] NULL,
PassCardTemplateUID [uniqueidentifier] NULL,
[Password] [nvarchar](50) NULL,
DeactivationControllerUID [uniqueidentifier] NULL,
[UserTime] [int] NOT NULL,
[IsHandicappedCard] [bit] NOT NULL,
ExternalKey nvarchar(40) NOT NULL DEFAULT '-1',
CONSTRAINT [PK_Card] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[PendingCard](
[UID] [uniqueidentifier] NOT NULL,
[CardUID] [uniqueidentifier] NOT NULL,
[ControllerUID] [uniqueidentifier] NOT NULL,
[Action] [int] NOT NULL,
CONSTRAINT [PK_PendingCard] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[CardDoor](
[UID] [uniqueidentifier] NOT NULL,
[DoorUID] [uniqueidentifier] NOT NULL,
[CardUID] [uniqueidentifier] NULL,
[AccessTemplateUID] [uniqueidentifier] NULL,
[EnterScheduleNo] [int] NOT NULL,
CONSTRAINT [PK_CardDoor] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[AccessTemplate](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NULL,
[Description] [nvarchar](max) NULL,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
CONSTRAINT [PK_AccessTemplate] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ScheduleZone](
[UID] [uniqueidentifier] NOT NULL,
[ZoneUID] [uniqueidentifier] NOT NULL CONSTRAINT "ScheduleZone_ZoneUID_Default" DEFAULT '00000000-0000-0000-0000-000000000000',
[DoorUID] [uniqueidentifier] NOT NULL CONSTRAINT "ScheduleZone_DoorUID_Default" DEFAULT '00000000-0000-0000-0000-000000000000',
[ScheduleUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_ScheduleZone] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TimeTrackDocument](
[UID] [uniqueidentifier] NOT NULL,
[EmployeeUID] [uniqueidentifier] NOT NULL,
[StartDateTime] [datetime] NOT NULL,
[EndDateTime] [datetime] NOT NULL,
[DocumentCode] [int] NOT NULL,
[Comment] [nvarchar](100) NULL,
[FileName] [nvarchar](100) NULL,
[DocumentDateTime] [datetime] NOT NULL,
[DocumentNumber] [int] NOT NULL,
[IsOutside] [bit] NOT NULL,
CONSTRAINT [PK_TimeTrackDocument] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Organisation](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](50) NULL,
[Description] [nvarchar](max) NULL,
[PhotoUID] [uniqueidentifier] NULL,
[IsDeleted] [bit] NOT NULL,
[RemovalDate] [datetime] NOT NULL,
[ChiefUID] [uniqueidentifier] NOT NULL CONSTRAINT "Organisation_ChiefUID_Default" DEFAULT '00000000-0000-0000-0000-000000000000',
[HRChiefUID] [uniqueidentifier] NOT NULL CONSTRAINT "Organisation_HRChiefUID_Default" DEFAULT '00000000-0000-0000-0000-000000000000',
Phone nvarchar(50) NULL,
ExternalKey nvarchar(40) NOT NULL DEFAULT '-1'
CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[OrganisationUser](
[UID] [uniqueidentifier] NOT NULL,
[UserUID] [uniqueidentifier] NOT NULL,
[OrganisationUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_OrganisationUser] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[OrganisationDoor](
[UID] [uniqueidentifier] NOT NULL,
[DoorUID] [uniqueidentifier] NOT NULL,
[OrganisationUID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_OrganisationDoor] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE NightSettings(
[UID] [uniqueidentifier] NOT NULL,
[OrganisationUID] [uniqueidentifier] NULL,
[NightStartTime] [bigint] NOT NULL,
[NightEndTime] [bigint] NOT NULL,
[IsNightSettingsEnabled] [bit] NOT NULL,
CONSTRAINT [PK_NightSettings] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TimeTrackDocumentType](
[UID] [uniqueidentifier] NOT NULL,
[Name] [nvarchar](max) NOT NULL,
[ShortName] [nvarchar](10) NOT NULL,
[DocumentCode] [int] NOT NULL,
[DocumentType] [int] NOT NULL,
[OrganisationUID] [uniqueidentifier] NOT NULL,
[IsSystem] [bit] NOT NULL
CONSTRAINT [PK_TimeTrackDocumentType] PRIMARY KEY CLUSTERED
(
[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

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

CREATE TABLE [dbo].[AccessTemplateDeactivatingReader](
	[UID] [uniqueidentifier] NOT NULL,
	[AccessTemplateUID] [uniqueidentifier] NOT NULL,
	[DeactivatingReaderUID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AccessTemplateDeactivatingReader] PRIMARY KEY CLUSTERED 
(
	[UID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE Patches (Id nvarchar(100) not null)
CREATE INDEX DayIntervalPartUIDIndex ON [dbo].[DayIntervalPart]([UID])
CREATE INDEX DayIntervalUIDIndex ON DayInterval([UID])
CREATE INDEX ScheduleDayUIDIndex ON [ScheduleDay]([UID])
CREATE INDEX ScheduleSchemeUIDIndex ON ScheduleScheme([UID])
CREATE INDEX ScheduleUIDIndex ON Schedule([UID])
CREATE INDEX AdditionalColumnUIDIndex ON AdditionalColumn([UID])
CREATE INDEX AdditionalColumnTypeUIDIndex ON AdditionalColumnType([UID])
CREATE INDEX PositionUIDIndex ON Position([UID])
CREATE INDEX EmployeeUIDIndex ON Employee([UID])
CREATE INDEX DepartmentUIDIndex ON Department([UID])
CREATE INDEX HolidayUIDIndex ON Holiday([UID])
CREATE INDEX CardUIDIndex ON Card([UID])
CREATE INDEX CardDoorUIDIndex ON CardDoor([UID])
CREATE INDEX ScheduleZoneUIDIndex ON ScheduleZone([UID])
CREATE INDEX OrganisationUIDIndex ON Organisation([UID])
CREATE INDEX PhotoUIDIndex ON Photo([UID])
CREATE INDEX NightSettingsUIDIndex ON NightSettings([UID])
CREATE INDEX TimeTrackDocumentTypeUIDIndex ON TimeTrackDocumentType([UID])
CREATE INDEX PassCardTemplateUIDIndex ON PassCardTemplate([UID])
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

ALTER TABLE [dbo].[AdditionalColumn] WITH NOCHECK ADD CONSTRAINT [FK_AdditionalColumn_Employee] FOREIGN KEY([EmployeeUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[AdditionalColumn] NOCHECK CONSTRAINT [FK_AdditionalColumn_Employee]

ALTER TABLE [dbo].[AdditionalColumn] WITH NOCHECK ADD CONSTRAINT [FK_AdditionalColumn_AdditionalColumnType] FOREIGN KEY([AdditionalColumnTypeUID])
REFERENCES [dbo].[AdditionalColumnType] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[AdditionalColumn] NOCHECK CONSTRAINT [FK_AdditionalColumn_AdditionalColumnType]

ALTER TABLE [dbo].[ScheduleDay] WITH NOCHECK ADD CONSTRAINT [FK_ScheduleDay_DayInterval] FOREIGN KEY([DayIntervalUid])
REFERENCES [dbo].[DayInterval] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[ScheduleDay] NOCHECK CONSTRAINT [FK_ScheduleDay_DayInterval]

ALTER TABLE [dbo].[ScheduleDay] WITH NOCHECK ADD CONSTRAINT [FK_ScheduleDay_ScheduleScheme] FOREIGN KEY([ScheduleSchemeUid])
REFERENCES [dbo].[ScheduleScheme] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[ScheduleDay] NOCHECK CONSTRAINT [FK_ScheduleDay_ScheduleScheme]

ALTER TABLE [dbo].[Department] WITH NOCHECK ADD CONSTRAINT [FK_Department_Department1] FOREIGN KEY([ParentDepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Department1]

ALTER TABLE [dbo].[Employee] WITH NOCHECK ADD CONSTRAINT [FK_Employee_Department1] FOREIGN KEY([DepartmentUid])
REFERENCES [dbo].[Department] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Department1]

ALTER TABLE [dbo].[Department] WITH NOCHECK ADD CONSTRAINT [FK_Department_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Photo]

ALTER TABLE [dbo].[Employee] WITH NOCHECK ADD CONSTRAINT [FK_Employee_Position] FOREIGN KEY([PositionUid])
REFERENCES [dbo].[Position] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Position]

ALTER TABLE [dbo].[Employee] WITH NOCHECK ADD CONSTRAINT [FK_Employee_Employee] FOREIGN KEY([EscortUID])
REFERENCES [dbo].Employee([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Employee]

ALTER TABLE [dbo].[Employee] WITH NOCHECK ADD CONSTRAINT [FK_Employee_Schedule] FOREIGN KEY([ScheduleUid])
REFERENCES [dbo].[Schedule] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Schedule]

ALTER TABLE [dbo].[Employee] WITH NOCHECK ADD CONSTRAINT [FK_Employee_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Photo]

ALTER TABLE [dbo].[Position] WITH NOCHECK ADD CONSTRAINT [FK_Position_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Position] NOCHECK CONSTRAINT [FK_Position_Photo]

ALTER TABLE [dbo].[DayIntervalPart] WITH NOCHECK ADD CONSTRAINT [FK_DayIntervalPart_DayInterval1] FOREIGN KEY([DayIntervalUid])
REFERENCES [dbo].[DayInterval] ([Uid])
ON UPDATE CASCADE
ON DELETE CASCADE
NOT FOR REPLICATION
ALTER TABLE [dbo].[DayIntervalPart] NOCHECK CONSTRAINT [FK_DayIntervalPart_DayInterval1]

ALTER TABLE [dbo].[ScheduleZone] WITH NOCHECK ADD CONSTRAINT [FK_ScheduleZone_Schedule] FOREIGN KEY([ScheduleUid])
REFERENCES [dbo].[Schedule] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[ScheduleZone] NOCHECK CONSTRAINT [FK_ScheduleZone_Schedule]

ALTER TABLE [dbo].[Schedule] WITH NOCHECK ADD CONSTRAINT [FK_Schedule_ScheduleScheme] FOREIGN KEY([ScheduleSchemeUid])
REFERENCES [dbo].[ScheduleScheme] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Schedule] NOCHECK CONSTRAINT [FK_Schedule_ScheduleScheme]

ALTER TABLE [dbo].[Department] WITH NOCHECK ADD CONSTRAINT [FK_Department_Employee1] FOREIGN KEY([ContactEmployeeUid])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Employee1]

ALTER TABLE [dbo].[Department] WITH NOCHECK ADD CONSTRAINT [FK_Department_Employee2] FOREIGN KEY([AttendantUid])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Employee2]

ALTER TABLE [dbo].[Card] WITH NOCHECK ADD CONSTRAINT [FK_Card_Employee] FOREIGN KEY([EmployeeUid])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Card] NOCHECK CONSTRAINT [FK_Card_Employee]

ALTER TABLE [dbo].[PendingCard] WITH NOCHECK ADD  CONSTRAINT [FK_PendingCard_Card] FOREIGN KEY([CardUid])
REFERENCES [dbo].[Card] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[PendingCard] NOCHECK CONSTRAINT [FK_PendingCard_Card]

ALTER TABLE [dbo].[Card] WITH NOCHECK ADD CONSTRAINT [FK_Card_AccessTemplate] FOREIGN KEY([AccessTemplateUid])
REFERENCES [dbo].[AccessTemplate] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Card] NOCHECK CONSTRAINT [FK_Card_AccessTemplate]

ALTER TABLE [dbo].[CardDoor] WITH NOCHECK ADD CONSTRAINT [FK_CardDoor_Card] FOREIGN KEY([CardUid])
REFERENCES [dbo].[Card] ([Uid])
ON DELETE CASCADE
NOT FOR REPLICATION

ALTER TABLE [dbo].[CardDoor] WITH NOCHECK ADD CONSTRAINT [FK_CardDoor_AccessTemplate] FOREIGN KEY([AccessTemplateUid])
REFERENCES [dbo].[AccessTemplate] ([Uid])
ON DELETE CASCADE
NOT FOR REPLICATION
ALTER TABLE [dbo].[CardDoor] NOCHECK CONSTRAINT [FK_CardDoor_AccessTemplate]

ALTER TABLE [dbo].[AdditionalColumnType] WITH NOCHECK ADD CONSTRAINT [FK_AdditionalColumnType_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION

ALTER TABLE [dbo].[Department] WITH NOCHECK ADD CONSTRAINT [FK_Department_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Department] NOCHECK CONSTRAINT [FK_Department_Organisation]

ALTER TABLE [dbo].[Schedule] WITH NOCHECK ADD CONSTRAINT [FK_Schedule_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Schedule] NOCHECK CONSTRAINT [FK_Schedule_Organisation]

ALTER TABLE [dbo].[Organisation] WITH NOCHECK ADD  CONSTRAINT [FK_Organisation_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Organisation] NOCHECK CONSTRAINT [FK_Organisation_Photo]

ALTER TABLE [dbo].[ScheduleScheme] WITH NOCHECK ADD CONSTRAINT [FK_ScheduleScheme_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[ScheduleScheme] NOCHECK CONSTRAINT [FK_ScheduleScheme_Organisation]

ALTER TABLE [dbo].[Position] WITH NOCHECK ADD CONSTRAINT [FK_Position_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Position] NOCHECK CONSTRAINT [FK_Position_Organisation]

ALTER TABLE [dbo].[DayInterval] WITH NOCHECK ADD CONSTRAINT [FK_DayInterval_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[DayInterval] NOCHECK CONSTRAINT [FK_DayInterval_Organisation]

ALTER TABLE [dbo].[Holiday] WITH NOCHECK ADD CONSTRAINT [FK_Holiday_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Holiday] NOCHECK CONSTRAINT [FK_Holiday_Organisation]

ALTER TABLE [dbo].[Employee] WITH NOCHECK ADD CONSTRAINT [FK_Employee_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[Employee] NOCHECK CONSTRAINT [FK_Employee_Organisation]

ALTER TABLE [dbo].[AccessTemplate] WITH NOCHECK ADD CONSTRAINT [FK_AccessTemplate_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[AccessTemplate] NOCHECK CONSTRAINT [FK_AccessTemplate_Organisation]

ALTER TABLE [dbo].[OrganisationDoor] WITH NOCHECK ADD CONSTRAINT [FK_OrganisationDoor_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[OrganisationDoor] NOCHECK CONSTRAINT [FK_OrganisationDoor_Organisation]

ALTER TABLE [dbo].[OrganisationUser] WITH NOCHECK ADD CONSTRAINT [FK_OrganisationUser_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[OrganisationUser] NOCHECK CONSTRAINT [FK_OrganisationUser_Organisation]

ALTER TABLE [dbo].[AdditionalColumn] WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalColumn_Photo] FOREIGN KEY([PhotoUID])
REFERENCES [dbo].[Photo] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[AdditionalColumn] NOCHECK CONSTRAINT [FK_AdditionalColumn_Photo]

ALTER TABLE [dbo].[TimeTrackException] WITH NOCHECK ADD  CONSTRAINT [FK_TimeTrackException_Employee] FOREIGN KEY([EmployeeUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION

ALTER TABLE [dbo].NightSettings WITH NOCHECK ADD CONSTRAINT [FK_NightSettings_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].NightSettings NOCHECK CONSTRAINT [FK_NightSettings_Organisation]

ALTER TABLE [dbo].[TimeTrackDocumentType] WITH NOCHECK ADD CONSTRAINT [FK_TimeTrackDocumentType_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[TimeTrackDocumentType] NOCHECK CONSTRAINT [FK_TimeTrackDocumentType_Organisation]

ALTER TABLE [dbo].[PassCardTemplate] WITH NOCHECK ADD CONSTRAINT [FK_PassCardTemplate_Organisation] FOREIGN KEY([OrganisationUid])
REFERENCES [dbo].[Organisation] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].[PassCardTemplate] NOCHECK CONSTRAINT [FK_PassCardTemplate_Organisation]

ALTER TABLE [dbo].Department WITH NOCHECK ADD CONSTRAINT [FK_Department_Employee] FOREIGN KEY([ChiefUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].Department NOCHECK CONSTRAINT [FK_Department_Employee]

ALTER TABLE [dbo].Organisation WITH NOCHECK ADD CONSTRAINT [FK_Organisation_Employee3] FOREIGN KEY([HRChiefUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].Organisation NOCHECK CONSTRAINT [FK_Organisation_Employee3]

ALTER TABLE [dbo].Organisation WITH NOCHECK ADD CONSTRAINT [FK_Organisation_Employee] FOREIGN KEY([ChiefUID])
REFERENCES [dbo].[Employee] ([Uid])
NOT FOR REPLICATION
ALTER TABLE [dbo].Organisation NOCHECK CONSTRAINT [FK_Organisation_Employee]

ALTER TABLE [dbo].[AccessTemplateDeactivatingReader]  WITH NOCHECK ADD  CONSTRAINT [FK_AccessTemplateDeactivatingReader_AccessTemplate] FOREIGN KEY([AccessTemplateUID])
REFERENCES [dbo].[AccessTemplate] ([UID])
NOT FOR REPLICATION 
ALTER TABLE [dbo].[AccessTemplateDeactivatingReader] NOCHECK CONSTRAINT [FK_AccessTemplateDeactivatingReader_AccessTemplate]

INSERT INTO Patches (Id) VALUES
('OrganisationUser')
INSERT INTO Patches (Id) VALUES
('AlterPatches')
INSERT INTO Patches (Id) VALUES
('Doors')
INSERT INTO Patches (Id) VALUES
('OrganisationZone')
INSERT INTO Patches (Id) VALUES
('DoorsEnterExit')
INSERT INTO Patches (Id) VALUES
('PendingCard')
INSERT INTO Patches (Id) VALUES
('CommonJournal')
INSERT INTO Patches (Id) VALUES
('PassJournal')
INSERT INTO Patches (Id) VALUES
('PendingCardControllerUID')
INSERT INTO Patches (Id) VALUES
('OrganisationCardTemplate')
INSERT INTO Patches (Id) VALUES
('RemoveCardSeries')
INSERT INTO Patches (Id) VALUES
('RemoveJournalCardSeries')
INSERT INTO Patches (Id) VALUES
('EventNamesDescription')
INSERT INTO Patches (Id) VALUES
('DoorsEnterExitUIDtoID')
INSERT INTO Patches (Id) VALUES
('DoorsExitUIDtoID')
INSERT INTO Patches (Id) VALUES
('AddJournalEmployeeUID')
INSERT INTO Patches (Id) VALUES
('RemoveDocument')
INSERT INTO Patches (Id) VALUES
('RemoveDocumentUIDIndex')
INSERT INTO Patches (Id) VALUES
('RemoveEmployeeReplacement')
INSERT INTO Patches (Id) VALUES
('RemoveEmployeeReplacementUIDIndex')
INSERT INTO Patches (Id) VALUES
('DoorsDropAntipassEscort')
INSERT INTO Patches (Id) VALUES
('CardPassCardTemplateUID')
INSERT INTO Patches (Id) VALUES
('JournalDropCardNo')
INSERT INTO Patches (Id) VALUES
('RemoveCardDoorParentType')
INSERT INTO Patches (Id) VALUES
('FK_CardDoor_Card_DROP')
INSERT INTO Patches (Id) VALUES
('PassJournalNullable')
INSERT INTO Patches (Id) VALUES
('SKDCardPasswordDeactivation')
INSERT INTO Patches (Id) VALUES
('FK_CardDoor_AccessTemplate_DROP')
INSERT INTO Patches (Id) VALUES
('RemoveCardDoorParentUID')
INSERT INTO Patches (Id) VALUES
('AddCardDoorCardUID')
INSERT INTO Patches (Id) VALUES
('AddAccessTemplateUID')
INSERT INTO Patches (Id) VALUES
('FK_CardDoor_Card_CascadeDelete')
INSERT INTO Patches (Id) VALUES
('FK_CardDoor_AccessTemplate_CascadeDelete')
INSERT INTO Patches (Id) VALUES
('Drop_FK_Journal_Card')
INSERT INTO Patches (Id) VALUES
('Journal_Detalisation')
INSERT INTO Patches (Id) VALUES
('Journal_NameText_nvarchar(max)')
INSERT INTO Patches (Id) VALUES
('Card_UserTime')
INSERT INTO Patches (Id) VALUES
('TimeTrackExceptions')
INSERT INTO Patches (Id) VALUES
('Schedule_AllowedLateAndEarlyLeave')
INSERT INTO Patches (Id) VALUES
('PassJournal_EnterTime_NotNull')
INSERT INTO Patches (Id) VALUES
('DropTable_Phone')
INSERT INTO Patches (Id) VALUES
('DropIndex_PhoneUIDIndex')
INSERT INTO Patches (Id) VALUES
('ScheduleZone_DropColumn_IsControl')
INSERT INTO Patches (Id) VALUES
('ScheduleZone_DropConstraint_IsControl')
INSERT INTO Patches (Id) VALUES
('Employee_DropColumn_Dismissed')
INSERT INTO Patches (Id) VALUES
('CreateTable_HolidaySettings')
INSERT INTO Patches (Id) VALUES
('CreateIndex_HolidaySettingsUIDIndex')
INSERT INTO Patches (Id) VALUES
('RenameDay')
INSERT INTO Patches (Id) VALUES
('RenameDayNamedIntervalUID')
INSERT INTO Patches (Id) VALUES
('RenameInterval')
INSERT INTO Patches (Id) VALUES
('RenameIntervalNamedIntervalUID')
INSERT INTO Patches (Id) VALUES
('RenameNamedInterval')
INSERT INTO Patches (Id) VALUES
('RenameTimeTrackException')
INSERT INTO Patches (Id) VALUES
('RenameTimeTrackExceptionDocumentType')
INSERT INTO Patches (Id) VALUES
('TimeTrackDocumentDateTimeAndNumber')
INSERT INTO Patches (Id) VALUES
('RenameHolidaySettings')
INSERT INTO Patches (Id) VALUES
('TimeTrackDocumentType')
INSERT INTO Patches (Id) VALUES
('PassCardTemplate')
INSERT INTO Patches (Id) VALUES
('DepartmentChiefUID')
INSERT INTO Patches (Id) VALUES
('OrganisationChiefUID')
INSERT INTO Patches (Id) VALUES
('DepartmentChiefUID_NOCHECK')
INSERT INTO Patches (Id) VALUES
('OrganisationChiefUID_NOCHECK')
INSERT INTO Patches (Id) VALUES
('HRChiefUID')
INSERT INTO Patches (Id) VALUES
('DepartmentPhone')
INSERT INTO Patches (Id) VALUES
('OrganisationPhone')
INSERT INTO Patches (Id) VALUES
('EmployeePhone')
INSERT INTO Patches (Id) VALUES
('ScheduleZone_Drop_IsDeleted')
INSERT INTO Patches (Id) VALUES
('CardDoor_Drop_IsDeleted')
INSERT INTO Patches (Id) VALUES
('DayIntervalPart_Drop_IsDeleted')
INSERT INTO Patches (Id) VALUES
('ScheduleDay_Drop_IsDeleted')
INSERT INTO Patches (Id) VALUES
('Add_Indexes')
INSERT INTO Patches (Id) VALUES
('Organisation_HRChief')
INSERT INTO Patches (Id) VALUES
('Journal_Indexes')
INSERT INTO Patches (Id) VALUES
('RenameCardDoorIntervals')
INSERT INTO Patches (Id) VALUES
('CardNoString')
INSERT INTO Patches (Id) VALUES
('RemoveEventNamesAndDescriptions')
INSERT INTO Patches (Id) VALUES
('Drop_Employee_Appointed')
INSERT INTO Patches (Id) VALUES
('EmployeeDescription')
INSERT INTO Patches (Id) VALUES
('Drop_Journal_And_PassJournal')
INSERT INTO Patches (Id) VALUES
('Create_Journal_And_PassJournal_Metadata')
INSERT INTO Patches (Id) VALUES
('Department_Chief_Default_Name')
INSERT INTO Patches (Id) VALUES
('Organisation_Chief_Default_Name')
INSERT INTO Patches (Id) VALUES
('Drop_table_OrganisationZone')
INSERT INTO Patches (Id) VALUES
('ScheduleZone_DoorUID')
INSERT INTO Patches (Id) VALUES
('Card_drop_IsDeleted')
INSERT INTO Patches (Id) VALUES
('Employee_LastEmployeeDayUpdate')
INSERT INTO Patches (Id) VALUES
('Card_Number_Int')
INSERT INTO Patches (Id) VALUES
('ExternalKeys')
INSERT INTO Patches (Id) VALUES
('DropEveningSettings')
INSERT INTO Patches (Id) VALUES
('TimeTrackDocument_FileName')
INSERT INTO Patches (Id) VALUES
('DropColumnDefaultConstraint')
INSERT INTO Patches (Id) VALUES
('AddIsHandicappedCardColumn')
INSERT INTO Patches (Id) VALUES
('DropColumnDefaultConstraint')
INSERT INTO Patches (Id) VALUES
('RemoveExitScheduleNoFromCardDoor')
INSERT INTO Patches (Id) VALUES
('NullableDocumentValidTo')
INSERT INTO Patches (Id) VALUES
('NullableDocumentGivenDate')
INSERT INTO Patches (Id) VALUES
('NullableBirthDate')
INSERT INTO Patches (Id) VALUES
('NullableDocumentType')
INSERT INTO Patches (Id) VALUES
('NullableGender')
INSERT INTO Patches (Id) VALUES
('AddAllowedAbsentLowThan')
INSERT INTO Patches (Id) VALUES
('AddNotAllowOvertimeLowerThan')
INSERT INTO Patches (Id) VALUES
('AddIsEnabledAllowLate')
INSERT INTO Patches (Id) VALUES
('AddIsEnabledAllowEarlyLeave')
INSERT INTO Patches (Id) VALUES
('AddIsAllowAbsent')
INSERT INTO Patches (Id) VALUES
('AddIsEnabledOvertime')
INSERT INTO Patches (Id) VALUES
('AddIsOutsideColumn')
INSERT INTO Patches (Id) VALUES
('AddIsSystemColumnInTimeTrackDocumentType')
INSERT INTO Patches (Id) VALUES
('AddIsNightSettingsEnabledColumn')
INSERT INTO Patches (Id) VALUES
('Attachment')
INSERT INTO Patches (Id) VALUES
('SP_CreateOrganisationTimeTrackDocumentTypes_Added')
INSERT INTO Patches (Id) VALUES
('SP_CreateOrganisationsTimeTrackDocumentTypes_Added')
INSERT INTO Patches (Id) VALUES
('OrganisationsTimeTrackDocumentTypes_Created')
INSERT INTO Patches (Id) VALUES
('FiltersTableAdded')
INSERT INTO Patches (Id) VALUES
('RemoveCredentialsStartDateField')
INSERT INTO Patches (Id) VALUES
('Table_AccessTemplateDeactivatingReader_Added')

DECLARE @OrgUid uniqueidentifier;
SET @OrgUid = NEWID();
INSERT INTO Organisation ([Uid],[Name],[IsDeleted],[RemovalDate]) VALUES (@OrgUid,'Организация',0,'01/01/1900')

EXEC [dbo].[CreateOrganisationsTimeTrackDocumentTypes]

DECLARE @Uid uniqueidentifier;
SET @Uid = NEWID();
INSERT INTO OrganisationUser ([Uid],[UserUID],[OrganisationUID]) VALUES (@Uid,'10e591fb-e017-442d-b176-f05756d984bb',@OrgUid)

SET NOEXEC OFF