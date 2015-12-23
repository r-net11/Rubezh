﻿using System;
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

            if (id.HasValue)
            {
                var departmentDetailsResult = ClientManager.FiresecService.GetDepartmentDetails(id.Value);
                departmentModel.Department = departmentDetailsResult.Result;
            }
            else
            {
                departmentModel.Department = new Department
                {
                    Name = "Новое подразделение",
                    ParentDepartmentUID = parentDepartmentId ?? Guid.Empty,
                    OrganisationUID = organisationId.Value
                };
            }

            var filter = new DepartmentFilter();
            filter.UIDs.Add(departmentModel.Department.ParentDepartmentUID);
            var departmentListResult = ClientManager.FiresecService.GetDepartmentList(filter);
            departmentModel.IsDepartmentSelected = departmentListResult.Result.Any();
            departmentModel.SelectedDepartment = departmentListResult.Result.FirstOrDefault() ?? new ShortDepartment();

            var employeeFilter = new EmployeeFilter { LogicalDeletationType = LogicalDeletationType.All, UIDs = new List<Guid> { departmentModel.Department.ChiefUID }, IsAllPersonTypes = true };
            var chiefResult = ClientManager.FiresecService.GetEmployeeList(employeeFilter);
            if (chiefResult.HasError)
            {
                throw new InvalidOperationException(chiefResult.Error);
            }
            departmentModel.IsChiefSelected = chiefResult.Result.Any();
            departmentModel.SelectedChief = chiefResult.Result.Select(e => ShortEmployeeModel.CreateFromModel(e)).FirstOrDefault() ?? new ShortEmployeeModel();


            departmentModel.Department.Photo = null;
            return new JsonNetResult { Data = departmentModel };
        }

        [HttpPost]
        public JsonNetResult DepartmentDetails(DepartmentDetailsViewModel departmentModel, bool isNew)
        {
            string error = DetailsValidateHelper.Validate(departmentModel.Department);

            if (!string.IsNullOrEmpty(error))
            {
                return new JsonNetResult {Data = error};
            }

            departmentModel.Department.ParentDepartmentUID = departmentModel.IsDepartmentSelected ? departmentModel.SelectedDepartment.UID : Guid.Empty;
            departmentModel.Department.ChiefUID = departmentModel.IsChiefSelected ? departmentModel.SelectedChief.UID : Guid.Empty;

            var operationResult = ClientManager.FiresecService.SaveDepartment(departmentModel.Department, isNew);

            return new JsonNetResult {Data = operationResult.Error};
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