using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using Infrastructure.Common.Windows;
using LinqKit;

namespace SKDDriver
{
	public class AdditionalColumnTypeTranslator : OrganizationElementTranslator<DataAccess.AdditionalColumnType, AdditionalColumnType, AdditionalColumnTypeFilter>
	{
		public AdditionalColumnTypeTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override OperationResult CanSave(AdditionalColumnType item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Тип колонки с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override AdditionalColumnType Translate(DataAccess.AdditionalColumnType tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.DataType = (DataType)tableItem.DataType;
			return result;
		}

		protected override void TranslateBack(DataAccess.AdditionalColumnType tableItem, AdditionalColumnType apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.DataType = (int?)apiItem.DataType;
		}

		public AdditionalColumnType Get(Guid? uid)
		{
			try
			{
				if (uid == null)
					throw new Exception("Тип колонки не найден в базе даных");
				var result = Table.Where(x => x != null &&
					!x.IsDeleted &&
					x.UID == uid.Value).FirstOrDefault();
				return Translate(result);
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return null;
			}
		}

		public List<Guid> GetTextColumnTypes()
		{
			return Table.Where(x => x.DataType == (int?)DataType.Text).Select(x => x.UID).ToList();
		}

		protected override Expression<Func<DataAccess.AdditionalColumnType, bool>> IsInFilter(AdditionalColumnTypeFilter filter)
		{
			var result = base.IsInFilter(filter);
			var names = filter.Names;
			if (names != null && names.Count != 0)
				result = result.And(e => names.Contains(e.Name));
			if (filter.Type.HasValue)
			{
				var dataType = (int)filter.Type.Value;
				result = result.And(e => e.DataType == dataType);
			}
			return result;
		}
	}
}