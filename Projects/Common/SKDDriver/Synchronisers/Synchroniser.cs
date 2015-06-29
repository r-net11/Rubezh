﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;
using System.Data.Entity;

namespace SKDDriver.DataClasses
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
		public string NameXml { get { return Name +  ".xml"; } }

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
				var result = new List<TExportItem>();
				var tableItems = _Table.Where(IsInFilter(filter));
				foreach (var item in tableItems)
				{
					var exportItem = Translate(item);
					exportItem.UID = item.UID;
					if (item.ExternalKey == "-1")
						item.ExternalKey = item.UID.ToString("N");
					exportItem.ExternalKey = item.ExternalKey;
					exportItem.IsDeleted = item.IsDeleted;
					exportItem.RemovalDate = item.RemovalDate.GetValueOrDefault();
					result.Add(exportItem);
					Context.SaveChanges();
				}
				return new OperationResult<List<TExportItem>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<TExportItem>>.FromError(e.Message);
			}
		}

		public virtual OperationResult Export(ExportFilter filter)
		{
			try
			{
				if (!Directory.Exists(filter.Path))
					return new OperationResult("Папка не существует");
				var getResult = Get(filter);
				if (getResult.HasError)
					return new OperationResult(getResult.Error);
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
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
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
					newTableItem.RemovalDate = DbServiceHelper.CheckDate(exportItem.RemovalDate);
					newTableItem.IsDeleted = exportItem.IsDeleted;
					TranslateBack(exportItem, newTableItem);
					_Table.Add(newTableItem);
				}
				Context.SaveChanges();
			}
		}


		void SaveForignKeys(List<TExportItem> exportItems)
		{
			foreach (var exportItem in exportItems)
			{
				var tableItem = _Table.FirstOrDefault(x => x.ExternalKey.Equals(exportItem.ExternalKey));
				if (tableItem != null)
				{
					UpdateForignKeys(exportItem, tableItem);
				}
                Context.SaveChanges();
			}
		}

		public virtual OperationResult Import(ImportFilter filter)
		{
			try
			{
				if (!Directory.Exists(filter.Path))
					return new OperationResult("Папка не существует");
				var fileName = Path.Combine(filter.Path, NameXml);
				File.Move(fileName, NameXml);
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
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public virtual OperationResult ImportForignKeys()
		{
			try
			{
				using (var stream = new FileStream(NameXml, FileMode.Open))
				{
					var serializer = new XmlSerializer(typeof(List<TExportItem>));
					var importItems = (List<TExportItem>)serializer.Deserialize(stream);
					if (importItems != null)
					{
						SaveForignKeys(importItems);
					}
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected virtual void BeforeSave(List<TExportItem> exportItems) { }
		protected virtual void UpdateForignKeys(TExportItem exportItem, TTableItem tableItem) { }
		public abstract TExportItem Translate(TTableItem tableItem);
		public abstract void TranslateBack(TExportItem exportItem, TTableItem tableItem);
		protected virtual Expression<Func<TTableItem, bool>> IsInFilter(ExportFilter filter)
		{
			var result = PredicateBuilder.True<TTableItem>();
			result = result.And(e => e != null);
			if(!filter.IsWithDeleted)
				result = result.And(e => !e.IsDeleted);
			return result;
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

		protected Guid GetUIDbyExternalKey<T>(string externalKey, DbSet<T> table)
			where T : class, IExternalKey
		{
			var organisation = table.FirstOrDefault(x => x.ExternalKey.Equals(externalKey));
			return organisation != null ? organisation.UID : Guid.Empty;
		}
	}
}
