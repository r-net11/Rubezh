using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecService.Report.Model
{
    public class EmployeeInfo : OrganisationBaseObjectInfo
    {
        public Guid? DepartmentUID { get; set; }
        public string Department { get; set; }
        public Guid? PositionUID { get; set; }
        public string Position { get; set; }
    }
}
