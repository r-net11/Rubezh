USE [Firesec]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedDay]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedDay]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedHoliday]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedHoliday]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedInterval]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedInterval]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedNamedInterval]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedNamedInterval]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedSchedule]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedSchedule]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedScheduleScheme]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedScheduleScheme]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedRegisterDevice]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedRegisterDevice]

GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedEmployee]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedEmployee]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedDepartment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedDepartment]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedEmployeeReplacement]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedEmployeeReplacement]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedPhone]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedPhone]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedDocument]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedDocument]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedPosition]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedPosition]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[MarkDeletedAdditionalColumn]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MarkDeletedAdditionalColumn]
GO
CREATE PROCEDURE [dbo].[MarkDeletedHoliday]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Holiday] WHERE Uid = @Uid)
		UPDATE [dbo].[Holiday] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedSchedule]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Schedule] WHERE Uid = @Uid)
		UPDATE [dbo].[Schedule] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedScheduleScheme]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[ScheduleScheme] WHERE Uid = @Uid)
		UPDATE [dbo].[ScheduleScheme] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedRegisterDevice]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[RegisterDevice] WHERE Uid = @Uid)
		UPDATE [dbo].[RegisterDevice] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedDay]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Day] WHERE Uid = @Uid)
		UPDATE [dbo].[Day] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedNamedInterval]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[NamedInterval] WHERE Uid = @Uid)
		UPDATE [dbo].[NamedInterval] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedInterval]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Interval] WHERE Uid = @Uid)
		UPDATE [dbo].[Interval] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END

GO
CREATE PROCEDURE [dbo].[MarkDeletedEmployee]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Employee] WHERE Uid = @Uid)
		UPDATE [dbo].[Employee] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedDepartment]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Department] WHERE Uid = @Uid)
		UPDATE [dbo].[Department] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedEmployeeReplacement]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[EmployeeReplacement] WHERE Uid = @Uid)
		UPDATE [dbo].[EmployeeReplacement] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedPhone]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Phone] WHERE Uid = @Uid)
		UPDATE [dbo].[Phone] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedDocument]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Document] WHERE Uid = @Uid)
		UPDATE [dbo].[Document] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedPosition]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[Position] WHERE Uid = @Uid)
		UPDATE [dbo].[Position] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[MarkDeletedAdditionalColumn]
	@Uid uniqueidentifier
AS
BEGIN
	IF EXISTS(SELECT Uid FROM [dbo].[AdditionalColumn] WHERE Uid = @Uid)
		UPDATE [dbo].[AdditionalColumn] SET 
			IsDeleted = 1,
			RemovalDate = GETDATE()
		WHERE Uid = @Uid
END

