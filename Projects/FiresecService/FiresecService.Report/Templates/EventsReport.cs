using Common;
using Localization.FiresecService.Report.Common;
using StrazhAPI;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using StrazhDAL;

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
			get { return CommonResources.SystemEventsReport; }
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EventsReportFilter>();
			var dataSet = new EventsDataSet();
			var archiveFilter = new ArchiveFilter {StartDate = filter.DateTimeFrom, EndDate = filter.DateTimeTo};

			if (!filter.JournalEventNameTypes.IsEmpty())
				archiveFilter.JournalEventNameTypes = filter.JournalEventNameTypes;

			if (!filter.JournalObjectTypes.IsEmpty())
				archiveFilter.JournalObjectTypes = filter.JournalObjectTypes;

			if (!filter.JournalEventSubsystemTypes.IsEmpty())
				archiveFilter.JournalSubsystemTypes = filter.JournalEventSubsystemTypes;

			if (!filter.JournalObjectTypes.IsEmpty())
				archiveFilter.JournalObjectTypes = filter.JournalObjectTypes;

			if (!filter.ObjectUIDs.IsEmpty())
				archiveFilter.ObjectUIDs = filter.ObjectUIDs;

			if (!filter.Employees.IsEmpty())
				archiveFilter.EmployeeUIDs = filter.Employees;

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
						dataRow.DeviceDateTime = journalItem.DeviceDateTime.Value;

					if (journalItem.JournalEventNameType != JournalEventNameType.NULL)
						dataRow.Name = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventNameType);

					if (journalItem.JournalEventDescriptionType != JournalEventDescriptionType.NULL)
					{
						dataRow.Description = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventDescriptionType);
						if (!string.IsNullOrEmpty(journalItem.DescriptionText))
							dataRow.Description += " " + journalItem.DescriptionText;
					}
					else
						dataRow.Description = journalItem.DescriptionText;

					switch (journalItem.JournalObjectType)
					{
						case JournalObjectType.SKDDevice:
							var skdDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (skdDevice != null)
								dataRow.Object = skdDevice.Name;
							break;

						case JournalObjectType.SKDZone:
							var skdZone = SKDManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (skdZone != null)
								dataRow.Object = skdZone.Name;
							break;

						case JournalObjectType.SKDDoor:
							var skdDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							if (skdDoor != null)
								dataRow.Object = skdDoor.Name;
							break;

						case JournalObjectType.VideoDevice:
							break;

						case JournalObjectType.None:
							dataRow.Object = journalItem.ObjectName ?? string.Empty;
							break;
					}

					dataRow.System = journalItem.JournalSubsystemType.ToDescription();
					dataRow.User = journalItem.UserName;

					dataSet.Data.Rows.Add(dataRow);
				}
			}
			return dataSet;
		}

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(ArchiveFilter archiveFilter)
		{
			try
			{
				var journalItems = new List<JournalItem>();
				using (var dataContext = new SqlConnection(SKDDatabaseService.JournalConnectionString))
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

		private string BuildQuery(ArchiveFilter archiveFilter)
		{
			var dateTimeTypeString = archiveFilter.UseDeviceDateTime ? "DeviceDate" : "SystemDate";

			var query =
				"SELECT * FROM Journal WHERE " +
				"\n " + dateTimeTypeString + " > '" + archiveFilter.StartDate.ToString("yyyy-MM-ddTHH:mm:ss") + "'" +
				"\n AND " + dateTimeTypeString + " < '" + archiveFilter.EndDate.ToString("yyyy-MM-ddTHH:mm:ss") + "'";

			if (archiveFilter.JournalEventNameTypes.Any())
			{
				query += "\n and (";
				var index = 0;
				foreach (var journalEventNameType in archiveFilter.JournalEventNameTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "Name = '" + (int)journalEventNameType + "'";
				}
				query += ")";
			}

			if (archiveFilter.JournalSubsystemTypes.Any())
			{
				query += "\n AND (";
				var index = 0;
				foreach (var journalSubsystemType in archiveFilter.JournalSubsystemTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "Subsystem = '" + (int)journalSubsystemType + "'";
				}
				query += ")";
			}

			if (archiveFilter.JournalObjectTypes.Any())
			{
				query += "\n AND (";
				var index = 0;
				foreach (var journalObjectType in archiveFilter.JournalObjectTypes)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "ObjectType = '" + (int)journalObjectType + "'";
				}
				query += ")";
			}

			if (archiveFilter.ObjectUIDs.Any())
			{
				query += "\n AND (";
				var index = 0;
				foreach (var objectUID in archiveFilter.ObjectUIDs)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					query += "ObjectUID = '" + objectUID + "'";
				}
				query += ")";
			}

			if (archiveFilter.EmployeeUIDs.Any())
			{
				query += "\n AND ((";
				var index = 0;
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

			if (archiveFilter.EmployeeUIDs.Any())
			{
				query += "\n OR (";
				var index = 0;
				foreach (var employeeUID in archiveFilter.EmployeeUIDs)
				{
					if (index > 0)
						query += "\n OR ";
					index++;
					using (var db = new SKDDatabaseService())
					{
						var shortEmployee = db.EmployeeTranslator.GetByUid(employeeUID);
						if (shortEmployee.Result != null)
							query += "UserName = '" + shortEmployee.Result.FIO + "'";
					}
				}
				query += "))";
			}

			if (archiveFilter.Users.Any())
			{
				query += "\n AND (";
				var index = 0;
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

		private JournalItem ReadOneJournalItem(SqlDataReader reader)
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