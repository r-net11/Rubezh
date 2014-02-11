USE [SKUD]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetDays]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetDays]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetHolidays]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetHolidays]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetIntervals]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetIntervals]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetNamedIntervals]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetNamedIntervals]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetSchedules]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetSchedules]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetScheduleSchemes]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetScheduleSchemes]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetRegisterDevices]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetRegisterDevices]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetEmployees]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetEmployees]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetDepartments]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetDepartments]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetEmployeeReplacements]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetEmployeeReplacements]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetPhones]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetPhones]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetDocuments]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetDocuments]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetPositions]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetPositions]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetAdditionalColumns]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetAdditionalColumns]

GO
CREATE PROCEDURE [dbo].[GetDays]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[Day]
END
GO
CREATE PROCEDURE [dbo].[GetHolidays]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[Holiday]
END
GO
CREATE PROCEDURE [dbo].[GetIntervals]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[Interval]
END
GO
CREATE PROCEDURE [dbo].[GetNamedIntervals]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[NamedInterval]
END
GO
CREATE PROCEDURE [dbo].[GetRegisterDevices]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[RegisterDevice]
END
GO
CREATE PROCEDURE [dbo].[GetSchedules]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[Schedule]
END
GO
CREATE PROCEDURE [dbo].[GetScheduleSchemes]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[ScheduleScheme]
END
GO
CREATE PROCEDURE [dbo].[GetEmployees]
AS
BEGIN
	SELECT 
		e.FirstName,
		p.Name
	FROM 
		[dbo].[Employee] e
	JOIN [dbo].[Position] p
	ON e.PositionUid = p.Uid
END
GO
CREATE PROCEDURE [dbo].[GetDepartments]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[Department]
END
GO
CREATE PROCEDURE [dbo].[GetEmployeeReplacements]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[EmployeeReplacement]
END
GO
CREATE PROCEDURE [dbo].[GetPhones]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[Phone]
END
GO
CREATE PROCEDURE [dbo].[GetDocuments]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[Document]
END
GO
CREATE PROCEDURE [dbo].[GetPositions]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[Position]
END
GO
CREATE PROCEDURE [dbo].[GetAdditionalColumns]
AS
BEGIN
	SELECT 
		*
	FROM 
		[dbo].[AdditionalColumn]
END