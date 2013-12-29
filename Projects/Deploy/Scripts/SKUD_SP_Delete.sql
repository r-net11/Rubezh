USE [Firesec]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteDay]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteDay]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteHoliday]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteHoliday]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteInterval]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteInterval]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteNamedInterval]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteNamedInterval]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteSchedule]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteSchedule]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteScheduleScheme]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteScheduleScheme]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteRegisterDevice]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteRegisterDevice]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteEmployee]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteEmployee]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteDepartment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteDepartment]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteEmployeeReplacement]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteEmployeeReplacement]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeletePhone]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeletePhone]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteDocument]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteDocument]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeletePosition]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeletePosition]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteAdditionalColumn]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteAdditionalColumn]
GO
CREATE PROCEDURE [dbo].[DeleteHoliday]
	@Uid uniqueidentifier
AS
BEGIN
	DELETE FROM [dbo].[Holiday] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteSchedule]
	@Uid uniqueidentifier
AS
BEGIN
	UPDATE [dbo].[RegisterDevice] SET ScheduleUid = NULL WHERE ScheduleUid = @Uid
	UPDATE [dbo].[Employee] SET ScheduleUid = NULL WHERE ScheduleUid = @Uid
	UPDATE [dbo].[EmployeeReplacement] SET ScheduleUid = NULL WHERE ScheduleUid = @Uid
	DELETE FROM [dbo].[Schedule] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteRegisterDevice]
	@Uid uniqueidentifier
AS
BEGIN
	DELETE FROM [dbo].[RegisterDevice] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteScheduleScheme]
	@Uid uniqueidentifier
AS
BEGIN
	UPDATE [dbo].[Schedule] SET ScheduleSchemeUid = NULL WHERE ScheduleSchemeUid = @Uid
	UPDATE [dbo].[Day] SET ScheduleSchemeUid = NULL WHERE ScheduleSchemeUid = @Uid
	DELETE FROM [dbo].[ScheduleScheme] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteDay]
	@Uid uniqueidentifier
AS
BEGIN
	DELETE FROM [dbo].[Day] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteNamedInterval]
	@Uid uniqueidentifier
AS
BEGIN
	UPDATE [dbo].[Interval] SET NamedIntervalUid = NULL WHERE NamedIntervalUid = @Uid
	UPDATE [dbo].[Day] SET NamedIntervalUid = NULL WHERE NamedIntervalUid = @Uid
	DELETE FROM [dbo].[NamedInterval] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteInterval]
	@Uid uniqueidentifier
AS
BEGIN
	DELETE FROM [dbo].[Interval] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteEmployee]
	@Uid uniqueidentifier
AS
BEGIN
	UPDATE [dbo].[AdditionalColumn] SET EmployeeUid = NULL WHERE EmployeeUid = @Uid
	UPDATE [dbo].[EmployeeReplacement] SET EmployeeUid = NULL WHERE EmployeeUid = @Uid
	UPDATE [dbo].[Department] SET ContactEmployeeUid = NULL WHERE ContactEmployeeUid = @Uid	
	UPDATE [dbo].[Department] SET AttendantUid = NULL WHERE AttendantUid = @Uid
	DELETE FROM [dbo].[Employee] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteDepartment]
	@Uid uniqueidentifier 
AS
BEGIN
	--UPDATE [dbo].[Department] SET ParentDepartmentUid = @Uid --WHERE ParentDepartmentUid = @Uid
	UPDATE [dbo].[Employee] SET DepartmentUid = NULL WHERE DepartmentUid = @Uid
	UPDATE [dbo].[EmployeeReplacement] SET DepartmentUid = NULL WHERE DepartmentUid = @Uid
	UPDATE [dbo].[Phone] SET DepartmentUid = NULL WHERE DepartmentUid = @Uid
	DELETE FROM [dbo].[Department] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteEmployeeReplacement]
	@Uid uniqueidentifier
AS
BEGIN
	DELETE FROM [dbo].[EmployeeReplacement] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeletePhone]
	@Uid uniqueidentifier
AS
BEGIN
	DELETE FROM [dbo].[Phone] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteDocument]
	@Uid uniqueidentifier
AS
BEGIN
	DELETE FROM [dbo].[Document] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeletePosition]
	@Uid uniqueidentifier
AS
BEGIN
	UPDATE [dbo].[Employee] SET PositionUid = NULL WHERE PositionUid = @Uid
	DELETE FROM [dbo].[Position] WHERE Uid = @Uid
END
GO
CREATE PROCEDURE [dbo].[DeleteAdditionalColumn]
	@Uid uniqueidentifier
AS
BEGIN
	DELETE FROM [dbo].[AdditionalColumn] WHERE Uid = @Uid
END