using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class CardDoorTranslator : WithFilterTranslator<DataAccess.CardDoor, CardDoor, CardDoorFilter>
	{
		public CardDoorTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{

		}

		protected override CardDoor Translate(DataAccess.CardDoor tableItem)
		{
			var result = base.Translate(tableItem);
			result.EnterIntervalID = tableItem.EnterIntervalID;
			result.ExitIntervalID = tableItem.ExitIntervalID;
			result.DoorUID = tableItem.DoorUID;
			result.CardUID = tableItem.CardUID;
			result.AccessTemplateUID = tableItem.AccessTemplateUID;
			result.CardSubsystemType = (CardSubsystemType)tableItem.CardSubsystemType;
			return result;
		}

		protected override void TranslateBack(DataAccess.CardDoor tableItem, CardDoor apiItem)
		{
			tableItem.EnterIntervalID = apiItem.EnterIntervalID;
			tableItem.ExitIntervalID = apiItem.ExitIntervalID;
			tableItem.DoorUID = apiItem.DoorUID;
			tableItem.CardUID = apiItem.CardUID;
			tableItem.AccessTemplateUID = apiItem.AccessTemplateUID;
			tableItem.CardSubsystemType = (byte)apiItem.CardSubsystemType;
		}

		public List<CardDoor> GetForCards(Guid cardUID)
		{
			var result = new List<CardDoor>();
			foreach (var CardDoorLink in Table.Where(x => x != null &&
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
				x.AccessTemplateUID == accessTemplateUID))
			{
				result.Add(Translate(CardDoorLink));
			}
			return result;
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

		public OperationResult RemoveFromAccessTemplate(Guid accessTemplateUID)
		{
			try
			{
				var databaseItems = Table.Where(x => x.AccessTemplateUID == accessTemplateUID);
				databaseItems.ForEach(x => Delete(x.UID));
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
			return result;
		}
	}
}