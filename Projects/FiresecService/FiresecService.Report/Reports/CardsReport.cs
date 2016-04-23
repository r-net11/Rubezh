using System;
using System.Data;
using System.Linq;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class CardsReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
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

			var dataSet = new CardsDataSet();
			if (!cardsResult.HasError)
			{
				dataProvider.GetEmployees(cardsResult.Result.Select(item => item.EmployeeUID.GetValueOrDefault()));
				foreach (var card in cardsResult.Result)
				{
					var dataRow = dataSet.Data.NewDataRow();
					dataRow.Type = card.IsInStopList ? "Деактивированный" : card.GKCardType.ToDescription();
					dataRow.Number = card.Number.ToString();
					var employee = dataProvider.GetEmployee(card.EmployeeUID.GetValueOrDefault());
					if (employee != null)
					{
						dataRow.Employee = employee.Name;
						dataRow.Organisation = employee.Organisation;
						dataRow.Department = employee.Department;
						dataRow.Position = employee.Position;
					}
					if (!card.IsInStopList)
						dataRow.Period = card.EndDate;
					dataSet.Data.Rows.Add(dataRow);
				}
			}
			return dataSet;
		}

	}
}
