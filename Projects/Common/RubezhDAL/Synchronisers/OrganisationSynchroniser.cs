using RubezhAPI;
using RubezhDAL.DataClasses;
using System;
using System.Data.Entity;
using System.Linq;

namespace RubezhDAL
{
	public class OrganisationSynchroniserBase : Synchroniser<RubezhAPI.SKD.ExportOrganisation, Organisation>
	{
		public OrganisationSynchroniserBase(DbSet<Organisation> table, DbService databaseService) : base(table, databaseService) { }

		public override RubezhAPI.SKD.ExportOrganisation Translate(Organisation item)
		{
			return new RubezhAPI.SKD.ExportOrganisation
			{
				Name = item.Name,
				Description = item.Description,
				Phone = item.Phone,
				MaxGKLevel = item.MaxGKLevel,

				ChiefUID = GetUID(item.ChiefUID),
				ChiefExternalKey = GetExternalKey(item.ChiefUID, item.Chief),
				HRChiefUID = GetUID(item.HRChiefUID),
				HRChiefExternalKey = GetExternalKey(item.HRChiefUID, item.HRChief),
			};
		}

		protected override void UpdateForignKeys(RubezhAPI.SKD.ExportOrganisation exportItem, Organisation tableItem, OrganisationHRCash hrCash)
		{
			tableItem.ChiefUID = GetUIDbyExternalKey(exportItem.ChiefExternalKey, hrCash.Employees);
			tableItem.HRChiefUID = GetUIDbyExternalKey(exportItem.HRChiefExternalKey, hrCash.Employees);
		}

		public override void TranslateBack(RubezhAPI.SKD.ExportOrganisation exportItem, Organisation tableItem)
		{
			tableItem.Name = exportItem.Name;
			tableItem.Description = exportItem.Description;
			tableItem.Phone = exportItem.Phone;
			tableItem.IsDeleted = exportItem.IsDeleted;
			tableItem.RemovalDate = exportItem.RemovalDate;
		}

		protected override string Name
		{
			get { throw new NotImplementedException(); }
		}

		protected override string XmlHeaderName
		{
			get { throw new NotImplementedException(); }
		}
	}

	public class OrganisationSynchroniser : OrganisationSynchroniserBase
	{
		public OrganisationSynchroniser(DbSet<Organisation> table, DbService databaseService) : base(table, databaseService) { }

		protected override IQueryable<Organisation> GetFilteredItems(RubezhAPI.SKD.ExportFilter filter)
		{
			return base.GetFilteredItems(filter).Where(x => x.UID == filter.OrganisationUID);
		}

		EmployeeSynchroniser EmployeeSynchroniser { get { return _DatabaseService.EmployeeTranslator.Synchroniser; } }
		PositionSynchroniser PositionSynchroniser { get { return _DatabaseService.PositionTranslator.Synchroniser; } }
		DepartmentSynchroniser DepartmentSynchroniser { get { return _DatabaseService.DepartmentTranslator.Synchroniser; } }
		Guid OrganisationUID;

		public override OperationResult Export(RubezhAPI.SKD.ExportFilter filter)
		{
			try
			{
				var organisationResult = base.Export(filter);
				if (organisationResult.HasError)
					return organisationResult;
				var employeeResult = EmployeeSynchroniser.Export(filter);
				if (employeeResult.HasError)
					return employeeResult;
				var PositionResult = PositionSynchroniser.Export(filter);
				if (PositionResult.HasError)
					return PositionResult;
				var DepartmentResult = DepartmentSynchroniser.Export(filter);
				if (DepartmentResult.HasError)
					return DepartmentResult;
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public override OperationResult Import(RubezhAPI.SKD.ImportFilter filter)
		{
			try
			{
				base.Import(filter);
				PositionSynchroniser.Import(filter);
				DepartmentSynchroniser.Import(filter);
				EmployeeSynchroniser.Import(filter);
				var hrCash = new OrganisationHRCash
				{
					OrganisationUID = OrganisationUID,
					Employees = Context.Employees.Where(x => filter.IsWithDeleted || !x.IsDeleted).ToList(),
					Departments = Context.Departments.Where(x => filter.IsWithDeleted || !x.IsDeleted).ToList(),
					Positions = Context.Positions.Where(x => filter.IsWithDeleted || !x.IsDeleted).ToList(),
				};
				ImportForignKeys(hrCash);
				DepartmentSynchroniser.ImportForignKeys(hrCash);
				EmployeeSynchroniser.ImportForignKeys(hrCash);
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected override string XmlHeaderName
		{
			get { return "ExportOrganisation"; }
		}

		protected override string Name
		{
			get { return "Organisation"; }
		}
		protected override void BeforeSave(System.Collections.Generic.List<RubezhAPI.SKD.ExportOrganisation> exportItems)
		{
			base.BeforeSave(exportItems);
			OrganisationUID = exportItems.FirstOrDefault().UID;
		}
	}

	public class OrgansiationListSynchroniser : OrganisationSynchroniserBase
	{
		public OrgansiationListSynchroniser(DbSet<Organisation> table, DbService databaseService) : base(table, databaseService) { }

		protected override string Name
		{
			get { return "OrgansiationList"; }
		}

		protected override string XmlHeaderName
		{
			get { return "ExportOrganisationList"; }
		}
	}
}