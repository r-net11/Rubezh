using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using RubezhDAL.DataClasses;
using System.Data.Entity;


namespace RubezhDAL
{
	public class JounalSynchroniser: IDisposable
	{
		DbSet<Journal> _Table;
		string Name { get { return "Journal"; } }
		public string NameXml { get { return Name + ".xml"; } }
		DatabaseContext Context;

		public JounalSynchroniser(DbService dbService)
		{
			Context = dbService.Context;
			_Table = Context.Journals;
		}

		public OperationResult<bool> Export(JournalExportFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (!Directory.Exists(filter.Path))
					throw new Exception("Папка не существует");
				var minDate = filter.MinDate.CheckDate();
				var maxDate = filter.MaxDate.CheckDate();
				var tableItems = _Table.Where(x => x.SystemDate >= minDate & x.SystemDate <= maxDate).ToList();
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
				return true;
			});
		}

		public ExportJournalItem Translate(Journal tableItem)
		{
			return new ExportJournalItem
			{
				UID = tableItem.UID,
				SystemDate = tableItem.SystemDate,
				DeviceDate = tableItem.DeviceDate != null ? tableItem.DeviceDate.Value : new DateTime(),
				EventName = ((JournalEventNameType)tableItem.Name).ToDescription(),
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
