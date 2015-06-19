﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecClient;
using FiresecService.Report.DataSources;
using Infrastructure.Common;

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
			get { return "Отчет по событиям системы контроля доступа"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EventsReportFilter>();
			var dataSet = new EventsDataSet();
			var archiveFilter = new ArchiveFilter();
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
			var journalItemsResult = GetFilteredJournalItems(archiveFilter);
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

						case JournalObjectType.GKDoor:
							var gkDoor = GKManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (gkDoor != null)
							{
								dataRow.Object = gkDoor.PresentationName;
							}
							break;

						case JournalObjectType.SKDDevice:
							var skdDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (skdDevice != null)
							{
								dataRow.Object = skdDevice.Name;
							}
							break;

						case JournalObjectType.SKDZone:
							var skdZone = SKDManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (skdZone != null)
							{
								dataRow.Object = skdZone.Name;
							}
							break;

						case JournalObjectType.SKDDoor:
							var skdDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (skdDoor != null)
							{
								dataRow.Object = skdDoor.Name;
							}
							break;

						case JournalObjectType.VideoDevice:
							//var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							//if (camera != null)
							//{
							//	dataRow.Object = camera.Name;
							//}
							break;

						case JournalObjectType.None:
							dataRow.Object = journalItem.ObjectName != null ? journalItem.ObjectName : "";
							break;
					}

					//if (dataRow.Object == null)
					//{
					//	dataRow.Object = journalItem.ObjectName;
					//}

					//if (dataRow.Object == null)
					//	dataRow.Object = "<Нет в конфигурации>";

					dataRow.System = journalItem.JournalSubsystemType.ToDescription();
					dataRow.User = journalItem.UserName;

					dataSet.Data.Rows.Add(dataRow);
				}
			}
			return dataSet;
		}

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(ArchiveFilter archiveFilter)
		{
			var ConnectionString = @"Data Source=.\" + GlobalSettingsHelper.GlobalSettings.DBServerName + ";Initial Catalog=Journal_" + "1" + ";Integrated Security=True;Language='English'";

			try
			{
				var journalItems = new List<JournalItem>();
				using (var dataContext = new SqlConnection(ConnectionString))
				{
					var query = BuildQuery(archiveFilter);
					var sqlCommand = new SqlCommand(query, dataContext);
					dataContext.Open();
					var reader = sqlCommand.ExecuteReader();
					while (reader.Read())
					{
						var journalItem = ReadOneJournalItem(reader);
						journalItems.Add(journalItem);
					}
				}
				journalItems.Reverse();
				return new OperationResult<List<JournalItem>>(journalItems);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Report401.GetFilteredJournalItems");
				return OperationResult<List<JournalItem>>.FromError(e.Message);
			}
		}

		string BuildQuery(ArchiveFilter archiveFilter)
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