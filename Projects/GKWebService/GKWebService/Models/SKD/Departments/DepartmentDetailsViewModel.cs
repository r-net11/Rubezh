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

        public ShortDepartment SelectedDepartment { get; set; }

        public bool IsDepartmentSelected { get; set; }

        public ShortEmployeeModel SelectedChief { get; set; }

        public bool IsChiefSelected { get; set; }
    }
}