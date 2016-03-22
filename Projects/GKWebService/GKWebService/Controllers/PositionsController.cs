using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using GKWebService.Models.SKD.Departments;
using GKWebService.Models.SKD.Positions;
using GKWebService.Utils;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Controllers
{
	[Authorize]
	public class PositionsController : Controller
    {
        // GET: Positions
        public ActionResult Index()
        {
            return View();
        }

        [ErrorHandler]
        public JsonResult GetOrganisations(PositionFilter positionFilter)
        {
            var positionsViewModel = new PositionsViewModel();
            positionsViewModel.Initialize(new PositionFilter
            {
                OrganisationUIDs = positionFilter.OrganisationUIDs,
                LogicalDeletationType = positionFilter.LogicalDeletationType
            });

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = positionsViewModel.Organisations,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PositionDetails()
        {
            return View();
        }

        [ErrorHandler]
        public JsonNetResult GetPositionDetails(Guid? organisationId, Guid? id)
        {
            var positionModel = new PositionDetailsViewModel()
            {
                Position = new Position()
            };

            if (!organisationId.HasValue)
            {
                return new JsonNetResult { Data = positionModel };
            }

            positionModel.Initialize(organisationId.Value, id);

            return new JsonNetResult { Data = positionModel };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult PositionDetails(PositionDetailsViewModel position, bool isNew)
        {
            var error = position.Save(isNew);

            return new JsonNetResult { Data = error };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult PositionPaste(PositionDetailsViewModel position)
        {
            var error = position.Paste();

            return new JsonNetResult { Data = error };
        }

        public ActionResult PositionEmployeeList()
        {
            return View();
        }

        [ErrorHandler]
        public JsonResult GetPositionEmployeeList(Guid positionId, Guid organisationId, bool isWithDeleted)
        {
            var filter = new EmployeeFilter
            {
                PositionUIDs = new List<Guid> { positionId },
                OrganisationUIDs = new List<Guid> { organisationId },
                LogicalDeletationType = isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active
            };
            var operationResult = EmployeeHelper.Get(filter);
            var employees = operationResult.Select(e => ShortEmployeeModel.CreateFromModel(e)).ToList();

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = employees,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ErrorHandler]
        public JsonResult SaveEmployeePosition(Guid employeeUID, Guid? positionUID, string name)
        {
            var operationResult = EmployeeHelper.SetPosition(employeeUID, positionUID, name);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        [ErrorHandler]
        public JsonNetResult GetChildEmployeeUIDs(Guid positionId)
        {
            var operationResult = PositionHelper.GetEmployeeUIDs(positionId);

            return new JsonNetResult { Data = operationResult };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult MarkDeleted(Guid uid, string name)
        {
            var operationResult = PositionHelper.MarkDeleted(uid, name);
            return new JsonNetResult { Data = operationResult };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult Restore(Guid uid, string name)
        {
            var operationResult = PositionHelper.Restore(uid, name);
            return new JsonNetResult { Data = operationResult };
        }
    }
}