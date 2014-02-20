using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;

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
		protected virtual Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = PredicateBuilder.True<TableT>();
			var IsDeletedExpression = PredicateBuilder.True<TableT>();
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					IsDeletedExpression = e => e != null && e.IsDeleted.GetValueOrDefault(false);
					break;
				case DeletedType.Not:
					IsDeletedExpression = e => e != null && !e.IsDeleted.GetValueOrDefault(false);
					break;
				default:
					IsDeletedExpression = e => e != null;
					break;
			}
			result = result.And(IsDeletedExpression);
			var uids = filter.Uids;
			if (uids != null && uids.Count != 0)
				result = result.And(e => uids.Contains(e.Uid));
			var removalDates = filter.RemovalDates;
			if (removalDates != null)
				result = result.And(e => e.RemovalDate == null || (e.RemovalDate >= removalDates.StartDate && e.RemovalDate <= removalDates.EndDate));
			return result;
		}

		public IEnumerable<ApiT> Get(FilterT filter)
		{
			try
			{
				List<ApiT> result = new List<ApiT>();
				IQueryable<TableT> query;
				if (filter == null)
					query = Table;
				else
					query = Table.Where(IsInFilter(filter));
				foreach (var item in query)
					result.Add(Translate(item));
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

		//protected static bool IsInDeleted(DataAccess.IDatabaseElement item, FilterBase filter)
		//{
		//    bool isDeleted = item.IsDeleted.GetValueOrDefault(false);
		//    switch (filter.WithDeleted)
		//    {
		//        case DeletedType.Deleted:
		//            return isDeleted;
		//        case DeletedType.Not:
		//            return !isDeleted;
		//        default:
		//            return true;
		//    }
		//}

		public static Expression<Func<TableT, bool>> IsInDeleted(FilterT filter)
		{
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					return e => e != null && e.IsDeleted.GetValueOrDefault(false);
				case DeletedType.Not:
					return e => e != null && !e.IsDeleted.GetValueOrDefault(false);
				default:
					return e => true;
			}
		}

		//protected static Expression<Func<TableT, bool>> IsInDateTimePeriod(DateTimePeriod dateTimePeriod)
		//{
		//    if (dateTimePeriod == null)
		//        return e => true;
		//    if (dateTime == null)
		//        return true;
		//    return e => e.DateTime >= dateTimePeriod.StartDate && dateTime <= dateTimePeriod.EndDate;
		//}

		//protected static Expression<Func<TableT, bool>> IsInList<T>(T? item, List<T> list)
		//    where T : struct
		//{
		//    if (list == null || list.Count == 0)
		//        return e => true;
		//    return e => list.Any(x => x.Equals(e));
		//}

		//protected static Expression<Func<TableT, bool>> IsInList<T>(List<T> list)
		//{
		//    if (list == null || list.Count == 0)
		//        return e => true;
		//    return e => list.Any(x => x.Equals(e));
		//}
		#endregion
	}
}
