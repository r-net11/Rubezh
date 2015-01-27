using System;
using System.Data.Linq;
using System.IO;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common;
using LinqKit;

namespace SKDDriver
{
	public class OrganisationSynchroniser : Synchroniser<ExportOrganisation, DataAccess.Organisation>
	{
		public OrganisationSynchroniser(Table<DataAccess.Organisation> table, SKDDatabaseService databaseService) : base(table, databaseService) { }

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

		EmployeeSynchroniser EmployeeSynchroniser { get { return _DatabaseService.EmployeeTranslator.Synchroniser; } }
		PositionSynchroniser PositionSynchroniser { get { return _DatabaseService.PositionTranslator.Synchroniser; } }
		DepartmentSynchroniser DepartmentSynchroniser { get { return _DatabaseService.DepartmentTranslator.Synchroniser; } }

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
				var zipName = "Export.zip";
				ZipHelper.IntoZip(NameXml, zipName);
				ZipHelper.IntoZip(EmployeeSynchroniser.NameXml, zipName);
				ZipHelper.IntoZip(PositionSynchroniser.NameXml, zipName);
				ZipHelper.IntoZip(DepartmentSynchroniser.NameXml, zipName);
				File.Delete(NameXml);
				File.Delete(EmployeeSynchroniser.NameXml);
				File.Delete(PositionSynchroniser.NameXml);
				File.Delete(DepartmentSynchroniser.NameXml);
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}

		}

		public OperationResult Import(string zipName)
		{
			try
			{
				Import(ZipHelper.FromZip(NameXml, zipName));
				PositionSynchroniser.Import(ZipHelper.FromZip(PositionSynchroniser.NameXml, zipName));
				DepartmentSynchroniser.Import(ZipHelper.FromZip(DepartmentSynchroniser.NameXml, zipName));
				EmployeeSynchroniser.Import(ZipHelper.FromZip(EmployeeSynchroniser.NameXml, zipName));
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
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
	}
}
