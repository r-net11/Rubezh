using System;
using System.Collections.Generic;
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

		protected override OperationResult CanSave(Position position)
		{
			bool hasSameName = Table.Any(x => x.Name == position.Name && 
				x.OrganisationUID == position.OrganisationUID && 
				x.UID != position.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return new OperationResult("Попытка добавления должности с совпадающим именем");
			return base.CanSave(position);
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
			var shortPosition = new ShortPosition
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				OrganisationUID = tableItem.OrganisationUID
			};
			return shortPosition;
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