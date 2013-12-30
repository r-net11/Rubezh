USE [SKUD]
DECLARE @Uid uniqueidentifier;

TRUNCATE TABLE [dbo].[Holiday]
SET @Uid = NEWID(); 
EXEC [dbo].[SaveHoliday] @Uid, 'Новый год', 'Transferred', '2013-31-12', '2013-28-12'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveHoliday] @Uid, '8 марта', 'Holiday', '2014-08-03'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveHoliday] @Uid, 'Старый Новый год', 'Redused', '2014-13-01', NULL, 2

TRUNCATE TABLE [dbo].[Document]
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, 'Документ1', 'Документ1', '2013-01-01', '2013-07-01'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, 'Документ2', 'Документ2', '2014-08-01', '2013-25-01'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDocument] @Uid, 'Документ3', 'Документ3', '2014-30-01', '2013-05-02'

delete from [dbo].[Interval]
delete from [dbo].[NamedInterval]
delete from [dbo].[Day]
delete from [dbo].[ScheduleScheme]
--ОХРАНА
DECLARE @GuardNamedIntervalUid uniqueidentifier;
SET @GuardNamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @GuardNamedIntervalUid, 'Охрана'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '08:00', '13:00', 'Day', @GuardNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '13:45', '17:45', 'Day', @GuardNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '18:30', '22:30', 'Day', @GuardNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '23:15', '03:15', 'DayNight', @GuardNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '04:00', '08:00', 'Night', @GuardNamedIntervalUid
DECLARE @GuardScheduleSchemeUid uniqueidentifier;
SET @GuardScheduleSchemeUid = NEWID();
EXEC [dbo].[SaveScheduleScheme] @GuardScheduleSchemeUid, 'Охрана', 'Shift', 4
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @GuardNamedIntervalUid, @GuardScheduleSchemeUid, 0
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @GuardScheduleSchemeUid, 1
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @GuardScheduleSchemeUid, 2
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @GuardScheduleSchemeUid, 3
--МОНТАЖ
DECLARE @Montage1NamedIntervalUid uniqueidentifier;
SET @Montage1NamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @Montage1NamedIntervalUid, 'Монтажный1'
DECLARE @Montage2NamedIntervalUid uniqueidentifier;
SET @Montage2NamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @Montage2NamedIntervalUid, 'Монтажный2'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '07:30', '11:00', 'Day', @Montage1NamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '11:30', '16:00', 'Day', @Montage1NamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '08:00', '11:30', 'Day', @Montage2NamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '12:00', '16:30', 'Day', @Montage2NamedIntervalUid
DECLARE @MontageScheduleSchemeUid uniqueidentifier;
SET @MontageScheduleSchemeUid = NEWID();
EXEC [dbo].[SaveScheduleScheme] @MontageScheduleSchemeUid, 'Монтаж', 'Week', 7
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage1NamedIntervalUid, @MontageScheduleSchemeUid, 0
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage2NamedIntervalUid, @MontageScheduleSchemeUid, 1
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage1NamedIntervalUid, @MontageScheduleSchemeUid, 2
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage2NamedIntervalUid, @MontageScheduleSchemeUid, 3
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @Montage1NamedIntervalUid, @MontageScheduleSchemeUid, 4
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @MontageScheduleSchemeUid, 5
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @MontageScheduleSchemeUid, 6
--ПЯТИДНЕВКА
DECLARE @WeeklyNamedIntervalUid uniqueidentifier;
SET @WeeklyNamedIntervalUid = NEWID();
EXEC [dbo].[SaveNamedInterval] @WeeklyNamedIntervalUid, 'Пятидневка'
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '08:00', '12:00', 'Day', @WeeklyNamedIntervalUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveInterval] @Uid, '13:00', '17:00', 'Day', @WeeklyNamedIntervalUid
DECLARE @WeeklyScheduleSchemeUid uniqueidentifier;
SET @WeeklyScheduleSchemeUid = NEWID();
EXEC [dbo].[SaveScheduleScheme] @WeeklyScheduleSchemeUid, 'Пятидневка', 'Week', 7
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 0
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 1
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 2
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 3
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, @WeeklyNamedIntervalUid, @WeeklyScheduleSchemeUid, 4
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @WeeklyScheduleSchemeUid, 5
SET @Uid = NEWID(); 
EXEC [dbo].[SaveDay] @Uid, NULL, @WeeklyScheduleSchemeUid, 6

delete from [dbo].[Schedule]
DECLARE @GuardScheduleUid uniqueidentifier;
SET @GuardScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @GuardScheduleUid, 'Охрана', @GuardScheduleSchemeUid
DECLARE @MontageScheduleUid uniqueidentifier;
SET @MontageScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @MontageScheduleUid , 'Монтаж', @MontageScheduleSchemeUid 
DECLARE @ITScheduleUid uniqueidentifier;
SET @ITScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @ITScheduleUid , 'IT', @WeeklyScheduleSchemeUid 
DECLARE @ConstructorshipScheduleUid uniqueidentifier;
SET @ConstructorshipScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @ConstructorshipScheduleUid , 'Конструкторы', @WeeklyScheduleSchemeUid  
DECLARE @GovernanceScheduleUid uniqueidentifier;
SET @GovernanceScheduleUid = NEWID();
EXEC [dbo].[SaveSchedule] @GovernanceScheduleUid , 'Руководство', @WeeklyScheduleSchemeUid

delete from [dbo].[RegisterDevice]
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, 1, @GuardScheduleUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, 1, @MontageScheduleUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, 1, @ITScheduleUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, 1, @ConstructorshipScheduleUid
SET @Uid = NEWID(); 
EXEC [dbo].[SaveRegisterDevice] @Uid, null, @GovernanceScheduleUid

delete from [dbo].[Position]
DECLARE @GuardPositionUid uniqueidentifier;
SET @GuardPositionUid = NEWID();
EXEC [dbo].[SavePosition] @GuardPositionUid, 'Охранник', 'Охранопроизводитель'
DECLARE @MainGuardPositionUid uniqueidentifier;
SET @MainGuardPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainGuardPositionUid , 'Ст. охранник', 'Старший охранопроизводитель'
DECLARE @MontagePositionUid uniqueidentifier;
SET @MontagePositionUid = NEWID();
EXEC [dbo].[SavePosition] @MontagePositionUid , 'Монтажник', 'Оператор монтажа'
DECLARE @MainMontagePositionUid uniqueidentifier;
SET @MainMontagePositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainMontagePositionUid , 'Ст. монтажник', 'Старший оператор монтажа'
DECLARE @ProgrammerPositionUid uniqueidentifier;
SET @ProgrammerPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ProgrammerPositionUid , 'Программист', 'Разработчик алгоритмов для ЭВМ'
DECLARE @MainProgrammerPositionUid uniqueidentifier;
SET @MainProgrammerPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainProgrammerPositionUid , 'Ст. программист', 'Старший разработчик алгоритмов для ЭВМ'
DECLARE @TesterPositionUid uniqueidentifier;
SET @TesterPositionUid = NEWID();
EXEC [dbo].[SavePosition] @TesterPositionUid , 'Тестировщик', 'Испытатель алгоритмов для ЭВМ'
DECLARE @MainTesterPositionUid uniqueidentifier;
SET @MainTesterPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainTesterPositionUid , 'Ст. тестировщик', 'Старший испытатель алгоритмов для ЭВМ'
DECLARE @ConstructorPositionUid uniqueidentifier;
SET @ConstructorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ConstructorPositionUid , 'Конструктор', 'Инженер-конструктор'
DECLARE @MainConstructorPositionUid uniqueidentifier;
SET @MainConstructorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @MainConstructorPositionUid , 'Ст. конструктор', 'Старший инженер-конструктор'
DECLARE @ProgrammistConstructorPositionUid uniqueidentifier;
SET @ProgrammistConstructorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @ProgrammistConstructorPositionUid , 'Программист-конструктор', 'Программописец и диковинок выдумщик'
DECLARE @AdministratorPositionUid uniqueidentifier;
SET @AdministratorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @AdministratorPositionUid , 'Администратор', 'Главный распорядитель'
DECLARE @DirectorPositionUid uniqueidentifier;
SET @DirectorPositionUid = NEWID();
EXEC [dbo].[SavePosition] @DirectorPositionUid , 'Директор', 'Руководитель компании'

delete from [dbo].[Employee]
DECLARE @Guard1EmployeeUid uniqueidentifier;
SET @Guard1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Guard1EmployeeUid , 'Сергей', 'Петрович', 'Иванов', @GuardPositionUid, null , @GuardScheduleUid, '12-05-2005'
DECLARE @Guard2EmployeeUid uniqueidentifier;
SET @Guard2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Guard2EmployeeUid , 'Петр', 'Сергеевич', 'Ивановский', @GuardPositionUid, null , @GuardScheduleUid, '12-05-2006'
DECLARE @MainGuardEmployeeUid uniqueidentifier;
SET @MainGuardEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainGuardEmployeeUid , 'Петр', 'Сергеевич', 'Ивановичус', @MainGuardPositionUid, null , @GuardScheduleUid, '12-05-2007'
DECLARE @Montage1EmployeeUid uniqueidentifier;
SET @Montage1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Montage1EmployeeUid , 'Иван', 'Сергеевич', 'Петров', @MontagePositionUid, null , @MontageScheduleUid, '12-05-2008'
DECLARE @Montage2EmployeeUid uniqueidentifier;
SET @Montage2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Montage2EmployeeUid , 'Сергей', 'Иванович', 'Петровишвили', @MontagePositionUid, null , @MontageScheduleUid, '12-05-2009'
DECLARE @MainMontageEmployeeUid uniqueidentifier;
SET @MainMontageEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainMontageEmployeeUid , 'Сергей', 'Сергеевич', 'Петровский', @MainMontagePositionUid, null , @MontageScheduleUid, '12-05-2010'
DECLARE @Programmer1EmployeeUid uniqueidentifier;
SET @Programmer1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Programmer1EmployeeUid , 'Петр', 'Иванович', 'Сергеев', @ProgrammerPositionUid, null , @ITScheduleUid, '12-05-2011'
DECLARE @Programmer2EmployeeUid uniqueidentifier;
SET @Programmer2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Programmer2EmployeeUid , 'Иван', 'Перович', 'Сергеевич', @ProgrammerPositionUid, null , @ITScheduleUid, '12-05-2012'
DECLARE @MainProgrammistEmployeeUid uniqueidentifier;
SET @MainProgrammistEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainProgrammistEmployeeUid , 'Иван', 'Иванович', 'Сергеевко', @MainProgrammerPositionUid, null , @ITScheduleUid, '12-05-2013'
DECLARE @Tester1EmployeeUid uniqueidentifier;
SET @Tester1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Tester1EmployeeUid , 'Сидор', 'Прохорович', 'Захарьин', @TesterPositionUid, null , @ITScheduleUid, '12-06-2013'
DECLARE @Tester2EmployeeUid uniqueidentifier;
SET @Tester2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Tester1EmployeeUid , 'Прохор', 'Сидорович', 'Захаров', @TesterPositionUid, null , @ITScheduleUid, '12-07-2013'
DECLARE @MainTesterEmployeeUid uniqueidentifier;
SET @MainTesterEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainTesterEmployeeUid , 'Прохор', 'Прохорович', 'Захаренко', @MainTesterPositionUid, null , @ITScheduleUid, '12-08-2013'
DECLARE @Constructor1EmployeeUid uniqueidentifier;
SET @Constructor1EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Constructor1EmployeeUid , 'Захар', 'Сидорович', 'Прохоров', @ConstructorPositionUid, null , @ConstructorshipScheduleUid, '12-09-2013'
DECLARE @Constructor2EmployeeUid uniqueidentifier;
SET @Constructor2EmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @Constructor2EmployeeUid , 'Сидор', 'Захарович', 'Прохорский', @ConstructorPositionUid, null , @ConstructorshipScheduleUid, '12-10-2013'
DECLARE @MainConstructorEmployeeUid uniqueidentifier;
SET @MainConstructorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @MainConstructorEmployeeUid , 'Захар', 'Захарович', 'Прохоревич', @MainConstructorPositionUid, null , @ConstructorshipScheduleUid, '12-11-2013'
DECLARE @AdministratorEmployeeUid uniqueidentifier;
SET @AdministratorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @AdministratorEmployeeUid , 'Захар', 'Прохорович', 'Сидоров', @AdministratorPositionUid, null , @GovernanceScheduleUid, '12-12-2013'
DECLARE @DirectorEmployeeUid uniqueidentifier;
SET @DirectorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @DirectorEmployeeUid , 'Прохор', 'Захарович', 'Сидоренко', @DirectorEmployeeUid , null , @GovernanceScheduleUid, '13-12-2013'
DECLARE @ProgrammistConstructorEmployeeUid uniqueidentifier;
SET @ProgrammistConstructorEmployeeUid = NEWID();
EXEC [dbo].[SaveEmployee] @ProgrammistConstructorEmployeeUid , 'Миямото', 'Дайтаро', 'Мусащи', @ProgrammistConstructorPositionUid , null , @ConstructorshipScheduleUid, '13-12-2001'

delete from [dbo].[Department]
DECLARE @MainDepartmentUid uniqueidentifier;
SET @MainDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @MainDepartmentUid , 'ООО "СКУДЪ"', 'Мануфактура купца 3 гильдии Сидоренко', null , @DirectorEmployeeUid, @AdministratorEmployeeUid
DECLARE @GovernanceDepartmentUid uniqueidentifier;
SET @GovernanceDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @GovernanceDepartmentUid , 'Руководство', 'Столоначальники и власть придержащие', @MainDepartmentUid , @AdministratorEmployeeUid, @DirectorEmployeeUid 
DECLARE @GuardDepartmentUid uniqueidentifier;
SET @GuardDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @GuardDepartmentUid , 'Охрана', 'Охранители и надсмотрщики', @MainDepartmentUid , @MainGuardEmployeeUid, @Guard1EmployeeUid 
DECLARE @MontageDepartmentUid uniqueidentifier;
SET @MontageDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @MontageDepartmentUid , 'Монтаж', 'Собиратели и установщики', @MainDepartmentUid , @MainMontageEmployeeUid, @Montage1EmployeeUid 
DECLARE @EngeneeringDepartmentUid uniqueidentifier;
SET @EngeneeringDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @EngeneeringDepartmentUid , 'Инженеры', 'Инженерия и иныя премудрости', @MainDepartmentUid , @ProgrammistConstructorEmployeeUid 
DECLARE @ITDepartmentUid uniqueidentifier;
SET @ITDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @ITDepartmentUid , 'IT', 'Алгоритмописатели и тестировщики', @EngeneeringDepartmentUid , @MainProgrammistEmployeeUid, @MainTesterEmployeeUid 
DECLARE @ProgrammerDepartmentUid uniqueidentifier;
SET @ProgrammerDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @ProgrammerDepartmentUid , 'Программисты', 'Алгоритмописатели', @ITDepartmentUid , @MainProgrammistEmployeeUid, @Programmer1EmployeeUid 
DECLARE @TesterDepartmentUid uniqueidentifier;
SET @TesterDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @TesterDepartmentUid , 'Тестировщики', 'Кода премудрого проверятели', @ITDepartmentUid , @MainTesterEmployeeUid , @Tester1EmployeeUid 
DECLARE @ConstructorshipDepartmentUid uniqueidentifier;
SET @ConstructorshipDepartmentUid = NEWID();
EXEC [dbo].[SaveDepartment] @ConstructorshipDepartmentUid , 'Конструкторы', 'Машин неведомых собиратели', @EngeneeringDepartmentUid , @MainConstructorEmployeeUid , @Constructor1EmployeeUid

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