using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class CardsReport : BaseReport<List<CardData>>
	{
		public override List<CardData> CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<CardsReportFilter>(f);
			var cardFilter = new CardFilter();
			cardFilter.EmployeeFilter = dataProvider.GetCardEmployeeFilter(filter);
			if ((filter.PassCardActive && filter.PassCardInactive) || (!filter.PassCardActive && !filter.PassCardInactive))
				cardFilter.DeactivationType = LogicalDeletationType.All;
			if (filter.PassCardActive && !filter.PassCardInactive)
				cardFilter.DeactivationType = LogicalDeletationType.Active;
			if (!filter.PassCardActive && filter.PassCardInactive)
				cardFilter.DeactivationType = LogicalDeletationType.Deleted;
			cardFilter.IsWithEndDate = filter.UseExpirationDate;
			if (filter.UseExpirationDate)
				switch (filter.ExpirationType)
				{
					case EndDateType.Day:
						cardFilter.EndDate = DateTime.Today.AddDays(1);
						break;
					case EndDateType.Week:
						cardFilter.EndDate = DateTime.Today.AddDays(7);
						break;
					case EndDateType.Month:
						cardFilter.EndDate = DateTime.Today.AddDays(31);
						break;
					case EndDateType.Arbitrary:
						cardFilter.EndDate = filter.ExpirationDate;
						break;
				}
			var cardsResult = dataProvider.DbService.CardTranslator.Get(cardFilter);

			var result = new List<CardData>();
			if (!cardsResult.HasError)
			{
				dataProvider.GetEmployees(cardsResult.Result.Select(item => item.EmployeeUID.GetValueOrDefault()));
				foreach (var card in cardsResult.Result)
				{
					var cardData = new CardData();
					cardData.Type = card.IsInStopList ? "Деактивированный" : card.GKCardType.ToDescription();
					cardData.Number = card.Number.ToString();
					var employee = dataProvider.GetEmployee(card.EmployeeUID.GetValueOrDefault());
					if (employee != null)
					{
						cardData.Employee = employee.Name;
						cardData.Organisation = employee.Organisation;
						cardData.Department = employee.Department;
						cardData.Position = employee.Position;
					}
					if (!card.IsInStopList)
						cardData.Period = card.EndDate;
					result.Add(cardData);
				}
			}
			return result;
		}

	}
}
