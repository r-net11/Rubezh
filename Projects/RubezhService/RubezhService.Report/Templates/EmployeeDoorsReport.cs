using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using RubezhAPI;
using RubezhService.Report.DataSources;
using Infrastructure.Common;

namespace RubezhService.Report.Templates
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
			get { return "Права доступа " + (GetFilter<EmployeeDoorsReportFilter>().IsEmployee ? "сотрудников" : "посетителей"); }
		}

		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
			Name = ReportTitle;
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

	public class CommonDoor
	{
		public string Name { get; private set; }
		public string EnterZoneName { get; private set; }
		public string ExitZoneName { get; private set; }
		public int NoDoor { get; private set; }
		public int NoEnterZone { get; private set; }
		public int NoExitZone { get; private set; }
		public GKDoorType Type {get;private set;}

		public CommonDoor(GKDoor door)
		{

			Name = door.PresentationName;
			NoDoor = door.No;
			Type = door.DoorType;
			var enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.EnterZoneUID);
			if (enterZone != null)
			{
				EnterZoneName = enterZone.PresentationName;
				NoEnterZone = enterZone.No;
			}
			else
				NoEnterZone = Int32.MaxValue;

			var exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.ExitZoneUID);
			if (exitZone != null)
			{
				ExitZoneName = exitZone.PresentationName;
				NoExitZone = exitZone.No;
			}
			else
				NoExitZone = Int32.MaxValue;

		}
	}
}