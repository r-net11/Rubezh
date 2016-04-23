using System.Data;
using System.Linq;
using Common;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EventsReport : BaseReport
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<EventsReportFilter>(f);
			var dataSet = new EventsDataSet();
			var archiveFilter = new JournalFilter();
			archiveFilter.StartDate = filter.DateTimeFrom;
			archiveFilter.EndDate = filter.DateTimeTo;
			if (filter.JournalEventNameTypes != null && filter.JournalEventNameTypes.Count > 0)
			{
				archiveFilter.JournalEventNameTypes = filter.JournalEventNameTypes;
			}
			if (filter.JournalObjectTypes != null && filter.JournalObjectTypes.Count > 0)
			{
				archiveFilter.JournalObjectTypes = filter.JournalObjectTypes;
			}
			if (filter.JournalEventSubsystemTypes != null && filter.JournalEventSubsystemTypes.Count > 0)
			{
				archiveFilter.JournalSubsystemTypes = filter.JournalEventSubsystemTypes;
			}
			if (filter.JournalObjectTypes != null && filter.JournalObjectTypes.Count > 0)
			{
				archiveFilter.JournalObjectTypes = filter.JournalObjectTypes;
			}
			if (filter.ObjectUIDs != null && filter.ObjectUIDs.Count > 0)
			{
				archiveFilter.ObjectUIDs = filter.ObjectUIDs;
			}
			if (filter.Employees != null && filter.Employees.Count > 0)
			{
				archiveFilter.EmployeeUIDs = filter.Employees;
			}
			if (!filter.Users.IsEmpty())
				archiveFilter.Users = filter.Users;
			var journalItemsResult = dataProvider.DbService.JournalTranslator.GetFilteredArchiveItems(archiveFilter);
			if (journalItemsResult.Result != null)
			{
				foreach (var journalItem in journalItemsResult.Result)
				{
					var dataRow = dataSet.Data.NewDataRow();
					dataRow.SystemDateTime = journalItem.SystemDateTime;
					if (journalItem.DeviceDateTime.HasValue)
					{
						dataRow.DeviceDateTime = journalItem.DeviceDateTime.Value;
					}

					if (journalItem.JournalEventNameType != JournalEventNameType.NULL)
					{
						dataRow.Name = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventNameType);
					}

					if (journalItem.JournalEventDescriptionType != JournalEventDescriptionType.NULL)
					{
						dataRow.Description = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventDescriptionType);
						if (!string.IsNullOrEmpty(journalItem.DescriptionText))
							dataRow.Description += " " + journalItem.DescriptionText;
					}
					else
					{
						dataRow.Description = journalItem.DescriptionText;
					}

					switch (journalItem.JournalObjectType)
					{
						case JournalObjectType.GKDevice:
							var device = GKManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (device != null)
							{
								dataRow.Object = device.PresentationName;
							}
							break;

						case JournalObjectType.GKZone:
							var zone = GKManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (zone != null)
							{
								dataRow.Object = zone.PresentationName;
							}
							break;

						case JournalObjectType.GKSKDZone:
							var SKDzone = GKManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (SKDzone != null)
							{
								dataRow.Object = SKDzone.PresentationName;
							}
							break;

						case JournalObjectType.GKDirection:
							var direction = GKManager.Directions.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (direction != null)
							{
								dataRow.Object = direction.PresentationName;
							}
							break;

						case JournalObjectType.GKPumpStation:
							var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (pumpStation != null)
							{
								var objectName = pumpStation.PresentationName;
							}
							break;

						case JournalObjectType.GKMPT:
							var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (mpt != null)
							{
								dataRow.Object = mpt.PresentationName;
							}
							break;

						case JournalObjectType.GKDelay:
							var delay = GKManager.Delays.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (delay != null)
							{
								dataRow.Object = delay.PresentationName;
							}
							break;

						case JournalObjectType.GKGuardZone:
							var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (guardZone != null)
							{
								dataRow.Object = guardZone.PresentationName;
							}
							break;

						case JournalObjectType.GKDoor:
							var gkDoor = GKManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (gkDoor != null)
							{
								dataRow.Object = gkDoor.PresentationName;
							}
							break;

						case JournalObjectType.Camera:
							//var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							//if (camera != null)
							//{
							//	dataRow.Object = camera.Name;
							//}
							break;

						case JournalObjectType.None:
						case JournalObjectType.GKUser:
							dataRow.Object = journalItem.ObjectName != null ? journalItem.ObjectName : "";
							break;
					}

					if (dataRow.IsObjectNull())
					{
						dataRow.Object = journalItem.ObjectName;
					}

					if (dataRow.IsObjectNull())
						dataRow.Object = "<Нет в конфигурации>";

					dataRow.System = journalItem.JournalSubsystemType.ToDescription();
					dataRow.User = journalItem.UserName;

					dataSet.Data.Rows.Add(dataRow);
				}
			}
			else
			{
				if (journalItemsResult.HasError)
					ThrowException(journalItemsResult.Error);
				else
					ThrowException("Exception was trown in EventsReport.CreateDataSet()");
			}
			return dataSet;
		}
	}
}
