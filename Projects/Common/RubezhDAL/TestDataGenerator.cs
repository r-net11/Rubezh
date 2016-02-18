using EntityFramework.BulkInsert.Extensions;
using Infrastructure.Common;
using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RubezhDAL.DataClasses
{
	public class TestDataGenerator
	{
		DatabaseContext Context;
		DbService _dbService;
		public TestDataGenerator(DbService dbService)
		{
			Context = dbService.Context;
			_dbService = dbService;
		}
		public OperationResult<bool> GenerateTestData(bool isAscending)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var cards = TestEmployeeCards();
				TestCardDoors(cards, true);
				return true;
			});
		}

		public OperationResult<bool> GenerateJournal()
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var random = new Random();
				//var journals = new List<Journal>();
				//for (int i = 0; i < 1500000; i++)
				//{
				//    var journal = new Journal
				//    {
				//        UID = Guid.NewGuid(),
				//        CardNo = 1,
				//        Description = 1,
				//        Name = 1,
				//        ObjectType = 1,
				//        ObjectUID = Guid.Empty,
				//        Subsystem = 1,
				//        SystemDate = DateTime.Now,
				//    };
				//    journals.Add(journal);    
				//}
				var journals = new List<RubezhAPI.Journal.JournalItem>();
				for (int i = 0; i < 25000; i++)
				{
					var journal = new RubezhAPI.Journal.JournalItem
					{
						UID = Guid.NewGuid(),
						CardNo = 1,
						JournalEventDescriptionType = RubezhAPI.Journal.JournalEventDescriptionType.NULL,
						JournalEventNameType = RubezhAPI.Journal.JournalEventNameType.Команда_оператора,
						JournalObjectType = RubezhAPI.Journal.JournalObjectType.None,
						JournalSubsystemType = RubezhAPI.Journal.JournalSubsystemType.System,
						SystemDateTime = DateTime.Now,
						ObjectUID = Guid.Empty
					};
					journals.Add(journal);
				}

				_dbService.JournalTranslator.AddRange(journals);
				//Context.BulkInsert(journals);
				//Context.SaveChanges();
				return true;
			});
		}

		public List<Guid> GetEmployeeCards()
		{
			return Context.Cards.Select(x => x.UID).ToList(); ;
		}

		public List<Guid> TestEmployeeCards()
		{
			Context.Database.Delete();
			int totalOrganisations = 1;
			int positionsPerOrganisation = 1000;
			int rootDepartmentsPerOrganisation = 100;
			int employeesPerOrganisation = 6600;
			int cardsPerEmployee = 10;

			int cardNumber = 0;
			Context.Configuration.AutoDetectChangesEnabled = false;
			Context.Configuration.ValidateOnSaveEnabled = false;
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var employees = new List<Employee>();
			var cards = new List<Card>();
			var positions = new List<Position>();
			var departments = new List<Department>();
			var random = new Random();
			for (int i = 0; i < totalOrganisations; i++)
			{
				var org = new Organisation { Name = "Тестовая Организация " + i, UID = Guid.NewGuid(), ExternalKey = "-1" };
				Context.Organisations.Add(org);

				var user = new OrganisationUser { UID = Guid.NewGuid(), UserUID = new Guid("10e591fb-e017-442d-b176-f05756d984bb"), OrganisationUID = org.UID };
				Context.OrganisationUsers.Add(user);

				var organisationPositions = new List<Position>();
				for (int j = 0; j < positionsPerOrganisation; j++)
				{
					var pos = new Position { Name = "Должность " + i + j, OrganisationUID = org.UID, UID = Guid.NewGuid(), RemovalDate = new DateTime(1900, 1, 1), ExternalKey = "-1" };
					organisationPositions.Add(pos);
				}

				var opranisationDepartments = new List<Department>();
				for (int j = 0; j < rootDepartmentsPerOrganisation; j++)
				{
					var dept = new Department { UID = Guid.NewGuid(), Name = "Подразделение " + i + j, OrganisationUID = org.UID };
					opranisationDepartments.Add(dept);
					for (int k = 0; k < 2; k++)
					{
						var dept2 = new Department { UID = Guid.NewGuid(), Name = "Подразделение " + i + j + k, OrganisationUID = org.UID, ParentDepartmentUID = dept.UID };
						opranisationDepartments.Add(dept2);
						for (int m = 0; m < 2; m++)
						{
							var dept3 = new Department { UID = Guid.NewGuid(), Name = "Подразделение " + i + j + k + m, OrganisationUID = org.UID, ParentDepartmentUID = dept2.UID };
							opranisationDepartments.Add(dept3);
							for (int n = 0; n < 2; n++)
							{
								var dept4 = new Department { UID = Guid.NewGuid(), Name = "Подразделение " + i + j + k + m + n, OrganisationUID = org.UID, ParentDepartmentUID = dept3.UID };
								opranisationDepartments.Add(dept4);
							}
						}
					}
				}

				for (int j = 0; j < employeesPerOrganisation; j++)
				{
					var empl = CreateEmployee(i.ToString() + j.ToString(), org.UID, opranisationDepartments[random.Next(15 * rootDepartmentsPerOrganisation)].UID, organisationPositions[random.Next(positionsPerOrganisation)].UID);
					employees.Add(empl);

					for (int k = 0; k < cardsPerEmployee; k++)
					{
						cardNumber++;
						var card = CreateCard(cardNumber, empl.UID);
						cards.Add(card);
					}
				}
				positions.AddRange(organisationPositions);
				departments.AddRange(opranisationDepartments);
			}

			switch (GlobalSettingsHelper.GlobalSettings.DbSettings.DbType)
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
					Context.Departments.AddRange(departments);
					Context.Positions.AddRange(positions);
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
			switch (GlobalSettingsHelper.GlobalSettings.DbSettings.DbType)
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

		public OperationResult<bool> GenerateEmployeeDays()
		{
			return DbServiceHelper.InTryCatch(() =>
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
							var scheduleSchemeType = (RubezhAPI.SKD.ScheduleSchemeType)scheduleScheme.Type;
							int dayNo = -1;
							switch (scheduleSchemeType)
							{
								case RubezhAPI.SKD.ScheduleSchemeType.Week:
									dayNo = (int)employeeDay.Date.DayOfWeek - 1;
									if (dayNo == -1)
										dayNo = 6;
									break;
								case RubezhAPI.SKD.ScheduleSchemeType.SlideDay:
									var ticksDelta = new TimeSpan(employeeDay.Date.Ticks - employee.ScheduleStartDate.Date.Ticks);
									var daysDelta = Math.Abs((int)ticksDelta.TotalDays);
									dayNo = daysDelta % schedule.ScheduleScheme.DaysCount;
									break;
								case RubezhAPI.SKD.ScheduleSchemeType.Month:
									dayNo = (int)employeeDay.Date.Day - 1;
									break;
							}
							var dayIntervalParts = scheduleScheme.ScheduleDays.FirstOrDefault(x => x.Number == dayNo).DayInterval.DayIntervalParts;
							foreach (var dayIntervalPart in dayIntervalParts)
							{
								employeeDay.DayIntervalsString += dayIntervalPart.BeginTimeTotalSeconds + "-" + dayIntervalPart.EndTimeTotalSeconds + ";";
							}
							employee.LastEmployeeDayUpdate = employeeDay.Date;
							Context.SaveChanges();
							result.Add(employeeDay);
						}
					}
				}
				Context.EmployeeDays.AddRange(result);
				return true;
			});
		}
	}
}

