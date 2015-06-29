using System;
using System.Data.Linq;
using System.Linq.Expressions;
using FiresecAPI;
//using FiresecAPI.SKD;
using LinqKit;
using SKDDriver.DataClasses;
using System.Data.Entity;

namespace SKDDriver
{
	public class OrganisationSynchroniser : Synchroniser<FiresecAPI.SKD.ExportOrganisation, Organisation>
	{
		public OrganisationSynchroniser(DbSet<Organisation> table, DbService databaseService) : base(table, databaseService) { }

		public void Initialize()
		{
			ListSynchroniser = new OrgansiationListSynchroniser(_Table, _DatabaseService);
		}

        public override FiresecAPI.SKD.ExportOrganisation Translate(Organisation item)
		{
            return new FiresecAPI.SKD.ExportOrganisation
			{
				Name = item.Name,
				Description = item.Description,
				Phone = item.Phone,

				ChiefUID = GetUID(item.ChiefUID),
				ChiefExternalKey = GetExternalKey(item.ChiefUID, item.Chief),
				HRChiefUID = GetUID(item.HRChiefUID),
				HRChiefExternalKey = GetExternalKey(item.HRChiefUID, item.HRChief),
			};
		}

        //protected override Expression<Func<DataAccess.Organisation, bool>> IsInFilter(FiresecAPI.SKD.ExportFilter filter)
        //{
        //    return base.IsInFilter(filter).And(x => x.UID == filter.OrganisationUID);
        //}

		//EmployeeSynchroniser EmployeeSynchroniser { get { return _DatabaseService.EmployeeTranslator1.Synchroniser; } }
		//PositionSynchroniser PositionSynchroniser { get { return _DatabaseService.PositionTranslator.Synchroniser; } }
		//DepartmentSynchroniser DepartmentSynchroniser { get { return _DatabaseService.DepartmentTranslator.Synchroniser; } }
		public OrgansiationListSynchroniser ListSynchroniser;

        public override OperationResult Export(FiresecAPI.SKD.ExportFilter filter)
		{
			try
			{
                //var organisationResult = base.Export(filter);
                //if (organisationResult.HasError)
                //    return organisationResult;
                //var employeeResult = _DatabaseService.EmployeeTranslator.Synchroniser.Export(filter);
                //if (employeeResult.HasError)
                //    return employeeResult;
                //var PositionResult = _DatabaseService.PositionTranslator.Synchroniser.Export(filter);
                //if (PositionResult.HasError)
                //    return PositionResult;
                //var DepartmentResult = _DatabaseService.DepartmentTranslator.Synchroniser.Export(filter);
                //if (DepartmentResult.HasError)
                //    return DepartmentResult;
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

        public override OperationResult Import(FiresecAPI.SKD.ImportFilter filter)
		{
			try
			{
				base.Import(filter);
				//PositionSynchroniser.Import(filter);
				//DepartmentSynchroniser.Import(filter);
				//EmployeeSynchroniser.Import(filter);
				ImportForignKeys();
				//DepartmentSynchroniser.ImportForignKeys();
				//EmployeeSynchroniser.ImportForignKeys();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

        protected override void UpdateForignKeys(FiresecAPI.SKD.ExportOrganisation exportItem, Organisation tableItem)
		{
			tableItem.ChiefUID = GetUIDbyExternalKey(exportItem.ChiefExternalKey, _DatabaseService.Context.Employees);
			tableItem.HRChiefUID = GetUIDbyExternalKey(exportItem.HRChiefExternalKey, _DatabaseService.Context.Employees);
		}

        public override void TranslateBack(FiresecAPI.SKD.ExportOrganisation exportItem, Organisation tableItem)
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

        //protected virtual Expression<Func<DataAccess.Organisation, bool>> IsInFilterList(FiresecAPI.SKD.ExportFilter filter)
        //{
        //    return base.IsInFilter(filter);
        //}

        protected virtual OperationResult ExportList(FiresecAPI.SKD.ExportFilter filter)
		{
			return base.Export(filter);
		}

        protected virtual OperationResult ImportList(FiresecAPI.SKD.ImportFilter filter)
		{
			return base.Import(filter);
		}
		#endregion
	}

	public class OrgansiationListSynchroniser : OrganisationSynchroniser
	{
		public OrgansiationListSynchroniser(DbSet<Organisation> table, DbService databaseService) : base(table, databaseService) { }

        //protected override Expression<Func<DataAccess.Organisation, bool>> IsInFilter(FiresecAPI.SKD.ExportFilter filter)
        //{
        //    return base.IsInFilterList(filter);
        //}

        public override OperationResult Export(FiresecAPI.SKD.ExportFilter filter)
		{
			return base.ExportList(filter);
		}

        public override OperationResult Import(FiresecAPI.SKD.ImportFilter filter)
		{
			return base.ImportList(filter);
		}

		protected override string Name
		{
			get { return "OrgansiationList"; }
		}
	}
}