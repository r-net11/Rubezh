using System;
using System.Data.Linq;
using System.Linq.Expressions;
using LinqKit;
using System.Data.Entity;
using System.Linq;

namespace RubezhDAL.DataClasses
{
    public class EmployeeSynchroniser : Synchroniser<FiresecAPI.SKD.ExportEmployee, Employee>
	{
		public EmployeeSynchroniser(DbSet<Employee> table, DbService databaseService) : base(table, databaseService) { }

        public override FiresecAPI.SKD.ExportEmployee Translate(Employee item)
		{
            return new FiresecAPI.SKD.ExportEmployee 
			{ 
				FirstName = item.FirstName, 
				SecondName = item.SecondName,
				LastName = item.LastName,
				CredentialsStartDate = item.CredentialsStartDate,
				DocumentNumber = item.DocumentNumber,
				BirthDate = item.BirthDate,
				BirthPlace = item.BirthPlace,
				DocumentGivenDate = item.DocumentGivenDate,
				DocumentGivenBy = item.DocumentGivenBy,
				DocumentValidTo = item.DocumentValidTo,
				DocumentDepartmentCode = item.DocumentDepartmentCode,
				Citizenship = item.Citizenship,
				Description = item.Description,
				Gender = item.Gender,
				Type = item.Type,
				LastEmployeeDayUpdate = item.LastEmployeeDayUpdate,
				ScheduleStartDate = item.ScheduleStartDate,
				
				OrganisationUID = GetUID(item.OrganisationUID),
				OrganisationExternalKey = GetExternalKey(item.OrganisationUID, item.Organisation),
				PositionUID = GetUID(item.PositionUID),
				PositionExternalKey = GetExternalKey(item.PositionUID, item.Position),
				DepartmentUID = GetUID(item.DepartmentUID),
				DepartmentExternalKey = GetExternalKey(item.DepartmentUID, item.Department),
				EscrortUID = GetUID(item.EscortUID),
				EscortExternalKey = GetExternalKey(item.EscortUID, item.Escort)
			};
		}

        protected override void UpdateForignKeys(FiresecAPI.SKD.ExportEmployee exportItem, Employee tableItem, OrganisationHRCash hrCash)
		{
			tableItem.OrganisationUID = hrCash.OrganisationUID;
			tableItem.PositionUID = GetUIDbyExternalKey(exportItem.PositionExternalKey, hrCash.Positions);
			tableItem.DepartmentUID = GetUIDbyExternalKey(exportItem.DepartmentExternalKey, hrCash.Departments);
			tableItem.EscortUID = GetUIDbyExternalKey(exportItem.EscortExternalKey, hrCash.Employees);
		}

		protected override IQueryable<Employee> GetFilteredItems(FiresecAPI.SKD.ExportFilter filter)
		{
			return base.GetFilteredItems(filter).Where(x => x.OrganisationUID == filter.OrganisationUID);
		}
		
        public override void TranslateBack(FiresecAPI.SKD.ExportEmployee exportItem, Employee tableItem)
		{
			tableItem.FirstName = exportItem.FirstName;
			tableItem.SecondName = exportItem.SecondName;
			tableItem.LastName = exportItem.LastName;
			tableItem.CredentialsStartDate = exportItem.CredentialsStartDate.CheckDate();
			tableItem.DocumentNumber = exportItem.DocumentNumber;
            tableItem.BirthDate = tableItem.BirthDate.CheckDate();
			tableItem.BirthPlace = tableItem.BirthPlace;
			tableItem.DocumentGivenDate = exportItem.DocumentGivenDate.CheckDate();
			tableItem.DocumentGivenBy = exportItem.DocumentGivenBy;
			tableItem.DocumentValidTo = exportItem.DocumentValidTo.CheckDate();
			tableItem.DocumentDepartmentCode = exportItem.DocumentDepartmentCode;
			tableItem.Citizenship = exportItem.Citizenship;
			tableItem.Description = exportItem.Description;
			tableItem.Gender = exportItem.Gender;
			tableItem.Type = exportItem.Type;
			tableItem.LastEmployeeDayUpdate = exportItem.LastEmployeeDayUpdate.CheckDate();
			tableItem.ScheduleStartDate = exportItem.ScheduleStartDate.CheckDate();
			tableItem.IsDeleted = exportItem.IsDeleted;
			tableItem.RemovalDate = exportItem.RemovalDate;
		}

		protected override string XmlHeaderName
		{
			get { return "ArrayOfExportEmployee"; }
		}
		
		protected override string Name
		{
			get { return "Employees"; }
		}
	}
}


