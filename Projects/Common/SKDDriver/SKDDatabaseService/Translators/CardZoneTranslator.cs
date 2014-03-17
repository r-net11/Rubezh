using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class CardZoneTranslator : IsDeletedTranslator<DataAccess.CardZoneLink, CardZone, CardZoneFilter>
	{
		public CardZoneTranslator(DataAccess.SKUDDataContext context)
			: base(context)
		{

		}

		protected override CardZone Translate(DataAccess.CardZoneLink tableItem)
		{
			var result = base.Translate(tableItem);
			result.IntervalType = (FiresecAPI.IntervalType)tableItem.IntervalType;
			result.IntervalUID = tableItem.IntervalUID;
			result.IsComission = tableItem.IsWithEscort;
			result.ZoneUID = tableItem.ZoneUID;
			result.ParentUID = tableItem.ParentUID;
			result.ParentType = (ParentType)tableItem.ParentType;
			result.IsAntiPassback = tableItem.IsAntipass;
			return result;
		}

		protected override void TranslateBack(DataAccess.CardZoneLink tableItem, CardZone apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.IntervalType = (int?)apiItem.IntervalType;
			tableItem.IntervalUID = apiItem.IntervalUID;
			tableItem.IsWithEscort = apiItem.IsComission;
			tableItem.ZoneUID = apiItem.ZoneUID;
			tableItem.ParentUID = apiItem.ParentUID;
			tableItem.ParentType = (int?)apiItem.ParentType;
			tableItem.IsAntipass = apiItem.IsAntiPassback;
		}

		public List<CardZone> Get(Guid parentUID, ParentType parentType)
		{
			var result = new List<CardZone>();
			foreach (var cardZoneLink in Table.Where(x => x != null &&
				!x.IsDeleted &&
				x.ParentUID == parentUID &&
				(ParentType)x.ParentType == parentType))
			{
				result.Add(Translate(cardZoneLink));
			}
			return result;
		}

		public OperationResult MarkDeleted(DataAccess.CardZoneLink databaseItem)
		{
			var operationResult = new OperationResult();
			try
			{
				if (databaseItem != null)
				{
					databaseItem.IsDeleted = true;
					databaseItem.RemovalDate = DateTime.Now;
				}
				Table.Context.SubmitChanges();
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult SaveFromCards(IEnumerable<SKDCard> cards)
		{
			var operationResult = new OperationResult();
			try
			{
				foreach (var card in cards)
				{
					var databaseItems = Table.Where(x => x.ParentUID == card.UID);
					databaseItems.ForEach(x => MarkDeleted(x));
					Save(card.CardZones);
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult SaveFromGUDs(IEnumerable<GUD> gUDs)
		{
			var operationResult = new OperationResult();
			try
			{
				foreach (var gUD in gUDs)
				{
					var databaseItems = Table.Where(x => x.ParentUID == gUD.UID);
					databaseItems.ForEach(x => MarkDeleted(x));
					Save(gUD.CardZones);
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected override Expression<Func<DataAccess.CardZoneLink, bool>> IsInFilter(CardZoneFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.CardZoneLink>();
			result = result.And(base.IsInFilter(filter));
			var cardUIDs = filter.CardUIDs;
			if (cardUIDs != null && cardUIDs.Count != 0)
				result = result.And(e => e.ParentUID.HasValue && cardUIDs.Contains(e.ParentUID.Value));
			var zoneUIDs = filter.ZoneUIDs;
			if (zoneUIDs != null && zoneUIDs.Count != 0)
				result = result.And(e => zoneUIDs.Contains(e.ZoneUID));
			var intervalUIDs = filter.IntervalUIDs;
			if (intervalUIDs != null && intervalUIDs.Count != 0)
				result = result.And(e => e.IntervalUID.HasValue && intervalUIDs.Contains(e.IntervalUID.Value));
			return result;
		}
	}
}