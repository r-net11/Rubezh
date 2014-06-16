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
			result.IntervalType = (IntervalType)tableItem.IntervalType;
			result.IntervalUID = tableItem.IntervalUID;
			result.IsComission = tableItem.IsWithEscort;
			result.DoorUID = tableItem.DoorUID;
			result.ParentUID = tableItem.ParentUID;
			result.IsAntiPassback = tableItem.IsAntipass;
			return result;
		}

		protected override void TranslateBack(DataAccess.CardDoor tableItem, CardDoor apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.IntervalType = (int?)apiItem.IntervalType;
			tableItem.IntervalUID = apiItem.IntervalUID;
			tableItem.IsWithEscort = apiItem.IsComission;
			tableItem.DoorUID = apiItem.DoorUID;
			tableItem.ParentUID = apiItem.ParentUID;
			tableItem.IsAntipass = apiItem.IsAntiPassback;
		}

		public List<CardDoor> Get(Guid parentUID)
		{
			var result = new List<CardDoor>();
			foreach (var CardDoorLink in Table.Where(x => x != null &&
				!x.IsDeleted &&
				x.ParentUID == parentUID))
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

		public OperationResult SaveFromCard(SKDCard card)
		{
			var operationResult = new OperationResult();
			try
			{
				var databaseItems = Table.Where(x => x.ParentUID == card.UID);
				databaseItems.ForEach(x => MarkDeleted(x));
				Save(card.CardDoors);
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult SaveFromAccessTemplate(AccessTemplate accessTemplate)
		{
			var operationResult = new OperationResult();
			try
			{
				var databaseItems = Table.Where(x => x.ParentUID == accessTemplate.UID);
				databaseItems.ForEach(x => MarkDeleted(x));
				Save(accessTemplate.CardDoors);
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
				var databaseItems = Table.Where(x => x.ParentUID == uid);
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
				result = result.And(e => e.ParentUID.HasValue && cardUIDs.Contains(e.ParentUID.Value));
			var DoorUIDs = filter.DoorUIDs;
			if (DoorUIDs != null && DoorUIDs.Count != 0)
				result = result.And(e => DoorUIDs.Contains(e.DoorUID));
			var intervalUIDs = filter.IntervalUIDs;
			if (intervalUIDs != null && intervalUIDs.Count != 0)
				result = result.And(e => e.IntervalUID.HasValue && intervalUIDs.Contains(e.IntervalUID.Value));
			return result;
		}
	}
}