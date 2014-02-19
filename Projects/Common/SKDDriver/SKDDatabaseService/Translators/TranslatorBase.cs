using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using System.Data.Linq;

namespace SKDDriver
{
	public abstract class TranslatorBase<TableT, ApiT, FilterT>
		where TableT : class,DataAccess.IDatabaseElement, new()
		where ApiT : SKDModelBase, new()
		where FilterT : FilterBase
	{
		protected Table<TableT> Table;

		public TranslatorBase(Table<TableT> table)
		{
			Table = table;
		}

		protected virtual ApiT Translate(TableT tableItem)
		{

			var result = new ApiT();
			result.UID = tableItem.Uid;
			result.IsDeleted = tableItem.IsDeleted;
			result.RemovalDate = tableItem.RemovalDate;
			return result;
		}
		protected virtual TableT TranslateBack(ApiT apiItem)
		{
			var result = new TableT();
			result.Uid = apiItem.UID;
			result.IsDeleted = apiItem.IsDeleted;
			result.RemovalDate = CheckDate(apiItem.RemovalDate);
			return result;
		}
		protected virtual void Update(TableT tableItem, ApiT apiItem)
		{
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = apiItem.RemovalDate;
		}
		protected virtual void Verify(ApiT item)
		{
			;
		}
		protected virtual bool IsInFilter(TableT item, FilterT filter)
		{
			if (item == null)
				return false;
			bool isInDeleted = IsInDeleted(item, filter);
			bool isInUids = IsInList<Guid>(item.Uid, filter.Uids);
			bool isInRemovalDates = IsInDateTimePeriod(item.RemovalDate, filter.RemovalDates);
			return isInDeleted && isInUids && isInRemovalDates;
		}

		public IEnumerable<ApiT> Get(FilterT filter)
		{
			try
			{
				List<ApiT> result = new List<ApiT>();
				if (filter == null)
					Table.ForEach(x => result.Add(Translate(x)));
				else foreach (var item in Table)
					{
						if (IsInFilter(item, filter))
							result.Add(Translate(item));
					}
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e);
				return new List<ApiT>();
			}
		}

		public void Save(IEnumerable<ApiT> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;
					Verify(item);
					var databaseItem = (from x in Table where x.Uid.Equals(item.UID) select x).FirstOrDefault();
					if (databaseItem != null)
						Update(databaseItem, item);
					else
						Table.InsertOnSubmit(TranslateBack(item));
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		public void MarkDeleted(IEnumerable<ApiT> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item != null)
					{
						var databaseItem = (from x in Table where x.Uid.Equals(item.UID) select x).FirstOrDefault();
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		#region Static
		protected static DateTime? CheckDate(DateTime? dateTime)
		{
			if (dateTime == null)
				return null;
			if (dateTime.Value.Year < 1754)
				return null;
			if (dateTime.Value.Year > 9998)
				return null;
			return dateTime;
		}

		protected static bool IsInDeleted(DataAccess.IDatabaseElement item, FilterBase filter)
		{
			bool isDeleted = item.IsDeleted.GetValueOrDefault(false);
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					return isDeleted;
				case DeletedType.Not:
					return !isDeleted;
				default:
					return true;
			}
		}

		protected static bool IsInDateTimePeriod(DateTime? dateTime, DateTimePeriod dateTimePeriod)
		{
			if (dateTimePeriod == null)
				return true;
			if (dateTime == null)
				return true;
			return dateTime >= dateTimePeriod.StartDate && dateTime <= dateTimePeriod.EndDate;
		}

		protected static bool IsInList<T>(T? item, List<T> list)
			where T : struct
		{
			if (list == null || list.Count == 0)
				return true;
			return list.Any(x => x.Equals(item));
		}

		protected static bool IsInList<T>(T item, List<T> list)
		{
			if (list == null || list.Count == 0)
				return true;
			return list.Any(x => x.Equals(item));
		}
		#endregion
	}
}
