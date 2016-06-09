using Common;
using StrazhAPI;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace StrazhDAL
{
	public class PassJounalSynchroniser
	{
		private readonly Table<DataAccess.PassJournal> _table;

		private static string Name { get { return "PassJournal"; } }

		public string NameXml { get { return Name + ".xml"; } }

		public PassJounalSynchroniser(Table<DataAccess.PassJournal> table)
		{
			_table = table;
		}

		public OperationResult Export(JournalExportFilter filter)
		{
			if (filter == null || string.IsNullOrEmpty(filter.Path))
				return new OperationResult("Path is null");

			try
			{
				Directory.CreateDirectory(filter.Path);
				var tableItems = _table.Where(x => x.EnterTime >= TranslatiorHelper.CheckDate(filter.MinDate) & x.EnterTime <= TranslatiorHelper.CheckDate(filter.MaxDate));
				var items = tableItems.Select(x => Translate(x)).ToList();
				var serializer = new XmlSerializer(typeof(List<ExportPassJournalItem>));

				using (var fileStream = File.Open(NameXml, FileMode.Create))
				{
					serializer.Serialize(fileStream, items);
				}

				var newPath = Path.Combine(filter.Path, NameXml);
				if (File.Exists(newPath))
					File.Delete(newPath);
				File.Move(NameXml, newPath);

				return new OperationResult();
			}
			catch (Exception e)
			{
				Logger.Error(e);
				return new OperationResult(e.Message);
			}
		}

		private static ExportPassJournalItem Translate(DataAccess.PassJournal tableItem)
		{
			return new ExportPassJournalItem
			{
				UID = tableItem.UID,
				EmployeeUID = tableItem.EmployeeUID,
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