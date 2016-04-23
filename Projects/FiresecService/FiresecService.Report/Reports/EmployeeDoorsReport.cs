using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecService.Report.DataSources;
using FiresecService.Report.Templates;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EmployeeDoorsReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<EmployeeDoorsReportFilter>(f);
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
			cardFilter.EmployeeFilter = dataProvider.GetCardEmployeeFilter(filter);
			cardFilter.DeactivationType = LogicalDeletationType.Active;
			cardFilter.LogicalDeletationType = LogicalDeletationType.Active;
			var cardsResult = dataProvider.DbService.CardTranslator.Get(cardFilter);

			var dataSet = new EmployeeDoorsDataSet();
			if (!cardsResult.HasError)
			{
				dataProvider.GetEmployees(cardsResult.Result.Select(item => item.EmployeeUID.GetValueOrDefault()));
				var accessTemplateFilter = new AccessTemplateFilter()
				{
					UIDs = cardsResult.Result.Where(item => item.AccessTemplateUID.HasValue && item.AccessTemplateUID != Guid.Empty).Select(item => item.AccessTemplateUID.Value).ToList()
				};
				var accessTemplates = dataProvider.DbService.AccessTemplateTranslator.Get(accessTemplateFilter);


				var doorMap = new Dictionary<Guid, CommonDoor>();
				foreach (var door in GKManager.Doors)
				{
					var commonDoor = new CommonDoor(door);
					if (!filter.Zones.IsEmpty())
					{
						if ((filter.ZoneIn && filter.Zones.Contains(door.EnterZoneUID)) || (filter.ZoneOut && filter.Zones.Contains(door.ExitZoneUID)))
							doorMap.Add(door.UID, commonDoor);
					}
					else
					{
						doorMap.Add(door.UID, commonDoor);
					}
				}

				Dictionary<int, string> intervalMap = new Dictionary<int, string>();
				var schedulesResult = dataProvider.DbService.GKScheduleTranslator.Get();
				if (!schedulesResult.HasError)
				{
					foreach (var interval in schedulesResult.Result)
					{
						intervalMap.Add(interval.No, interval.Name);
					}
				}

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
					var employee = dataProvider.GetEmployee(card.EmployeeUID.GetValueOrDefault());
					if (!filter.Schedules.IsEmpty())
						cardDoors = cardDoors.Where(item =>
							(filter.ScheduleEnter && filter.Schedules.Contains(item.EnterScheduleNo)) ||
							(filter.ScheduleExit && filter.Schedules.Contains(item.ExitScheduleNo)));
					foreach (var cardDoor in cardDoors)
						if (doorMap.ContainsKey(cardDoor.DoorUID))
						{
							var door = doorMap[cardDoor.DoorUID];

							var dataRow = dataSet.Data.NewDataRow();
							dataRow.Type = card.GKCardType.ToDescription();
							dataRow.Number = card.Number.ToString();
							if (employee != null)
							{
								dataRow.Employee = employee.Name;
								dataRow.Organisation = employee.Organisation;
								dataRow.Department = employee.Department;
								dataRow.Position = employee.Position;
							}
							dataRow.ZoneIn = door.EnterZoneName;
							dataRow.ZoneOut = door.ExitZoneName;
							dataRow.NoEnterZone = door.NoEnterZone;
							dataRow.NoExitZone = door.NoExitZone;

							if (intervalMap.ContainsKey(cardDoor.EnterScheduleNo))
								dataRow.Enter = intervalMap[cardDoor.EnterScheduleNo];
							if (door.Type != GKDoorType.OneWay && intervalMap.ContainsKey(cardDoor.ExitScheduleNo))
								dataRow.Exit = intervalMap[cardDoor.ExitScheduleNo];
							dataRow.AccessPoint = door.Name;
							dataRow.NoDoor = door.NoDoor;
							dataSet.Data.Rows.Add(dataRow);
						}
				}
			}
			return dataSet;
		}
	}
}
