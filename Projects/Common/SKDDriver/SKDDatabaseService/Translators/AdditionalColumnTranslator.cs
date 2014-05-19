using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class AdditionalColumnTranslator : TranslatorBase<DataAccess.AdditionalColumn, AdditionalColumn, AdditionalColumnFilter>
	{
		public AdditionalColumnTranslator(DataAccess.SKDDataContext context, PhotoTranslator photoTranslator, AdditionalColumnTypeTranslator additionalColumnTypeTranslator)
			: base(context)
		{
			PhotoTranslator = photoTranslator;
			AdditionalColumnTypeTranslator = additionalColumnTypeTranslator;
		}

		PhotoTranslator PhotoTranslator;
		AdditionalColumnTypeTranslator AdditionalColumnTypeTranslator;

		protected override AdditionalColumn Translate(DataAccess.AdditionalColumn tableItem)
		{
			var result = base.Translate(tableItem);
			result.EmployeeUID = tableItem.EmployeeUID;
			result.AdditionalColumnType = AdditionalColumnTypeTranslator.Get(tableItem.AdditionalColumnTypeUID);
			result.Photo = GetResult(PhotoTranslator.GetSingle(tableItem.PhotoUID));
			result.TextData = tableItem.TextData;
			return result;
		}

		protected override void TranslateBack(DataAccess.AdditionalColumn tableItem, AdditionalColumn apiItem)
		{
			tableItem.EmployeeUID = apiItem.EmployeeUID;
			tableItem.AdditionalColumnTypeUID = apiItem.AdditionalColumnType.UID;
			if(apiItem.Photo != null)
				tableItem.PhotoUID = apiItem.Photo.UID;
			tableItem.TextData = apiItem.TextData;
		}

		public OperationResult<IEnumerable<ShortEmployee>> SetTextColumns(OperationResult<IEnumerable<ShortEmployee>> employeesResult)
		{
			try
			{
				var textColumnTypes = AdditionalColumnTypeTranslator.GetTextColumnTypes();
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
				var result = new OperationResult<IEnumerable<ShortEmployee>>();
				result.Result = employees;
				return result;
			}
			catch (Exception e)
			{
				return new OperationResult<IEnumerable<ShortEmployee>>(e.Message);
			}
			
		}

		public override OperationResult Save(IEnumerable<AdditionalColumn> apiItems)
		{
			var photosToSave = new List<Photo>();
			var photosToDelete = new List<Guid>();
			foreach (var apiItem in apiItems)
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == apiItem.UID);
				if (tableItem == null)
					continue;
				if (tableItem.PhotoUID == null)
					photosToSave.Add(apiItem.Photo);
				else if (apiItem.Photo == null)
					photosToDelete.Add(tableItem.PhotoUID.Value);
				else if (tableItem.PhotoUID != apiItem.Photo.UID)
				{
					photosToSave.Add(apiItem.Photo);
					photosToDelete.Add(tableItem.PhotoUID.Value);
				}
			}
			var photoSaveResult = PhotoTranslator.Save(photosToSave);
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

	}
}


