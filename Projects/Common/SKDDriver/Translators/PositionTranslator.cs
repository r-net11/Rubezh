using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;

namespace SKDDriver
{
	public class PositionTranslator : WithShortTranslator<DataAccess.Position, Position, PositionFilter, ShortPosition>
	{
		public PositionTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override OperationResult CanSave(Position item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return new OperationResult("Попытка добавления должности с совпадающим именем");
			return new OperationResult();
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.Employees.Any(x => x.PositionUID == uid && !x.IsDeleted))
				return new OperationResult("Невозможно удалить должность, пока она указана у действующих сотрудников");
			return base.CanDelete(uid);
		}

		protected override Position Translate(DataAccess.Position tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.Photo = GetResult(DatabaseService.PhotoTranslator.GetSingle(tableItem.PhotoUID));
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

		protected override ShortPosition TranslateToShort(DataAccess.Position tableItem)
		{
			var result = base.TranslateToShort(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			return result;
		}
		
		public override OperationResult Save(Position apiItem)
		{
			if (apiItem.Photo != null && apiItem.Photo.Data != null && apiItem.Photo.Data.Count() > 0)
			{
				var photoSaveResult = DatabaseService.PhotoTranslator.Save(apiItem.Photo);
				if (photoSaveResult.HasError)
					return photoSaveResult;
			}
			return base.Save(apiItem);
		}
	}
}