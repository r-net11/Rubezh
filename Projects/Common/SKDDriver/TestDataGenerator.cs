using FiresecAPI;
using FiresecClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

        //var posUIDs = new List<Guid>();
        //for (int j = 0; j < 1000; j++)
        //{
        //    var pos = new DataAccess.Position { Name = "Должность " + i + j, OrganisationUID = org.UID, UID = Guid.NewGuid(), RemovalDate = new DateTime(1900, 1, 1), ExternalKey = "-1" };
        //    Context.Positions.InsertOnSubmit(pos);
        //    posUIDs.Add(pos.UID);
        //}
        //var deptUIDs = new List<Guid>();
        //for (int j = 0; j < 100; j++)
        //{
        //    var dept = CreateDept("Подразделение " + i + j, org.UID);
        //    deptUIDs.Add(dept.UID);
        //    Context.Departments.InsertOnSubmit(dept);
        //    for (int k = 0; k < 2; k++)
        //    {
        //        var dept2 = CreateDept("Подразделение " + i + j + k, org.UID, dept.UID);
        //        deptUIDs.Add(dept2.UID);
        //        Context.Departments.InsertOnSubmit(dept2);
        //        for (int m = 0; m < 2; m++)
        //        {
        //            var dept3 = CreateDept("Подразделение " + i + j + k + m, org.UID, dept2.UID);
        //            deptUIDs.Add(dept3.UID);
        //            Context.Departments.InsertOnSubmit(dept3);
        //            for (int n = 0; n < 2; n++)
        //            {
        //                var dept4 = CreateDept("Подразделение " + i + j + k + m + n, org.UID, dept3.UID);
        //                deptUIDs.Add(dept4.UID);
        //                Context.Departments.InsertOnSubmit(dept4);
        //            }
        //        }
        //    }
        //}
        //for (int j = 0; j < 500; j++)
        //{
        //    var empl = CreateEmpl("Сотрудник " + i + j + "0", org.UID, deptUIDs.FirstOrDefault(), posUIDs.FirstOrDefault());
        //    Context.Employees.InsertOnSubmit(empl);
        //}

        public List<Guid> TestEmployeeCards()
        {
            DeleteAll();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var employees = new List<Employee>();
            var cards = new List<Card>();
            for (int i = 0; i < 1; i++)
            {
                var org = new Organisation { Name = "Тестовая Организация " + i, UID = Guid.NewGuid(), ExternalKey = "-1" };
                Context.Organisations.Add(org);
                var user = new OrganisationUser { UID = Guid.NewGuid(), UserUID = new Guid("10e591fb-e017-442d-b176-f05756d984bb"), OrganisationUID = org.UID };
                Context.OrganisationUsers.Add(user);
                
                for (int j = 0; j < 65535; j++)
                {
                    var empl = CreateEmployee(j.ToString(), org.UID);
                    employees.Add(empl);
                    var card = CreateCard(j, empl.UID);
                    cards.Add(card);
                }
            }
            stopwatch.Stop();
            Trace.WriteLine("GenerateModels " + stopwatch.Elapsed);

            stopwatch = new Stopwatch();
            stopwatch.Start();
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
            stopwatch.Stop();
            Trace.WriteLine("Context.SaveChanges " + stopwatch.Elapsed);
            
            return cards.Select(x => x.UID).ToList();
        }

        public void TestCardDoors(List<Guid> cardUIDs, bool isAscending)
        {
            int k = 0;
            int totalDoorsCount = GKManager.Doors.Count;
            foreach (var cardUID in cardUIDs)
            {
                k++;
                int doorsCount = isAscending ? k < totalDoorsCount ? k : totalDoorsCount : 10;
                foreach (var door in GKManager.Doors.Take(doorsCount))
                {
                    var cardDoor = CreateCardDoor(cardUID, door.UID);
                    Context.CardDoors.Add(cardDoor);
                }
            }
            Context.SaveChanges();
        }

        void DeleteAll()
        {
            Context.Database.ExecuteSqlCommand("DELETE FROM \"AccessTemplate\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"AdditionalColumn\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"AdditionalColumnType\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"CardDoor\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Card\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"CurrentConsumption\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"DayIntervalPart\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"DayInterval\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Department\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Employee\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"GKMetadata\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Holiday\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"NightSetting\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Organisation\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"PassCardTemplate\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Photo\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Position\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"ScheduleDay\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"ScheduleScheme\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"Schedule\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"ScheduleZone\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"TimeTrackDocument\"");
            Context.Database.ExecuteSqlCommand("DELETE FROM \"TimeTrackDocumentType\"");
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
                        employeeDay.AllowedLate = schedule.AllowedLate;
                        employeeDay.AllowedEarlyLeave = schedule.AllowedEarlyLeave;
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
                                employeeDay.DayIntervalsString += dayIntervalPart.BeginTime + "-" + dayIntervalPart.EndTime + ";";
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
