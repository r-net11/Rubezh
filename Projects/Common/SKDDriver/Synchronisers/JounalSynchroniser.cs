using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class PassJounalSynchroniser
	{
		Table<DataAccess.PassJournal> _Table;
		string Name { get { return "PassJournal"; } }
		string XmlHeaderName { get {return "ArrayOfPassJournal"; }}
		public string NameXml { get { return Name +  ".xml"; } }

		public PassJounalSynchroniser(Table<DataAccess.PassJournal> table)
		{
			_Table = table;
		}

		public OperationResult<List<DataAccess.PassJournal>> Get()
		{
			try
			{
				return new OperationResult<List<DataAccess.PassJournal>> { Result = _Table.ToList() };
			}
			catch (Exception e)
			{
				return new OperationResult<List<DataAccess.PassJournal>>(e.Message);
			}
		}

		public OperationResult Export()
		{
			try
			{
				var getResult = Get();
				if (getResult.HasError)
					return new OperationResult(getResult.Error);
				var items = getResult.Result;
				var serializer = new XmlSerializer(typeof(List<DataAccess.PassJournal>));
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

		void Save(List<DataAccess.PassJournal> exportItems)
		{
			foreach (var exportItem in exportItems)
			{
				//var tableItem = _Table.FirstOrDefault(x => x.ExternalKey.Equals(exportItem.ExternalKey));
				//if (tableItem != null)
				//{
				//    TranslateBack(exportItem, tableItem);
				//}
				//else
				//{
				//    var newTableItem = new TTableItem();
				//    if (exportItem.UID != Guid.Empty)
				//        newTableItem.UID = exportItem.UID;
				//    else
				//        newTableItem.UID = Guid.NewGuid();
				//    newTableItem.ExternalKey = exportItem.ExternalKey;
				//    newTableItem.RemovalDate = TranslatiorHelper.CheckDate(exportItem.RemovalDate);
				//    newTableItem.IsDeleted = exportItem.IsDeleted;
				//    TranslateBack(exportItem, newTableItem);
				//    _Table.InsertOnSubmit(newTableItem);
				//}
				//_Table.Context.SubmitChanges();
			}
		}


		public OperationResult Import(Stream stream)
		{
			try
			{
				//var serializer = new XmlSerializer(typeof(List<DataAccess.PassJournal>));
				//var importItems = (List<TExportItem>)serializer.Deserialize(stream);
				//if (importItems != null)
				//{
				//    BeforeSave(importItems);
				//    Save(importItems);
				//}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		Expression<Func<DataAccess.PassJournal, bool>> IsInFilter(Guid uid)
		{
			var result = PredicateBuilder.True<DataAccess.PassJournal>();
			result = result.And(e => e != null);
			return result;
		}

		Guid GetUID(Guid? uid)
		{
			return uid != null ? uid.Value : Guid.Empty;
		}
	}
}
