USE [Firesec]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetAllEmployees]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetAllEmployees]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteEmployee]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteEmployee]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetEmployeeCard]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetEmployeeCard]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveEmployeeCard]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveEmployeeCard]
GO

IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetPositions]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetPositions]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeletePosition]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeletePosition]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SavePosition]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SavePosition]
GO

IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetGroups]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetGroups]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteGroup]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveGroup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveGroup]
GO

IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetDepartments]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetDepartments]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteDepartment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteDepartment]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[SaveDepartment]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveDepartment]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAllEmployees]
	@ClockNumber nvarchar(max) = NULL,
	@LastName nvarchar(max)= NULL,
	@FirstName nvarchar(max)= NULL,
	@SecondName nvarchar(max)= NULL,
	@Group nvarchar(max)= NULL,
	@Department nvarchar(max)= NULL,
	@Position nvarchar(max)= NULL
AS
BEGIN
	SELECT 
		e.Id,
		e.ClockNumber,
		p.LastName,
		p.FirstName,
		p.SecondName,
		g.Value AS [Group],
		d.Value AS Department,
		pos.Value AS Position
	FROM 
		[dbo].[Employee] e
		INNER JOIN [dbo].[Person] p ON e.PersonId = p.Id
		LEFT OUTER JOIN [dbo].[Group] g ON e.GroupId = g.Id
		LEFT OUTER JOIN [dbo].[Department] d ON e.DepartmentId = d.Id
		LEFT OUTER JOIN [dbo].[Position] pos ON e.PositionId = pos.Id
	WHERE
		e.Deleted = 0 AND
		(@ClockNumber IS NULL	OR e.ClockNumber LIKE '%' + @ClockNumber + '%') AND
		(@LastName IS NULL		OR p.LastName LIKE @LastName + '%') AND
		(@FirstName IS NULL		OR p.FirstName LIKE @FirstName + '%') AND
		(@SecondName IS NULL	OR p.SecondName LIKE @SecondName + '%') AND
		(@Group IS NULL			OR g.Value LIKE @Group + '%') AND
		(@Department IS NULL	OR d.Value LIKE @Department + '%') AND
		(@Position IS NULL		OR pos.Value LIKE @Position + '%') 
	ORDER BY
		p.LastName,
		p.FirstName,
		p.SecondName
END
GO
CREATE PROCEDURE [dbo].[DeleteEmployee]
	@Id int
AS
BEGIN
	UPDATE dbo.Employee SET Deleted = 1 WHERE Id = @Id
	RETURN @@ROWCOUNT
END
GO
CREATE PROCEDURE [dbo].[GetEmployeeCard]
	@Id int
AS
BEGIN
	SELECT 
		e.Id,
		e.ClockNumber,
		e.Comment,
		e.DepartmentId,
		e.Email,
		e.GroupId,
		e.Phone,
		e.PositionId,
		p.Address,
		p.AddressFact,
		p.BirthPlace,
		p.Birthday,
		p.Cell,
		p.FirstName,
		p.ITN,
		p.LastName,
		p.PassportCode,
		p.PassportDate,
		p.PassportEmitter,
		p.PassportNumber,
		p.PassportSerial,
		p.Photo,
		p.SNILS,
		p.SecondName,
		p.SexId
	FROM 
		[dbo].[Employee] e
		INNER JOIN [dbo].[Person] p ON e.PersonId = p.Id
	WHERE
		e.Deleted = 0 AND e.Id = @id
		
	SELECT * FROM [dbo].[Department]
	SELECT * FROM [dbo].[Sex]
	SELECT * FROM [dbo].[Position] 
	SELECT * FROM [dbo].[Group] 
END
GO
CREATE PROCEDURE [dbo].[SaveEmployeeCard]
	@Id int = NULL OUTPUT,
	@LastName nvarchar(max)= NULL,
	@FirstName nvarchar(max) = NULL,
	@SecondName nvarchar(max) = NULL,
	@ClockNumber nvarchar(max) = NULL,
	@Comment nvarchar(max) = NULL,
	@DepartmentId int = NULL,
	@Email nvarchar(max) = NULL,
	@GroupId int = NULL,
	@Phone nvarchar(max) = NULL,
	@PositionId int = NULL,
	@Address nvarchar(max) = NULL,
	@AddressFact nvarchar(max) = NULL,
	@BirthPlace nvarchar(max) = NULL,
	@Birthday datetime = NULL,
	@Cell nvarchar(max) = NULL,
	@ITN nvarchar(max) = NULL,
	@PassportCode nvarchar(max) = NULL,
	@PassportDate datetime = NULL,
	@PassportEmitter nvarchar(max) = NULL,
	@PassportNumber nvarchar(max) = NULL,
	@PassportSerial nvarchar(max) = NULL,
	@Photo varbinary(max) = NULL,
	@SNILS nvarchar(max) = NULL,
	@SexId int = NULL
AS
BEGIN
	DECLARE @PersonId int
	SELECT @PersonId = PersonId FROM [dbo].[Employee] WHERE [Id] = @Id
	
	IF ((@Id IS NOT NULL) AND (@PersonId IS NOT NULL))
		BEGIN
			UPDATE [dbo].[Person] SET 
				LastName = @LastName,
				FirstName = @FirstName,
				SecondName = @SecondName,
				Address = @Address,
				AddressFact = @AddressFact,
				BirthPlace = @BirthPlace,
				Birthday = @Birthday,
				Cell = @Cell,
				ITN = @ITN,
				PassportCode = @PassportCode,
				PassportDate = @PassportDate,
				PassportEmitter = @PassportEmitter,
				PassportNumber = @PassportNumber,
				PassportSerial = @PassportSerial,
				Photo = @Photo,
				SNILS = @SNILS,
				SexId = @SexId	
			WHERE Id = @PersonId
			UPDATE [dbo].[Employee] SET 
				ClockNumber = @ClockNumber,
				Comment = @Comment,
				DepartmentId = @DepartmentId,
				Email = @Email,
				GroupId = @GroupId,
				Phone = @Phone,
				PositionId = @PositionId
			WHERE Id = @Id
		END
	ELSE
		BEGIN
			INSERT INTO [dbo].[Person] 
				(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
			VALUES 
				(@LastName,@FirstName,@SecondName,@Photo,@Address,@AddressFact,@BirthPlace,@Birthday,@Cell,@ITN,@PassportCode,@PassportDate,@PassportEmitter,@PassportNumber,@PassportSerial,@SNILS,@SexId);
			INSERT INTO [dbo].[Employee] 
				(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
			VALUES 
				(SCOPE_IDENTITY(),@ClockNumber,@Comment,@DepartmentId,@Email,@GroupId,@Phone,@PositionId,0);
			SET @Id = SCOPE_IDENTITY()
		END
END
GO

CREATE PROCEDURE [dbo].[GetPositions]
AS
BEGIN
	SELECT 
		Id, 
		Value
	FROM 
		[dbo].[Position] 
	ORDER BY
		Value
END
GO
CREATE PROCEDURE [dbo].[DeletePosition]
	@Id int
AS
BEGIN
	DELETE FROM dbo.Position WHERE Id = @Id
	RETURN @@ROWCOUNT
END
GO
CREATE PROCEDURE [dbo].[SavePosition]
	@Id int = NULL OUTPUT,
	@Value nvarchar(max)= NULL
AS
BEGIN
	IF EXISTS(SELECT Id FROM [dbo].[Position] WHERE [Id] = @Id)
		UPDATE [dbo].[Position] SET Value = @Value WHERE Id = @Id
	ELSE
		BEGIN
			INSERT INTO [dbo].[Position] (Value) VALUES (@Value)
			SET @Id = SCOPE_IDENTITY()
		END
END
GO

CREATE PROCEDURE [dbo].[GetGroups]
AS
BEGIN
	SELECT 
		Id, 
		Value
	FROM 
		[dbo].[Group] 
	ORDER BY
		Value
END
GO
CREATE PROCEDURE [dbo].[DeleteGroup]
	@Id int
AS
BEGIN
	DELETE FROM [dbo].[Group] WHERE Id = @Id
	RETURN @@ROWCOUNT
END
GO
CREATE PROCEDURE [dbo].[SaveGroup]
	@Id int = NULL OUTPUT,
	@Value nvarchar(max)= NULL
AS
BEGIN
	IF EXISTS(SELECT Id FROM [dbo].[Group] WHERE [Id] = @Id)
		UPDATE [dbo].[Group] SET Value = @Value WHERE Id = @Id
	ELSE
		BEGIN
			INSERT INTO [dbo].[Group] (Value) VALUES (@Value)
			SET @Id = SCOPE_IDENTITY()
		END
END
GO

CREATE PROCEDURE [dbo].[GetDepartments]
AS
BEGIN
	SELECT 
		Id, 
		DepartmentId,
		Value
	FROM 
		[dbo].[Department]
	ORDER BY
		DepartmentId,
		Value
END
GO
CREATE PROCEDURE [dbo].[DeleteDepartment]
	@Id int
AS
BEGIN
	UPDATE dbo.Department SET DepartmentId = NULL WHERE DepartmentId = @Id
	DELETE FROM dbo.Department WHERE Id = @Id
	RETURN @@ROWCOUNT
END
GO
CREATE PROCEDURE [dbo].[SaveDepartment]
	@Id int = NULL OUTPUT,
	@DepartmentId int = NULL,
	@Value nvarchar(max)= NULL
AS
BEGIN
	IF EXISTS(SELECT Id FROM [dbo].[Department] WHERE [Id] = @Id)
		UPDATE [dbo].[Department] SET Value = @Value, DepartmentId = @DepartmentId WHERE Id = @Id
	ELSE
		BEGIN
			INSERT INTO [dbo].[Department] (DepartmentId, Value) VALUES (@DepartmentId, @Value)
			SET @Id = SCOPE_IDENTITY()
		END
END
GO
