using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using RubezhAPI;
using RubezhAPI.Journal;
using System.Linq;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
	public class JournalController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult JournalFilter()
		{
			return View();
		}

		public JsonResult GetJournal()
		{
			var apiItems = JournalHelper.Get(new JournalFilter());
			var list = apiItems.Select(x => new JournalModel()
			{
				Desc = x.JournalEventDescriptionType.ToDescription(),
				SystemDate = x.SystemDateTime.ToString(),
				Name = x.JournalEventNameType.ToDescription(),
				Object = x.JournalObjectType.ToDescription(),
				DeviceDate = x.DeviceDateTime.ToString(),
				Subsystem = x.JournalSubsystemType.ToDescription(),
				SubsystemImage = GetSubsystemImage(x), 
				User = x.UserName
			}).ToList();
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		string GetSubsystemImage(JournalItem journalModel)
		{
			switch (journalModel.JournalSubsystemType)
			{
				case JournalSubsystemType.System:
					return "PC";	
				case JournalSubsystemType.GK:
					return "Chip";
				case JournalSubsystemType.SKD:
					return "Controller";
				case JournalSubsystemType.Video:
					return "Camera";
				default:
					return "no";
			}
		}
	}
}