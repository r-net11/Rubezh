using RubezhAPI.Journal;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.DataProviders.SKD
{
	public class JournalHelper
	{
		public static IEnumerable<JournalItem> Get(JournalFilter filter)
		{
			var result = ClientManager.RubezhService.GetFilteredJournalItems(filter);
			return Common.ThrowErrorIfExists(result);
		}
	}
}