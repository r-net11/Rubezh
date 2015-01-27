using System;
using Common;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Linq;
using SKDDriver;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using System.Collections.Generic;
using FiresecService.Report.Model;

namespace FiresecService.Report.Templates
{
    public partial class Report411 : BaseReport
    {
        public Report411()
        {
            InitializeComponent();
        }

        public override string ReportTitle
        {
            get { return "Сведения о пропусках"; }
        }
        protected override DataSet CreateDataSet(DataProvider dataProvider)
        {
            var filter = GetFilter<ReportFilter411>();

            var useEmployeesFilter = false;
            var employees = new List<Guid>();
            if (!filter.Employees.IsEmpty() || !filter.Departments.IsEmpty() || !filter.Positions.IsEmpty() || !filter.Organisations.IsEmpty())
            {
                useEmployeesFilter = true;
                employees = dataProvider.GetEmployees(filter).Select(item => item.UID).ToList();
            }

            var cardFilter = new CardFilter();
            var cardsResult = dataProvider.DatabaseService.CardTranslator.Get(cardFilter);

            var dataSet = new DataSet411();
            if (!cardsResult.HasError)
            {
                foreach (var card in cardsResult.Result)
                {
                    if (useEmployeesFilter && !employees.Contains(card.EmployeeUID))
                        continue;

                    if (filter.PassCardPermanent || filter.PassCardTemprorary || filter.PassCardOnceOnly || filter.PassCardForcing || filter.PassCardLocked)
                    {
                        if (filter.PassCardPermanent && card.CardType != CardType.Constant)
                            continue;
                        if (filter.PassCardTemprorary && card.CardType != CardType.Temporary)
                            continue;
                        if (filter.PassCardOnceOnly && card.CardType != CardType.OneTime)
                            continue;
                        if (filter.PassCardForcing && card.CardType != CardType.Duress)
                            continue;
                        if (filter.PassCardLocked && card.CardType != CardType.Blocked)
                            continue;
                    }

                    if (filter.PassCardActive && !filter.PassCardInactive && card.IsDeleted)
                        continue;
                    if (!filter.PassCardActive && filter.PassCardInactive && !card.IsDeleted)
                        continue;

                    if (filter.UseExpirationDate)
                    {
                        var endDate = DateTime.Now;
                        if (filter.ExpirationType == EndDateType.Day)
                            endDate = DateTime.Now.AddDays(1);
                        if (filter.ExpirationType == EndDateType.Week)
                            endDate = DateTime.Now.AddDays(7);
                        if (filter.ExpirationType == EndDateType.Month)
                            endDate = DateTime.Now.AddDays(31);
                        if (filter.ExpirationType == EndDateType.Arbitrary)
                            endDate = filter.ExpirationDate;

                        if (card.EndDate > endDate)
                            continue;
                    }

                    var employeeResult = dataProvider.DatabaseService.EmployeeTranslator.GetSingle(card.EmployeeUID);

                    var dataRow = dataSet.Data.NewDataRow();
                    dataRow.Type = card.CardType.ToDescription();
                    dataRow.Number = card.Number.ToString();
                    if (employeeResult.Result != null)
                    {
                        dataRow.Employee = employeeResult.Result.Name;
                        var organisationResult = dataProvider.DatabaseService.OrganisationTranslator.GetSingle(employeeResult.Result.OrganisationUID);
                        if (organisationResult.Result != null)
                        {
                            dataRow.Organisation = organisationResult.Result.Name;
                        }
                        if (employeeResult.Result.Department != null)
                        {
                            dataRow.Department = employeeResult.Result.Department.Name;
                        }
                        if (employeeResult.Result.Position != null)
                        {
                            dataRow.Position = employeeResult.Result.Position.Name;
                        }
                    }
                    dataRow.Period = card.EndDate.ToString();
                    dataSet.Data.Rows.Add(dataRow);
                }
            }
            return dataSet;
        }
    }
}