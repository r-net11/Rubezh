using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using GKWebService.Models;

namespace GKWebService.Controllers
{
    public class EmployeesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetOrganisations()
        {
            var employeeModels = new List<EmployeeModel>();
            var organisationFilter = new OrganisationFilter ();
            var organisations = OrganisationHelper.Get(organisationFilter);
            employeeModels.AddRange(InitializeOrganisations(organisations));
            var employeeFilter = new EmployeeFilter();
            var employees = EmployeeHelper.Get(employeeFilter);
            employeeModels.AddRange(InitializeEmployees(employees, employeeModels));

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = employeeModels,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<EmployeeModel> InitializeEmployees(IEnumerable<ShortEmployee> employees, IEnumerable<EmployeeModel> organisations)
        {
            return employees.Select(e => EmployeeModel.CreateFromModel(e, organisations)).ToList();
        }

        private IEnumerable<EmployeeModel> InitializeOrganisations(IEnumerable<Organisation> organisations)
        {
            return organisations.Select(EmployeeModel.CreateFromOrganisation).ToList();
        }
    }
}