using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using GKWebService.Models.SKD.Organisations;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Controllers
{
    public class OrganisationsController : Controller
    {
        // GET: Organisations
        public ActionResult Index()
        {
            return View();
        }

        [ErrorHandler]
        public JsonResult GetOrganisations(OrganisationFilter filter)
        {
            var result = OrganisationHelper.Get(new OrganisationFilter { LogicalDeletationType = filter.LogicalDeletationType});

            return Json(new { Organisations = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrganisationDetails()
        {
            return View();
        }

        [ErrorHandler]
        public JsonNetResult GetOrganisationDetails(Guid? id)
        {
            var organisationModel = new OrganisationDetailsViewModel()
            {
                Organisation = new OrganisationDetails(),
            };

            organisationModel.Initialize(id);

            return new JsonNetResult { Data = organisationModel };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult OrganisationDetails(OrganisationDetailsViewModel organisation, bool isNew)
        {
            var error = organisation.Save(isNew);

            return new JsonNetResult { Data = error };
        }

        [ErrorHandler]
        public JsonNetResult GetOrganisationUsers(Organisation organisation)
        {
            var users = RubezhClient.ClientManager.SecurityConfiguration.Users.Select(u => new OrganisationUserViewModel(organisation, u));

            return new JsonNetResult { Data = new {Users = users } };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult SetUsersChecked(Organisation organisation, OrganisationUserViewModel user)
        {
            organisation = user.SetUserChecked(organisation);

            return new JsonNetResult { Data = organisation };
        }

        [ErrorHandler]
        public JsonNetResult GetOrganisationDoors(Organisation organisation)
        {
            var doors = GKManager.DeviceConfiguration.Doors.Select(u => new OrganisationDoorViewModel(organisation, u));

            return new JsonNetResult { Data = new {Doors = doors } };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult SetDoorsChecked(Organisation organisation, OrganisationDoorViewModel door)
        {
            organisation = door.SetDoorChecked(organisation);

            return new JsonNetResult { Data = organisation };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult IsDoorLinked(Guid organisationId, OrganisationDoorViewModel door)
        {
            var result = door.IsDoorLinked(organisationId);

            return new JsonNetResult { Data = result };
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult MarkDeleted(Guid uid, string name)
        {
            var operationResult = OrganisationHelper.MarkDeleted(uid, name);
            return new JsonNetResult { Data = operationResult };
        }

        [ErrorHandler]
        public JsonNetResult IsAnyOrganisationItems(Guid uid)
        {
            var operationResult = OrganisationHelper.IsAnyItems(uid);
            return new JsonNetResult { Data = operationResult};
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult Restore(Guid uid, string name)
        {
            var operationResult = OrganisationHelper.Restore(uid, name);
            return new JsonNetResult { Data = operationResult };
        }
    }
}