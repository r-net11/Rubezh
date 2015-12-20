using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using GKWebService.Utils;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Controllers
{
    public class HrController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Departments()
        {
            return View();
        }

        public ActionResult Positions()
        {
            return View();
        }

        public ActionResult AdditionalColumnTypes()
        {
            return View();
        }

        public ActionResult Cards()
        {
            return View();
        }

        public ActionResult AccessTemplates()
        {
            return View();
        }

        public ActionResult PassCardTemplates()
        {
            return View();
        }

        public ActionResult Organisations()
        {
            return View();
        }

        public ActionResult EmployeeSelectionDialog()
        {
            return View();
        }

        public JsonNetResult GetDepartmentEmployees(Guid id)
        {
            var filter = new EmployeeFilter {DepartmentUIDs = new List<Guid> {id}};
            return GetEmployees(filter);
        }

        public JsonNetResult GetOrganisationEmployees(Guid id)
        {
            var filter = new EmployeeFilter {OrganisationUIDs = new List<Guid> {id}};
            return GetEmployees(filter);
        }

        public JsonNetResult GetEmptyDepartmentEmployees(Guid id)
        {
            var filter = new EmployeeFilter
            {
                OrganisationUIDs = new List<Guid> {id},
                IsEmptyDepartment = true
            };

            return GetEmployees(filter);
        }

        public JsonNetResult GetEmptyPositionEmployees(Guid id)
        {
            var filter = new EmployeeFilter
            {
                OrganisationUIDs = new List<Guid> {id},
                IsEmptyPosition = true
            };

            return GetEmployees(filter);
        }

        private JsonNetResult GetEmployees(EmployeeFilter filter)
        {
            var operationResult = ClientManager.FiresecService.GetEmployeeList(filter);
            if (operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }

            var employees = operationResult.Result.Select(e => ShortEmployeeModel.CreateFromModel(e));

            return new JsonNetResult { Data = new { Employees = employees } };
        }

        public ActionResult HrFilter()
		{
			return View();
		}

        public ActionResult OrganisationsFilter()
		{
			return View();
		}

        public JsonResult GetOrganisationsFilter(bool isWithDeleted)
        {
            var filter = new OrganisationFilter { UserUID = ClientManager.CurrentUser.UID };
            if (isWithDeleted)
            {
                filter.LogicalDeletationType = LogicalDeletationType.All;
            }

            var organisationsResult = ClientManager.FiresecService.GetOrganisations(filter);
            if (organisationsResult.HasError)
            {
                throw new InvalidOperationException(organisationsResult.Error);
            }

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = organisationsResult.Result,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DepartmentsFilter()
		{
			return View();
		}

        public ActionResult PositionsFilter()
		{
			return View();
		}

        public ActionResult EmployeesFilter()
		{
			return View();
		}

		public JsonNetResult GetFilter(Guid? id)
		{
			return new JsonNetResult { Data = new EmployeeFilter() };
		}


	}
}