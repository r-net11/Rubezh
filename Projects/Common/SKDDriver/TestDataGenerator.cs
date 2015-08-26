using FiresecAPI;
using FiresecClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EntityFramework.BulkInsert.Extensions;
using System.Data.SqlClient;
using EntityFramework.BulkInsert.Providers;
using Infrastructure.Common;

namespace SKDDriver.DataClasses
{
    public class TestDataGenerator
    {
        DatabaseContext Context;
        public TestDataGenerator(DbService dbService)
        {
            Context = dbService.Context;
        }
        public OperationResult GenerateTestData(bool isAscending)
        {
            try
            {
                var cards = TestEmployeeCards();
                TestCardDoors(cards, true);
                return new OperationResult();
            }
            catch (Exception e)
            {
                return new OperationResult(e.Message);
            }
        }

        public OperationResult GenerateJournal()
        {
            try
            {
                var journals = new List<Journal>();
                for (int i = 0; i < 100000; i++)
                {
                    var journal = new Journal
                    {
                        UID = Guid.NewGuid(),
                        CardNo = 1,
                        Description = 1,
                        Name = 1,
                        ObjectType = 1,
                        ObjectUID = Guid.Empty,
                        Subsystem = 1,
                        SystemDate = DateTime.Now,
                    };
                    journals.Add(journal);    
                }
                Context.Journals.AddRange(journals);
                Context.SaveChanges();
                return new OperationResult();
            }
            catch (Exception e)
            {
                return new OperationResult(e.Message);
            }
        }

        public List<Guid> GetEmployeeCards()
        {
            return Context.Cards.Select(x => x.UID).ToList(); ;
        }

        public List<Guid> TestEmployeeCards()
        {
            //DeleteAll();
			Context.Configuration.AutoDetectChangesEnabled = false;
			Context.Configuration.ValidateOnSaveEnabled = false;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var employees = new List<Employee>();
            var cards = new List<Card>();
			var positions = new List<Position>();
			var departments = new List<Department>();
			var random = new Random();
            for (int i = 0; i < 1; i++)
            {
                var org = new Organisation { Name = "Тестовая Организация " + i, UID = Guid.NewGuid(), ExternalKey = "-1" };
                Context.Organisations.Add(org);

				var user = new OrganisationUser { UID = Guid.NewGuid(), UserUID = new Guid("10e591fb-e017-442d-b176-f05756d984bb"), OrganisationUID = org.UID };
				Context.OrganisationUsers.Add(user);

				for (int j = 0; j < 1000; j++)
				{
					var pos = new Position { Name = "Должность " + i + j, OrganisationUID = org.UID, UID = Guid.NewGuid(), RemovalDate = new DateTime(1900, 1, 1), ExternalKey = "-1" };
					positions.Add(pos);
				}
				for (int j = 0; j < 100; j++)
				{
					var dept = new Department{ UID = Guid.NewGuid(), Name = "Подразделение " + i + j, OrganisationUID = org.UID };
					departments.Add(dept);
					for (int k = 0; k < 2; k++)
					{
						var dept2 = new Department { UID = Guid.NewGuid(), Name = "Подразделение " + i + j + k, OrganisationUID = org.UID, ParentDepartmentUID = dept.UID };
						departments.Add(dept2);
						for (int m = 0; m < 2; m++)
						{
							var dept3 = new Department { UID = Guid.NewGuid(), Name = "Подразделение " + i + j + k + m, OrganisationUID = org.UID, ParentDepartmentUID = dept2.UID };
							departments.Add(dept3);
							for (int n = 0; n < 2; n++)
							{
								var dept4 = new Department { UID = Guid.NewGuid(), Name = "Подразделение " + i + j + k + m + n, OrganisationUID = org.UID, ParentDepartmentUID = dept3.UID };
								departments.Add(dept4);
							}
						}
					}
				}
				//for (int j = 0; j < 500; j++)
				//{
				//    var empl = CreateEmpl("Сотрудник " + i + j + "0", org.UID, deptUIDs.FirstOrDefault(), posUIDs.FirstOrDefault());
				//    Context.Employees.InsertOnSubmit(empl);
				//}
                
                for (int j = 0; j < 65535; j++)
                {
					var empl = CreateEmployee(j.ToString(), org.UID, departments[random.Next(1500)].UID, positions[random.Next(1000)].UID);
                    employees.Add(empl);
                    var card = CreateCard(j, empl.UID);
                    cards.Add(card);
                }
            }

			switch (GlobalSettingsHelper.GlobalSettings.DbType)
			{
				case DbType.MsSql:
					Context.BulkInsert(positions);
					Context.BulkInsert(departments);
					Context.BulkInsert(employees);
					Context.BulkInsert(cards);
					Context.SaveChanges();
					break;
				case DbType.Postgres:
					bool isBreak = false;
					int currentPage = 0;
					int pageSize = 10000;
					while (!isBreak)
					{
						var employeePortion = employees.Skip(currentPage * pageSize).Take(pageSize).ToList();
						var cardPortion = cards.Skip(currentPage * pageSize).Take(pageSize).ToList();
						Context.Employees.AddRange(employeePortion);
						Context.Cards.AddRange(cardPortion);
						Context.SaveChanges();
						isBreak = cardPortion.Count < pageSize;
						currentPage++;
					}
					break;
			}
			
			return cards.Select(x => x.UID).ToList();
        }

        public void TestCardDoors(List<Guid> cardUIDs, bool isAscending)
        {
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			int k = 0;
            int totalDoorsCount = GKManager.Doors.Count;
			var cardDoors = new List<CardDoor>();
            foreach (var cardUID in cardUIDs)
            {
                k++;
                int doorsCount = isAscending ? k < totalDoorsCount ? k : totalDoorsCount : 10;
                foreach (var door in GKManager.Doors.Take(doorsCount))
                {
                    var cardDoor = CreateCardDoor(cardUID, door.UID);
					cardDoors.Add(cardDoor);
                }
            }
			switch (GlobalSettingsHelper.GlobalSettings.DbType)
			{
				case DbType.MsSql:
					Context.BulkInsert(cardDoors);
					Context.SaveChanges();
					break;
				case DbType.Postgres:
					bool isBreak = false;
					int currentPage = 0;
					int pageSize = 10000;
					while (!isBreak)
					{
						var cardDoorPortion = cardDoors.Skip(currentPage * pageSize).Take(pageSize).ToList();
						Context.CardDoors.AddRange(cardDoorPortion);
						Context.SaveChanges();
						isBreak = cardDoorPortion.Count < pageSize;
						currentPage++;
					}
					break;
			}
        }

        void DeleteAll()
        {
            Context.Database.ExecuteSqlCommand("DELETE FROM \"AccessTemplates\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"AdditionalColumns\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"AdditionalColumnTypes\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"CardDoors\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Cards\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"CurrentConsumptions\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"DayIntervalParts\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"DayIntervals\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Departments\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Employees\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Holidays\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"NightSettings\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Organisations\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"PassCardTemplates\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Photos\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Positions\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"ScheduleDays\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"ScheduleSchemes\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Schedules\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"ScheduleZones\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"TimeTrackDocuments\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"TimeTrackDocumentTypes\"");
        }
        Department CreateDepartment(string name, Guid orgUID, Guid? parentUID = null)
        {
            return new Department
            {
                Name = name,
                OrganisationUID = orgUID,
                UID = Guid.NewGuid(),
                ParentDepartmentUID = parentUID,
                RemovalDate = _minDate,
                ExternalKey = "-1"
            };
        }
        Employee CreateEmployee(string no, Guid orgUID, Guid? deptUID = null, Guid? posUID = null)
        {
            return new Employee
            {
                LastName = "Фамилия " + no,
                FirstName = "Имя " + no,
                SecondName = "Отчество " + no,
                DepartmentUID = deptUID,
                PositionUID = posUID,
                OrganisationUID = orgUID,
                UID = Guid.NewGuid(),
                BirthDate = _minDate,
                CredentialsStartDate = _minDate,
                DocumentGivenDate = _minDate,
                DocumentValidTo = _minDate,
                LastEmployeeDayUpdate = _minDate,
                ScheduleStartDate = _minDate,
                ExternalKey = "-1",
                Type = 0
            };
        }
        static DateTime _minDate = new DateTime(1900, 1, 1);
        Card CreateCard(int no, Guid emplUID)
        {
            return new Card
            {
                UID = Guid.NewGuid(),
                Number = no,
                EmployeeUID = emplUID,
                GKCardType = 0,
                EndDate = _minDate,
                ExternalKey = "-1",
                GKLevel = 0,
                GKLevelSchedule = 0
            };
        }

        CardDoor CreateCardDoor(Guid cardUID, Guid doorUID)
        {
            return new CardDoor
            {
                UID = Guid.NewGuid(),
                CardUID = cardUID,
                DoorUID = doorUID,
                EnterScheduleNo = 1,
                ExitScheduleNo = 1
            };
        }

        public OperationResult GenerateEmployeeDays()
        {
            try
            {
                var result = new List<EmployeeDay>();
                var employees = Context.Employees.Where(x => !x.IsDeleted);
                foreach (var employee in employees)
                {

                    var employeeDay = new EmployeeDay();
                    employeeDay.EmployeeUID = employee.UID;
                    var schedule = Context.Schedules.FirstOrDefault(x => x.UID == employee.ScheduleUID);
                    if (schedule != null)
                    {
                        employeeDay.IsIgnoreHoliday = schedule.IsIgnoreHoliday;
                        employeeDay.IsOnlyFirstEnter = schedule.IsOnlyFirstEnter;
						employeeDay.AllowedLateTimeSpan = schedule.AllowedLateTimeSpan;
						employeeDay.AllowedEarlyLeaveTimeSpan = schedule.AllowedEarlyLeaveTimeSpan;
                        employeeDay.Date = DateTime.Now;
                        var scheduleScheme = schedule.ScheduleScheme;
                        if (scheduleScheme != null)
                        {
                            var scheduleSchemeType = (FiresecAPI.SKD.ScheduleSchemeType)scheduleScheme.Type;
                            int dayNo = -1;
                            switch (scheduleSchemeType)
                            {
                                case FiresecAPI.SKD.ScheduleSchemeType.Week:
                                    dayNo = (int)employeeDay.Date.DayOfWeek - 1;
                                    if (dayNo == -1)
                                        dayNo = 6;
                                    break;
                                case FiresecAPI.SKD.ScheduleSchemeType.SlideDay:
                                    var ticksDelta = new TimeSpan(employeeDay.Date.Ticks - employee.ScheduleStartDate.Date.Ticks);
                                    var daysDelta = Math.Abs((int)ticksDelta.TotalDays);
                                    dayNo = daysDelta % schedule.ScheduleScheme.DaysCount;
                                    break;
                                case FiresecAPI.SKD.ScheduleSchemeType.Month:
                                    dayNo = (int)employeeDay.Date.Day - 1;
                                    break;
                            }
                            var dayIntervalParts = scheduleScheme.ScheduleDays.FirstOrDefault(x => x.Number == dayNo).DayInterval.DayIntervalParts;
                            foreach (var dayIntervalPart in dayIntervalParts)
                            {
								employeeDay.DayIntervalsString += dayIntervalPart.BeginTimeSpan + "-" + dayIntervalPart.EndTimeSpan + ";";
                            }
                            employee.LastEmployeeDayUpdate = employeeDay.Date;
                            Context.SaveChanges();
                            result.Add(employeeDay);
                        }
                    }
                }
                Context.EmployeeDays.AddRange(result);
                return new OperationResult();
            }
            catch (Exception e)
            {
                return new OperationResult(e.Message);
            }
        }
    }
}

