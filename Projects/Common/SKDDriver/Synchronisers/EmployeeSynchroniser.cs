using System;
using System.Data.Linq;
using System.Linq.Expressions;
using LinqKit;
using System.Data.Entity;

namespace SKDDriver.DataClasses
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

        protected override void UpdateForignKeys(FiresecAPI.SKD.ExportEmployee exportItem, Employee tableItem)
		{
			tableItem.OrganisationUID = GetUIDbyExternalKey(exportItem.OrganisationExternalKey, _DatabaseService.Context.Organisations);
			tableItem.PositionUID = GetUIDbyExternalKey(exportItem.PositionExternalKey, _DatabaseService.Context.Positions);
			tableItem.DepartmentUID = GetUIDbyExternalKey(exportItem.DepartmentExternalKey, _DatabaseService.Context.Departments);
			tableItem.EscortUID = GetUIDbyExternalKey(exportItem.EscortExternalKey, _DatabaseService.Context.Employees);
		}
		
        //protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(ExportFilter filter)
        //{
        //    //return new Expression<Func<DataAccess.Employee, bool>>; base.IsInFilter(filter).And(x => x.OrganisationUID == filter.OrganisationUID);
        //}

        public override void TranslateBack(FiresecAPI.SKD.ExportEmployee exportItem, Employee tableItem)
		{
			tableItem.FirstName = exportItem.FirstName;
			tableItem.SecondName = exportItem.SecondName;
			tableItem.LastName = exportItem.LastName;
			tableItem.CredentialsStartDate = DbServiceHelper.CheckDate(exportItem.CredentialsStartDate);
			tableItem.DocumentNumber = exportItem.DocumentNumber;
            tableItem.BirthDate = DbServiceHelper.CheckDate(tableItem.BirthDate);
			tableItem.BirthPlace = tableItem.BirthPlace;
            tableItem.DocumentGivenDate = DbServiceHelper.CheckDate(exportItem.DocumentGivenDate);
			tableItem.DocumentGivenBy = exportItem.DocumentGivenBy;
            tableItem.DocumentValidTo = DbServiceHelper.CheckDate(exportItem.DocumentValidTo);
			tableItem.DocumentDepartmentCode = exportItem.DocumentDepartmentCode;
			tableItem.Citizenship = exportItem.Citizenship;
			tableItem.Description = exportItem.Description;
			tableItem.Gender = exportItem.Gender;
			tableItem.Type = exportItem.Type;
            tableItem.LastEmployeeDayUpdate = DbServiceHelper.CheckDate(exportItem.LastEmployeeDayUpdate);
            tableItem.ScheduleStartDate = DbServiceHelper.CheckDate(exportItem.ScheduleStartDate);
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


