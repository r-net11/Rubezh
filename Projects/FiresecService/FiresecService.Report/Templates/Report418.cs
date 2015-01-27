using System;
using System.Collections.Generic;
using System.Data;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using SKDDriver;

namespace FiresecService.Report.Templates
{
    public partial class Report418 : BaseReport
    {
        public Report418()
        {
            InitializeComponent();
        }

        public override string ReportTitle
        {
            get { return "Справка о сотруднике/посетителе"; }
        }
        protected override DataSet CreateDataSet(DataProvider dataProvider)
        {
            var filter = GetFilter<ReportFilter418>();
            dataProvider.LoadCache();
            if (filter.Employees == null)
                filter.Employees = new List<Guid>();
            if (filter.Departments == null)
                filter.Departments = new List<Guid>();
            if (filter.Positions == null)
                filter.Positions = new List<Guid>();
            if (filter.Organisations == null)
                filter.Organisations = new List<Guid>();
            var employeeFilter = new EmployeeFilter();
            employeeFilter.OrganisationUIDs = filter.Organisations;
            employeeFilter.DepartmentUIDs = filter.Departments;
            employeeFilter.PositionUIDs = filter.Positions;
            employeeFilter.UIDs = filter.Employees;
            var employeesResult = dataProvider.DatabaseService.EmployeeTranslator.Get(employeeFilter);
            var dataSet = new DataSet418();
            if (employeesResult.Result != null)
            {
                foreach (var employee in employeesResult.Result)
                {
                    var dataRow = dataSet.Data.NewDataRow();
                    dataRow.Document = employee.DocumentType.ToDescription();
                    dataRow.DocumentNumber = employee.DocumentNumber;
                    dataRow.FirstName = employee.FirstName;
                    dataRow.SecondName = employee.SecondName;
                    dataRow.Sex = employee.Gender.ToDescription();
                    if (employee.Photo != null)
                        dataRow.Photo = employee.Photo.Data;
                    dataRow.Organisation = dataProvider.Organisations[employee.OrganisationUID].Name;
                    if (employee.Department != null)
                        dataRow.Department = employee.Department.Name;
                    if (employee.Position != null)
                        dataRow.Position = employee.Position.Name;
                    dataSet.Data.Rows.Add(dataRow);
                }
            }
            return dataSet;
        }
    }
}