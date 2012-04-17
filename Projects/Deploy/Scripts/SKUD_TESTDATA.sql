USE [Firesec]
GO

DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[Person];
GO

INSERT INTO [dbo].[Person] VALUES ('Иванов','Иван','Иваныч','12-21-75','12-21-05','Вано');
INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), 'ОТК','Бугор','Comment1', 0);
INSERT INTO [dbo].[Person] VALUES ('Петров','Петр','Петрович','03-18-86','12-21-05','Петька');
INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), 'Охрана','Сесурити','Comment2', 0);
INSERT INTO [dbo].[Person] VALUES ('Сидоров','Сидор','Сидорович',NULL,'12-21-05','Сид');
INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), 'Директорат','Генеральный','Comment3', 0);

GO