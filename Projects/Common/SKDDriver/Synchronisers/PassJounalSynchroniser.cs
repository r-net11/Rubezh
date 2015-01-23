using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FiresecAPI;

namespace SKDDriver
{
	public class PassJounalSynchroniser
	{
		Table<DataAccess.PassJournal> _Table;
		string Name { get { return "PassJournal"; } }
		public string NameXml { get { return Name + ".xml"; } }

		public PassJounalSynchroniser(Table<DataAccess.PassJournal> table)
		{
			_Table = table;
		}

		public OperationResult Export(DateTime minDate, DateTime maxDate)
		{
			try
			{
				var tableItems = _Table.Where(x => x.EnterTime >= minDate & x.EnterTime <= maxDate);
				var items = tableItems.Select(x => Translate(x));
				var serializer = new XmlSerializer(typeof(IEnumerable<ExportPassJournalItem>));
				using (var fileStream = File.Open(NameXml, FileMode.Create))
				{
					serializer.Serialize(fileStream, items);
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		ExportPassJournalItem Translate(DataAccess.PassJournal tableItem)
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
