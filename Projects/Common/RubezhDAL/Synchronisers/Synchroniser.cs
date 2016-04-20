using RubezhAPI;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace RubezhDAL.DataClasses
{
	public abstract class Synchroniser<TExportItem, TTableItem>
		where TTableItem : class, IExternalKey, new()
		where TExportItem : IExportItem
	{
		protected DbSet<TTableItem> _Table;
		protected DbService _DatabaseService;
		protected DatabaseContext Context;
		protected abstract string Name { get; }
		protected abstract string XmlHeaderName { get; }
		public string NameXml { get { return Name + ".xml"; } }

		public Synchroniser(DbSet<TTableItem> table, DbService databaseService)
		{
			_Table = table;
			_DatabaseService = databaseService;
			Context = databaseService.Context;
		}

		public OperationResult<List<TExportItem>> Get(ExportFilter filter)
		{
			try
			{
				foreach (var item in _Table.Where(item => item.ExternalKey == null || item.ExternalKey == "-1" || item.ExternalKey == string.Empty))
					item.ExternalKey = item.UID.ToString("N");
				Context.SaveChanges();
				var result = new List<TExportItem>();
				var tableItems = GetFilteredItems(filter);
				foreach (var item in tableItems)
				{
					var exportItem = Translate(item);
					exportItem.UID = item.UID;
					exportItem.ExternalKey = item.ExternalKey;
					exportItem.IsDeleted = item.IsDeleted;
					exportItem.RemovalDate = item.RemovalDate.GetValueOrDefault();
					result.Add(exportItem);
				}
				return new OperationResult<List<TExportItem>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<TExportItem>>.FromError(e.Message);
			}
		}

		public virtual OperationResult<bool> Export(ExportFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (!Directory.Exists(filter.Path))
					throw new Exception("Папка не существует");
				var getResult = Get(filter);
				if (getResult.HasError)
					throw new Exception(getResult.Error);
				var items = getResult.Result;
				var serializer = new XmlSerializer(typeof(List<TExportItem>));
				using (var fileStream = File.Open(NameXml, FileMode.Create))
				{
					serializer.Serialize(fileStream, items);
				}
				var newPath = Path.Combine(filter.Path, NameXml);
				if (File.Exists(newPath))
					File.Delete(newPath);
				File.Move(NameXml, newPath);
				return true;
			});
		}

		void Save(List<TExportItem> exportItems)
		{
			foreach (var exportItem in exportItems)
			{
				var tableItem = _Table.FirstOrDefault(x => x.ExternalKey.Equals(exportItem.ExternalKey));
				if (tableItem != null)
				{
					TranslateBack(exportItem, tableItem);
				}
				else
				{
					var newTableItem = new TTableItem();
					if (exportItem.UID != Guid.Empty)
						newTableItem.UID = exportItem.UID;
					else
						newTableItem.UID = Guid.NewGuid();
					newTableItem.ExternalKey = exportItem.ExternalKey;
					newTableItem.RemovalDate = exportItem.RemovalDate.CheckDate();
					newTableItem.IsDeleted = exportItem.IsDeleted;
					TranslateBack(exportItem, newTableItem);
					_Table.Add(newTableItem);
				}
				Context.SaveChanges();
			}
		}


		void SaveForignKeys(List<TExportItem> exportItems, OrganisationHRCash hrCash)
		{
			foreach (var exportItem in exportItems)
			{
				var tableItem = _Table.FirstOrDefault(x => x.ExternalKey.Equals(exportItem.ExternalKey));
				if (tableItem != null)
				{
					UpdateForignKeys(exportItem, tableItem, hrCash);
				}
				Context.SaveChanges();
			}
		}

		public virtual OperationResult<bool> Import(ImportFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (!Directory.Exists(filter.Path))
					throw new Exception("Папка не существует");
				var fileName = Path.Combine(filter.Path, NameXml);
				File.Copy(fileName, NameXml, true);
				using (var stream = new FileStream(NameXml, FileMode.Open))
				{
					var serializer = new XmlSerializer(typeof(List<TExportItem>));
					var importItems = (List<TExportItem>)serializer.Deserialize(stream);
					if (!filter.IsWithDeleted)
						importItems = importItems.Where(x => !x.IsDeleted).ToList();
					if (importItems != null)
					{
						BeforeSave(importItems);
						Save(importItems);
					}
				}
				return true;
			});
		}

		public virtual OperationResult<bool> ImportForignKeys(OrganisationHRCash hrCash)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				using (var stream = new FileStream(NameXml, FileMode.Open))
				{
					var serializer = new XmlSerializer(typeof(List<TExportItem>));
					var importItems = (List<TExportItem>)serializer.Deserialize(stream);
					if (importItems != null)
					{
						SaveForignKeys(importItems, hrCash);
					}
				}
				return true;
			});
		}

		protected virtual void BeforeSave(List<TExportItem> exportItems) { }
		protected virtual void UpdateForignKeys(TExportItem exportItem, TTableItem tableItem, OrganisationHRCash hrCash) { }
		public abstract TExportItem Translate(TTableItem tableItem);
		public abstract void TranslateBack(TExportItem exportItem, TTableItem tableItem);
		protected virtual IQueryable<TTableItem> GetFilteredItems(ExportFilter filter)
		{
			var result = GetTableItems().Where(x => x != null);
			if (!filter.IsWithDeleted)
				result = result.Where(x => !x.IsDeleted);
			return result;
		}
		protected virtual IQueryable<TTableItem> GetTableItems()
		{
			return _Table;
		}

		protected Guid GetUID(Guid? uid)
		{
			return uid != null ? uid.Value : Guid.Empty;
		}

		protected string GetExternalKey(Guid? uid, IExternalKey exportItem)
		{
			if (exportItem == null)
				return "-1";
			if (exportItem.ExternalKey == "-1")
				return exportItem.UID.ToString();
			return exportItem.ExternalKey;
		}

		protected Guid? GetUIDbyExternalKey<T>(string externalKey, IEnumerable<T> table)
			where T : class, IExternalKey
		{
			var tableItem = table.FirstOrDefault(x => x.ExternalKey.Equals(externalKey));
			return tableItem != null ? (Guid?)tableItem.UID : null;
		}
	}
}
