USE [Firesec]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[GetAllEmployees]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetAllEmployees]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] WHERE id = object_id(N'[dbo].[DeleteEmployee]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetAllEmployees]
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
END
GO

CREATE PROCEDURE [dbo].[DeleteEmployee]
	@Id int
AS
BEGIN
	UPDATE dbo.Employee SET Deleted = 1 WHERE Id = @Id
END
GO

