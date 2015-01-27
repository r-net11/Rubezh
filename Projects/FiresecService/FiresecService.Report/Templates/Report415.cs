using Common;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using SKDDriver;
using DevExpress.XtraReports.UI;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FiresecService.Report.Templates
{
    public partial class Report415 : BaseReport
	{
		public Report415()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
            get { return "Список подразделений организации"; }
		}
        protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
            var filter = GetFilter<ReportFilter415>();
            var databaseService = new SKDDatabaseService();
            dataProvider.LoadCache();
            if (filter.Organisations.IsEmpty())
                filter.Organisations = new List<Guid>() { dataProvider.Organisations.First(org => !org.Value.IsDeleted).Value.UID };

            var departmentFilter = new DepartmentFilter()
            {
                OrganisationUIDs = filter.Organisations ?? new List<Guid>(),
                UIDs = filter.Departments ?? new List<Guid>()
            };
            var departments = dataProvider.DatabaseService.DepartmentTranslator.Get(departmentFilter);
            var ds = new DataSet415();
            if (departments.Result != null)
                departments.Result.ForEach(department =>
                {
                    var row = ds.Data.NewDataRow();
                    row.Department = department.Name;
                    row.Phone = department.Phone;
                    //row.Chief = department.ChiefUID;
                    row.ParentDepartment = department.ParentDepartmentUID.HasValue ? dataProvider.Departments[department.ParentDepartmentUID.Value].Name : string.Empty;
                    row.Description = department.Description;
                    ds.Data.AddDataRow(row);
                });
			return ds;
		}
	}
}
