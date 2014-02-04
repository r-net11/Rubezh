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
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveRegisterDevice]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveRegisterDevice]
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
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveJournal]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveJournal]

GO
CREATE PROCEDURE [dbo].[SaveInterval]
	@Uid uniqueidentifier,
	@BeginDate datetime = NULL,
	@EndDate datetime = NULL,
	@Transition nvarchar(10) = null,
	@NamedIntervalUid uniqueidentifier = NULL,
	@IsDeleted bit = NULL,
	@RemovalDate datetime = NULL

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Interval] WHERE Uid = @Uid)
		UPDATE [dbo].[Interval]   SET 
			Uid = @Uid,
			BeginDate = @BeginDate,
			EndDate = @EndDate,
			Transition = @Transition,
			NamedIntervalUid = @NamedIntervalUid,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Interval]   
				(Uid,BeginDate,EndDate,Transition,NamedIntervalUid,IsDeleted,RemovalDate)
			VALUES	
				(@Uid,@BeginDate,@EndDate,@Transition,@NamedIntervalUid,@IsDeleted,@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveNamedInterval]
	@Uid uniqueidentifier,
	@Name nvarchar(50) = NULL,
	@IsDeleted bit = NULL,
	@RemovalDate datetime = NULL

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[NamedInterval] WHERE Uid = @Uid)
		UPDATE [dbo].[NamedInterval]  SET 
			Uid = @Uid,
			Name = @Name,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[NamedInterval]  
				(Uid,Name,IsDeleted,RemovalDate)
			VALUES	
				(@Uid,@Name,@IsDeleted,@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveDay]
	@Uid uniqueidentifier,
	@NamedIntervalUid uniqueidentifier = NULL,
	@ScheduleSchemeUid uniqueidentifier = NULL,
	@Number int = NULL,
	@IsDeleted bit = NULL,
	@RemovalDate datetime = NULL

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
			INSERT INTO [dbo].[Day] 
				(Uid,NamedIntervalUid,ScheduleSchemeUid,Number,IsDeleted,RemovalDate)
			VALUES	
				(@Uid,@NamedIntervalUid,@ScheduleSchemeUid,@Number,@IsDeleted,@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveScheduleScheme]
	@Uid uniqueidentifier,
	@Name nvarchar(50) = NULL,
	@Type nvarchar(50) = NULL,
	@Length int = NULL,
	@IsDeleted bit = NULL,
	@RemovalDate datetime = NULL

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[ScheduleScheme] WHERE Uid = @Uid)
		UPDATE [dbo].[ScheduleScheme] SET 
			Uid = @Uid,
			Name = @Name,
			Type = @Type,
			Length = @Length,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[ScheduleScheme] 
				(Uid,Name,Type,Length,IsDeleted,RemovalDate)
			VALUES	
				(@Uid,@Name,@Type,@Length,@IsDeleted,@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveRegisterDevice]
	@Uid uniqueidentifier,
	@CanControl bit = NULL,
	@ScheduleUid uniqueidentifier,
	@IsDeleted bit = NULL,
	@RemovalDate datetime = NULL

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[RegisterDevice] WHERE Uid = @Uid)
		UPDATE [dbo].RegisterDevice SET 
			Uid = @Uid,
			CanControl = @CanControl,
			ScheduleUid = @ScheduleUid,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[RegisterDevice] 
				(Uid,CanControl,ScheduleUid,IsDeleted,RemovalDate)
			VALUES	
				(@Uid,@CanControl,@ScheduleUid,@IsDeleted,@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveSchedule]
	@Uid uniqueidentifier,
	@Name nvarchar(50)= NULL,
	@ScheduleSchemeUid uniqueidentifier,
	@IsDeleted bit = NULL,
	@RemovalDate datetime = NULL

AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Schedule] WHERE Uid = @Uid)
		UPDATE [dbo].[Schedule] SET 
			Uid = @Uid,
			Name = @Name,
			ScheduleSchemeUid = @ScheduleSchemeUid,
			IsDeleted = @IsDeleted,
			RemovalDate = @RemovalDate
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Schedule] 
				(Uid,Name,ScheduleSchemeUid,IsDeleted,RemovalDate)
			VALUES	
				(@Uid,@Name,@ScheduleSchemeUid,@IsDeleted,@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveHoliday]
	@Uid uniqueidentifier,
	@Name nvarchar(50)= NULL,
	@Type nvarchar(50)= NULL,
	@Date datetime = NULL,
	@TransferDate datetime = NULL,
	@Reduction int = NULL,
	@IsDeleted bit = NULL,
	@RemovalDate datetime = NULL

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
			RemovalDate = @RemovalDate
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Holiday] 
				(Uid,[Name], [Type], [Date], [TransferDate], [Reduction], [IsDeleted], [RemovalDate])
			VALUES	
				(@Uid,@Name, @Type, @Date, @TransferDate, @Reduction, @IsDeleted, @RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveEmployee]
	@Uid [uniqueidentifier],
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
	IF EXISTS(SELECT Uid FROM [dbo].[Employee] WHERE Uid = @Uid)
		UPDATE [dbo].[Employee] SET 
			[Uid] = @Uid,
			[FirstName] = @FirstName,
			[SecondName] = @SecondName,
			[LastName] = @LastName,
			[PositionUid] = @PositionUid,
			[DepartmentUid] = @DepartmentUid,
			[ScheduleUid] = @ScheduleUid,
			[Appointed] = @Appointed,
			[Dismissed] = @Dismissed,
			[IsDeleted] = @IsDeleted,
			[RemovalDate] = @RemovalDate
		WHERE Uid = @Uid
	ELSE
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
				[RemovalDate] )
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
				@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveDepartment]
	@Uid [uniqueidentifier] ,
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
			[RemovalDate] = @RemovalDate
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
				[RemovalDate])
			VALUES (
				@Uid,
				@Name, 
				@Description ,
				@ParentDepartmentUid,
				@ContactEmployeeUid,
				@AttendantUid,
				@IsDeleted,
				@RemovalDate)
		END
END
GO
CREATE PROCEDURE [dbo].[SaveEmployeeReplacement]
	@Uid [uniqueidentifier] ,	
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
			[ScheduleUid] = @ScheduleUid
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
				[ScheduleUid] )
			VALUES (
				@Uid,
				@IsDeleted,
				@RemovalDate,
				@BeginDate,
				@EndDate,
				@EmployeeUid,
				@DepartmentUid,
				@ScheduleUid)
		END
END

GO
CREATE PROCEDURE [dbo].[SavePhone]
	@Uid [uniqueidentifier] ,
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
			[DepartmentUid] = @DepartmentUid 	
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Phone] (
				[Uid],
				[IsDeleted] ,
				[RemovalDate],
				[Name] ,
				[NumberString] ,
				[DepartmentUid] )
			VALUES (
				@Uid,
				@IsDeleted,
				@RemovalDate,
				@Name ,
				@NumberString ,
				@DepartmentUid )
		END
END

GO
CREATE PROCEDURE [dbo].[SaveDocument]
	@Uid [uniqueidentifier] ,
	@Name [nvarchar](50) = NULL,
	@Description [nvarchar](max) = NULL,
	@IssueDate [datetime] = NULL,
	@LaunchDate [datetime] = NULL,
	@IsDeleted [bit] = NULL,
	@RemovalDate [datetime] = NULL
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
			[LaunchDate] = @LaunchDate
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
				[LaunchDate] )
			VALUES (
				@Uid,
				@IsDeleted,
				@RemovalDate,
				@Name ,
				@Description,
				@IssueDate,
				@LaunchDate)
		END
END

GO
CREATE PROCEDURE [dbo].[SavePosition]
	@Uid [uniqueidentifier] ,
	@Name [nvarchar](50) = NULL,
	@Description [nvarchar](max) = NULL,
	@IsDeleted [bit] = NULL,
	@RemovalDate [datetime] = NULL
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Position] WHERE Uid = @Uid)
		UPDATE [dbo].[Position] SET 
			[Uid] = @Uid ,
			[IsDeleted] = @IsDeleted,
			[RemovalDate] = @RemovalDate,
			[Name] = @Name ,
			[Description] = @Description
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Position] (
				[Uid],
				[IsDeleted] ,
				[RemovalDate],
				[Name] ,
				[Description])
			VALUES (
				@Uid,
				@IsDeleted,
				@RemovalDate,
				@Name ,
				@Description)
		END
END

GO
CREATE PROCEDURE [dbo].[SaveAdditionalColumn]
	@Uid [uniqueidentifier] ,
	@Name [nvarchar](50) = NULL,
	@Description [nvarchar](max) = NULL,
	@Type [nvarchar](50) = NULL,
	@TextData [text] = NULL,
	@GraphicsData [binary](8000) = NULL,
	@EmployeeUid [uniqueidentifier] = NULL,
	@IsDeleted [bit] = NULL,
	@RemovalDate [datetime] = NULL
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[AdditionalColumn] WHERE Uid = @Uid)
		UPDATE [dbo].[AdditionalColumn] SET 
			[Uid] = @Uid ,
			[IsDeleted] = @IsDeleted,
			[RemovalDate] = @RemovalDate,
			[Name] = @Name ,
			[Description] = @Description,
			[Type] = @Type ,
			[TextData] = @TextData,
			[GraphicsData] =@GraphicsData,
			[EmployeeUid] =@EmployeeUid
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[AdditionalColumn] (
				[Uid],
				[IsDeleted] ,
				[RemovalDate],
				[Name] ,
				[Description],
				[Type] ,
				[TextData] ,
				[GraphicsData] ,
				[EmployeeUid] )
			VALUES (
				@Uid,
				@IsDeleted,
				@RemovalDate,
				@Name ,
				@Description,
				@Type ,
				@TextData,
				@GraphicsData,
				@EmployeeUid)
		END
END

GO
CREATE PROCEDURE [dbo].[SaveJournal]
	@Uid [uniqueidentifier],
	@SysemDate [datetime] = NULL,
	@DeviceDate [datetime] = NULL,
	@Name [nvarchar](50) = NULL,
	@Description [nvarchar](max) = NULL,
	@DeviceNo [int] = NULL,
	@IpPort [nvarchar](50) = NULL,
	@CardNo [int] = NULL	
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Journal] WHERE Uid = @Uid)
		UPDATE [dbo].[Journal] SET 
			[Uid] = @Uid,
			[SysemDate] = @SysemDate,
			[DeviceDate] = @DeviceDate,
			[Name]= @Name,
			[Description] = @Description,
			[DeviceNo] = @DeviceNo,
			[IpPort] = @IpPort,
			[CardNo] = @CardNo
		WHERE Uid = @Uid
	ELSE
		BEGIN
			INSERT INTO [dbo].[Journal] (
				[Uid],
				[SysemDate],
				[DeviceDate],
				[Name],
				[Description],
				[DeviceNo],
				[IpPort],
				[CardNo])
			VALUES (
				@Uid,
				@SysemDate,
				@DeviceDate,
				@Name,
				@Description,
				@DeviceNo,
				@IpPort,
				@CardNo)
		END
END

	