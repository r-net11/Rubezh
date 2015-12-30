using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using GKWebService.Models.SKD.Common;
using GKWebService.Models.SKD.Departments;
using GKWebService.Utils;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Controllers
{
    public class DepartmentsController : Controller
    {
        // GET: Departments
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
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
        public JsonNetResult DepartmentDetails(DepartmentDetailsViewModel departmentModel, bool isNew)
        {
            var error = departmentModel.Save(isNew);
            return new JsonNetResult {Data = error };
        }

        public ActionResult DepartmentEmployeeList()
        {
            return View();
        }

        public JsonResult GetDepartmentEmployeeList(Guid departmentId, Guid organisationId, bool isWithDeleted, Guid chiefId)
        {
            var filter = new EmployeeFilter
            {
                DepartmentUIDs = new List<Guid> { departmentId },
                OrganisationUIDs = new List<Guid> { organisationId },
                LogicalDeletationType = isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active
            };
            var operationResult = ClientManager.FiresecService.GetEmployeeList(filter);
            if (operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }

            var employees = operationResult.Result.Select(e => ShortEmployeeModel.CreateFromModel(e)).ToList();

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
        public JsonNetResult GetChildEmployeeUIDs(Guid departmentId)
        {
            var operationResult = ClientManager.FiresecService.GetChildEmployeeUIDs(departmentId);

            if (operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }

            return new JsonNetResult { Data = operationResult.Result};
        }

        [HttpPost]
        public JsonResult SaveEmployeeDepartment(Guid employeeUID, Guid? departmentUID, string name)
        {
            var operationResult = ClientManager.FiresecService.SaveEmployeeDepartment(employeeUID, departmentUID, name);

            return Json(operationResult.HasError, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDepartmentChief(Guid departmentUID, Guid? employeeUID, string name)
        {
            var result = ClientManager.FiresecService.SaveDepartmentChief(departmentUID, employeeUID, name);

            return Json(result.HasError, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonNetResult MarkDeleted(Guid uid)
        {
            var getDepartmentsResult = ClientManager.FiresecService.GetDepartmentList(new DepartmentFilter {UIDs = new List<Guid> { uid } });
            if (getDepartmentsResult.HasError)
            {
                throw new InvalidOperationException(getDepartmentsResult.Error);
            }
            var department = getDepartmentsResult.Result.FirstOrDefault();

            var operationResult = ClientManager.FiresecService.MarkDeletedDepartment(department);
            return new JsonNetResult { Data = operationResult != null && operationResult.HasError && !operationResult.Error.Contains("Ошибка БД") };
        }

        [HttpPost]
        public JsonNetResult Restore(Guid uid)
        {
            var filter = new DepartmentFilter
            {
                UIDs = new List<Guid> { uid },
                LogicalDeletationType = LogicalDeletationType.All
            };
            var getDepartmentsResult = ClientManager.FiresecService.GetDepartmentList(filter);
            if (getDepartmentsResult.HasError)
            {
                throw new InvalidOperationException(getDepartmentsResult.Error);
            }
            var department = getDepartmentsResult.Result.FirstOrDefault();

            var operationResult = ClientManager.FiresecService.RestoreDepartment(department);
            return new JsonNetResult { Data = operationResult != null && operationResult.HasError && !operationResult.Error.Contains("Ошибка БД") };
        }
    }
}