﻿using System;
using System.Data;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using Infrastructure.Common;

namespace FiresecService.Report.Templates
{
	public partial class CardsReport : BaseReport
	{
		public CardsReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Альбомная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return true; }
		}

		public override string ReportTitle
		{
			get { return "Сведения о пропусках"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<CardsReportFilter>();
			if (!filter.PassCardActive && !filter.PassCardForcing && !filter.PassCardInactive && !filter.PassCardLocked && !filter.PassCardOnceOnly && !filter.PassCardPermanent && !filter.PassCardTemprorary)
			{
				filter.PassCardActive = true;
				filter.PassCardForcing = true;
				filter.PassCardInactive = true;
				filter.PassCardLocked = true;
				filter.PassCardOnceOnly = true;
				filter.PassCardPermanent = true;
				filter.PassCardTemprorary = true;
			}

			var cardFilter = new CardFilter();
			cardFilter.EmployeeFilter = dataProvider.GetCardEmployeeFilter(filter);
			if (filter.PassCardForcing)
				cardFilter.CardTypes.Add(CardType.Duress);
			if (filter.PassCardLocked)
				cardFilter.CardTypes.Add(CardType.Blocked);
			if (filter.PassCardOnceOnly)
				cardFilter.CardTypes.Add(CardType.OneTime);
			if (filter.PassCardPermanent)
				cardFilter.CardTypes.Add(CardType.Constant);
			if (filter.PassCardTemprorary)
				cardFilter.CardTypes.Add(CardType.Temporary);
			cardFilter.DeactivationType = filter.PassCardInactive ? (cardFilter.CardTypes.Count > 0 ? LogicalDeletationType.All : LogicalDeletationType.Deleted) : LogicalDeletationType.Active;
			cardFilter.IsWithInactive = filter.PassCardInactive;
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
					dataRow.Type = card.IsInStopList ? "Деактивированный" : card.CardType.ToDescription();
					dataRow.Number = card.Number.ToString();
					var employee = dataProvider.GetEmployee(card.EmployeeUID.GetValueOrDefault());
					if (employee != null)
					{
						dataRow.Employee = employee.Name;
						dataRow.Organisation = employee.Organisation;
						dataRow.Department = employee.Department;
						dataRow.Position = employee.Position;
					}
					if (!card.IsInStopList && (card.CardType == CardType.Duress || card.CardType == CardType.Temporary || !GlobalSettingsHelper.GlobalSettings.UseStrazhBrand))
						dataRow.Period = card.EndDate;
					dataSet.Data.Rows.Add(dataRow);
				}
			}
			return dataSet;
		}
	}
}