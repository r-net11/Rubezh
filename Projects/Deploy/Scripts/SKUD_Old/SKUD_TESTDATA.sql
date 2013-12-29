USE [Firesec]
GO

DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[Person];
DELETE FROM [dbo].[Position];
DELETE FROM [dbo].[Group];
DELETE FROM [dbo].[Department];
GO

INSERT INTO [dbo].[Position] (Value) VALUES ('Сесурити')
INSERT INTO [dbo].[Position] (Value) VALUES ('Директор')
INSERT INTO [dbo].[Position] (Value) VALUES ('Бугор')
INSERT INTO [dbo].[Position] (Value) VALUES ('КлинМенеджер')
INSERT INTO [dbo].[Position] (Value) VALUES ('Администратор')
INSERT INTO [dbo].[Position] (Value) VALUES ('Дворник')
GO

INSERT INTO [dbo].[Group] (Value) VALUES ('Сотрудник')
INSERT INTO [dbo].[Group] (Value) VALUES ('Непонятно кто')
INSERT INTO [dbo].[Group] (Value) VALUES ('Совместитель')
INSERT INTO [dbo].[Group] (Value) VALUES ('Руководство')
GO

INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('Руководство', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('Служба маркетинга', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('Основное производство', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('Вспомогательное производство', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('Служба безопасности', NULL) 
INSERT INTO [dbo].[Department] (Value, DepartmentId) VALUES ('Транспортный цех', NULL) 
GO

DECLARE @DepId int

SELECT @DepId = MIN(Id) FROM [dbo].[Department]
INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('Иванов','Иван','Иваныч',NULL,NULL,NULL,NULL,'01-01-01',NULL,'777',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'1',NULL,@DepId,NULL,NULL,NULL,NULL,0);

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('Петров','Петр','Петрович',NULL,NULL,NULL,NULL,'03-12-18',NULL,'666',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'2',NULL,@DepId,NULL,NULL,NULL,NULL,0);

SELECT @DepId = MAX(Id) FROM [dbo].[Department]
INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('Сидоров','Иван','Петрович',NULL,NULL,NULL,NULL,'12-12-05',NULL,'999',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO [dbo].[Employee] 
	(PersonId, ClockNumber, Comment, DepartmentId, Email, GroupId, Phone, PositionId, Deleted)
VALUES 
	(SCOPE_IDENTITY(),'3',NULL,@DepId,NULL,NULL,NULL,NULL,0);

INSERT INTO [dbo].[Person] 
	(LastName, FirstName, SecondName, Photo, Address, AddressFact, BirthPlace, Birthday, Cell, ITN, PassportCode, PassportDate, PassportEmitter, PassportNumber, PassportSerial, SNILS, SexId)
VALUES 
	('Сидоров','Сидор','Сидорович',NULL,NULL,NULL,NULL,'12-12-05',NULL,'999',NULL,NULL,NULL,NULL,NULL,NULL,NULL);
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