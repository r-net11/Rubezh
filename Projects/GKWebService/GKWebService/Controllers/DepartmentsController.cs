using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using GKWebService.Models.SKD.Common;
using GKWebService.Models.SKD.Departments;
using GKWebService.Utils;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Controllers
{
	[Authorize]
	public class DepartmentsController : Controller
    {
        // GET: Departments
        public ActionResult Index()
        {
            return View();
        }

        [ErrorHandler]
        public JsonResult GetOrganisations(DepartmentFilter departmentFilter)
        {
            var departmentViewModel = new DepartmentsViewModel();
            departmentViewModel.Initialize(new DepartmentFilter
            {
                OrganisationUIDs = departmentFilter.OrganisationUIDs,
                LogicalDeletationType = departmentFilter.LogicalDeletationType
            });
            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = departmentViewModel.Organisations,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DepartmentDetails()
        {
            return View();
        }

        [ErrorHandler]
        public JsonNetResult GetDepartmentDetails(Guid? organisationId, Guid? id, Guid? parentDepartmentId)
        {
            var departmentModel = new DepartmentDetailsViewModel()
            {
                Department = new Department()
            };

            if (!organisationId.HasValue)
            {
                return new JsonNetResult { Data = departmentModel };
            }

            departmentModel.Initialize(organisationId, id, parentDepartmentId);

            return new JsonNetResult { Data = departmentModel };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult DepartmentDetails(DepartmentDetailsViewModel departmentModel, bool isNew)
        {
            var error = departmentModel.Save(isNew);
            return new JsonNetResult {Data = error };
        }

        public ActionResult DepartmentEmployeeList()
        {
            return View();
        }

        [ErrorHandler]
        public JsonResult GetDepartmentEmployeeList(Guid departmentId, Guid organisationId, bool isWithDeleted, Guid chiefId)
        {
            var filter = new EmployeeFilter
            {
                DepartmentUIDs = new List<Guid> { departmentId },
                OrganisationUIDs = new List<Guid> { organisationId },
                LogicalDeletationType = isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active
            };
            var operationResult = EmployeeHelper.Get(filter);

            var employees = operationResult.Select(e => ShortEmployeeModel.CreateFromModel(e)).ToList();

            var chief = employees.FirstOrDefault(e => e.UID == chiefId);
            if (chief != null)
            {
                chief.IsChief = true;
            }

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = employees,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ErrorHandler]
        public JsonNetResult GetChildEmployeeUIDs(Guid departmentId)
        {
            var operationResult = DepartmentHelper.GetChildEmployeeUIDs(departmentId);

            return new JsonNetResult { Data = operationResult};
        }

        [HttpPost]
        [ErrorHandler]
        public JsonResult SaveEmployeeDepartment(Guid employeeUID, Guid? departmentUID, string name)
        {
            var operationResult = EmployeeHelper.SetDepartment(employeeUID, departmentUID, name);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ErrorHandler]
        public JsonResult SaveDepartmentChief(Guid departmentUID, Guid? employeeUID, string name)
        {
            var result = DepartmentHelper.SaveChief(departmentUID, employeeUID, name);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult MarkDeleted(Guid uid)
        {
            var getDepartmentsResult = DepartmentHelper.Get(new DepartmentFilter {UIDs = new List<Guid> { uid } });
            var department = getDepartmentsResult.FirstOrDefault();

            var operationResult = DepartmentHelper.MarkDeleted(department);
            return new JsonNetResult { Data = operationResult };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult Restore(Guid uid)
        {
            var filter = new DepartmentFilter
            {
                UIDs = new List<Guid> { uid },
                LogicalDeletationType = LogicalDeletationType.All
            };
            var getDepartmentsResult = DepartmentHelper.Get(filter);
            var department = getDepartmentsResult.FirstOrDefault();

            var operationResult = DepartmentHelper.Restore(department);
            return new JsonNetResult { Data = operationResult };
        }
    }
}