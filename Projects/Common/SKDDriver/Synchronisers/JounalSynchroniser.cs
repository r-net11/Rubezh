using StrazhAPI;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace StrazhDAL
{
	public class JounalSynchroniser : IDisposable
	{
		private Table<DataAccess.Journal> _Table;

		private string Name { get { return "Journal"; } }

		public string NameXml { get { return Name + ".xml"; } }

		public static string ConnectionString { get; set; }

		private DataAccess.JournalDataContext Context;

		public JounalSynchroniser()
		{
			Context = new DataAccess.JournalDataContext(ConnectionString);
			_Table = Context.Journals;
		}

		public OperationResult Export(JournalExportFilter filter)
		{
			try
			{
				if (!Directory.Exists(filter.Path))
					return new OperationResult("Папка не существует");
				var tableItems = _Table.Where(x => x.SystemDate >= TranslatiorHelper.CheckDate(filter.MinDate) & x.SystemDate <= TranslatiorHelper.CheckDate(filter.MaxDate));
				var items = tableItems.Select(x => Translate(x)).ToList();
				var serializer = new XmlSerializer(typeof(List<ExportJournalItem>));
				using (var fileStream = File.Open(NameXml, FileMode.Create))
				{
					serializer.Serialize(fileStream, items);
				}
				if (filter.Path != null)
				{
					var newPath = Path.Combine(filter.Path, NameXml);
					if (File.Exists(newPath))
						File.Delete(newPath);
					File.Move(NameXml, newPath);
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public ExportJournalItem Translate(DataAccess.Journal tableItem)
		{
			return new ExportJournalItem
			{
				UID = tableItem.UID,
				SystemDate = tableItem.SystemDate,
				DeviceDate = tableItem.DeviceDate != null ? tableItem.DeviceDate.Value : new DateTime(),
				EventName = Enum.IsDefined(typeof(JournalEventNameType), tableItem.Name) ? ((JournalEventNameType)tableItem.Name).ToDescription() : tableItem.NameText,
				EventDescription = Enum.IsDefined(typeof(JournalEventDescriptionType), tableItem.Description) ? ((JournalEventDescriptionType)tableItem.Description).ToDescription() : tableItem.DescriptionText,
				SubsystemType = ((JournalSubsystemType)tableItem.Subsystem).ToDescription(),
				UserName = tableItem.UserName,
				UserUID = tableItem.EmployeeUID != null ? tableItem.EmployeeUID.Value : Guid.Empty
			};
		}

		public void Dispose()
		{
			Context.Dispose();
		}
	}

	public class ExportJournalItem
	{
		public Guid UID { get; set; }

		public DateTime SystemDate { get; set; }

		public DateTime DeviceDate { get; set; }

		public string EventName { get; set; }

		public string EventDescription { get; set; }

		public string SubsystemType { get; set; }

		public Guid UserUID { get; set; }

		public string UserName { get; set; }
	}
}