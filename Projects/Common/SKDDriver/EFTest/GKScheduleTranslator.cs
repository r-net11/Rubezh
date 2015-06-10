using System.Data.Entity;
namespace SKDDriver.DataClasses
{
	public class GKScheduleTranslator
	{

	}

	public class SKDDbContext : DbContext
	{
		ContextType _contextType;

		public SKDDbContext(string connectionStringName, ContextType contextType)
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

		public DbSet<GKSchedule> PassJournals { get; set; }
		public DbSet<Employee> Employees { get; set; }
	}

	enum ContextType
	{
		MSSQL,
		PostgreSQL
	}
}
