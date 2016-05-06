using StrazhAPI;
using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public class AdditionalColumnTypeTranslator : WithShortTranslator<DataAccess.AdditionalColumnType, AdditionalColumnType, AdditionalColumnTypeFilter, ShortAdditionalColumnType>
	{
		public AdditionalColumnTypeTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override OperationResult CanSave(AdditionalColumnType item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return new OperationResult("Тип колонки с таким же названием уже содержится в базе данных");
			else
				return new OperationResult();
		}

		protected override AdditionalColumnType Translate(DataAccess.AdditionalColumnType tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.DataType = (AdditionalColumnDataType)tableItem.DataType;
			result.PersonType = (PersonType)tableItem.PersonType;
			result.IsInGrid = tableItem.IsInGrid;
			return result;
		}

		protected override ShortAdditionalColumnType TranslateToShort(DataAccess.AdditionalColumnType tableItem)
		{
			var result = base.TranslateToShort(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.DataType = (AdditionalColumnDataType)tableItem.DataType;
			result.IsInGrid = tableItem.IsInGrid;
			return result;
		}

		protected override void TranslateBack(DataAccess.AdditionalColumnType tableItem, AdditionalColumnType apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.DataType = (int?)apiItem.DataType;
			tableItem.PersonType = (int)apiItem.PersonType;
			tableItem.IsInGrid = apiItem.IsInGrid;
		}

		public AdditionalColumnType Get(Guid? uid)
		{
			if (uid == null)
				return null;
			try
			{
				var result = Table.Where(x => x != null &&
					!x.IsDeleted &&
					x.UID == uid.Value).FirstOrDefault();
				return result != null ? Translate(result) : null;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public List<Guid> GetTextColumnTypes()
		{
			return Table.Where(x => x.DataType == (int?)AdditionalColumnDataType.Text).Select(x => x.UID).ToList();
		}

		protected override Expression<Func<DataAccess.AdditionalColumnType, bool>> IsInFilter(AdditionalColumnTypeFilter filter)
		{
			var result = base.IsInFilter(filter);
			if (filter.Type.HasValue)
			{
				var dataType = (int)filter.Type.Value;
				result = result.And(e => e.DataType == dataType);
			}
			result = result.And(e => e.PersonType == (int?)filter.PersonType);
			return result;
		}
	}
}