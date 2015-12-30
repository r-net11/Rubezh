using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using GKWebService.Models.SKD.Departments;
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

        public JsonResult GetEmployeesFilter(bool isWithDeleted, PersonType selectedPersonType)
        {
            var employeeModels = new List<ShortEmployeeModel>();

            var organisationFilter = new OrganisationFilter { LogicalDeletationType = isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active };
            var organisations = ClientManager.FiresecService.GetOrganisations(organisationFilter).Result;
            var initializedOrganisations = InitializeOrganisations(organisations);

            var employees = ClientManager.FiresecService.GetEmployeeList(new EmployeeFilter
            {
                LogicalDeletationType = isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active,
                PersonType = selectedPersonType
            }).Result;
            var initializedEmployees = InitializeEmployees(employees, initializedOrganisations);

            foreach (var organisation in initializedOrganisations)
            {
                employeeModels.Add(organisation);
                employeeModels.AddRange(initializedEmployees.Where(e => e.OrganisationUID == organisation.UID));
            }

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = employeeModels,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonNetResult GetFilter(Guid? id)
		{
			return new JsonNetResult { Data = new EmployeeFilter() };
		}

        private List<ShortEmployeeModel> InitializeEmployees(IEnumerable<ShortEmployee> employees, IEnumerable<ShortEmployeeModel> organisations)
        {
            return employees.Select(e => ShortEmployeeModel.CreateFromModel(e, organisations)).ToList();
        }

        private List<ShortEmployeeModel> InitializeOrganisations(IEnumerable<Organisation> organisations)
        {
            return organisations.Select(ShortEmployeeModel.CreateFromOrganisation).ToList();
        }


    }
}