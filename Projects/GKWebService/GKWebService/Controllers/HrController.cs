using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using GKWebService.Models.SKD.Departments;
using GKWebService.Models.SKD.Employees;
using GKWebService.Models.SKD.Positions;
using GKWebService.Utils;
using RubezhAPI.Models;
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

        public ActionResult PhotoSelection()
        {
            return View();
        }

        [ErrorHandler]
        public JsonNetResult GetHr()
        {
            var personTypes = new List<string>();
            if (ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View))
                personTypes.Add(PersonType.Employee.ToString());
            if (ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View))
                personTypes.Add(PersonType.Guest.ToString());
            var selectedPersonType = personTypes.FirstOrDefault();
            return new JsonNetResult {Data = new
            {
                PersonTypes = personTypes,
                SelectedPersonType = selectedPersonType,
                CanSelectEmployees,
                CanSelectPositions,
                CanSelectDepartments,
                CanSelectAdditionalColumns,
                CanSelectCards,
                CanSelectAccessTemplates,
                CanSelectPassCardTemplates,
                CanSelectOrganisations,
                IsEmployeesEditAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Employees_Edit),
                IsGuestEditAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Guests_Edit),
                IsDepartmentsEditAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Departments_Etit),
                IsPositionsEditAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Positions_Etit),
                IsAccessTemplatesEditAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_AccessTemplates_Etit),
                IsEmployeesViewAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Employees_View),
                IsEmployeesEditCardTypeAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Employees_Edit_CardType),
                IsCardsEditAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Cards_Etit),
                IsOrganisationsDoorsAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Organisations_Doors),
                IsOrganisationsUsersAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Organisations_Users),
                IsOrganisationsAddRemoveAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Organisations_AddRemove),
                IsOrganisationsEditAllowed = ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Organisations_Edit),
            }
            };
        }

        private bool CanSelectEmployees
        {
            get
            {
                return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View) || ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View);
            }
        }

        private bool CanSelectPositions
        {
            get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Positions_View); }
        }

        private bool CanSelectDepartments
        {
            get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Departments_View); }
        }

        private bool CanSelectAdditionalColumns
        {
            get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_AdditionalColumns_View); }
        }

        private bool CanSelectCards
        {
            get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Cards_View); }
        }

        private bool CanSelectAccessTemplates
        {
            get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_AccessTemplates_View); }
        }

        private bool CanSelectPassCardTemplates
        {
            get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_PassCards_View); }
        }

        private bool CanSelectOrganisations
        {
            get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Organisations_View); }
        }

        [ErrorHandler]
        public JsonNetResult GetDepartmentEmployees(Guid id)
        {
            var filter = new EmployeeFilter {DepartmentUIDs = new List<Guid> {id}};
            return GetEmployees(filter);
        }

        [ErrorHandler]
        public JsonNetResult GetOrganisationEmployees(Guid id)
        {
            var filter = new EmployeeFilter {OrganisationUIDs = new List<Guid> {id}};
            return GetEmployees(filter);
        }

        [ErrorHandler]
        public JsonNetResult GetOrganisationDepartmentEmployees(Guid organisationId, Guid? departmentId)
        {
            var filter = new EmployeeFilter();
            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
            {
                filter.DepartmentUIDs.Add(departmentId.Value);
            }
            filter.OrganisationUIDs.Add(organisationId);
            return GetEmployees(filter);
        }

        [ErrorHandler]
        public JsonNetResult GetEmptyDepartmentEmployees(Guid id)
        {
            var filter = new EmployeeFilter
            {
                OrganisationUIDs = new List<Guid> {id},
                IsEmptyDepartment = true
            };

            return GetEmployees(filter);
        }

        [ErrorHandler]
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
            var operationResult = EmployeeHelper.Get(filter);

            var employees = operationResult.Select(e => ShortEmployeeModel.CreateFromModel(e));

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

        [ErrorHandler]
        public JsonResult GetOrganisationsFilter(bool isWithDeleted)
        {
            var filter = new OrganisationFilter { UserUID = ClientManager.CurrentUser.UID };
            if (isWithDeleted)
            {
                filter.LogicalDeletationType = LogicalDeletationType.All;
            }

            var organisationsResult = OrganisationHelper.Get(filter);

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = organisationsResult,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DepartmentsFilter()
		{
			return View();
		}

        [ErrorHandler]
        public JsonResult GetDepartmentsFilter(bool isWithDeleted)
        {
            var filter = new DepartmentFilter { UserUID = ClientManager.CurrentUser.UID };
            if (isWithDeleted)
            {
                filter.LogicalDeletationType = LogicalDeletationType.All;
            }

            var departmentViewModel = new DepartmentsViewModel();
            departmentViewModel.Initialize(filter);

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = departmentViewModel.Organisations,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PositionsFilter()
		{
			return View();
		}

        [ErrorHandler]
        public JsonResult GetPositionsFilter(bool isWithDeleted)
        {
            var filter = new PositionFilter { UserUID = ClientManager.CurrentUser.UID };
            if (isWithDeleted)
            {
                filter.LogicalDeletationType = LogicalDeletationType.All;
            }

            var positionViewModel = new PositionsViewModel();
            positionViewModel.Initialize(filter);

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = positionViewModel.Organisations,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeesFilter()
		{
			return View();
		}

        [ErrorHandler]
        public JsonResult GetEmployeesFilter(bool isWithDeleted, PersonType selectedPersonType)
        {
            var employeeFilter = new EmployeeFilter
            {
                LogicalDeletationType = isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active,
                PersonType = selectedPersonType
            };

            var employeesViewModel = new EmployeesViewModel();
            employeesViewModel.Initialize(employeeFilter);

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = employeesViewModel.Organisations,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ErrorHandler]
        public JsonNetResult GetFilter(Guid? id)
		{
			return new JsonNetResult { Data = new EmployeeFilter() };
		}
    }
}