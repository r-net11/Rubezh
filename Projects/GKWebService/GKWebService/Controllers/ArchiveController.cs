using GKWebService.DataProviders.SKD;
using GKWebService.Models;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace GKWebService.Controllers
{
    public class ArchiveController : Controller
    {
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult ArchiveFilter()
		{
			return View();
		}

		AutoResetEvent _autoResetEvent;
		List<JournalItem> _journalItems;
		[HttpPost]
		public JsonResult GetArchive(GKWebService.Models.JournalFilter filter)
		{
			_autoResetEvent = new AutoResetEvent(false);
			var journalFilter = CreateApiFilter(filter);
			return GetJournalPage(journalFilter, filter.Page.HasValue ? filter.Page.Value : 1);
		}

		JsonResult GetJournalPage(RubezhAPI.Journal.JournalFilter journalFilter, int pageNo)
		{
			SafeFiresecService.CallbackOperationResultEvent += OnCallbackOperationResult;
			var result = ClientManager.FiresecService.BeginGetArchivePage(journalFilter, pageNo);
			if (!result.HasError)
			{
				if (!_autoResetEvent.WaitOne(TimeSpan.FromSeconds(10)))
				{
					_journalItems = new List<JournalItem>();
				}
			}
			SafeFiresecService.CallbackOperationResultEvent -= OnCallbackOperationResult;
			var list = _journalItems.Select(x => new JournalModel(x)).ToList();
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		
		public JsonResult GetMaxPage(GKWebService.Models.JournalFilter filter)
		{
			var journalFilter = CreateApiFilter(filter);
			var countResult = ClientManager.FiresecService.GetArchiveCount(journalFilter);
			var result = (!countResult.HasError ? countResult.Result : 1) / journalFilter.PageSize + 1;
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetFilter()
		{
			var result = new JournalFilterJson
			{
				MinDate = DateTime.Now.AddDays(-7),
				MaxDate = DateTime.Now,
				Events = JournalController.GetFilterEvents(),
				Objects = JournalController.GetFilterObjects()
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		RubezhAPI.Journal.JournalFilter CreateApiFilter(GKWebService.Models.JournalFilter filter)
		{
			var journalFilter = new RubezhAPI.Journal.JournalFilter();
			if (filter != null)
			{
				if (filter.ObjectUids != null)
					journalFilter.ObjectUIDs = filter.ObjectUids;
				if (filter.Events != null)
				{
					foreach (var filterEvent in filter.Events)
					{
						switch (filterEvent.Type)
						{
							case 0:
								journalFilter.JournalSubsystemTypes.Add((JournalSubsystemType)filterEvent.Value);
								break;
							case 1:
								journalFilter.JournalEventNameTypes.Add((JournalEventNameType)filterEvent.Value);
								break;
							case 2:
								journalFilter.JournalEventDescriptionTypes.Add((JournalEventDescriptionType)filterEvent.Value);
								break;
							default:
								break;
						}
					}
				}
				journalFilter.StartDate = filter.BeginDate.HasValue ? filter.BeginDate.Value : DateTime.Now.AddDays(-1);
				journalFilter.EndDate = filter.EndDate.HasValue ? filter.EndDate.Value : DateTime.Now;
			};
			return journalFilter;
		}

		void OnCallbackOperationResult(CallbackOperationResult callbackOperationResult)
		{
			if (callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.GetArchivePage)
			{
				_journalItems = callbackOperationResult.JournalItems;
				_autoResetEvent.Set();
			}
		}
	}
}