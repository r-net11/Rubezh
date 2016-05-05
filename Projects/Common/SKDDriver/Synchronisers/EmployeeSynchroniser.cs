using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Data.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public class EmployeeSynchroniser : Synchroniser<ExportEmployee, DataAccess.Employee>
	{
		public EmployeeSynchroniser(Table<DataAccess.Employee> table, SKDDatabaseService databaseService)
			: base(table, databaseService)
		{
		}

		public override ExportEmployee Translate(DataAccess.Employee item)
		{
			return new ExportEmployee
			{
				FirstName = item.FirstName,
				SecondName = item.SecondName,
				LastName = item.LastName,
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
				Type = item.Type != null ? item.Type.Value : -1,
				LastEmployeeDayUpdate = item.LastEmployeeDayUpdate,
				ScheduleStartDate = item.ScheduleStartDate,

				OrganisationUID = GetUID(item.OrganisationUID),
				OrganisationExternalKey = GetExternalKey(item.OrganisationUID, item.Organisation),
				PositionUID = GetUID(item.PositionUID),
				PositionExternalKey = GetExternalKey(item.PositionUID, item.Position),
				DepartmentUID = GetUID(item.DepartmentUID),
				DepartmentExternalKey = GetExternalKey(item.DepartmentUID, item.Department),
				EscrortUID = GetUID(item.EscortUID),
				EscortExternalKey = GetExternalKey(item.EscortUID, item.Employee1)
			};
		}

		protected override void UpdateForignKeys(ExportEmployee exportItem, DataAccess.Employee tableItem)
		{
			tableItem.OrganisationUID = GetUIDbyExternalKey(exportItem.OrganisationExternalKey, _DatabaseService.Context.Organisations);
			tableItem.PositionUID = GetUIDbyExternalKey(exportItem.PositionExternalKey, _DatabaseService.Context.Positions);
			tableItem.DepartmentUID = GetUIDbyExternalKey(exportItem.DepartmentExternalKey, _DatabaseService.Context.Departments);
			tableItem.EscortUID = GetUIDbyExternalKey(exportItem.EscortExternalKey, _DatabaseService.Context.Employees);
		}

		protected override Expression<Func<DataAccess.Employee, bool>> IsInFilter(ExportFilter filter)
		{
			return base.IsInFilter(filter).And(x => x.OrganisationUID == filter.OrganisationUID);
		}

		public override void TranslateBack(ExportEmployee exportItem, DataAccess.Employee tableItem)
		{
			tableItem.FirstName = exportItem.FirstName;
			tableItem.SecondName = exportItem.SecondName;
			tableItem.LastName = exportItem.LastName;
			tableItem.DocumentNumber = exportItem.DocumentNumber;
			tableItem.BirthDate = tableItem.BirthDate;
			tableItem.BirthPlace = tableItem.BirthPlace;
			tableItem.DocumentGivenDate = exportItem.DocumentGivenDate;
			tableItem.DocumentGivenBy = exportItem.DocumentGivenBy;
			tableItem.DocumentValidTo = exportItem.DocumentValidTo;
			tableItem.DocumentDepartmentCode = exportItem.DocumentDepartmentCode;
			tableItem.Citizenship = exportItem.Citizenship;
			tableItem.Description = exportItem.Description;
			tableItem.Gender = exportItem.Gender;
			tableItem.Type = exportItem.Type;
			tableItem.LastEmployeeDayUpdate = TranslatiorHelper.CheckDate(exportItem.LastEmployeeDayUpdate);
			tableItem.ScheduleStartDate = TranslatiorHelper.CheckDate(exportItem.ScheduleStartDate);
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