using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhDAL.DataClasses;
using System.Data.Entity;

namespace RubezhDAL
{
	public class PassJounalSynchroniser
	{
		DbSet<PassJournal> _Table;
		string Name { get { return "PassJournal"; } }
		public string NameXml { get { return Name + ".xml"; } }

		public PassJounalSynchroniser(DbSet<PassJournal> table)
		{
			_Table = table;
		}

		public OperationResult<bool> Export(JournalExportFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (!Directory.Exists(filter.Path))
					throw new Exception("Папка не существует");
				var tableItems = _Table.Where(x => x.EnterTime >= filter.MinDate.CheckDate() & x.EnterTime <= filter.MaxDate.CheckDate());
				var items = tableItems.Select(x => Translate(x)).ToList();
				var serializer = new XmlSerializer(typeof(List<ExportPassJournalItem>));
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

		ExportPassJournalItem Translate(PassJournal tableItem)
		{
			return new ExportPassJournalItem
			{
				UID = tableItem.UID,
				EmployeeUID = tableItem.EmployeeUID.GetValueOrDefault(),
				EnterDateTime = tableItem.EnterTime,
				ExitDateTime = tableItem.ExitTime != null ? tableItem.ExitTime.Value : new DateTime(),
				ZoneUID = tableItem.ZoneUID
			};
		}
	}

	public class ExportPassJournalItem
	{
		public Guid UID { get; set; }
		public Guid EmployeeUID { get; set; }
		public Guid ZoneUID { get; set; }
		public DateTime EnterDateTime { get; set; }
		public DateTime ExitDateTime { get; set; }
	}
}
