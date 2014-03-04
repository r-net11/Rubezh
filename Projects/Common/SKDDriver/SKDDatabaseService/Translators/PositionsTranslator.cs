using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class PositionTranslator : OrganizationTranslatorBase<DataAccess.Position, Position, PositionFilter>
	{
		public PositionTranslator(Table<DataAccess.Position> table, DataAccess.SKUDDataContext context)
			: base(table, context)
		{

		}

		protected override OperationResult CanSave(Position item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name && x.OrganizationUid == item.OrganizationUid && x.Uid != item.UID);
			if (sameName)
				return new OperationResult("Попытка добавления должности с совпадающим именем");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Position item)
		{
			if (Context.Employee.Any(x => x.PositionUid == item.UID && x.OrganizationUid == item.OrganizationUid && !x.IsDeleted))
				return new OperationResult("Не могу удалить должность, пока она указана у действующих сотрудников");
			bool sameName = Table.Any(x => x.Name == item.Name && x.OrganizationUid == item.OrganizationUid && x.Uid != item.UID);
			if (sameName)
				return new OperationResult("Попытка добавления должности с совпадающим именем");
			return base.CanSave(item);
		}

		protected override Position Translate(DataAccess.Position tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			return result;
		}

		protected override void TranslateBack(DataAccess.Position tableItem, Position apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
		}

		protected override Expression<Func<DataAccess.Position, bool>> IsInFilter(PositionFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Position>();
			result = result.And(base.IsInFilter(filter));
			var names = filter.Names;
			if (names != null && names.Count != 0)
				result = result.And(e => names.Contains(e.Name));
			return result;
		}
	}
}