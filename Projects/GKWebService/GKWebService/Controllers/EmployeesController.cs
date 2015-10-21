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
		public JsonNetResult EmployeeDetails(Employee employee)
        {
			var operationResult = ClientManager.FiresecService.SaveEmployee(employee, employee.UID == Guid.Empty);

			return new JsonNetResult { Data = operationResult.Result };
        }

        [HttpPost]
		public JsonNetResult SaveChief(SaveChiefParams @params)
        {
			var result = ClientManager.FiresecService.SaveOrganisationChief(@params.OrganisationUID, @params.EmployeeUID, @params.OrganisationName);

			return new JsonNetResult { Data = !result.HasError };
        }

        [HttpPost]
		public JsonNetResult SaveHRChief(SaveChiefParams @params)
        {
			var result = ClientManager.FiresecService.SaveOrganisationHRChief(@params.OrganisationUID, @params.EmployeeUID, @params.OrganisationName);

			return new JsonNetResult { Data = !result.HasError };
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
            Employee employee;
	        if (id.HasValue)
	        {
		        employee = EmployeeHelper.GetDetails(id);
	        }
	        else
	        {
		        employee = new Employee();
				employee.BirthDate = DateTime.Now;
				employee.CredentialsStartDate = DateTime.Now;
				employee.DocumentGivenDate = DateTime.Now;
				employee.DocumentValidTo = DateTime.Now;
				employee.RemovalDate = DateTime.Now;
				employee.ScheduleStartDate = DateTime.Now;
			}
	        employee.Photo = null;
            employee.AdditionalColumns.ForEach(c => c.Photo = null);
            return new JsonNetResult {Data = employee};
        }

        public JsonNetResult GetOrganisation(Guid? id)
        {
			var filter = new OrganisationFilter();
			filter.UIDs.Add(id.Value);
			var operationResult = ClientManager.FiresecService.GetOrganisations(filter);
			return new JsonNetResult { Data = operationResult.Result.FirstOrDefault() };
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

	public class SaveChiefParams
	{
		public Guid OrganisationUID { get; set; }
		public Guid? EmployeeUID { get; set; }
		public string OrganisationName { get; set; }
	}
}