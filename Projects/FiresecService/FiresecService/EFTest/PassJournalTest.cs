using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace FiresecService.EFTest
{
	public class EFTest
	{
		public static void Test()
		{
			Trace.WriteLine("PostgreSQL");
			using (var postgresEFTester = new EFTester("PassJournalDbContext", ContextType.PostgreSQL))
			{
				postgresEFTester.Test();
			}

			//Trace.WriteLine("MSSQL");
			//using (var msEFTester = new EFTester("passjournalEntities", ContextType.MSSQL))
			//{
			//    msEFTester.Test();
			//}
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
			//var employee = new Employee
			//{
			//    FirstName = "TestEmpl5551 Name",
			//    SecondName = "TestEmpl5551 SecName",
			//    LastName = "TestEmpl5551 LstName"
			//};
			//for (int i = 0; i < 5000; i++)
			//{
			//    var passJournal = new PassJournal
			//    {
			//        ZoneUID = Guid.Empty,
			//        EnterTime = new DateTime(3000, 1, 1),
			//        Employee = employee
			//    };
			//    _context.PassJournals.Add(passJournal);
			//}
			//_context.SaveChanges();

			//var passJournals = from x in context.PassJournals.Include("Employee") where x.EmployeeUID != null orderby x.EmployeeUID select x;
			//foreach (var item in passJournals)
			//{
			//    Trace.WriteLine(string.Format("{0} {1} {2}", item.UID, item.EmployeeUID, item.Employee != null ? item.Employee.FirstName : ""));
			//}

			//var employeepassjournals = from x in context.EmployeePassJournals select x;
			//foreach (var item in employeepassjournals)
			//{
			//    Trace.WriteLine(string.Format("{0} {1}", item.UID, item.LastName));
			//}

			//var employees = _context.Employees.ToList();
			//var passjournals = _context.PassJournals.ToList();
			//var employeepassjournals2 = context.GetEmployeePassJournals();

			PagingTestAction += passJournals => 
			{
				Trace.WriteLine("++++++NEXTPORTION++++++");
				foreach (var passJournalItem in passJournals)
				{
					Trace.WriteLine(string.Format("{0} {1}", passJournalItem.UID, passJournalItem.EmployeeUID));
				}
			};
			PagingTest();
			_context.SaveChanges();
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


	class PassJournal
	{
		public PassJournal()
		{
			UID = Guid.NewGuid();
		}

		[Key]
		[Column("uid")]
		public Guid UID { get; set; }

		[Column("employeeuid")]
		public Guid? EmployeeUID { get; set; }

		[Column("zoneuid")]
		public Guid? ZoneUID { get; set; }

		[Column("entertime")]
		public DateTime EnterTime { get; set; }

		[Column("exittime")]
		public DateTime? ExitTime { get; set; }

		public Employee Employee { get; set; }
	}

	class Employee
	{
		public Employee()
		{
			UID = Guid.NewGuid();
			PassJournals = new HashSet<PassJournal>();
		}

		[Key]
		[Column("uid")]
		public Guid UID { get; set; }

		[Column("firstname")]
		public string FirstName { get; set; }

		[Column("secondname")]
		public string SecondName { get; set; }

		[Column("lastname")]
		public string LastName { get; set; }

		public ICollection<PassJournal> PassJournals { get; set; }
	}

	class EmployeePassJournal
	{
		[Key]
		public Guid passjournaluid { get; set; }

		public string employeelastname { get; set; }
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
			modelBuilder.Entity<PassJournal>().ToTable("passjournal", schemaStr);
			modelBuilder.Entity<Employee>().ToTable("employee", schemaStr);
			//modelBuilder.Entity<EmployeePassJournal>().ToTable("employeepassjournal", schemaStr);
		}

		public ICollection<EmployeePassJournal> GetEmployeePassJournals()
		{
			//return Database.SqlQuery<EmployeePassJournal>("EXEC getemployeepassjournals").ToList();
			return Database.SqlQuery<EmployeePassJournal>("SELECT * from sum_n_product_with_tab ();").ToList();
		}

		public DbSet<PassJournal> PassJournals { get; set; }
		public DbSet<Employee> Employees { get; set; }
		public DbSet<EmployeePassJournal> EmployeePassJournals { get; set; }
	}

	enum ContextType
	{
		MSSQL,
		PostgreSQL
	}
}
