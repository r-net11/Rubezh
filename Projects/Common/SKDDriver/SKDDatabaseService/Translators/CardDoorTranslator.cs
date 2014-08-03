using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class CardDoorTranslator : IsDeletedTranslator<DataAccess.CardDoor, CardDoor, CardDoorFilter>
	{
		public CardDoorTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override CardDoor Translate(DataAccess.CardDoor tableItem)
		{
			var result = base.Translate(tableItem);
			result.EnterIntervalType = (IntervalType)tableItem.EnterIntervalType;
			result.EnterIntervalID = tableItem.EnterIntervalID;
			result.ExitIntervalType = (IntervalType)tableItem.ExitIntervalType;
			result.ExitIntervalID = tableItem.ExitIntervalID;
			result.DoorUID = tableItem.DoorUID;
			result.CardUID = tableItem.CardUID;
			result.AccessTemplateUID = tableItem.AccessTemplateUID;
			return result;
		}

		protected override void TranslateBack(DataAccess.CardDoor tableItem, CardDoor apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.EnterIntervalType = (int?)apiItem.EnterIntervalType;
			tableItem.EnterIntervalID = apiItem.EnterIntervalID;
			tableItem.ExitIntervalType = (int?)apiItem.ExitIntervalType;
			tableItem.ExitIntervalID = apiItem.ExitIntervalID;
			tableItem.DoorUID = apiItem.DoorUID;
			tableItem.CardUID = apiItem.CardUID;
			tableItem.AccessTemplateUID = apiItem.AccessTemplateUID;
		}

		public List<CardDoor> GetForCards(Guid cardUID)
		{
			var result = new List<CardDoor>();
			foreach (var CardDoorLink in Table.Where(x => x != null &&
				!x.IsDeleted &&
				x.CardUID == cardUID))
			{
				result.Add(Translate(CardDoorLink));
			}
			return result;
		}

		public List<CardDoor> GetForAccessTemplate(Guid accessTemplateUID)
		{
			var result = new List<CardDoor>();
			foreach (var CardDoorLink in Table.Where(x => x != null &&
				!x.IsDeleted &&
				x.AccessTemplateUID == accessTemplateUID))
			{
				result.Add(Translate(CardDoorLink));
			}
			return result;
		}

		public OperationResult MarkDeleted(DataAccess.CardDoor databaseItem)
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

		public OperationResult RemoveFromCard(SKDCard card)
		{
			try
			{
				var databaseItems = Table.Where(x => x.CardUID == card.UID);
				databaseItems.ForEach(x => Delete(x.UID));
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult RemoveFromAccessTemplate(AccessTemplate accessTemplate)
		{
			try
			{
				var databaseItems = Table.Where(x => x.AccessTemplateUID == accessTemplate.UID);
				databaseItems.ForEach(x => Delete(x.UID));
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult MarkDeletefFromAccessTemplate(Guid uid)
		{
			var operationResult = new OperationResult();
			try
			{
				var databaseItems = Table.Where(x => x.AccessTemplateUID == uid);
				databaseItems.ForEach(x => MarkDeleted(x));
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected override Expression<Func<DataAccess.CardDoor, bool>> IsInFilter(CardDoorFilter filter)
		{
			var result = base.IsInFilter(filter);
			var cardUIDs = filter.CardUIDs;
			if (cardUIDs != null && cardUIDs.Count != 0)
				result = result.And(e => e.CardUID.HasValue && cardUIDs.Contains(e.CardUID.Value));
			var DoorUIDs = filter.DoorUIDs;
			if (DoorUIDs != null && DoorUIDs.Count != 0)
				result = result.And(e => DoorUIDs.Contains(e.DoorUID));
			var intervalIDs = filter.IntervalIDs;
			if (intervalIDs != null && intervalIDs.Count != 0)
				result = result.And(e => e.EnterIntervalID > 0 && intervalIDs.Contains(e.EnterIntervalID));
			return result;
		}
	}
}