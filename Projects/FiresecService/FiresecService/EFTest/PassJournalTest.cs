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
			using (var db = new PassJournalContext("PassJournalDbContext", ContextType.PostgreSQL))
			{
				PassJournalTest(db);
			}
			Trace.WriteLine("MSSQL");
			using (var db2 = new PassJournalContext("passjournalEntities", ContextType.MSSQL))
			{
				PassJournalTest(db2);
			}
		}

		static void PassJournalTest(PassJournalContext context)
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
				context.PassJournals.Add(passJournal);
			}
			context.SaveChanges();

			var passJournals = from x in context.PassJournals.Include("Employee") where x.EmployeeUID != null orderby x.EmployeeUID select x;
			foreach (var item in passJournals)
			{
				Trace.WriteLine(string.Format("{0} {1} {2}", item.UID, item.EmployeeUID, item.Employee != null ? item.Employee.FirstName : ""));
			}

			context.SaveChanges();
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
