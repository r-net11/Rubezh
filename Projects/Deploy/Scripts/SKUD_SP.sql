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
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllEmployees]
AS
BEGIN
	SELECT 
		e.Id,
		e.PersonId,
		p.LastName,
		p.FirstName,
		p.SecondName,
		DateDiff (Year, p.Birthday, getdate()) AS Age,
		e.Department,
		e.Position,
		e.Comment
	FROM 
		[dbo].[Employee] e
		INNER JOIN [dbo].[Person] p ON e.PersonId = p.Id
	WHERE
		e.Deleted = 0
	ORDER BY
		p.LastName,
		p.FirstName,
		p.SecondName
END
GO

CREATE PROCEDURE [dbo].[DeleteEmployee]
	@Id int,
	@Count int OUTPUT
AS
BEGIN
	UPDATE dbo.Employee SET Deleted = 1 WHERE Id = @Id
	SET @Count = @@ROWCOUNT
END
GO

CREATE PROCEDURE [dbo].[GetEmployeeCard]
	@Id int
AS
BEGIN
	SELECT 
		e.Id,
		e.PersonId,
		p.LastName,
		p.FirstName,
		p.SecondName,
		DateDiff (Year, p.Birthday, getdate()) AS Age,
		e.Department,
		e.Position,
		e.Comment
	FROM 
		[dbo].[Employee] e
		INNER JOIN [dbo].[Person] p ON e.PersonId = p.Id
	WHERE
		e.Deleted = 0 AND e.Id = @id
END
GO
CREATE PROCEDURE [dbo].[SaveEmployeeCard]
	@Id int = NULL OUTPUT,
	@LastName nvarchar(max)= NULL,
	@FirstName nvarchar(max) = NULL,
	@SecondName nvarchar(max) = NULL,
	@Birthday datetime = NULL,
	@Sex datetime = NULL,
	@Department nvarchar(max) = NULL,
	@Position nvarchar(max) = NULL,
	@Comment nvarchar(max) = NULL
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
				Birthday = @Birthday,
				Sex = @Sex
			WHERE Id = @PersonId
			UPDATE [dbo].[Employee] SET 
				Department = @Department,
				Position = @Position,
				Comment = @Comment
			WHERE Id = @Id
		END
	ELSE
		BEGIN
			INSERT INTO [dbo].[Person] VALUES (@LastName,@FirstName,@SecondName,@Birthday,@Sex,NULL);
			INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), @Department,@Position,@Comment, 0);
			SET @Id = SCOPE_IDENTITY()
		END
END
GO

