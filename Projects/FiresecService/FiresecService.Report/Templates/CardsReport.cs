using FiresecService.Report.DataSources;
using Localization.FiresecService.Report.Common;
using StrazhAPI;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using System;
using System.Data;
using System.Linq;

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
			get { return CommonResources.PasscardsInfo; }
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<CardsReportFilter>();
			if (!filter.PassCardActive && !filter.PassCardForcing && !filter.PassCardInactive && !filter.PassCardLocked && !filter.PassCardGuest && !filter.PassCardPermanent && !filter.PassCardTemprorary)
			{
				filter.PassCardActive = true;
				filter.PassCardForcing = true;
				filter.PassCardInactive = true;
				filter.PassCardLocked = true;
				filter.PassCardGuest = true;
				filter.PassCardPermanent = true;
				filter.PassCardTemprorary = true;
			}

			var cardFilter = new CardFilter { EmployeeFilter = dataProvider.GetCardEmployeeFilter(filter) };
			if (filter.PassCardForcing)
				cardFilter.CardTypes.Add(CardType.Duress);
			if (filter.PassCardLocked)
				cardFilter.CardTypes.Add(CardType.Blocked);
			if (filter.PassCardGuest)
				cardFilter.CardTypes.Add(CardType.Guest);
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
			var cardsResult = dataProvider.DatabaseService.CardTranslator.Get(cardFilter);

			var dataSet = new CardsDataSet();
			if (!cardsResult.HasError)
			{
				dataProvider.GetEmployees(cardsResult.Result.Select(item => item.EmployeeUID));
				foreach (var card in cardsResult.Result)
				{
					var dataRow = dataSet.Data.NewDataRow();
					dataRow.Type = card.IsInStopList ? CommonResources.Deactivated : card.CardType.ToDescription();
					dataRow.Number = card.Number.ToString();
					var employee = dataProvider.GetEmployee(card.EmployeeUID);
					if (employee != null)
					{
						dataRow.Employee = employee.Name;
						dataRow.Organisation = employee.Organisation;
						dataRow.Department = employee.Department;
						dataRow.Position = employee.Position;
					}
					if (!card.IsInStopList && (card.CardType == CardType.Duress || card.CardType == CardType.Temporary || card.CardType == CardType.Guest))
						dataRow.Period = card.EndDate;
					if (card.AllowedPassCount.HasValue)
						dataRow.AllowedPassCount = card.AllowedPassCount.Value;
					dataSet.Data.Rows.Add(dataRow);
				}
			}
			return dataSet;
		}

		protected override void BeforeReportPrint()
		{
			base.BeforeReportPrint();

			this.xrTableCell8.Text = CommonResources.Type;
			this.xrTableCell9.Text = CommonResources.Number;
			this.xrTableCell10.Text = CommonResources.Employee;
			this.xrTableCell11.Text = CommonResources.Organization;
			this.xrTableCell12.Text = CommonResources.Department;
			this.xrTableCell13.Text = CommonResources.Position;
			this.xrTableCell14.Text = CommonResources.Validity;
			this.xrTableCell16.Text = CommonResources.PassNumbers;
		}
	}
}