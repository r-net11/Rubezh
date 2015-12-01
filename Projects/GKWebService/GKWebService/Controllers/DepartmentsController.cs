using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using GKWebService.Models.SKD.Departments;
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
    }
}