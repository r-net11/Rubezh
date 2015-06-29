using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace SKDDriver.EFTest
{
	public static class EFTest
	{
		public static void Test()
		{
			using (var postgresEFTester = new EFTester("SKDDbContext", ContextType.PostgreSQL))
			{
				postgresEFTester.Test();
			}
		}
	}

	class EFTester : IDisposable
	{
		ContextType _contextType;
		public string _connectionString;
		PassJournalContext _context;
		public event Action<List<PassJournal>> PagingTestAction;

		public EFTester(string connectionString, ContextType contextType)
		{
			_contextType = contextType;
			_connectionString = connectionString;
			_context = new PassJournalContext(_connectionString, _contextType);
		}

		public void Test()
		{
			var employee = new Employee
			{
				FirstName = "TestEmpl5551 Name",
				SecondName = "TestEmpl5551 SecName",
				LastName = "TestEmpl5551 LstName"
			};
			for (int i = 0; i < 5; i++)
			{
				var passJournal = new PassJournal
				{
					ZoneUID = Guid.Empty,
					EnterTime = new DateTime(3000, 1, 1),
					Employee = employee
				};
				_context.PassJournals.Add(passJournal);
			}
			_context.SaveChanges();

			var passJournals = (from x in _context.PassJournals//.Include("Employee") where x.EmployeeUID != null 
							   orderby x.EmployeeUID select x).ToList();
			foreach (var item in passJournals)
			{
				Trace.WriteLine(string.Format("{0} {1} {2}", item.UID, item.EmployeeUID, item.Employee.FirstName));
			}

			//var employeepassjournals = from x in _context.EmployeePassJournals select x;
			//foreach (var item in employeepassjournals)
			//{
			//    Trace.WriteLine(string.Format("{0} {1}", item.UID, item.LastName));
			//}

			
			var employees = _context.Employees.ToList();
			var passjournals = _context.PassJournals.ToList();
			//var employeepassjournals2 = context.GetEmployeePassJournals();

			//PagingTestAction += passJournals => 
			//{
			//    Trace.WriteLine("++++++NEXTPORTION++++++");
			//    foreach (var passJournalItem in passJournals)
			//    {
			//        Trace.WriteLine(string.Format("{0} {1}", passJournalItem.UID, passJournalItem.EmployeeUID));
			//    }
			//};
			//PagingTest();
			//_context.SaveChanges();
		}

		void PagingTest()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			var passJournals = _context.PassJournals.OrderBy(x => x.EmployeeUID);
			var journalItems = new List<PassJournal>();
			var pageSize = 2511;
			int page = 0;
			bool isBreak = false;
			while (!isBreak)
			{
				var query = passJournals.Skip(page * pageSize).Take(pageSize);
				journalItems = query.ToList();
				page++;
				isBreak = journalItems.Count < pageSize;
				Trace.WriteLine(stopWatch.Elapsed);
				PublishNewItemsPortion(journalItems);
			}
			stopWatch.Stop();
		}

		void PublishNewItemsPortion(List<PassJournal> journalItems)
		{
			if (PagingTestAction != null)
				PagingTestAction(journalItems.ToList());
			journalItems.Clear();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}

	public class PassJournal
	{
		public PassJournal()
		{
			UID = Guid.NewGuid();
		}

		[Key]
		public Guid UID { get; set; }

		public Guid? EmployeeUID { get; set; }

		public Guid? ZoneUID { get; set; }

		public DateTime EnterTime { get; set; }

		public DateTime? ExitTime { get; set; }

		public virtual Employee Employee { get; set; }
	}

	public class Employee
	{
		public Employee()
		{
			UID = Guid.NewGuid();
			PassJournals = new HashSet<PassJournal>();
			IsDeleted = false;
		}

		[Key]
		public Guid UID { get; set; }

		public string FirstName { get; set; }

		public string SecondName { get; set; }

		public string LastName { get; set; }

		public virtual ICollection<PassJournal> PassJournals { get; set; }

		public bool IsDeleted { get; set; }
	}

	class PassJournalContext : DbContext
	{
		ContextType _contextType;

		public PassJournalContext(string connectionStringName, ContextType contextType)
			: base(connectionStringName)
		{
			_contextType = contextType;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			string schemaStr;
			switch (_contextType)
			{
				case ContextType.MSSQL:
					schemaStr = "dbo";
					break;
				case ContextType.PostgreSQL:
					schemaStr = "public";
					break;
				default:
					schemaStr = "";
					break;
			}
			modelBuilder.Entity<PassJournal>().ToTable("PassJournal", schemaStr);
			modelBuilder.Entity<Employee>().ToTable("Employee", schemaStr);
		}

		public DbSet<PassJournal> PassJournals { get; set; }
		public DbSet<Employee> Employees { get; set; }
	}

	enum ContextType
	{
		MSSQL,
		PostgreSQL
	}
}
