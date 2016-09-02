using Common;
using Localization.FiresecService.Report.Common;
using StrazhAPI;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using FiresecService.Report.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
			get { return CommonResources.AccessZone + (GetFilter<EmployeeAccessReportFilter>().IsEmployee ? CommonResources.Employees : CommonResources.Visitors); }
		}

		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
			Name = ReportTitle;
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeAccessReportFilter>();
			if (!filter.PassCardActive && !filter.PassCardForcing && !filter.PassCardLocked && !filter.PassCardGuest && !filter.PassCardPermanent && !filter.PassCardTemprorary)
			{
				filter.PassCardActive = true;
				filter.PassCardForcing = true;
				filter.PassCardLocked = true;
				filter.PassCardGuest = true;
				filter.PassCardPermanent = true;
				filter.PassCardTemprorary = true;
			}

			var cardFilter = new CardFilter();
			cardFilter.EmployeeFilter = dataProvider.GetCardEmployeeFilter(filter);
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

				var zoneMap = new Dictionary<Guid, Tuple<Tuple<SKDZone, string>, Tuple<SKDZone, string>>>();

				foreach (var door in SKDManager.Doors)
				{
					if (door == null || zoneMap.ContainsKey(door.UID)) continue;

					SKDZone enterZone = null;
					if (filter.Zones.IsEmpty() || filter.Zones.Contains(door.InDevice.ZoneUID))
						enterZone = SKDManager.Zones.FirstOrDefault(x => x.UID == door.InDevice.ZoneUID);

					SKDZone exitZone = null;
					if (filter.Zones.IsEmpty() || filter.Zones.Contains(door.OutDevice.ZoneUID))
						exitZone = SKDManager.Zones.FirstOrDefault(x => x.UID == door.OutDevice.ZoneUID);

					Tuple<SKDZone, string> enterZoneTuple = null;
					if (enterZone != null)
						enterZoneTuple = new Tuple<SKDZone, string>(enterZone, enterZone.Name);

					Tuple<SKDZone, string> exitZoneTuple = null;
					if (exitZone != null)
						exitZoneTuple = new Tuple<SKDZone, string>(exitZone, exitZone.Name);

					var value = new Tuple<Tuple<SKDZone, string>, Tuple<SKDZone, string>>(enterZoneTuple, exitZoneTuple);
					zoneMap.Add(door.UID, value);
				}

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

						if (accessTemplate != null)
							foreach (var door in accessTemplate.CardDoors.Where(item => !cardDoorUIDs.Contains(item.DoorUID)))
								AddRow(dataSet, employee, card, door, accessTemplate, zoneMap, addedZones);
					}
				}
			}
			return dataSet;
		}

		private void AddRow(EmployeeAccessDataSet ds, EmployeeInfo employee, SKDCard card, CardDoor door, AccessTemplate template, Dictionary<Guid, Tuple<Tuple<SKDZone, string>, Tuple<SKDZone, string>>> zoneMap, List<Guid> addedZones)
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
			if (zones.Item1 != null && !addedZones.Contains(zones.Item1.Item1.UID))
			{
				var row1 = ds.Data.NewDataRow();
				row1.ItemArray = dataRow.ItemArray;
				row1.Zone = zones.Item1.Item2;
				ds.Data.AddDataRow(row1);
				addedZones.Add(zones.Item1.Item1.UID);
			}
			if (zones.Item2 != null && !addedZones.Contains(zones.Item2.Item1.UID))
			{
				var row2 = ds.Data.NewDataRow();
				row2.ItemArray = dataRow.ItemArray;
				row2.Zone = zones.Item2.Item2;
				ds.Data.AddDataRow(row2);
				addedZones.Add(zones.Item2.Item1.UID);
			}
		}
	}
}