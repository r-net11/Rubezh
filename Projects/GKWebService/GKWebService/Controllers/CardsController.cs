using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GKWebService.DataProviders.SKD;
using GKWebService.Models.SKD.Cards;
using GKWebService.Models.SKD.Positions;
using GKWebService.Utils;
using RubezhAPI.SKD;

namespace GKWebService.Controllers
{
    public class CardsController : Controller
    {
        // GET: Cards
        public ActionResult Index()
        {
            return View();
        }

        [ErrorHandler]
        public JsonResult GetOrganisations(CardFilter cardFilter)
        {
            var cardsViewModel = new CardsViewModel();
            cardsViewModel.Initialize(new CardFilter
            {
                OrganisationUIDs = cardFilter.OrganisationUIDs,
                LogicalDeletationType = cardFilter.LogicalDeletationType
            });

            dynamic result = new
            {
                page = 1,
                total = 100,
                records = 100,
                rows = cardsViewModel.RootItems,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ErrorHandler]
        public JsonNetResult MarkDeleted(Guid uid)
        {
            var card = CardHelper.Get(new CardFilter { UIDs = new List<Guid> { uid }, LogicalDeletationType = LogicalDeletationType.All }).Single();

            var operationResult = CardHelper.Delete(card);
            return new JsonNetResult { Data = operationResult };
        }
    }
}