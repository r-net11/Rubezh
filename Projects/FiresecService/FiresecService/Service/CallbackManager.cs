using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace FiresecService.Service
{
	public static class CallbackManager
	{
		static List<CallbackResultSaver> CallbackResultSavers = new List<CallbackResultSaver>();
		static int Index = 0;

		public static void Add(CallbackResult callbackResult)
		{
			CallbackResultSavers.RemoveAll(x => (DateTime.Now - x.DateTime) > TimeSpan.FromMinutes(1));

			var callbackResultSaver = new CallbackResultSaver()
			{
				CallbackResult = callbackResult,
				Index = Index++,
				DateTime = DateTime.Now
			};
			CallbackResultSavers.Add(callbackResultSaver);
		}

		public static List<CallbackResult> Get(int index)
		{
			var result = new List<CallbackResult>();
			foreach (var callbackResultSaver in CallbackResultSavers)
			{
				if (callbackResultSaver.Index > index)
				{
					result.Add(callbackResultSaver.CallbackResult);
				}
			}
			return result;
		}

		public static int GetLastIndex()
		{
			return Index;
		}
	}

	public class CallbackResultSaver
	{
		public CallbackResult CallbackResult { get; set; }
		public int Index { get; set; }
		public DateTime DateTime { get; set; }
	}
}