using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKUDModule.Models
{
	public static class TestHelper
	{
		public static List<Group> Groups;
		public static List<Position> Positions;
		public static List<Department> Departments;

		static TestHelper()
		{
			Groups = new List<Group>();
			Groups.Add(new Group { Name = "Сотрудник" });
			Groups.Add(new Group { Name = "Старший сотрудник" });
			Groups.Add(new Group { Name = "Служба охраны" });

			Positions = new List<Position>();
			Positions.Add(new Position { Name = "Правитель" });
			Positions.Add(new Position { Name = "Мореплаватель" });

			Departments = new List<Department>();
			Departments.Add(new Department { Name = "Италия", Id = 1 });
			Departments.Add(new Department { Name = "Англия", Id = 2 });
		}
		
		public static List<Employee> GenerateTestData()
		{
			var employes = new List<Employee>();
			employes.Add(new Employee
			{
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

			employes.Add(new Employee
			{
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

			employes.Add(new Employee
			{
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

			employes.Add(new Employee
			{
				Department = Departments.FirstOrDefault(x => x.Name == "Англия"),
				Person = new Person()
				{
					FirstName = "Джеймс",
					SecondName = "Джеймс",
					LastName = "Кук",
					Address = "Мартон",
					AddressFact = "Гавайские острова",
					BirthDate = new DateTime(1779, 2, 14),
					BirthPlace = "ХаМартоннтингдон",
					Gender = Gender.Male
				},
				Email = "cook@seasailors.com",
				Group = Groups.FirstOrDefault(x => x.Name == "Сотрудник"),
				Position = Positions.FirstOrDefault(x => x.Name == "Мореплаватель"),
				Phone = "214 - 1779"
			});

			return employes;
		}
	}
}
