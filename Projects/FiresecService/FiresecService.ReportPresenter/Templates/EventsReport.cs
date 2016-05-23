using Common;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.SKD.ReportFilters;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class EventsReport : BaseReport
	{
		public EventsReport()
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
			get { return "Отчет по событиям"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EventsReportFilter>();
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

		string BuildQuery(JournalFilter archiveFilter)
		{
			string dateTimeTypeString;
			if (archiveFilter.UseDeviceDateTime)
				dateTimeTypeString = "DeviceDate";
			else
				dateTimeTypeString = "SystemDate";

			var query =
				"SELECT * FROM Journal WHERE " +
				"\n " + dateTimeTypeString + " > '" + archiveFilter.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
				"\n AND " + dateTimeTypeString + " < '" + archiveFilter.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";

			if (archiveFilter.JournalEventNameTypes.Count > 0)
			{
				query += "\n and (";
				int index = 0;
				foreach (var journalEventNameType in archiveFilter.JournalEventNameTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "Name = '" + (int)journalEventNameType + "'";
				}
				query += ")";
			}

			if (archiveFilter.JournalSubsystemTypes.Count > 0)
			{
				query += "\n AND (";
				int index = 0;
				foreach (var journalSubsystemType in archiveFilter.JournalSubsystemTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "Subsystem = '" + (int)journalSubsystemType + "'";
				}
				query += ")";
			}

			if (archiveFilter.JournalObjectTypes.Count > 0)
			{
				query += "\n AND (";
				int index = 0;
				foreach (var journalObjectType in archiveFilter.JournalObjectTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "ObjectType = '" + (int)journalObjectType + "'";
				}
				query += ")";
			}

			if (archiveFilter.ObjectUIDs.Count > 0)
			{
				query += "\n AND (";
				int index = 0;
				foreach (var objectUID in archiveFilter.ObjectUIDs)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "ObjectUID = '" + objectUID + "'";
				}
				query += ")";
			}

			if (archiveFilter.EmployeeUIDs.Count > 0)
			{
				query += "\n AND (";
				int index = 0;
				foreach (var employeeUID in archiveFilter.EmployeeUIDs)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					//query += "EmployeeUID = '" + employeeUID + "'";
					query += "ObjectUID = '" + employeeUID + "'";
				}
				query += ")";
			}

			if (archiveFilter.Users.Count > 0)
			{
				query += "\n AND (";
				int index = 0;
				foreach (var user in archiveFilter.Users)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "UserName = '" + user + "'";
				}
				query += ")";
			}
			return query;
		}

		JournalItem ReadOneJournalItem(SqlDataReader reader)
		{
			var journalItem = new JournalItem();

			if (!reader.IsDBNull(reader.GetOrdinal("DescriptionText")))
				journalItem.DescriptionText = reader.GetString(reader.GetOrdinal("DescriptionText"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectName")))
				journalItem.ObjectName = reader.GetString(reader.GetOrdinal("ObjectName"));

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectType")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("ObjectType"));
				if (Enum.IsDefined(typeof(JournalObjectType), intValue))
					journalItem.JournalObjectType = (JournalObjectType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("ObjectUID")))
				journalItem.ObjectUID = reader.GetGuid(reader.GetOrdinal("ObjectUID"));

			if (!reader.IsDBNull(reader.GetOrdinal("Subsystem")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("Subsystem"));
				if (Enum.IsDefined(typeof(JournalSubsystemType), intValue))
					journalItem.JournalSubsystemType = (JournalSubsystemType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("UID")))
				journalItem.UID = reader.GetGuid(reader.GetOrdinal("UID"));

			if (!reader.IsDBNull(reader.GetOrdinal("UserName")))
				journalItem.UserName = reader.GetString(reader.GetOrdinal("UserName"));

			if (!reader.IsDBNull(reader.GetOrdinal("SystemDate")))
				journalItem.SystemDateTime = reader.GetDateTime(reader.GetOrdinal("SystemDate"));

			if (!reader.IsDBNull(reader.GetOrdinal("DeviceDate")))
				journalItem.DeviceDateTime = reader.GetDateTime(reader.GetOrdinal("DeviceDate"));

			if (!reader.IsDBNull(reader.GetOrdinal("Name")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("Name"));
				if (Enum.IsDefined(typeof(JournalEventNameType), intValue))
					journalItem.JournalEventNameType = (JournalEventNameType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("Description")))
			{
				var intValue = (int)reader.GetValue(reader.GetOrdinal("Description"));
				if (Enum.IsDefined(typeof(JournalEventDescriptionType), intValue))
					journalItem.JournalEventDescriptionType = (JournalEventDescriptionType)intValue;
			}

			if (!reader.IsDBNull(reader.GetOrdinal("Detalisation")))
			{
				var detalisationString = reader.GetString(reader.GetOrdinal("Detalisation"));
				journalItem.JournalDetalisationItems = JournalDetalisationItem.StringToList(detalisationString);
			}
			return journalItem;
		}
	}
}