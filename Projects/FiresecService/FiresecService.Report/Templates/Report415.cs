using Common;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using SKDDriver;

namespace FiresecService.Report.Templates
{
    public partial class Report415 : BaseReport
    {
        private DepartmentFilter _filter;

        public Report415()
        {
            InitializeComponent();
        }

        public override string ReportTitle
        {
            get { return "415. Отчет \"Список отделов организации\""; }
        }

        public override void ApplyFilter(SKDReportFilter filter)
        {
            base.ApplyFilter(filter);
            var reportFilter = (ReportFilter415)filter;
            _filter = new DepartmentFilter()
            {
                OrganisationUIDs = reportFilter.Organisations,
                UIDs = reportFilter.Departments,
            };
        }

        protected override void DataSourceRequered()
        {
//select Department.Name as Department, Department.Description as Description, Department.Phone as Phone,  Employee.LastName as Chief,  Department_1.Name as ParentDepartment
//  from ((dbo.Department Department
//  left outer join dbo.Employee Employee on (Employee.UID = Department.ChiefUID))
//  left outer join dbo.Department Department_1 on (Department_1.UID = Department.ParentDepartmentUID))
            base.DataSourceRequered();
            using (var service = new SKDDatabaseService())
            {
                var result = service.DepartmentTranslator.GetList(_filter);
                if (result != null && !result.HasError && result.Result != null)
                {
                    var ds = new DataSet415();
                    result.Result.ForEach(item => ds.Data.AddDataRow(item.Name, item.Phone, item.ChiefUID.ToString(), item.ParentDepartmentUID.ToString(), item.Description));
                    DataSource = ds;
                }
            }
        }
    }
}
