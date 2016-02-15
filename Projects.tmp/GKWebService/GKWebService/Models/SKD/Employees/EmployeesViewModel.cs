using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using GKWebService.Models.SKD.Common;
using RubezhAPI.SKD;

namespace GKWebService.Models.SKD.Employees
{
    public class EmployeesViewModel : OrganisationBaseViewModel<ShortEmployee, EmployeeFilter, EmployeeViewModel>
    {
        protected override IEnumerable<ShortEmployee> GetModels(EmployeeFilter filter)
        {
            return EmployeeHelper.Get(filter);
        }
    }
}