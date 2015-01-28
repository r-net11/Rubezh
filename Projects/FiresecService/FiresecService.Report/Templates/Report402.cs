using System;
using FiresecAPI;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;

namespace FiresecService.Report.Templates
{
    public partial class Report402 : BaseReport
    {
        public Report402()
        {
            InitializeComponent();
        }

        public override string ReportTitle
        {
            get { return "Маршрут сотрудника/посетителя"; }
        }
        protected override DataSet CreateDataSet(DataProvider dataProvider)
        {
            var filter = GetFilter<ReportFilter402>();

            var ds = new DataSet402();
            var employees = dataProvider.GetEmployees(filter);

            var zoneUIDs = new List<Guid>();
            if (filter.Zones != null)
                zoneUIDs.AddRange(filter.Zones);
            if (filter.Doors != null)
                foreach (var doorUID in filter.Doors)
                {
                    var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
                    if (door != null)
                    {
                        if (door.InDevice != null && door.InDevice.ZoneUID != Guid.Empty)
                            zoneUIDs.Add(door.InDevice.ZoneUID);
                        if (door.OutDevice != null && door.OutDevice.ZoneUID != Guid.Empty)
                            zoneUIDs.Add(door.OutDevice.ZoneUID);
                    }
                }
            var organisations = dataProvider.DatabaseService.OrganisationTranslator.Get(new OrganisationFilter());

            var result = new List<SKDDriver.DataAccess.PassJournal>();
            foreach (var employee in employees)
            {
                var employeeRow = ds.Employee.NewEmployeeRow();
                employeeRow.UID = employee.UID;
                employeeRow.Name = employee.Name;
                employeeRow.Department = employee.Department;
                employeeRow.Position = employee.Position;
                employeeRow.Organisation = employee.Organisation;
                ds.Employee.AddEmployeeRow(employeeRow);

                if (dataProvider.DatabaseService.PassJournalTranslator != null)
                {
                    var passJournal2 = dataProvider.DatabaseService.PassJournalTranslator.GetEmployeeRoot(employee.UID, zoneUIDs, filter.DateTimeFrom, filter.DateTimeTo);
                    if (passJournal2 != null)
                        foreach (var pass in passJournal2)
                        {
                            var row = ds.Data.NewDataRow();
                            row.EmployeeRow = employeeRow;
                            //row.Zone;
                            //row.PassCard;
                            //row.Door;
                            row.DateTime = pass.EnterTime;
                            ds.Data.AddDataRow(row);
                            if (pass.ExitTime.HasValue)
                            {
                                var row2 = ds.Data.NewDataRow();
                                row2.ItemArray = row.ItemArray;
                                row2.DateTime = pass.ExitTime.Value;
                                ds.Data.AddDataRow(row2);
                            }
                        }
                }
            }
            return ds;
        }
    }
}
