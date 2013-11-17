using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKUDModule.Models
{
	public static class TestDataHelper
	{
		public static List<Group> Groups { get; private set; }
		public static List<Position> Positions { get; private set; }
		public static List<Department> Departments { get; private set; }
		public static List<Employee> Employees { get; private set; }

		static TestDataHelper()
		{
			Groups = new List<Group>();
			Groups.Add(new Group { Name = "Сотрудник", Id = 1 });
			Groups.Add(new Group { Name = "Старший сотрудник", Id = 2 });
			Groups.Add(new Group { Name = "Служба охраны", Id = 3 });

			Positions = new List<Position>();
			Positions.Add(new Position { Name = "Правитель", Id = 1 });
			Positions.Add(new Position { Name = "Мореплаватель", Id = 2 });

			Departments = new List<Department>();
			Departments.Add(new Department { Name = "Италия", Id = 1 });
			Departments.Add(new Department { Name = "Англия", Id = 2 });

			Employees = new List<Employee>();
			Employees.Add(new Employee
			{
				Id = 1,
				Department = Departments.FirstOrDefault(x => x.Name == "Италия"),
				Person = new Person()
				{
					FirstName = "Бенито",
					SecondName = "Амилькаре",
					LastName = "Муссолини",
					Address = "Предаппио",
					AddressFact = "Джулино-ди-Меццегра",
					BirthDate = new DateTime(1883, 7, 29),
					BirthPlace = "Предаппио",
					Gender = Gender.Male
				},
				Email = "benito@dictators.com",
				Group = Groups.FirstOrDefault(x => x.Name == "Служба охраны"),
				Position = Positions.FirstOrDefault(x => x.Name == "Правитель"),
				Phone = "729 - 1883"
			});

			Employees.Add(new Employee
			{
				Id = 2,
				Department = Departments.FirstOrDefault(x => x.Name == "Италия"),
				Person = new Person()
				{
					FirstName = "Америго",
					SecondName = "Джеронимо",
					LastName = "Веспуччи",
					Address = "Флоренция",
					AddressFact = "Севилья",
					BirthDate = new DateTime(1454, 3, 9),
					BirthPlace = "Флоренция",
					Gender = Gender.Male
				},
				Email = "amerigo@sailors.com",
				Group = Groups.FirstOrDefault(x => x.Name == "Сотрудник"),
				Position = Positions.FirstOrDefault(x => x.Name == "Мореплаватель"),
				Phone = "309 - 1454"
			});

			Employees.Add(new Employee
			{
				Id = 3,
				Department = Departments.FirstOrDefault(x => x.Name == "Англия"),
				Person = new Person()
				{
					FirstName = "Оливер",
					SecondName = "Роберт",
					LastName = "Кромвель",
					Address = "Хантингдон",
					AddressFact = "Лондон",
					BirthDate = new DateTime(1658, 9, 3),
					BirthPlace = "Хантингдон",
					Gender = Gender.Male
				},
				Email = "сromwell@protectors.com",
				Group = Groups.FirstOrDefault(x => x.Name == "Старший сотрудник"),
				Position = Positions.FirstOrDefault(x => x.Name == "Правитель"),
				Phone = "903 - 1658"
			});

			Employees.Add(new Employee
			{
				Id = 4,
				Department = Departments.FirstOrDefault(x => x.Name == "Англия"),
				Person = new Person()
				{
					FirstName = "Джеймс",
					SecondName = "Джеймс",
					LastName = "Кук",
					Address = "Мартон",
					AddressFact = "Гавайские острова",
					BirthDate = new DateTime(1779, 2, 14),
					BirthPlace = "Мартон",
					Gender = Gender.Male
				},
				Email = "cook@seasailors.com",
				Group = Groups.FirstOrDefault(x => x.Name == "Сотрудник"),
				Position = Positions.FirstOrDefault(x => x.Name == "Мореплаватель"),
				Phone = "214 - 1779"
			});
		}

		public static void AddEmployee(Employee employee)
		{
			var sameEmployee = Employees.FirstOrDefault(x => x.Id == employee.Id);
			if (sameEmployee != null)
			{
				Employees.Remove(sameEmployee);
			}
			Employees.Add(employee);
		}

		public static void AddGroup(Group group)
		{
			var sameGroup = Groups.FirstOrDefault(x => x.Id == group.Id);
			if (sameGroup != null)
			{
				Groups.Remove(sameGroup);
			}
			Groups.Add(group);
		}
		

		public static void AddPosition(Position position)
		{
			var samePosition = Positions.FirstOrDefault(x => x.Id == position.Id);
			if (samePosition != null)
			{
				Positions.Remove(samePosition);
			}
			Positions.Add(position);
		}

		public static void AddDepartment(Department department)
		{
			var sameDepartment = Departments.FirstOrDefault(x => x.Id == department.Id);
			if (sameDepartment != null)
			{
				Departments.Remove(sameDepartment);
			}
			Departments.Add(department);
		}

		public static void RemoveEmployee(Employee employee)
		{
			Employees.Remove(employee);
		}

		public static void RemoveGroup(Group group)
		{
			Groups.Remove(group);
		}

		public static void RemovePosition(Position position)
		{
			Positions.Remove(position);
		}

		public static void RemoveDepartment(Department department)
		{
			Departments.Remove(department);
		}
	}
}
