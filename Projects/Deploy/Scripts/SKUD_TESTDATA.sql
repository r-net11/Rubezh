USE [Firesec]
GO

DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[Person];
GO

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('Иванов','Иван','Иваныч',NULL,NULL,NULL,NULL,'12-21-75',NULL,'777',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'1',NULL,NULL,NULL,NULL,NULL,NULL,0);

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('Петров','Петр','Петрович',NULL,NULL,NULL,NULL,'03-18-86',NULL,'666',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'2',NULL,NULL,NULL,NULL,NULL,NULL,0);

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('Сидоров','Сидор','Сидорович',NULL,NULL,NULL,NULL,'12-21-05',NULL,'999',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'3',NULL,NULL,NULL,NULL,NULL,NULL,0);

--INSERT INTO [dbo].[Person] VALUES ('Иванов','Иван','Иваныч','12-21-75','12-21-05','Вано');
--INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), 'ОТК','Бугор','Comment1', 0);
--INSERT INTO [dbo].[Person] VALUES ('Петров','Петр','Петрович','03-18-86','12-21-05','Петька');
--INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), 'Охрана','Сесурити','Comment2', 0);
--INSERT INTO [dbo].[Person] VALUES ('Сидоров','Сидор','Сидорович',NULL,'12-21-05','Сид');
--INSERT INTO [dbo].[Employee] VALUES (SCOPE_IDENTITY(), 'Директорат','Генеральный','Comment3', 0);

GO