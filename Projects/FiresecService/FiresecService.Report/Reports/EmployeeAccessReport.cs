using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecService.Report.DataSources;
using FiresecService.Report.Model;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EmployeeAccessReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
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

			var dataSet = new EmployeeAccessDataSet();
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
						AddRow(dataSet, employee, card, door, null, zoneMap, addedZones);
					if (!accessTemplates.HasError && card.AccessTemplateUID.HasValue)
					{
						var cardDoorUIDs = card.CardDoors.Select(item => item.DoorUID);
						var accessTemplate = accessTemplates.Result.FirstOrDefault(item => item.UID == card.AccessTemplateUID.Value);
						if (accessTemplates != null)
							foreach (var door in accessTemplate.CardDoors.Where(item => !cardDoorUIDs.Contains(item.DoorUID)))
								AddRow(dataSet, employee, card, door, accessTemplate, zoneMap, addedZones);
					}
				}
			}
			return dataSet;
		}

		private void AddRow(EmployeeAccessDataSet ds, EmployeeInfo employee, SKDCard card, CardDoor door, AccessTemplate template, Dictionary<Guid, Tuple<Tuple<GKSKDZone, string>, Tuple<GKSKDZone, string>>> zoneMap, List<Guid> addedZones)
		{
			if (!zoneMap.ContainsKey(door.DoorUID))
				return;
			var zones = zoneMap[door.DoorUID];
			var dataRow = ds.Data.NewDataRow();
			dataRow.Type = card.GKCardType.ToDescription();
			dataRow.Number = card.Number.ToString();
			if (employee != null)
			{
				dataRow.Employee = employee.Name;
				dataRow.Organisation = employee.Organisation;
				dataRow.Department = employee.Department;
				dataRow.Position = employee.Position;
			}
			if (template != null)
				dataRow.Template = template.Name;
			if (zones.Item1 != null && !addedZones.Contains(zones.Item1.Item1.UID))
			{
				var row1 = ds.Data.NewDataRow();
				row1.ItemArray = dataRow.ItemArray;
				row1.Zone = zones.Item1.Item2;
				row1.No = zones.Item1.Item1.No;
				ds.Data.AddDataRow(row1);
				addedZones.Add(zones.Item1.Item1.UID);
			}
			if (zones.Item2 != null && !addedZones.Contains(zones.Item2.Item1.UID))
			{
				var row2 = ds.Data.NewDataRow();
				row2.ItemArray = dataRow.ItemArray;
				row2.Zone = zones.Item2.Item2;
				row2.No = zones.Item2.Item1.No;
				ds.Data.AddDataRow(row2);
				addedZones.Add(zones.Item2.Item1.UID);
			}
		}
	}
}
