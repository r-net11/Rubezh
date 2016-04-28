using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SKDDriver
{
	public class PassJounalSynchroniser
	{
		private Table<DataAccess.PassJournal> _Table;

		private string Name { get { return "PassJournal"; } }

		public string NameXml { get { return Name + ".xml"; } }

		public PassJounalSynchroniser(Table<DataAccess.PassJournal> table)
		{
			_Table = table;
		}

		public OperationResult Export(JournalExportFilter filter)
		{
			try
			{
				if (!Directory.Exists(filter.Path))
					return new OperationResult(Resources.Language.Synchronisers.PassJournalSynchroniser.Export_Error);
				var tableItems = _Table.Where(x => x.EnterTime >= TranslatiorHelper.CheckDate(filter.MinDate) & x.EnterTime <= TranslatiorHelper.CheckDate(filter.MaxDate));
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
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		private ExportPassJournalItem Translate(DataAccess.PassJournal tableItem)
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