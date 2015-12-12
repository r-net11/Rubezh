using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using GKWebService.Models.SKD.Departments;
using GKWebService.Models.SKD.Positions;
using GKWebService.Utils;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Controllers
{
    public class PositionsController : Controller
    {
        // GET: Positions
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetOrganisations(PositionFilter positionFilter)
        {
            var positionsViewModel = new PositionsViewModel();
            positionsViewModel.Initialize(positionFilter);

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
        public JsonNetResult PositionDetails(PositionDetailsViewModel position, bool isNew)
        {
            var error = position.Save(isNew);

            return new JsonNetResult { Data = error };
        }

        public ActionResult PositionEmployeeList()
        {
            return View();
        }

        public JsonResult GetPositionEmployeeList(Guid positionId, Guid organisationId, bool isWithDeleted)
        {
            var filter = new EmployeeFilter
            {
                PositionUIDs = new List<Guid> { positionId },
                OrganisationUIDs = new List<Guid> { organisationId },
                LogicalDeletationType = isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active
            };
            var operationResult = ClientManager.FiresecService.GetEmployeeList(filter);
            if (operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }

            var employees = operationResult.Result.Select(e => ShortEmployeeModel.CreateFromModel(e)).ToList();

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
        public JsonResult SaveEmployeePosition(Guid employeeUID, Guid? positionUID, string name)
        {
            var operationResult = ClientManager.FiresecService.SaveEmployeePosition(employeeUID, positionUID, name);

            return Json(operationResult.HasError, JsonRequestBehavior.AllowGet);
        }
    }
}