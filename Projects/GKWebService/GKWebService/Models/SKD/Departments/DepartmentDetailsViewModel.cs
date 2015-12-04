using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.SKD;

namespace GKWebService.Models.SKD.Departments
{
    public class DepartmentDetailsViewModel
    {
        public Department Department { get; set; }

        public Department SelectedDepartment { get; set; }
    }
}