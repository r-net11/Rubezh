using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using GKWebService.Models.SKD.Positions;
using GKWebService.Utils;
using RubezhAPI.SKD;

namespace GKWebService.Controllers
{
    public class AccessTemplatesController : Controller
    {
        // GET: AccessTemplate
        public ActionResult Index()
        {
            return View();
        }

        [ErrorHandler]
        public JsonResult GetOrganisations(AccessTemplateFilter accessTemplateFilter)
        {
            var accessTemplatesViewModel = new AccessTemplatesViewModel();
            accessTemplatesViewModel.Initialize(new AccessTemplateFilter
            {
                OrganisationUIDs = accessTemplateFilter.OrganisationUIDs,
                LogicalDeletationType = accessTemplateFilter.LogicalDeletationType
            });

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = accessTemplatesViewModel.Organisations,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ErrorHandler]
        public JsonNetResult GetDoors(Guid id)
        {
            var accessTemplate = AccessTemplateHelper.Get(new AccessTemplateFilter {UIDs = new List<Guid> {id} }).Single();
            var doors = ReadOnlyAccessDoorModel.InitializeDoors(accessTemplate.CardDoors);
            return new JsonNetResult { Data = doors };
        }

    }
}