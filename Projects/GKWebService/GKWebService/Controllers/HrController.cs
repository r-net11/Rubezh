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
            var operationResult = ClientManager.FiresecService.GetEmployeeList(filter);
            if (operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }

            var employees = operationResult.Result.Select(e => ShortEmployeeModel.CreateFromModel(e));

            return new JsonNetResult { Data = new {Employees = employees} };
        }

        public ActionResult HrFilter()
		{
			return View();
		}

		public JsonNetResult GetFilter(Guid? id)
		{
			return new JsonNetResult { Data = new EmployeeFilter() };
		}
	}
}