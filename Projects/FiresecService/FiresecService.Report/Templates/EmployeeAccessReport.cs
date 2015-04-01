﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using FiresecService.Report.Model;
using FiresecClient;
using FiresecAPI.GK;

namespace FiresecService.Report.Templates
{
	public partial class EmployeeAccessReport : BaseReport
	{
		public EmployeeAccessReport()
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
			get { return @"Доступ в зоны сотрудников (посетителей)"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeAccessReportFilter>();
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

			var dataSet = new EmployeeAccessDataSet();
			if (!cardsResult.HasError)
			{
				dataProvider.GetEmployees(cardsResult.Result.Select(item => item.EmployeeUID));
				var accessTemplateFilter = new AccessTemplateFilter()
				{
					UIDs = cardsResult.Result.Where(item => item.AccessTemplateUID.HasValue && item.AccessTemplateUID != Guid.Empty).Select(item => item.AccessTemplateUID.Value).ToList()
				};
				var accessTemplates = dataProvider.DatabaseService.AccessTemplateTranslator.Get(accessTemplateFilter);

				var zoneMap = new Dictionary<Guid, Tuple<Tuple<Guid, string>, Tuple<Guid, string>>>();
				SKDManager.Doors.ForEach(door =>
				{
					if (door != null && !zoneMap.ContainsKey(door.UID))
					{
						var zone1 = door.InDevice != null && door.InDevice.Zone != null && (filter.Zones.IsEmpty() || filter.Zones.Contains(door.InDevice.Zone.UID)) ? door.InDevice.Zone : null;
						var zone2 = door.OutDevice != null && door.OutDevice.Zone != null && (filter.Zones.IsEmpty() || filter.Zones.Contains(door.OutDevice.Zone.UID)) ? door.OutDevice.Zone : null;
						if (zone1 != null || zone2 != null)
						{
							if (zone1 == zone2)
								zone2 = null;
							var value = new Tuple<Tuple<Guid, string>, Tuple<Guid, string>>(new Tuple<Guid, string>(zone1.UID, zone1.PresentationName), new Tuple<Guid, string>(zone2.UID, zone2.PresentationName));
							zoneMap.Add(door.UID, value);
						}
					}
				});
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

						Tuple<Guid, string> enterZoneTuple = null;
						if (enterZone != null)
						{
							enterZoneTuple = new Tuple<Guid, string>(enterZone.UID, enterZone.PresentationName);
						}
						Tuple<Guid, string> exitZoneTuple = null;
						if (enterZone != null)
						{
							exitZoneTuple = new Tuple<Guid, string>(exitZone.UID, exitZone.PresentationName);
						}
						var value = new Tuple<Tuple<Guid, string>, Tuple<Guid, string>>(enterZoneTuple, exitZoneTuple);
						zoneMap.Add(door.UID, value);
					}
				});

				foreach (var card in cardsResult.Result)
				{
					var employee = dataProvider.GetEmployee(card.EmployeeUID);
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
		private void AddRow(EmployeeAccessDataSet ds, EmployeeInfo employee, SKDCard card, CardDoor door, AccessTemplate template, Dictionary<Guid, Tuple<Tuple<Guid, string>, Tuple<Guid, string>>> zoneMap, List<Guid> addedZones)
		{
			if (!zoneMap.ContainsKey(door.DoorUID))
				return;
			var zones = zoneMap[door.DoorUID];
			var dataRow = ds.Data.NewDataRow();
			dataRow.Type = card.CardType.ToDescription();
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
			if (zones.Item1 != null && !addedZones.Contains(zones.Item1.Item1))
			{
				var row1 = ds.Data.NewDataRow();
				row1.ItemArray = dataRow.ItemArray;
				row1.Zone = zones.Item1.Item2;
				ds.Data.AddDataRow(row1);
				addedZones.Add(zones.Item1.Item1);
			}
			if (zones.Item2 != null && !addedZones.Contains(zones.Item1.Item1))
			{
				var row2 = ds.Data.NewDataRow();
				row2.ItemArray = dataRow.ItemArray;
				row2.Zone = zones.Item2.Item2;
				ds.Data.AddDataRow(row2);
				addedZones.Add(zones.Item2.Item1);
			}
		}
	}
}