using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class CardTranslator : IsDeletedTranslator<DataAccess.Card, SKDCard, CardFilter>
	{
		public CardTranslator(DataAccess.SKDDataContext context, CardZoneTranslator cardsTranslator)
			: base(context)
		{
			CardZonesTranslator = cardsTranslator;
		}

		CardZoneTranslator CardZonesTranslator;

		protected override OperationResult CanSave(SKDCard item)
		{
			bool isSameSeriesNo = Table.Any(x => x.Number == item.Number &&
				x.Series == item.Series &&
				!x.IsDeleted &&
				x.UID != item.UID);
			if (isSameSeriesNo)
				return new OperationResult("Попытка добавить карту с повторяющейся комбинацией серии и номера");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(SKDCard item)
		{
			if (Context.Employees.Any(x => x.UID == item.HolderUID &&
					!x.IsDeleted))
				return new OperationResult("Не могу удалить карту, пока она указана у действующих сотрудников");
			return base.CanSave(item);
		}

		public override OperationResult MarkDeleted(IEnumerable<SKDCard> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;
					CardZonesTranslator.MarkDeleted(item.CardZones);
				}
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return base.MarkDeleted(items);
		}

		protected override SKDCard Translate(DataAccess.Card tableItem)
		{
			var result = base.Translate(tableItem);
			result.HolderUID = tableItem.EmployeeUID;
			result.Number = tableItem.Number;
			result.Series = tableItem.Series;
			result.CardType = (CardType)tableItem.CardType;
			result.StartDate = tableItem.StartDate;
			result.EndDate = tableItem.EndDate;
			result.AccessTemplateUID = tableItem.AccessTemplateUID;
			result.CardZones = CardZonesTranslator.Get(tableItem.UID);
			result.IsInStopList = tableItem.IsInStopList;
			result.StopReason = tableItem.StopReason;
			result.CardTemplateUID = tableItem.CardTemplateUID;

			var employee = Context.Employees.FirstOrDefault(x => x.UID == tableItem.EmployeeUID);
			if (employee != null)
				result.EmployeeName = employee.LastName + " " + employee.FirstName + " " + employee.SecondName;
			return result;
		}

		protected override void TranslateBack(DataAccess.Card tableItem, SKDCard apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Number = apiItem.Number;
			tableItem.Series = apiItem.Series;
			tableItem.EmployeeUID = apiItem.HolderUID;
			tableItem.CardType = (int)apiItem.CardType;
			tableItem.StartDate = CheckDate(apiItem.StartDate);
			tableItem.EndDate = CheckDate(apiItem.EndDate);
			tableItem.IsInStopList = apiItem.IsInStopList;
			tableItem.StopReason = apiItem.StopReason;
			tableItem.AccessTemplateUID = apiItem.AccessTemplateUID;
			tableItem.CardTemplateUID = apiItem.CardTemplateUID;
		}

		public override OperationResult Save(SKDCard item)
		{
			var updateZonesResult = CardZonesTranslator.SaveFromCard(item);
			if (updateZonesResult.HasError)
				return updateZonesResult;
			return base.Save(item);
		}

		public OperationResult SaveTemplate(SKDCard apiItem)
		{
			try
			{
				var tableItem = Table.Where(x => x.UID == apiItem.UID).FirstOrDefault();
				if (tableItem == null)
					return new OperationResult("Карта не найдена в базе данных");
				tableItem.CardTemplateUID = apiItem.CardTemplateUID;
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected override Expression<Func<DataAccess.Card, bool>> IsInFilter(CardFilter filter)
		{
			var result = base.IsInFilter(filter);

			switch (filter.DeactivationType)
			{
				case LogicalDeletationType.Deleted:
					result = result.And(e => e.IsInStopList);
					break;
				case LogicalDeletationType.Active:
					result = result.And(e => !e.IsInStopList);
					break;
			}

			if (filter.FirstSeries > 0)
			{
				result = result.And(e => e.Series >= filter.FirstSeries);
			}
			if (filter.LastSeries > 0)
			{
				result = result.And(e => e.Series <= filter.LastSeries);
			}
			if (filter.FirstNos > 0)
			{
				result = result.And(e => e.Number >= filter.FirstNos);
			}
			if (filter.LastNos > 0)
			{
				result = result.And(e => e.Number <= filter.LastNos);
			}

			return result;
		}
	}
}