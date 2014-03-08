USE [SKUD]
SET DATEFORMAT dmy;
DECLARE @Uid uniqueidentifier;

DELETE FROM Organization
delete from [dbo].[Holiday]
delete from [dbo].[Document]
delete from [dbo].[Interval]
delete from [dbo].[NamedInterval]
delete from [dbo].[Day]
delete from [dbo].[ScheduleScheme]
delete from [dbo].[Schedule]
delete from [dbo].[Position]
delete from [dbo].[Employee]
delete from [dbo].[Department]


DECLARE @Organization1Uid uniqueidentifier;
SET @Organization1Uid = NEWID();
EXEC SaveOrganization @Organization1Uid, 'СКУД', 'ООО СКУДЪ',0,'01/01/1900'

SET @Uid = NEWID(); 
EXEC [dbo].[SaveHoliday] @Uid, @Organization1Uid, 'Новый год', 2, '31/12/2013', '28/12/2013',NULL,0,'01/01/1900' 
SET @Uid = NEWID(); 
EXEC [dbo].[SaveHoliday] @Uid, @Organization1Uid, '8 марта', 0, '08/03/2014',NULL,NULL,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveHoliday] @Uid, @Organization1Uid, 'Старый Новый год', 1, '13/01/2014', NULL, 2,0,'01/01/1900'

SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, @Organization1Uid, 123, 'Документ1', 'Документ1Организации1', '01/01/2013', '07/01/2013',0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, @Organization1Uid, 258, 'Документ2', 'Документ2Организации1', '08/01/2014', '25/01/2013',0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, @Organization1Uid, 753, 'Документ3', 'Документ3Организации1', '30/01/2014', '05/02/2013',0,'01/01/1900'

--ОХРАНА
DECLARE @GuardNamedIntervalUid uniqueidentifier;
SET @GuardNamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @GuardNamedIntervalUid, @Organization1Uid, 'Охрана',0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '08:00', '13:00', 0, @GuardNamedIntervalUid,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '13:45', '17:45', 0, @GuardNamedIntervalUid,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '18:30', '22:30', 0, @GuardNamedIntervalUid,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '23:15', '03:15', 2, @GuardNamedIntervalUid,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '04:00', '08:00', 1, @GuardNamedIntervalUid,0,'01/01/1900'
DECLARE @GuardScheduleSchemeUid uniqueidentifier;
SET @GuardScheduleSchemeUid = NEWID();
EXEC [dbo].[SaveScheduleScheme] @GuardScheduleSchemeUid, @Organization1Uid, 'Охрана', 1, 4,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @GuardNamedIntervalUid, @GuardScheduleSchemeUid, 0,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, NULL, @GuardScheduleSchemeUid, 1,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, NULL, @GuardScheduleSchemeUid, 2,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, NULL, @GuardScheduleSchemeUid, 3,0,'01/01/1900'
--МОНТАЖ
DECLARE @Montage1NamedIntervalUid uniqueidentifier;
SET @Montage1NamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @Montage1NamedIntervalUid, @Organization1Uid, 'Монтажный1',0,'01/01/1900'
DECLARE @Montage2NamedIntervalUid uniqueidentifier;
SET @Montage2NamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @Montage2NamedIntervalUid, @Organization1Uid, 'Монтажный2',0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '07:30', '11:00', 0, @Montage1NamedIntervalUid,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '11:30', '16:00', 0, @Montage1NamedIntervalUid,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '08:00', '11:30', 0, @Montage2NamedIntervalUid,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '12:00', '16:30', 0, @Montage2NamedIntervalUid,0,'01/01/1900'
DECLARE @MontageScheduleSchemeUid uniqueidentifier;
SET @MontageScheduleSchemeUid = NEWID();
EXEC [dbo].[SaveScheduleScheme] @MontageScheduleSchemeUid, @Organization1Uid, 'Монтаж', 0, 7,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @Montage1NamedIntervalUid, @MontageScheduleSchemeUid, 0,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @Montage2NamedIntervalUid, @MontageScheduleSchemeUid, 1,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @Montage1NamedIntervalUid, @MontageScheduleSchemeUid, 2,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @Montage2NamedIntervalUid, @MontageScheduleSchemeUid, 3,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @Montage1NamedIntervalUid, @MontageScheduleSchemeUid, 4,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, NULL, @MontageScheduleSchemeUid, 5,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, NULL, @MontageScheduleSchemeUid, 6,0,'01/01/1900'
--ПЯТИДНЕВКА
DECLARE @WeeklyNamedIntervalUid uniqueidentifier;
SET @WeeklyNamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @WeeklyNamedIntervalUid, @Organization1Uid, 'Пятидневка',0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '08:00', '12:00', 0, @WeeklyNamedIntervalUid,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '13:00', '17:00', 0, @WeeklyNamedIntervalUid,0,'01/01/1900'
DECLARE @WeeklyScheduleSchemeUid uniqueidentifier;
SET @WeeklyScheduleSchemeUid = NEWID();
EXEC [dbo].[SaveScheduleScheme] @WeeklyScheduleSchemeUid, @Organization1Uid, 'Пятидневка', 0, 7,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 0,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 1,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 2,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 3,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 4,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, NULL, @WeeklyScheduleSchemeUid, 5,0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Organization1Uid, NULL, @WeeklyScheduleSchemeUid, 6,0,'01/01/1900'


DECLARE @GuardScheduleUid uniqueidentifier;
SET @GuardScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @GuardScheduleUid, @Organization1Uid, 'Охрана', @GuardScheduleSchemeUid,0,'01/01/1900'
DECLARE @MontageScheduleUid uniqueidentifier;
SET @MontageScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @MontageScheduleUid, @Organization1Uid, 'Монтаж', @MontageScheduleSchemeUid,0,'01/01/1900' 
DECLARE @ITScheduleUid uniqueidentifier;
SET @ITScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @ITScheduleUid , @Organization1Uid, 'IT', @WeeklyScheduleSchemeUid,0,'01/01/1900' 
DECLARE @ConstructorshipScheduleUid uniqueidentifier;
SET @ConstructorshipScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @ConstructorshipScheduleUid , @Organization1Uid, 'Конструкторы', @WeeklyScheduleSchemeUid,0,'01/01/1900'  
DECLARE @GovernanceScheduleUid uniqueidentifier;
SET @GovernanceScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @GovernanceScheduleUid , @Organization1Uid, 'Руководство', @WeeklyScheduleSchemeUid,0,'01/01/1900'


DECLARE @GuardPositionUid uniqueidentifier;
SET @GuardPositionUid = NEWID();
EXEC [dbo].[SavePosition] @GuardPositionUid, @Organization1Uid, 'Охранник', 'Охранопроизводитель',0,'01/01/1900'
DECLARE @MainGuardPositionUid uniqueidentifier;
SET @MainGuardPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainGuardPositionUid , @Organization1Uid, 'Ст. охранник', 'Старший охранопроизводитель',0,'01/01/1900'
DECLARE @MontagePositionUid uniqueidentifier;
SET @MontagePositionUid = NEWID();
EXEC [dbo].[SavePosition] @MontagePositionUid , @Organization1Uid, 'Монтажник', 'Оператор монтажа',0,'01/01/1900'
DECLARE @MainMontagePositionUid uniqueidentifier;
SET @MainMontagePositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainMontagePositionUid , @Organization1Uid, 'Ст. монтажник', 'Старший оператор монтажа',0,'01/01/1900'
DECLARE @ProgrammerPositionUid uniqueidentifier;
SET @ProgrammerPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ProgrammerPositionUid , @Organization1Uid, 'Программист', 'Разработчик алгоритмов для ЭВМ',0,'01/01/1900'
DECLARE @MainProgrammerPositionUid uniqueidentifier;
SET @MainProgrammerPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainProgrammerPositionUid , @Organization1Uid, 'Ст. программист', 'Старший разработчик алгоритмов для ЭВМ',0,'01/01/1900'
DECLARE @TesterPositionUid uniqueidentifier;
SET @TesterPositionUid = NEWID();
EXEC [dbo].[SavePosition] @TesterPositionUid , @Organization1Uid, 'Тестировщик', 'Испытатель алгоритмов для ЭВМ',0,'01/01/1900'
DECLARE @MainTesterPositionUid uniqueidentifier;
SET @MainTesterPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainTesterPositionUid , @Organization1Uid, 'Ст. тестировщик', 'Старший испытатель алгоритмов для ЭВМ',0,'01/01/1900'
DECLARE @ConstructorPositionUid uniqueidentifier;
SET @ConstructorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ConstructorPositionUid , @Organization1Uid, 'Конструктор', 'Инженер-конструктор',0,'01/01/1900'
DECLARE @MainConstructorPositionUid uniqueidentifier;
SET @MainConstructorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainConstructorPositionUid , @Organization1Uid, 'Ст. конструктор', 'Старший инженер-конструктор',0,'01/01/1900'
DECLARE @ProgrammistConstructorPositionUid uniqueidentifier;
SET @ProgrammistConstructorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ProgrammistConstructorPositionUid , @Organization1Uid, 'Программист-конструктор', 'Программописец и диковинок выдумщик',0,'01/01/1900'
DECLARE @AdministratorPositionUid uniqueidentifier;
SET @AdministratorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @AdministratorPositionUid , @Organization1Uid, 'Администратор', 'Главный распорядитель',0,'01/01/1900'
DECLARE @DirectorPositionUid uniqueidentifier;
SET @DirectorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @DirectorPositionUid , @Organization1Uid, 'Директор', 'Руководитель компании',0,'01/01/1900'


DECLARE @Guard1EmployeeUid uniqueidentifier;
SET @Guard1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Guard1EmployeeUid , @Organization1Uid, 'Сергей', 'Петрович', 'Иванов', @GuardPositionUid, null , @GuardScheduleUid, '12/05/2005','01/01/1900',0,'01/01/1900'
DECLARE @Guard2EmployeeUid uniqueidentifier;
SET @Guard2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Guard2EmployeeUid , @Organization1Uid, 'Петр', 'Сергеевич', 'Ивановский', @GuardPositionUid, null , @GuardScheduleUid, '12/05/2006','01/01/1900',0,'01/01/1900'
DECLARE @MainGuardEmployeeUid uniqueidentifier;
SET @MainGuardEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainGuardEmployeeUid , @Organization1Uid, 'Петр', 'Сергеевич', 'Ивановичус', @MainGuardPositionUid, null , @GuardScheduleUid, '12/05/2007','01/01/1900',0,'01/01/1900'
DECLARE @Montage1EmployeeUid uniqueidentifier;
SET @Montage1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Montage1EmployeeUid ,  @Organization1Uid,'Иван', 'Сергеевич', 'Петров', @MontagePositionUid, null , @MontageScheduleUid, '12/05/2008','01/01/1900',0,'01/01/1900'
DECLARE @Montage2EmployeeUid uniqueidentifier;
SET @Montage2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Montage2EmployeeUid , @Organization1Uid, 'Сергей', 'Иванович', 'Петровишвили', @MontagePositionUid, null , @MontageScheduleUid, '12/05/2009','01/01/1900',0,'01/01/1900'
DECLARE @MainMontageEmployeeUid uniqueidentifier;
SET @MainMontageEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainMontageEmployeeUid , @Organization1Uid, 'Сергей', 'Сергеевич', 'Петровский', @MainMontagePositionUid, null , @MontageScheduleUid, '12/05/2010','01/01/1900',0,'01/01/1900'
DECLARE @Programmer1EmployeeUid uniqueidentifier;
SET @Programmer1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Programmer1EmployeeUid , @Organization1Uid, 'Петр', 'Иванович', 'Сергеев', @ProgrammerPositionUid, null , @ITScheduleUid, '12/05/2011','01/01/1900',0,'01/01/1900'
DECLARE @Programmer2EmployeeUid uniqueidentifier;
SET @Programmer2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Programmer2EmployeeUid , @Organization1Uid, 'Иван', 'Перович', 'Сергеевич', @ProgrammerPositionUid, null , @ITScheduleUid, '12/05/2012','01/01/1900',0,'01/01/1900'
DECLARE @MainProgrammistEmployeeUid uniqueidentifier;
SET @MainProgrammistEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainProgrammistEmployeeUid , @Organization1Uid, 'Иван', 'Иванович', 'Сергеевко', @MainProgrammerPositionUid, null , @ITScheduleUid, '12/05/2013','01/01/1900',0,'01/01/1900'
DECLARE @Tester1EmployeeUid uniqueidentifier;
SET @Tester1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Tester1EmployeeUid , @Organization1Uid, 'Сидор', 'Прохорович', 'Захарьин', @TesterPositionUid, null , @ITScheduleUid, '12/06/2013','01/01/1900',0,'01/01/1900'
DECLARE @Tester2EmployeeUid uniqueidentifier;
SET @Tester2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Tester1EmployeeUid , @Organization1Uid, 'Прохор', 'Сидорович', 'Захаров', @TesterPositionUid, null , @ITScheduleUid, '12/07/2013','01/01/1900',0,'01/01/1900'
DECLARE @MainTesterEmployeeUid uniqueidentifier;
SET @MainTesterEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainTesterEmployeeUid , @Organization1Uid, 'Прохор', 'Прохорович', 'Захаренко', @MainTesterPositionUid, null , @ITScheduleUid, '12/08/2013','01/01/1900',0,'01/01/1900'
DECLARE @Constructor1EmployeeUid uniqueidentifier;
SET @Constructor1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Constructor1EmployeeUid , @Organization1Uid, 'Захар', 'Сидорович', 'Прохоров', @ConstructorPositionUid, null , @ConstructorshipScheduleUid, '12/09/2013','01/01/1900',0,'01/01/1900'
DECLARE @Constructor2EmployeeUid uniqueidentifier;
SET @Constructor2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Constructor2EmployeeUid , @Organization1Uid, 'Сидор', 'Захарович', 'Прохорский', @ConstructorPositionUid, null , @ConstructorshipScheduleUid, '12/10/2013','01/01/1900',0,'01/01/1900'
DECLARE @MainConstructorEmployeeUid uniqueidentifier;
SET @MainConstructorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainConstructorEmployeeUid , @Organization1Uid, 'Захар', 'Захарович', 'Прохоревич', @MainConstructorPositionUid, null , @ConstructorshipScheduleUid, '12/11/2013','01/01/1900',0,'01/01/1900'
DECLARE @AdministratorEmployeeUid uniqueidentifier;
SET @AdministratorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @AdministratorEmployeeUid , @Organization1Uid, 'Захар', 'Прохорович', 'Сидоров', @AdministratorPositionUid, null , @GovernanceScheduleUid, '12/12/2013','01/01/1900',0,'01/01/1900'
DECLARE @DirectorEmployeeUid uniqueidentifier;
SET @DirectorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @DirectorEmployeeUid , @Organization1Uid, 'Прохор', 'Захарович', 'Сидоренко', @DirectorEmployeeUid , null , @GovernanceScheduleUid, '13/12/2013','01/01/1900',0,'01/01/1900'
DECLARE @ProgrammistConstructorEmployeeUid uniqueidentifier;
SET @ProgrammistConstructorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @ProgrammistConstructorEmployeeUid , @Organization1Uid, 'Миямото', 'Дайтаро', 'Мусащи', @ProgrammistConstructorPositionUid , null , @ConstructorshipScheduleUid, '13/12/2001','01/01/1900',0,'01/01/1900'

DECLARE @MainDepartmentUid uniqueidentifier;
SET @MainDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @MainDepartmentUid , @Organization1Uid, 'ООО "СКУДЪ"', 'Мануфактура купца 3 гильдии Сидоренко', null , @DirectorEmployeeUid, @AdministratorEmployeeUid,0,'01/01/1900'
DECLARE @GovernanceDepartmentUid uniqueidentifier;
SET @GovernanceDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @GovernanceDepartmentUid ,  @Organization1Uid,'Руководство', 'Столоначальники и власть придержащие', @MainDepartmentUid , @AdministratorEmployeeUid, @DirectorEmployeeUid ,0,'01/01/1900'
DECLARE @GuardDepartmentUid uniqueidentifier;
SET @GuardDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @GuardDepartmentUid , @Organization1Uid, 'Охрана', 'Охранители и надсмотрщики', @MainDepartmentUid , @MainGuardEmployeeUid, @Guard1EmployeeUid,0,'01/01/1900' 
DECLARE @MontageDepartmentUid uniqueidentifier;
SET @MontageDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @MontageDepartmentUid , @Organization1Uid, 'Монтаж', 'Собиратели и установщики', @MainDepartmentUid , @MainMontageEmployeeUid, @Montage1EmployeeUid,0,'01/01/1900' 
DECLARE @EngeneeringDepartmentUid uniqueidentifier;
SET @EngeneeringDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @EngeneeringDepartmentUid , @Organization1Uid, 'Инженеры', 'Инженерия и иныя премудрости', @MainDepartmentUid , @ProgrammistConstructorEmployeeUid,NULL,0,'01/01/1900' 
DECLARE @ITDepartmentUid uniqueidentifier;
SET @ITDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @ITDepartmentUid , @Organization1Uid, 'IT', 'Алгоритмописатели и тестировщики', @EngeneeringDepartmentUid , @MainProgrammistEmployeeUid, @MainTesterEmployeeUid,0,'01/01/1900' 
DECLARE @ProgrammerDepartmentUid uniqueidentifier;
SET @ProgrammerDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @ProgrammerDepartmentUid , @Organization1Uid, 'Программисты', 'Алгоритмописатели', @ITDepartmentUid , @MainProgrammistEmployeeUid, @Programmer1EmployeeUid,0,'01/01/1900' 
DECLARE @TesterDepartmentUid uniqueidentifier;
SET @TesterDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @TesterDepartmentUid , @Organization1Uid, 'Тестировщики', 'Кода премудрого проверятели', @ITDepartmentUid , @MainTesterEmployeeUid , @Tester1EmployeeUid,0,'01/01/1900' 
DECLARE @ConstructorshipDepartmentUid uniqueidentifier;
SET @ConstructorshipDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @ConstructorshipDepartmentUid , @Organization1Uid, 'Конструкторы', 'Машин неведомых собиратели', @EngeneeringDepartmentUid , @MainConstructorEmployeeUid , @Constructor1EmployeeUid,0,'01/01/1900'

UPDATE [dbo].[Employee] SET [DepartmentUid]=@GovernanceDepartmentUid WHERE [Uid]=@DirectorEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@GovernanceDepartmentUid WHERE [Uid]=@AdministratorEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@GuardDepartmentUid WHERE [Uid]=@Guard1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@GuardDepartmentUid WHERE [Uid]=@Guard2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@GuardDepartmentUid WHERE [Uid]=@MainGuardEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@MontageDepartmentUid WHERE [Uid]=@Montage1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@MontageDepartmentUid WHERE [Uid]=@Montage2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@MontageDepartmentUid WHERE [Uid]=@MainMontageEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@EngeneeringDepartmentUid WHERE [Uid]=@ProgrammistConstructorEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ProgrammerDepartmentUid WHERE [Uid]=@Programmer1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ProgrammerDepartmentUid WHERE [Uid]=@Programmer2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ProgrammerDepartmentUid WHERE [Uid]=@MainProgrammistEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@TesterDepartmentUid WHERE [Uid]=@Tester1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@TesterDepartmentUid WHERE [Uid]=@Tester2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@TesterDepartmentUid WHERE [Uid]=@MainTesterEmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ConstructorshipDepartmentUid WHERE [Uid]=@Constructor1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ConstructorshipDepartmentUid WHERE [Uid]=@Constructor2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@ConstructorshipDepartmentUid WHERE [Uid]=@MainConstructorEmployeeUid

SET @Uid = NEWID(); 
EXEC [dbo].[SaveGuest] @Uid, @Organization1Uid, 'Дмитрий', 'Анатольевич', 'Медведев',0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveGuest] @Uid, @Organization1Uid, 'Сергей', 'Кожугетович', 'Шойгу',0,'01/01/1900'

DECLARE @Organization2Uid uniqueidentifier;
SET @Organization2Uid = NEWID();
EXEC SaveOrganization @Organization2Uid, 'McDonalds', 'McDonalds Restaurants Inc',0,'01/01/1900'

DECLARE @JanitorPositionUid uniqueidentifier;
SET @JanitorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @JanitorPositionUid, @Organization2Uid, 'Уборщик', 'Уборкопроизводитель',0,'01/01/1900'
DECLARE @KitchenerPositionUid uniqueidentifier;
SET @KitchenerPositionUid = NEWID();
EXEC [dbo].[SavePosition] @KitchenerPositionUid, @Organization2Uid, 'Кухарка', 'Кухонный работник',0,'01/01/1900'
DECLARE @ManagerPositionUid uniqueidentifier;
SET @ManagerPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ManagerPositionUid , @Organization2Uid, 'Менеджер', 'Управляющий',0,'01/01/1900'

DECLARE @Janitor1EmployeeUid uniqueidentifier;
SET @Janitor1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Janitor1EmployeeUid , @Organization2Uid, 'Иван', 'Иванович', 'Уборщиков', @JanitorPositionUid , null , null, '12/05/1995','01/01/1900',0,'01/01/1900'
DECLARE @Janitor2EmployeeUid uniqueidentifier;
SET @Janitor2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Janitor2EmployeeUid , @Organization2Uid, 'Петр', 'Петрович', 'Чистильщиков', @JanitorPositionUid , null , null, '12/05/1996','01/01/1900',0,'01/01/1900'
DECLARE @Janitor3EmployeeUid uniqueidentifier;
SET @Janitor3EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Janitor3EmployeeUid , @Organization2Uid, 'Сергей', 'Сергеевич', 'Мойщиков', @JanitorPositionUid , null , null, '12/05/1997','01/01/1900',0,'01/01/1900'
DECLARE @Janitor4EmployeeUid uniqueidentifier;
SET @Janitor4EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Janitor4EmployeeUid , @Organization2Uid, 'Андрей', 'Андреевич', 'Оттерательщиков', @JanitorPositionUid , null , null, '12/05/1998','01/01/1900',0,'01/01/1900'
DECLARE @Kitchener1EmployeeUid uniqueidentifier;
SET @Kitchener1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Kitchener1EmployeeUid , @Organization2Uid, 'Иван', 'Петрович', 'Сосискин', @KitchenerPositionUid , null , null, '12/05/1995','01/01/1900',0,'01/01/1900'
DECLARE @Kitchener2EmployeeUid uniqueidentifier;
SET @Kitchener2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Kitchener2EmployeeUid , @Organization2Uid, 'Петр', 'Иванович', 'Пельменько', @KitchenerPositionUid , null , null, '12/06/1995','01/01/1900',0,'01/01/1900'
DECLARE @Kitchener3EmployeeUid uniqueidentifier;
SET @Kitchener3EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Kitchener3EmployeeUid , @Organization2Uid, 'Сергей', 'Андреевич', 'Булочкин', @KitchenerPositionUid , null , null, '12/07/1995','01/01/1900',0,'01/01/1900'
DECLARE @Kitchener4EmployeeUid uniqueidentifier;
SET @Kitchener4EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Kitchener4EmployeeUid , @Organization2Uid, 'Андрей', 'Сергеевич', 'Пирожечкин', @KitchenerPositionUid , null , null, '12/08/1995','01/01/1900',0,'01/01/1900'
DECLARE @Manager1EmployeeUid uniqueidentifier;
SET @Manager1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Manager1EmployeeUid , @Organization2Uid, 'Иван', 'Андреевич', 'Начальников', @ManagerPositionUid , null , null, '22/08/1999','01/01/1900',0,'01/01/1900'
DECLARE @Manager2EmployeeUid uniqueidentifier;
SET @Manager2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Manager2EmployeeUid , @Organization2Uid, 'Петр', 'Сергеевич', 'Бригадиркин', @ManagerPositionUid , null , null, '31/12/2000','01/01/1900',0,'01/01/1900'

DECLARE @Restaurant1DepartmentUid uniqueidentifier;
SET @Restaurant1DepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @Restaurant1DepartmentUid , @Organization2Uid, 'ул. Кирова', 'Кирова угол Чапаева', null , @Manager1EmployeeUid, @Kitchener1EmployeeUid,0,'01/01/1900'
DECLARE @Janitors1DepartmentUid uniqueidentifier;
SET @Janitors1DepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @Janitors1DepartmentUid , @Organization2Uid, 'Уборщики', 'Уборщики на Кирова', @Restaurant1DepartmentUid,NULL,NULL,0,'01/01/1900'
DECLARE @Kitcheners1DepartmentUid uniqueidentifier;
SET @Kitcheners1DepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @Kitcheners1DepartmentUid , @Organization2Uid, 'Кухарки', 'Кухарки на Кирова', @Restaurant1DepartmentUid,NULL,NULL,0,'01/01/1900'
DECLARE @Management1DepartmentUid uniqueidentifier;
SET @Management1DepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @Management1DepartmentUid , @Organization2Uid, 'Управление', 'Управление на Кирова', @Restaurant1DepartmentUid,NULL,NULL,0,'01/01/1900'
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Management1DepartmentUid WHERE [Uid]=@Manager1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Kitcheners1DepartmentUid WHERE [Uid]=@Kitchener1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Kitcheners1DepartmentUid WHERE [Uid]=@Kitchener2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Janitors1DepartmentUid WHERE [Uid]=@Janitor1EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Janitors1DepartmentUid WHERE [Uid]=@Janitor2EmployeeUid

DECLARE @Restaurant2DepartmentUid uniqueidentifier;
SET @Restaurant2DepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @Restaurant2DepartmentUid , @Organization2Uid, 'ул. Чернышевского', 'Чернышевского около "Реала"', null , @Manager2EmployeeUid, @Kitchener2EmployeeUid,0,'01/01/1900'
DECLARE @Janitors2DepartmentUid uniqueidentifier;
SET @Janitors2DepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @Janitors2DepartmentUid , @Organization2Uid, 'Уборщики', 'Уборщики на Чернышевского', @Restaurant2DepartmentUid,NULL,NULL,0,'01/01/1900'
DECLARE @Kitcheners2DepartmentUid uniqueidentifier;
SET @Kitcheners2DepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @Kitcheners2DepartmentUid , @Organization2Uid, 'Кухарки', 'Кухарки на Чернышевского', @Restaurant2DepartmentUid,NULL,NULL,0,'01/01/1900'
DECLARE @Management2DepartmentUid uniqueidentifier;
SET @Management2DepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @Management1DepartmentUid , @Organization2Uid, 'Управление', 'Управление на Чернышевского', @Restaurant2DepartmentUid,NULL,NULL,0,'01/01/1900'
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Management2DepartmentUid WHERE [Uid]=@Manager2EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Kitcheners2DepartmentUid WHERE [Uid]=@Kitchener3EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Kitcheners2DepartmentUid WHERE [Uid]=@Kitchener4EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Janitors2DepartmentUid WHERE [Uid]=@Janitor3EmployeeUid
UPDATE [dbo].[Employee] SET [DepartmentUid]=@Janitors2DepartmentUid WHERE [Uid]=@Janitor4EmployeeUid

SET @Uid = NEWID(); 
EXEC [dbo].[SaveGuest] @Uid, @Organization2Uid, 'Владимир', 'Александрович', 'Колокольцев',0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveGuest] @Uid, @Organization2Uid, 'Рашид', 'Гумарович', 'Нургалиев',0,'01/01/1900'

SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, @Organization2Uid, 486, 'Документ1', 'Документ1Организации2', '01/01/2013', '07/01/2013',0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, @Organization2Uid, 729, 'Документ2', 'Документ2Организации2', '08/01/2014', '25/01/2013',0,'01/01/1900'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, @Organization2Uid, 123, 'Документ3', 'Документ3Организации2', '30/01/2014', '05/02/2013',0,'01/01/1900'

