using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecService.Report.Templates;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EmployeeDoorsReport : BaseReport<List<EmployeeDoorsData>>
	{
		public override List<EmployeeDoorsData> CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
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

			var result = new List<EmployeeDoorsData>();
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

							var data = new EmployeeDoorsData();
							data.Type = card.GKCardType.ToDescription();
							data.Number = card.Number.ToString();
							if (employee != null)
							{
								data.Employee = employee.Name;
								data.Organisation = employee.Organisation;
								data.Department = employee.Department;
								data.Position = employee.Position;
							}
							data.ZoneIn = door.EnterZoneName;
							data.ZoneOut = door.ExitZoneName;
							data.NoEnterZone = door.NoEnterZone;
							data.NoExitZone = door.NoExitZone;

							if (intervalMap.ContainsKey(cardDoor.EnterScheduleNo))
								data.Enter = intervalMap[cardDoor.EnterScheduleNo];
							if (door.Type != GKDoorType.OneWay && intervalMap.ContainsKey(cardDoor.ExitScheduleNo))
								data.Exit = intervalMap[cardDoor.ExitScheduleNo];
							data.AccessPoint = door.Name;
							data.NoDoor = door.NoDoor;
							result.Add(data);
						}
				}
			}
			return result;
		}
	}
}
