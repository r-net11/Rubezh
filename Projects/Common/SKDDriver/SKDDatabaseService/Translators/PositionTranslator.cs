using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class PositionTranslator : OrganizationElementTranslator<DataAccess.Position, Position, PositionFilter>
	{
		public PositionTranslator(DataAccess.SKDDataContext context, PhotoTranslator photoTranslator)
			: base(context)
		{
			PhotoTranslator = photoTranslator;
		}

		PhotoTranslator PhotoTranslator;

		protected override OperationResult CanSave(Position item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name && 
				x.OrganizationUID == item.OrganizationUID && 
				x.UID != item.UID &&
				!x.IsDeleted);
			if (sameName)
				return new OperationResult("Попытка добавления должности с совпадающим именем");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Position item)
		{
			if (Context.Employees.Any(x => x.PositionUID == item.UID && x.OrganizationUID == item.OrganizationUID && !x.IsDeleted))
				return new OperationResult("Не могу удалить должность, пока она указана у действующих сотрудников");
			return base.CanSave(item);
		}

		protected override Position Translate(DataAccess.Position tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.Photo = GetResult(PhotoTranslator.GetSingle(tableItem.PhotoUID));
			return result;
		}

		protected override void TranslateBack(DataAccess.Position tableItem, Position apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			if(apiItem.Photo != null)
				tableItem.PhotoUID = apiItem.Photo.UID;
		}

		ShortPosition TranslateToShort(DataAccess.Position tableItem)
		{
			var shortPosition = new ShortPosition
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				OrganisationUID = tableItem.OrganizationUID
			};
			return shortPosition;
		}

		public OperationResult<IEnumerable<ShortPosition>> GetList(PositionFilter filter)
		{
			try
			{
				var result = new List<ShortPosition>();
				foreach (var tableItem in GetTableItems(filter))
				{
					var shortPosition = TranslateToShort(tableItem);
					result.Add(shortPosition);
				}
				var operationResult = new OperationResult<IEnumerable<ShortPosition>>();
				operationResult.Result = result;
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult<IEnumerable<ShortPosition>>(e.Message);
			}
		}

		public ShortPosition GetSingleShort(Guid? uid)
		{
			if (uid == null)
				return null;
			var tableItem = Table.Where(x => x.UID.Equals(uid.Value)).FirstOrDefault();
			if (tableItem == null)
				return null;
			return TranslateToShort(tableItem);
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

		public override OperationResult Save(IEnumerable<Position> apiItems)
		{
			foreach (var item in apiItems)
			{
				var photoSaveResult = PhotoTranslator.Save(new List<Photo> { item.Photo });
				if (photoSaveResult.HasError)
					return photoSaveResult;
			}
			return base.Save(apiItems);
		}
	}
}