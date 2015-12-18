using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        public JsonResult GetOrganisations(OrganisationFilter filter)
        {
            var result = ClientManager.FiresecService.GetOrganisations(filter);

            if (result.HasError)
            {
                throw new InvalidOperationException(result.Error);
            }

            return Json(new { Organisations = result.Result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrganisationDetails()
        {
            return View();
        }

        public JsonNetResult GetOrganisationDetails(Guid? id)
        {
            var organisationModel = new OrganisationDetailsViewModel()
            {
                Organisation = new OrganisationDetails()
            };

            organisationModel.Initialize(id);

            return new JsonNetResult { Data = organisationModel };
        }

        [HttpPost]
        public JsonNetResult OrganisationDetails(OrganisationDetailsViewModel organisation, bool isNew)
        {
            var error = organisation.Save(isNew);

            return new JsonNetResult { Data = error };
        }

        public JsonNetResult GetOrganisationUsers(Organisation organisation)
        {
            var users = ClientManager.SecurityConfiguration.Users.Select(u => new OrganisationUserViewModel(organisation, u));

            return new JsonNetResult { Data = new {Users = users } };
        }

        [HttpPost]
        public JsonNetResult SetUsersChecked(Organisation organisation, OrganisationUserViewModel user)
        {
            organisation = user.SetUserChecked(organisation);

            return new JsonNetResult { Data = organisation };
        }
        public JsonNetResult GetOrganisationDoors(Organisation organisation)
        {
            var doors = GKManager.DeviceConfiguration.Doors.Select(u => new OrganisationDoorViewModel(organisation, u));

            return new JsonNetResult { Data = new {Doors = doors } };
        }

        [HttpPost]
        public JsonNetResult SetDoorsChecked(Organisation organisation, OrganisationDoorViewModel door)
        {
            organisation = door.SetDoorChecked(organisation);

            return new JsonNetResult { Data = organisation };
        }

        [HttpPost]
        public JsonNetResult IsDoorLinked(Guid organisationId, OrganisationDoorViewModel door)
        {
            var result = door.IsDoorLinked(organisationId);

            return new JsonNetResult { Data = result };
        }

        [HttpPost]
        public JsonNetResult MarkDeleted(Guid uid, string name)
        {
            var operationResult = ClientManager.FiresecService.MarkDeletedOrganisation(uid, name);
            return new JsonNetResult { Data = operationResult != null && operationResult.HasError && !operationResult.Error.Contains("Ошибка БД") };
        }

        public JsonNetResult IsAnyOrganisationItems(Guid uid)
        {
            var operationResult = ClientManager.FiresecService.IsAnyOrganisationItems(uid);
            if (operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }
            return new JsonNetResult { Data = operationResult.Result};
        }

        [HttpPost]
        public JsonNetResult Restore(Guid uid, string name)
        {
            var operationResult = ClientManager.FiresecService.RestoreOrganisation(uid, name);
            return new JsonNetResult { Data = operationResult != null && operationResult.HasError && !operationResult.Error.Contains("Ошибка БД") };
        }
    }
}