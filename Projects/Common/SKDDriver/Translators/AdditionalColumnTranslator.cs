using StrazhAPI;
using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public class AdditionalColumnTranslator : WithFilterTranslator<DataAccess.AdditionalColumn, AdditionalColumn, AdditionalColumnFilter>
	{
		public AdditionalColumnTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override AdditionalColumn Translate(DataAccess.AdditionalColumn tableItem)
		{
			var result = base.Translate(tableItem);
			result.EmployeeUID = tableItem.EmployeeUID;
			result.AdditionalColumnType = DatabaseService.AdditionalColumnTypeTranslator.Get(tableItem.AdditionalColumnTypeUID);
			result.Photo = GetResult(DatabaseService.PhotoTranslator.GetSingle(tableItem.PhotoUID));
			result.TextData = tableItem.TextData;
			return result;
		}

		protected override void TranslateBack(DataAccess.AdditionalColumn tableItem, AdditionalColumn apiItem)
		{
			tableItem.EmployeeUID = apiItem.EmployeeUID;
			tableItem.AdditionalColumnTypeUID = apiItem.AdditionalColumnType.UID;
			if (apiItem.Photo != null)
				tableItem.PhotoUID = apiItem.Photo.UID;
			tableItem.TextData = apiItem.TextData;
		}

		public OperationResult<IEnumerable<ShortEmployee>> SetTextColumns(OperationResult<IEnumerable<ShortEmployee>> employeesResult)
		{
			try
			{
				var textColumnTypes = DatabaseService.AdditionalColumnTypeTranslator.GetTextColumnTypes();
				var employees = employeesResult.Result;
				foreach (var employee in employees)
				{
					employee.TextColumns = new List<TextColumn>();
					var tableItems = from x in Table where x.EmployeeUID == employee.UID && textColumnTypes.Contains(x.AdditionalColumnTypeUID.Value) select x;
					foreach (var item in tableItems)
					{
						employee.TextColumns.Add(
							new TextColumn
							{
								ColumnTypeUID = textColumnTypes.FirstOrDefault(x => x == item.AdditionalColumnTypeUID.Value),
								Text = item.TextData
							});
					}
				}
				return new OperationResult<IEnumerable<ShortEmployee>>(employees);
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<ShortEmployee>>.FromError(e.Message);
			}
		}

		public List<TextColumn> GetTextColumns(Guid employeeUID)
		{
			var textColumnTypes = DatabaseService.AdditionalColumnTypeTranslator.GetTextColumnTypes();
			return GetTextColumns(employeeUID, textColumnTypes, Table);
		}

		public List<TextColumn> GetTextColumns(Guid employeeUID, List<Guid> textColumnTypes, IEnumerable<DataAccess.AdditionalColumn> tableItems)
		{
			var textColumns = new List<TextColumn>();
			foreach (var item in (from x in tableItems where x.EmployeeUID == employeeUID && textColumnTypes.Contains(x.AdditionalColumnTypeUID.Value) select x))
			{
				textColumns.Add(new TextColumn
				{
					ColumnTypeUID = textColumnTypes.FirstOrDefault(x => x == item.AdditionalColumnTypeUID.Value),
					Text = item.TextData
				});
			}
			return textColumns;
		}

		public override OperationResult Save(IEnumerable<AdditionalColumn> apiItems)
		{
			var photosToSave = new List<Photo>();
			var photosToDelete = new List<Guid>();
			foreach (var apiItem in apiItems)
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == apiItem.UID);
				if (tableItem == null)
				{
					if (apiItem.Photo != null)
						photosToSave.Add(apiItem.Photo);
					continue;
				}
				if (tableItem.PhotoUID == null && apiItem.Photo == null)
					continue;
				else if (tableItem.PhotoUID == null && apiItem.Photo != null)
					photosToSave.Add(apiItem.Photo);
				else if (tableItem.PhotoUID != null && apiItem.Photo == null)
					photosToDelete.Add(tableItem.PhotoUID.Value);
				else if (tableItem.PhotoUID != apiItem.Photo.UID)
				{
					photosToSave.Add(apiItem.Photo);
					photosToDelete.Add(tableItem.PhotoUID.Value);
				}
			}
			var photoSaveResult = DatabaseService.PhotoTranslator.Save(photosToSave);
			if (photoSaveResult.HasError)
				return photoSaveResult;
			//var photoDeleteResult = PhotoTranslator.Delete(photosToDelete);
			//if (photoDeleteResult.HasError)
			//	return photoDeleteResult;
			return base.Save(apiItems);
		}

		protected override Expression<Func<DataAccess.AdditionalColumn, bool>> IsInFilter(AdditionalColumnFilter filter)
		{
			var result = base.IsInFilter(filter);
			var employeeUIDs = filter.EmployeeUIDs;
			if (employeeUIDs != null && employeeUIDs.Count != 0)
				result = result.And(e => e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value));
			var columnTypeUIDs = filter.ColumnTypeUIDs;
			if (columnTypeUIDs != null && columnTypeUIDs.Count != 0)
				result = result.And(e => e.AdditionalColumnTypeUID != null && columnTypeUIDs.Contains(e.AdditionalColumnTypeUID.Value));
			return result;
		}

		public OperationResult DeleteAllByType(Guid typeUID)
		{
			try
			{
				if (typeUID == Guid.Empty)
					return new OperationResult(Resources.Language.Translators.AdditionalColumnTranslator.DeleteAllByType_Error);
				var databaseItems = Table.Where(x => x.AdditionalColumnTypeUID.Equals(typeUID));
				if (databaseItems != null && databaseItems.Count() > 0)
				{
					Table.DeleteAllOnSubmit(databaseItems);
					Context.SubmitChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
	}
}