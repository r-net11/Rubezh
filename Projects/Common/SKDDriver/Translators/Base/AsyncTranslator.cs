using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;
using System.Threading;

namespace SKDDriver.DataClasses
{
	public abstract class AsyncTranslator<TTableItem, TApiItem, TFilter>
		where TTableItem : class, new()
		where TApiItem : class, new()
		where TFilter : API.IAsyncFilter
	{
		ITranslatorGet<TTableItem, TApiItem, TFilter> ParentTranslator;

		public static Thread CurrentThread;

		public AsyncTranslator(ITranslatorGet<TTableItem, TApiItem, TFilter> translator)
		{
			ParentTranslator = translator;
		}

		public event Action<DbCallbackResult> PortionReady;
		public void BeginGet(TFilter filter, Action<DbCallbackResult> portionReady)
		{
			if (!filter.IsLoad)
				return;
			if (portionReady != null)
			{
				PortionReady -= portionReady;
				PortionReady += portionReady;
			}
			DbService.IsAbort = false;
			var pageSize = 1000;
			var portion = new List<TApiItem>();
			int itemNo = 0;
			foreach (var item in ParentTranslator.GetFilteredTableItems(filter, ParentTranslator.GetTableItems()))
			{
				itemNo++;
				//portion.Add(ParentTranslator.Translate(item));
				if (itemNo % pageSize == 0)
				{
					PublishNewItemsPortion(portion, filter.ClientUID, false);
					portion = new List<TApiItem>();
				}
			}
			PublishNewItemsPortion(portion, filter.ClientUID, true);
		}

		void PublishNewItemsPortion(List<TApiItem> items, Guid uid, bool isLastPortion)
		{
			if (PortionReady != null)
			{
				var result = new DbCallbackResult
				{
					ClientUID = uid,
					IsLastPortion = isLastPortion
				};
				GetCollection(result).AddRange(items);
				PortionReady(result);
			}
		}

		public abstract List<TApiItem> GetCollection(DbCallbackResult callbackResult);
	}
}