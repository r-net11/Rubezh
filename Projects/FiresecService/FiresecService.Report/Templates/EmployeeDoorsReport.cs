using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using FiresecClient;
using FiresecAPI.GK;
using Infrastructure.Common;

namespace FiresecService.Report.Templates
{
	public partial class EmployeeDoorsReport : BaseReport
	{
		public EmployeeDoorsReport()
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
			get { return "Права доступа сотрудников (посетителей)"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeDoorsReportFilter>();
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

			var dataSet = new EmployeeDoorsDataSet();
			if (!cardsResult.HasError)
			{
				dataProvider.GetEmployees(cardsResult.Result.Select(item => item.EmployeeUID));
				var accessTemplateFilter = new AccessTemplateFilter()
				{
					UIDs = cardsResult.Result.Where(item => item.AccessTemplateUID.HasValue && item.AccessTemplateUID != Guid.Empty).Select(item => item.AccessTemplateUID.Value).ToList()
				};
				var accessTemplates = dataProvider.DatabaseService.AccessTemplateTranslator.Get(accessTemplateFilter);


				var doorMap = new Dictionary<Guid, CommonDoor>();
				foreach (var door in SKDManager.Doors)
				{
					var commonDoor = new CommonDoor(door);
					if (!filter.Zones.IsEmpty())
					{
						if ((filter.ZoneIn && door.InDevice != null && filter.Zones.Contains(door.InDevice.ZoneUID)) || (filter.ZoneOut && door.OutDevice != null && filter.Zones.Contains(door.OutDevice.ZoneUID)))
							doorMap.Add(door.UID, commonDoor);
					}
					else
					{
						doorMap.Add(door.UID, commonDoor);
					}
				}
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

				Dictionary<int, string> intervalMap = new Dictionary<int,string>();
				if (GlobalSettingsHelper.GlobalSettings.UseStrazhBrand)
				{
					foreach (var interval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals)
					{
						intervalMap.Add(interval.ID, interval.Name);
					}
				}
				else
				{
					foreach (var interval in GKManager.DeviceConfiguration.Schedules)
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
							dataRow.ZoneIn = door.EnterZoneName;
							dataRow.ZoneOut = door.ExitZoneName;
							if (intervalMap.ContainsKey(cardDoor.EnterScheduleNo))
								dataRow.Enter = intervalMap[cardDoor.EnterScheduleNo];
							if (intervalMap.ContainsKey(cardDoor.ExitScheduleNo))
								dataRow.Exit = intervalMap[cardDoor.ExitScheduleNo];
							dataRow.AccessPoint = door.Name;
							dataSet.Data.Rows.Add(dataRow);
						}
				}
			}
			return dataSet;
		}
	}

	public class CommonDoor
	{
		public string Name { get; private set; }
		public string EnterZoneName { get; private set; }
		public string ExitZoneName { get; private set; }

		public CommonDoor(SKDDoor door)
		{
			Name = door.PresentationName;
			if (door.InDevice != null && door.InDevice.Zone != null)
				EnterZoneName = door.InDevice.Zone.PresentationName;
			if (door.OutDevice != null && door.OutDevice.Zone != null)
				ExitZoneName = door.OutDevice.Zone.PresentationName;
		}

		public CommonDoor(GKDoor door)
		{
			Name = door.PresentationName;
			var enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.EnterZoneUID);
			if(enterZone != null)
				EnterZoneName = enterZone.PresentationName;
			var exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.ExitZoneUID);
			if (exitZone != null)
				ExitZoneName = exitZone.PresentationName;
		}
	}
}