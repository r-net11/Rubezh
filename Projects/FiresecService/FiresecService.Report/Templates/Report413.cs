using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Linq;
using FiresecAPI.SKD.ReportFilters;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecAPI;
using SKDDriver;
using Common;

namespace FiresecService.Report.Templates
{
    public partial class Report413 : BaseReport
    {
        public Report413()
        {
            InitializeComponent();
        }

        public override string ReportTitle
        {
            get { return "Права доступа сотрудников/посетителей"; }
        }
        protected override DataSet CreateDataSet(DataProvider dataProvider)
        {
            var filter = GetFilter<ReportFilter413>();
            if (!filter.PassCardActive && !filter.PassCardForcing && !filter.PassCardLocked && !filter.PassCardOnceOnly && !filter.PassCardPermanent && !filter.PassCardTemprorary)
            {
                filter.PassCardActive = true;
                filter.PassCardForcing = true;
                filter.PassCardLocked = true;
                filter.PassCardOnceOnly = true;
                filter.PassCardPermanent = true;
                filter.PassCardTemprorary = true;
            }
            if (!filter.ZoneIn && !filter.ZoneOut)
            {
                filter.ZoneIn = true;
                filter.ZoneOut = true;
            }
            if (!filter.ScheduleEnter && !filter.ScheduleExit)
            {
                filter.ScheduleEnter = true;
                filter.ScheduleExit = true;
            }

            var cardFilter = new CardFilter();
            if (dataProvider.IsEmployeeFilter(filter))
                cardFilter.EmployeeFilter = dataProvider.GetEmployeeFilter(filter);
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
            cardFilter.DeactivationType = LogicalDeletationType.Active;
            cardFilter.LogicalDeletationType = LogicalDeletationType.Active;
            var cardsResult = dataProvider.DatabaseService.CardTranslator.Get(cardFilter);

            var dataSet = new DataSet413();
            if (!cardsResult.HasError)
            {
                dataProvider.GetEmployees(cardsResult.Result.Select(item => item.EmployeeUID));
                var accessTemplateFilter = new AccessTemplateFilter()
                {
                    UIDs = cardsResult.Result.Where(item => item.AccessTemplateUID.HasValue && item.AccessTemplateUID != Guid.Empty).Select(item => item.AccessTemplateUID.Value).ToList()
                };
                var accessTemplates = dataProvider.DatabaseService.AccessTemplateTranslator.Get(accessTemplateFilter);
                IEnumerable<SKDDoor> doors = SKDManager.Doors;
                if (!filter.Zones.IsEmpty())
                    doors = doors.Where(item =>
                        (filter.ZoneIn && item.InDevice != null && filter.Zones.Contains(item.InDevice.ZoneUID)) ||
                        (filter.ZoneOut && item.OutDevice != null && filter.Zones.Contains(item.OutDevice.ZoneUID)));
                var doorMap = doors.ToDictionary(item => item.UID);
                var intervalMap = SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals.ToDictionary(item => item.ID);
                foreach (var card in cardsResult.Result)
                {
                    IEnumerable<CardDoor> cardDoors = card.CardDoors;
                    if (!accessTemplates.HasError && card.AccessTemplateUID.HasValue && card.AccessTemplateUID.Value != Guid.Empty)
                    {
                        var accessTemplate = accessTemplates.Result.FirstOrDefault(item => item.UID == card.AccessTemplateUID.Value);
                        var cardDoorUIDs = card.CardDoors.Select(item => item.DoorUID);
                        if (accessTemplate != null)
                            cardDoors = cardDoors.Union(accessTemplate.CardDoors.Where(item => !cardDoorUIDs.Contains(item.DoorUID)));
                    }
                    var employee = dataProvider.GetEmployee(card.EmployeeUID);
                    if (!filter.Schedules.IsEmpty())
                        cardDoors = cardDoors.Where(item =>
                            (filter.ScheduleEnter && filter.Schedules.Contains(item.EnterScheduleNo)) ||
                            (filter.ScheduleExit && filter.Schedules.Contains(item.ExitScheduleNo)));
                    foreach (var cardDoor in cardDoors)
                        if (doorMap.ContainsKey(cardDoor.DoorUID))
                        {
                            var door = doorMap[cardDoor.DoorUID];

                            var dataRow = dataSet.Data.NewDataRow();
                            dataRow.Type = card.CardType.ToDescription();
                            dataRow.Number = card.Number.ToString();
                            if (employee != null)
                            {
                                dataRow.Employee = employee.Name;
                                dataRow.Organisation = employee.Organisation;
                                dataRow.Department = employee.Department;
                                dataRow.Position = employee.Position;
                            }
                            if (door.InDevice != null && door.InDevice.Zone != null)
                                dataRow.ZoneIn = door.InDevice.Zone.PresentationName;
                            if (door.OutDevice != null && door.OutDevice.Zone != null)
                                dataRow.ZoneOut = door.OutDevice.Zone.PresentationName;
                            if (intervalMap.ContainsKey(cardDoor.EnterScheduleNo))
                                dataRow.Enter = intervalMap[cardDoor.EnterScheduleNo].Name;
                            if (intervalMap.ContainsKey(cardDoor.ExitScheduleNo))
                                dataRow.Exit = intervalMap[cardDoor.ExitScheduleNo].Name;
                            dataRow.AccessPoint = door.PresentationName;
                            dataSet.Data.Rows.Add(dataRow);
                        }
                }
            }
            return dataSet;
        }
    }
}
