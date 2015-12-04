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
            departmentViewModel.Initialize(departmentFilter);

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

        /*public JsonNetResult GetDepartmentDetails(Guid organisationId, Guid? id, Guid? parentDepartmentId)*/
        public JsonNetResult GetDepartmentDetails(Guid? id)
        {
            /*var departmentModel = new DepartmentDetailsViewModel();*/
            Department department;
            if (id.HasValue)
            {
                var operationResult = ClientManager.FiresecService.GetDepartmentDetails(id.Value);
                department = operationResult.Result;
            }
            else
            {
                department = new Department
                {
                    Name = "Новое подразделение"
                };
            }
            department.Photo = null;
            return new JsonNetResult { Data = department };
        }

        [HttpPost]
        public JsonNetResult DepartmentDetails(Department department, bool isNew)
        {
            string error = DetailsValidateHelper.Validate(department);

            if (!string.IsNullOrEmpty(error))
            {
                return new JsonNetResult {Data = error};
            }

            var operationResult = ClientManager.FiresecService.SaveDepartment(department, isNew);

            return new JsonNetResult {Data = operationResult.Error};
        }
    }
}