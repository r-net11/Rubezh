using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecService.Report.Model;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EmployeeAccessReport : BaseReport<List<EmployeeAccessData>>
	{
		public override List<EmployeeAccessData> CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<EmployeeAccessReportFilter>(f);
			if (!filter.PassCardActive && !filter.PassCardForcing && !filter.PassCardLocked && !filter.PassCardOnceOnly && !filter.PassCardPermanent && !filter.PassCardTemprorary)
			{
				filter.PassCardActive = true;
				filter.PassCardForcing = true;
				filter.PassCardLocked = true;
				filter.PassCardOnceOnly = true;
				filter.PassCardPermanent = true;
				filter.PassCardTemprorary = true;
			}

			var cardFilter = new CardFilter();
			cardFilter.EmployeeFilter = dataProvider.GetCardEmployeeFilter(filter);
			cardFilter.DeactivationType = LogicalDeletationType.Active;
			cardFilter.LogicalDeletationType = LogicalDeletationType.Active;
			var cardsResult = dataProvider.DbService.CardTranslator.Get(cardFilter);

			var result = new List<EmployeeAccessData>();
			if (!cardsResult.HasError)
			{
				dataProvider.GetEmployees(cardsResult.Result.Select(item => item.EmployeeUID.GetValueOrDefault()));
				var accessTemplateFilter = new AccessTemplateFilter()
				{
					UIDs = cardsResult.Result.Where(item => item.AccessTemplateUID.HasValue && item.AccessTemplateUID != Guid.Empty).Select(item => item.AccessTemplateUID.Value).ToList()
				};
				var accessTemplates = dataProvider.DbService.AccessTemplateTranslator.Get(accessTemplateFilter);

				var zoneMap = new Dictionary<Guid, Tuple<Tuple<GKSKDZone, string>, Tuple<GKSKDZone, string>>>();
				GKManager.Doors.ForEach(door =>
				{
					if (door != null && !zoneMap.ContainsKey(door.UID))
					{
						GKSKDZone enterZone = null;
						if (filter.Zones.IsEmpty() || filter.Zones.Contains(door.EnterZoneUID))
						{
							enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.EnterZoneUID);
						}
						GKSKDZone exitZone = null;
						if (filter.Zones.IsEmpty() || filter.Zones.Contains(door.ExitZoneUID))
						{
							exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.ExitZoneUID);
						}

						Tuple<GKSKDZone, string> enterZoneTuple = null;
						if (enterZone != null)
						{
							enterZoneTuple = new Tuple<GKSKDZone, string>(enterZone, enterZone.PresentationName);
						}
						Tuple<GKSKDZone, string> exitZoneTuple = null;
						if (exitZone != null)
						{
							exitZoneTuple = new Tuple<GKSKDZone, string>(exitZone, exitZone.PresentationName);
						}
						var value = new Tuple<Tuple<GKSKDZone, string>, Tuple<GKSKDZone, string>>(enterZoneTuple, exitZoneTuple);
						zoneMap.Add(door.UID, value);
					}
				});

				foreach (var card in cardsResult.Result)
				{
					var employee = dataProvider.GetEmployee(card.EmployeeUID.GetValueOrDefault());
					var addedZones = new List<Guid>();
					foreach (var door in card.CardDoors)
						AddRow(result, employee, card, door, null, zoneMap, addedZones);
					if (!accessTemplates.HasError && card.AccessTemplateUID.HasValue)
					{
						var cardDoorUIDs = card.CardDoors.Select(item => item.DoorUID);
						var accessTemplate = accessTemplates.Result.FirstOrDefault(item => item.UID == card.AccessTemplateUID.Value);
						if (accessTemplates != null)
							foreach (var door in accessTemplate.CardDoors.Where(item => !cardDoorUIDs.Contains(item.DoorUID)))
								AddRow(result, employee, card, door, accessTemplate, zoneMap, addedZones);
					}
				}
			}
			return result;
		}

		private void AddRow(List<EmployeeAccessData> ds, EmployeeInfo employee, SKDCard card, CardDoor door, AccessTemplate template, Dictionary<Guid, Tuple<Tuple<GKSKDZone, string>, Tuple<GKSKDZone, string>>> zoneMap, List<Guid> addedZones)
		{
			if (!zoneMap.ContainsKey(door.DoorUID))
				return;
			var zones = zoneMap[door.DoorUID];
			var data = new EmployeeAccessData();
			data.Type = card.GKCardType.ToDescription();
			data.Number = card.Number.ToString();
			if (employee != null)
			{
				data.Employee = employee.Name;
				data.Organisation = employee.Organisation;
				data.Department = employee.Department;
				data.Position = employee.Position;
			}
			if (template != null)
				data.Template = template.Name;
			if (zones.Item1 != null && !addedZones.Contains(zones.Item1.Item1.UID))
			{
				var data1 = new EmployeeAccessData();
				data1.ItemArray = data.ItemArray;
				data1.Zone = zones.Item1.Item2;
				data1.No = zones.Item1.Item1.No;
				ds.Add(data1);
				addedZones.Add(zones.Item1.Item1.UID);
			}
			if (zones.Item2 != null && !addedZones.Contains(zones.Item2.Item1.UID))
			{
				var data2 = new EmployeeAccessData();
				data2.ItemArray = data.ItemArray;
				data2.Zone = zones.Item2.Item2;
				data2.No = zones.Item2.Item1.No;
				ds.Add(data2);
				addedZones.Add(zones.Item2.Item1.UID);
			}
		}
	}
}
