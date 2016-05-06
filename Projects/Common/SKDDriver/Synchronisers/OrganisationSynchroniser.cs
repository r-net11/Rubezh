using StrazhAPI;
using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Data.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public class OrganisationSynchroniser : Synchroniser<ExportOrganisation, DataAccess.Organisation>
	{
		public OrganisationSynchroniser(Table<DataAccess.Organisation> table, SKDDatabaseService databaseService)
			: base(table, databaseService)
		{
		}

		public void Initialize()
		{
			ListSynchroniser = new OrgansiationListSynchroniser(_Table, _DatabaseService);
		}

		public override ExportOrganisation Translate(DataAccess.Organisation item)
		{
			return new ExportOrganisation
			{
				Name = item.Name,
				Description = item.Description,
				Phone = item.Phone,

				ChiefUID = GetUID(item.ChiefUID),
				ChiefExternalKey = GetExternalKey(item.ChiefUID, item.Employee),
				HRChiefUID = GetUID(item.HRChiefUID),
				HRChiefExternalKey = GetExternalKey(item.HRChiefUID, item.Employee1),
			};
		}

		protected override Expression<Func<DataAccess.Organisation, bool>> IsInFilter(ExportFilter filter)
		{
			return base.IsInFilter(filter).And(x => x.UID == filter.OrganisationUID);
		}

		private EmployeeSynchroniser EmployeeSynchroniser { get { return _DatabaseService.EmployeeTranslator.Synchroniser; } }

		private PositionSynchroniser PositionSynchroniser { get { return _DatabaseService.PositionTranslator.Synchroniser; } }

		private DepartmentSynchroniser DepartmentSynchroniser { get { return _DatabaseService.DepartmentTranslator.Synchroniser; } }

		public OrgansiationListSynchroniser ListSynchroniser;

		public override OperationResult Export(ExportFilter filter)
		{
			try
			{
				var organisationResult = base.Export(filter);
				if (organisationResult.HasError)
					return organisationResult;
				var employeeResult = _DatabaseService.EmployeeTranslator.Synchroniser.Export(filter);
				if (employeeResult.HasError)
					return employeeResult;
				var PositionResult = _DatabaseService.PositionTranslator.Synchroniser.Export(filter);
				if (PositionResult.HasError)
					return PositionResult;
				var DepartmentResult = _DatabaseService.DepartmentTranslator.Synchroniser.Export(filter);
				if (DepartmentResult.HasError)
					return DepartmentResult;
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public override OperationResult Import(ImportFilter filter)
		{
			try
			{
				base.Import(filter);
				PositionSynchroniser.Import(filter);
				DepartmentSynchroniser.Import(filter);
				EmployeeSynchroniser.Import(filter);
				ImportForignKeys();
				DepartmentSynchroniser.ImportForignKeys();
				EmployeeSynchroniser.ImportForignKeys();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected override void UpdateForignKeys(ExportOrganisation exportItem, DataAccess.Organisation tableItem)
		{
			tableItem.ChiefUID = GetUIDbyExternalKey(exportItem.ChiefExternalKey, _DatabaseService.Context.Employees);
			tableItem.HRChiefUID = GetUIDbyExternalKey(exportItem.HRChiefExternalKey, _DatabaseService.Context.Employees);
		}

		public override void TranslateBack(ExportOrganisation exportItem, DataAccess.Organisation tableItem)
		{
			tableItem.Name = exportItem.Name;
			tableItem.Description = exportItem.Description;
			tableItem.Phone = exportItem.Phone;
		}

		protected override string XmlHeaderName
		{
			get { return "ArrayOfExportOrganisation"; }
		}

		protected override string Name
		{
			get { return "Organisations"; }
		}

		#region ExportList

		protected virtual Expression<Func<DataAccess.Organisation, bool>> IsInFilterList(ExportFilter filter)
		{
			return base.IsInFilter(filter);
		}

		protected virtual OperationResult ExportList(ExportFilter filter)
		{
			return base.Export(filter);
		}

		protected virtual OperationResult ImportList(ImportFilter filter)
		{
			return base.Import(filter);
		}

		#endregion ExportList
	}

	public class OrgansiationListSynchroniser : OrganisationSynchroniser
	{
		public OrgansiationListSynchroniser(Table<DataAccess.Organisation> table, SKDDatabaseService databaseService)
			: base(table, databaseService)
		{
		}

		protected override Expression<Func<DataAccess.Organisation, bool>> IsInFilter(ExportFilter filter)
		{
			return base.IsInFilterList(filter);
		}

		public override OperationResult Export(ExportFilter filter)
		{
			return base.ExportList(filter);
		}

		public override OperationResult Import(ImportFilter filter)
		{
			return base.ImportList(filter);
		}

		protected override string Name
		{
			get { return "OrgansiationList"; }
		}
	}
}