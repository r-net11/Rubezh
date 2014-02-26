using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class CardZonesTranslator:TranslatorBase<DataAccess.CardZoneLink, CardZone, CardZoneFilter>
	{
		public CardZonesTranslator(Table<DataAccess.CardZoneLink> table, DataAccess.SKUDDataContext context)
			: base(table, context)
		{

		}

		protected override CardZone Translate(DataAccess.CardZoneLink tableItem)
		{
			var result = base.Translate(tableItem);
			result.IntervalType = (FiresecAPI.IntervalType)tableItem.IntervalType;
			result.IntervalUID = tableItem.IntervalUid;
			result.IsComission = tableItem.IsWithEscort;
			result.ZoneUID = tableItem.ZoneUid;
			result.ParentUID = tableItem.CardUid;
			return result;
		}

		protected override DataAccess.CardZoneLink TranslateBack(CardZone apiItem)
		{
			var result = base.TranslateBack(apiItem);
			result.IntervalType = (int)apiItem.IntervalType;
			result.IntervalUid = apiItem.IntervalUID;
			result.IsWithEscort = apiItem.IsComission;
			result.ZoneUid = apiItem.ZoneUID;
			result.CardUid = apiItem.ParentUID;
			return result;
		}

		protected override void Update(DataAccess.CardZoneLink tableItem, CardZone apiItem)
		{
			base.Update(tableItem, apiItem);
			tableItem.IntervalType = (int?)apiItem.IntervalType;
			tableItem.IntervalUid = apiItem.IntervalUID;
			tableItem.IsWithEscort = apiItem.IsComission;
			tableItem.ZoneUid = apiItem.ZoneUID;
			tableItem.CardUid = apiItem.ParentUID;
		}

		public List<CardZone> Get(DataAccess.Card card)
		{
			var result = new List<CardZone>();
			foreach (var cardZoneLink in card.CardZoneLink)
			{
				if (cardZoneLink != null)
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

		public OperationResult UpdateZones(IEnumerable<SKDCard> cards)
		{
			var operationResult = new OperationResult();
			try
			{
				foreach (var card in cards)
				{
					var databaseItems = Table.Where(x => x.CardUid == card.UID);
					databaseItems.ForEach(x => MarkDeleted(x));
					var apiCardZones = card.CardZones;
					if(apiCardZones != null)
						Save(apiCardZones);
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
			var cardUIDs = filter.CardUids;
			if (cardUIDs != null && cardUIDs.Count != 0)
				result = result.And(e => e.CardUid.HasValue && cardUIDs.Contains(e.CardUid.Value));
			var zoneUIDs = filter.ZoneUids;
			if (zoneUIDs != null && zoneUIDs.Count != 0)
				result = result.And(e => zoneUIDs.Contains(e.ZoneUid));
			var intervalUIDs = filter.IntervalUids;
			if (intervalUIDs != null && intervalUIDs.Count != 0)
				result = result.And(e => e.IntervalUid.HasValue && intervalUIDs.Contains(e.IntervalUid.Value));
			return result;
		}
	}
}

