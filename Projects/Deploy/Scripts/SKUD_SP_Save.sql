USE [SKUD]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveDay]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveDay]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveHoliday]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveHoliday]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveInterval]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveInterval]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveNamedInterval]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveNamedInterval]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveSchedule]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveSchedule]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveScheduleScheme]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveScheduleScheme]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveEmployee]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveEmployee]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveDepartment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveDepartment]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveEmployeeReplacement]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveEmployeeReplacement]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SavePhone]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SavePhone]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveDocument]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveDocument]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SavePosition]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SavePosition]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveAdditionalColumn]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveAdditionalColumn]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveGuest]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveGuest]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveOrganization]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveOrganization]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveCard]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveCard]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SavePhoto]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SavePhoto]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveAccessTemplate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveAccessTemplate]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveAdditionalColumn]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveAdditionalColumn]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveAdditionalColumnType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveAdditionalColumnType]


GO
CREATE PROCEDURE [dbo].[SaveInterval]
	@Uid uniqueidentifier,
	@BeginDate datetime = NULL,
	@EndDate datetime = NULL,
	@TransitionType nvarchar(10) = null,
	@NamedIntervalUid uniqueidentifier = NULL,
	@IsDeleted bit ,
	@RemovalDate datetime 

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Interval] WHERE Uid = @Uid)
		UPDATE [dbo].[Interval]   SET 
			Uid = @Uid,
			BeginDate = @BeginDate,
			EndDate = @EndDate,
			TransitionType = @TransitionType,
			NamedIntervalUid = @NamedIntervalUid,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Interval] (
				Uid,
				BeginDate,
				EndDate,
				TransitionType,
				NamedIntervalUid,
				IsDeleted,
				RemovalDate)
			VALUES (
				@Uid,
				@BeginDate,
				@EndDate,
				@TransitionType,
				@NamedIntervalUid,
				@IsDeleted,
				@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveNamedInterval]
	@Uid uniqueidentifier,
	@OrganizationUid uniqueidentifier = NULL,
	@Name nvarchar(50) = NULL,
	@SlideTime SqlTime = '00:00',
	@IsDeleted bit = 0,
	@RemovalDate datetime = '01/01/1900' 
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[NamedInterval] WHERE Uid = @Uid)
		UPDATE [dbo].[NamedInterval]  SET 
			Uid = @Uid,
			Name = @Name,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate,
			SlideTime = @SlideTime,
			OrganizationUid = @OrganizationUid 
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[NamedInterval] (
				Uid,
				Name,
				IsDeleted,
				RemovalDate,
				SlideTime,
				OrganizationUid)
			VALUES (
				@Uid,
				@Name,
				@IsDeleted,
				@RemovalDate,
				@SlideTime,
				@OrganizationUid)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveDay]
	@Uid uniqueidentifier,
	@NamedIntervalUid uniqueidentifier = NULL,
	@ScheduleSchemeUid uniqueidentifier = NULL,
	@Number int = NULL,
	@IsDeleted bit = 0,
	@RemovalDate datetime = '01/01/1900'

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Day] WHERE Uid = @Uid)
		UPDATE [dbo].[Day] SET 
			Uid = @Uid,
			NamedIntervalUid = @NamedIntervalUid,
			ScheduleSchemeUid = @ScheduleSchemeUid,
			Number = @Number,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Day] (
				Uid,
				NamedIntervalUid,
				ScheduleSchemeUid,
				Number,
				IsDeleted,
				RemovalDate)
			VALUES	(
				@Uid,
				@NamedIntervalUid,
				@ScheduleSchemeUid,
				@Number,
				@IsDeleted,
				@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveScheduleScheme]
	@Uid uniqueidentifier,
	@OrganizationUid uniqueidentifier = NULL,
	@Name nvarchar(50) = NULL,
	@Type nvarchar(50) = NULL,
	@IsDeleted bit = 0,
	@RemovalDate datetime  = '01/01/1900'

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[ScheduleScheme] WHERE Uid = @Uid)
		UPDATE [dbo].[ScheduleScheme] SET 
			Uid = @Uid,
			Name = @Name,
			Type = @Type,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate,
			OrganizationUid = @OrganizationUid 
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[ScheduleScheme] (
				Uid,
				Name,
				Type,
				IsDeleted,
				RemovalDate,
				OrganizationUid)
			VALUES	(
				@Uid,
				@Name,
				@Type,
				@IsDeleted,
				@RemovalDate,
				@OrganizationUid)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveSchedule]
	@Uid uniqueidentifier,
	@OrganizationUid uniqueidentifier = NULL,
	@Name nvarchar(50)= NULL,
	@ScheduleSchemeUid uniqueidentifier,
	@IsDeleted bit ,
	@RemovalDate datetime 

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Schedule] WHERE Uid = @Uid)
		UPDATE [dbo].[Schedule] SET 
			Uid = @Uid,
			Name = @Name,
			ScheduleSchemeUid = @ScheduleSchemeUid,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate,
			OrganizationUid = @OrganizationUid 
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Schedule] (
				Uid,
				Name,
				ScheduleSchemeUid,
				IsDeleted,
				RemovalDate,
				OrganizationUid)
			VALUES (
				@Uid,
				@Name,
				@ScheduleSchemeUid,
				@IsDeleted,
				@RemovalDate,
				@OrganizationUid)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveHoliday]
	@Uid uniqueidentifier,
	@OrganizationUid uniqueidentifier = NULL,
	@Name nvarchar(50)= NULL,
	@Type int ,
	@Date SqlDate ,
	@TransferDate SqlDate ,
	@Reduction SqlTime ,
	@IsDeleted bit ,
	@RemovalDate datetime 

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Holiday] WHERE Uid = @Uid)
		UPDATE [dbo].[Holiday] SET 
			Name = @Name,
			Type = @Type,
			Date = @Date,
			TransferDate = @TransferDate,
			Reduction = @Reduction,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate,
			OrganizationUid = @OrganizationUid 
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Holiday](
				Uid,
				[Name], 
				[Type], 
				[Date], 
				[TransferDate], 
				[Reduction], 
				[IsDeleted], 
				[RemovalDate],
				OrganizationUid)
			VALUES (
				@Uid,
				@Name, 
				@Type, 
				@Date, 
				@TransferDate, 
				@Reduction, 
				@IsDeleted, 
				@RemovalDate,
				@OrganizationUid)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveEmployee]
	@Uid [uniqueidentifier],
	@OrganizationUid uniqueidentifier = NULL,
	@FirstName [nvarchar](50) = NULL,
	@SecondName [nvarchar](50) = NULL,
	@LastName [nvarchar](50) = NULL,
	@PositionUid [uniqueidentifier] = NULL,
	@DepartmentUid [uniqueidentifier] = NULL,
	@ScheduleUid [uniqueidentifier] = NULL,
	@Appointed [datetime] = NULL,
	@Dismissed [datetime] = NULL,
	@IsDeleted [bit] = NULL,
	@RemovalDate [datetime] = NULL

AS
BEGIN
	INSERT INTO [dbo].[Employee] (
			[Uid] ,
			[FirstName] ,
			[SecondName] ,
			[LastName] ,
			[PositionUid] ,
			[DepartmentUid] ,
			[ScheduleUid] ,
			[Appointed] ,
			[Dismissed] ,
			[IsDeleted] ,
			[RemovalDate],
			OrganizationUid,
			[Type],
			TabelNo,
			CredentialsStartDate )
		VALUES (
			@Uid ,
			@FirstName ,
			@SecondName ,
			@LastName ,
			@PositionUid ,
			@DepartmentUid ,
			@ScheduleUid ,
			@Appointed ,
			@Dismissed ,
			@IsDeleted ,
			@RemovalDate,
			@OrganizationUid,
			0,
			-1,
			'01/01/1900')
END
GO
CREATE PROCEDURE [dbo].[SaveGuest]
	@Uid [uniqueidentifier],
	@OrganizationUid uniqueidentifier = NULL,
	@FirstName [nvarchar](50) = NULL,
	@SecondName [nvarchar](50) = NULL,
	@LastName [nvarchar](50) = NULL,
	@IsDeleted [bit] = NULL,
	@RemovalDate [datetime] = NULL

AS
BEGIN
	INSERT INTO [dbo].[Employee] (
				[Uid] ,
				[FirstName] ,
				[SecondName] ,
				[LastName] ,
				[PositionUid] ,
				[DepartmentUid] ,
				[ScheduleUid] ,
				[Appointed] ,
				[Dismissed] ,
				[IsDeleted] ,
				[RemovalDate],
				OrganizationUid,
				[Type],
				TabelNo,
				CredentialsStartDate )
			VALUES (
				@Uid ,
				@FirstName ,
				@SecondName ,
				@LastName ,
				NULL ,
				NULL ,
				NULL ,
				'01/01/1900' ,
				'01/01/1900' ,
				@IsDeleted ,
				@RemovalDate,
				@OrganizationUid,
				1,
				-1,
				'01/01/1900')
END
GO
CREATE PROCEDURE [dbo].[SaveDepartment]
	@Uid [uniqueidentifier] ,
	@OrganizationUid uniqueidentifier = NULL,
	@Name [nvarchar](50) = NULL,
	@Description [nvarchar](max) = NULL,
	@ParentDepartmentUid [uniqueidentifier] = NULL,
	@ContactEmployeeUid [uniqueidentifier] = NULL,
	@AttendantUid [uniqueidentifier] = NULL,
	@IsDeleted [bit] = NULL,
	@RemovalDate [datetime] = NULL
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Department] WHERE Uid = @Uid)
		UPDATE [dbo].[Department] SET 
			[Uid] = @Uid ,
			[Name] = @Name, 
			[Description] = @Description ,
			[ParentDepartmentUid] = @ParentDepartmentUid ,
			[ContactEmployeeUid] = @ContactEmployeeUid ,
			[AttendantUid] = @AttendantUid,
			[IsDeleted] = @IsDeleted,
			[RemovalDate] = @RemovalDate,
			OrganizationUid = @OrganizationUid 
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Department] (
				[Uid],
				[Name], 
				[Description] ,
				[ParentDepartmentUid] ,
				[ContactEmployeeUid],
				[AttendantUid],
				[IsDeleted] ,
				[RemovalDate],
				OrganizationUid)
			VALUES (
				@Uid,
				@Name, 
				@Description ,
				@ParentDepartmentUid,
				@ContactEmployeeUid,
				@AttendantUid,
				@IsDeleted,
				@RemovalDate,
				@OrganizationUid)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveEmployeeReplacement]
	@Uid [uniqueidentifier] ,
	@OrganizationUid uniqueidentifier = NULL,	
	@BeginDate [datetime] = NULL,
	@EndDate [datetime] = NULL,
	@EmployeeUid [uniqueidentifier] = NULL,
	@DepartmentUid [uniqueidentifier] = NULL,
	@ScheduleUid [uniqueidentifier] = NULL,
	@IsDeleted [bit] = NULL,
	@RemovalDate [datetime] = NULL
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[EmployeeReplacement] WHERE Uid = @Uid)
		UPDATE [dbo].[EmployeeReplacement] SET 
			[Uid] = @Uid ,
			[IsDeleted] = @IsDeleted,
			[RemovalDate] = @RemovalDate,
			[BeginDate] = @BeginDate,
			[EndDate] = @EndDate,
			[EmployeeUid] = @EmployeeUid,
			[DepartmentUid] = @DepartmentUid,
			[ScheduleUid] = @ScheduleUid,
			OrganizationUid = @OrganizationUid 
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[EmployeeReplacement] (
				[Uid],
				[IsDeleted] ,
				[RemovalDate],
				[BeginDate] ,
				[EndDate] ,
				[EmployeeUid] ,
				[DepartmentUid] ,
				[ScheduleUid],
				OrganizationUid )
			VALUES (
				@Uid,
				@IsDeleted,
				@RemovalDate,
				@BeginDate,
				@EndDate,
				@EmployeeUid,
				@DepartmentUid,
				@ScheduleUid,
				@OrganizationUid)
		END
END

GO
CREATE PROCEDURE [dbo].[SavePhone]
	@Uid [uniqueidentifier] ,
	@OrganizationUid uniqueidentifier = NULL,
	@Name [nvarchar](50) = NULL,
	@NumberString [nvarchar](50) = NULL,
	@DepartmentUid [uniqueidentifier] = NULL,
	@IsDeleted [bit] = NULL,
	@RemovalDate [datetime] = NULL
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Phone] WHERE Uid = @Uid)
		UPDATE [dbo].[Phone] SET 
			[Uid] = @Uid ,
			[IsDeleted] = @IsDeleted,
			[RemovalDate] = @RemovalDate,
			[Name] = @Name ,
			[NumberString] = @NumberString ,
			[DepartmentUid] = @DepartmentUid,
			OrganizationUid = @OrganizationUid  	
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Phone] (
				[Uid],
				[IsDeleted] ,
				[RemovalDate],
				[Name] ,
				[NumberString] ,
				[DepartmentUid],
				OrganizationUid )
			VALUES (
				@Uid,
				@IsDeleted,
				@RemovalDate,
				@Name ,
				@NumberString ,
				@DepartmentUid ,
				@OrganizationUid)
		END
END

GO
CREATE PROCEDURE [dbo].[SaveDocument]
	@Uid [uniqueidentifier] ,
	@OrganizationUid uniqueidentifier = NULL,	
	@No [int] ,
	@Name [nvarchar](50) ,
	@Description [nvarchar](max) ,
	@IssueDate [datetime] ,
	@LaunchDate [datetime] ,
	@IsDeleted [bit] ,
	@RemovalDate [datetime] 
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Document] WHERE Uid = @Uid)
		UPDATE [dbo].[Document] SET 
			[Uid] = @Uid ,
			[IsDeleted] = @IsDeleted,
			[RemovalDate] = @RemovalDate,
			[Name] = @Name ,
			[Description] = @Description,
			[IssueDate] = @IssueDate,
			[LaunchDate] = @LaunchDate,
			OrganizationUid = @OrganizationUid,
			[No] = @No
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Document] (
				[Uid],
				[IsDeleted] ,
				[RemovalDate],
				[Name] ,
				[Description] ,
				[IssueDate] ,
				[LaunchDate],
				OrganizationUid,
				[No] )
			VALUES (
				@Uid,
				@IsDeleted,
				@RemovalDate,
				@Name ,
				@Description,
				@IssueDate,
				@LaunchDate,
				@OrganizationUid,
				@No)
		END
END

GO
CREATE PROCEDURE [dbo].[SavePosition]
	@Uid [uniqueidentifier] ,
	@OrganizationUid uniqueidentifier = NULL,
	@Name [nvarchar](50) ,
	@Description [nvarchar](max) ,
	@IsDeleted [bit] ,
	@RemovalDate [datetime] 
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Position] WHERE Uid = @Uid)
		UPDATE [dbo].[Position] SET 
			[Uid] = @Uid ,
			[IsDeleted] = @IsDeleted,
			[RemovalDate] = @RemovalDate,
			[Name] = @Name ,
			[Description] = @Description,
			OrganizationUid = @OrganizationUid 
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Position] (
				[Uid],
				[IsDeleted] ,
				[RemovalDate],
				[Name] ,
				[Description],
				OrganizationUid )
			VALUES (
				@Uid,
				@IsDeleted,
				@RemovalDate,
				@Name ,
				@Description,
				@OrganizationUid)
		END
END

GO
CREATE PROCEDURE [dbo].[SaveOrganization]
	@Uid [uniqueidentifier] ,
	@Name [nvarchar](50) = NULL,
	@Description [nvarchar](max) = NULL,
	@IsDeleted [bit] = NULL,
	@RemovalDate [datetime] = NULL
AS
BEGIN
	INSERT INTO [dbo].[Organization] (
		[Uid],
		[IsDeleted] ,
		[RemovalDate],
		[Name] ,
		[Description] )
	VALUES (
		@Uid,
		@IsDeleted,
		@RemovalDate,
		@Name ,
		@Description )
END

GO
CREATE PROCEDURE SaveCard
	@UID uniqueidentifier,
	@IsDeleted bit,
	@RemovalDate datetime,	
	@Series int,
	@Number int,
	@EmployeeUID uniqueidentifier = NULL,
	@AccessTemplateUID uniqueidentifier = NULL,
	@ValidFrom datetime,
	@ValidTo datetime,
	@IsInStopList bit,
	@StopReason text = NULL
AS
BEGIN
	INSERT INTO Card (UID,IsDeleted,RemovalDate,Series,Number,EmployeeUID,AccessTemplateUID,StartDate,EndDate,IsInStopList,StopReason,CardType)
	VALUES (@UID,@IsDeleted,@RemovalDate,@Series,@Number,@EmployeeUID,@AccessTemplateUID,@ValidFrom,@ValidTo,@IsInStopList,@StopReason,0)
END

GO
CREATE PROCEDURE SaveAccessTemplate
	@UID uniqueidentifier,
	@OrganizationUID uniqueidentifier,
	@Name nvarchar(50) = NULL,
	@Description nvarchar(max) = NULL,
	@IsDeleted bit,
	@RemovalDate datetime
AS
BEGIN
	INSERT INTO AccessTemplate (
		UID,
		Name,
		Description,
		IsDeleted,
		RemovalDate,
		OrganizationUID)
	VALUES (
		@UID,
		@Name,
		@Description,
		@IsDeleted ,
		@RemovalDate ,
		@OrganizationUID )
END

GO
CREATE PROCEDURE SaveAdditionalColumnType
	@UID uniqueidentifier,
	@OrganizationUID uniqueidentifier = NULL,
	@Name nvarchar(50) = NULL,
	@Description nvarchar(max) = NULL,
	@DataType int = NULL,
	@PersonType int,
	@IsDeleted bit,
	@RemovalDate datetime
AS
BEGIN
	INSERT INTO AdditionalColumnType (
		UID,
		OrganizationUID,
		Name,
		Description,
		DataType,
		PersonType,
		IsDeleted,
		RemovalDate,
		IsInGrid)
	VALUES (
		@UID,
		@OrganizationUID ,
		@Name ,
		@Description ,
		@DataType ,
		@PersonType,
		@IsDeleted ,
		@RemovalDate,
		0 )
END	

GO
CREATE PROCEDURE SaveAdditionalColumn
	@UID uniqueidentifier,
	@EmployeeUID uniqueidentifier = NULL,
	@AdditionalColumnTypeUID uniqueidentifier = NULL,
	@TextData text = NULL,
	@GraphicsDataUID uniqueidentifier = NULL
AS
BEGIN
	INSERT INTO AdditionalColumn (
		UID,
		EmployeeUID,
		AdditionalColumnTypeUID ,
		TextData ,
		PhotoUID )
	VALUES (
		@UID,
		@EmployeeUID ,
		@AdditionalColumnTypeUID ,
		@TextData ,
		@GraphicsDataUID )
END	

GO
CREATE PROCEDURE SavePhoto
	@UID uniqueidentifier,
	@Data varbinary(MAX)
AS
BEGIN
	INSERT INTO Photo (
		UID,
		Data )
	VALUES (
		@UID ,
		@Data )
END	