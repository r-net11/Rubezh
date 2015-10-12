using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using GKWebService.Models;
using GKWebService.Utils;

namespace GKWebService.Controllers
{
    public class EmployeesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmployeeDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmployeeDetails( Employee employee)
        {
			var operationResult = ClientManager.FiresecService.SaveEmployee(employee, employee.UID == Guid.Empty);

            return Json(new { Status = operationResult });
        }

        public JsonResult GetOrganisations()
        {
            var employeeModels = new List<ShortEmployeeModel>();
            var organisationFilter = new OrganisationFilter ();
			var organisations = ClientManager.FiresecService.GetOrganisations(organisationFilter).Result;
            employeeModels.AddRange(InitializeOrganisations(organisations));
            var employeeFilter = new EmployeeFilter();
			var employees = ClientManager.FiresecService.GetEmployeeList(employeeFilter).Result;
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

        public JsonNetResult GetEmployeeDetails(Guid? id)
        {
            Employee employee = (id.HasValue ? EmployeeHelper.GetDetails(id) : new Employee());
            employee.Photo = null;
            employee.AdditionalColumns.ForEach(c => c.Photo = null);
            return new JsonNetResult {Data = employee};
        }

        private IEnumerable<ShortEmployeeModel> InitializeEmployees(IEnumerable<ShortEmployee> employees, IEnumerable<ShortEmployeeModel> organisations)
        {
            return employees.Select(e => ShortEmployeeModel.CreateFromModel(e, organisations)).ToList();
        }

        private IEnumerable<ShortEmployeeModel> InitializeOrganisations(IEnumerable<Organisation> organisations)
        {
            return organisations.Select(ShortEmployeeModel.CreateFromOrganisation).ToList();
        }
    }
}