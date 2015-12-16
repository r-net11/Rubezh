using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.Models;
using GKWebService.Models.SKD.Organisations;
using GKWebService.Utils;
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
    }
}