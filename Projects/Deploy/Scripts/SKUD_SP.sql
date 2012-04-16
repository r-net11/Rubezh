USE [Firesec]
GO
IF EXISTS (SELECT * FROM [dbo].[sysobjects] where id = object_id(N'[dbo].[GetAllEmployees]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[GetAllEmployees]
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
		DateDiff (Year, p.Birthday, getdate()) as Age,
		e.Department,
		e.Position,
		e.Comment
	FROM 
		[dbo].[Employee] e
		INNER JOIN [dbo].[Person] p on e.PersonId = p.Id
END
GO

-- FOR TEST ONLY - DEBUG
DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[Person];
GO
INSERT INTO [dbo].[Person] values ('Иванов','Иван','Иваныч','12-21-75','12-21-05','Вано');
INSERT INTO [dbo].[Employee] values (SCOPE_IDENTITY(), 'ОТК','Бугор','Comment1');
INSERT INTO [dbo].[Person] values ('Петров','Петр','Петрович','03-18-86','12-21-05','Петька');
INSERT INTO [dbo].[Employee] values (SCOPE_IDENTITY(), 'Охрана','Сесурити','Comment2');
INSERT INTO [dbo].[Person] values ('Сидоров','Сидор','Сидорович',NULL,'12-21-05','Сид');
INSERT INTO [dbo].[Employee] values (SCOPE_IDENTITY(), 'Директорат','Генеральный','Comment3');