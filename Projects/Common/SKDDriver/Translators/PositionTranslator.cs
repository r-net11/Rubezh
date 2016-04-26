using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Linq;

namespace SKDDriver
{
	public class PositionTranslator : EmployeeTranslatorBase<DataAccess.Position, Position, PositionFilter, ShortPosition>
	{
		public PositionTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
			Synchroniser = new PositionSynchroniser(Table, DatabaseService);
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
				return new OperationResult(Resources.Language.PositionTranslator.CanSave);
			return new OperationResult();
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
			if (tableItem.ExternalKey == null)
				tableItem.ExternalKey = "-1";
			if (apiItem.Photo != null)
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
			var photoSaveResult = DatabaseService.PhotoTranslator.SaveOrDelete(apiItem.Photo);
			if (photoSaveResult.HasError)
				return photoSaveResult;
			return base.Save(apiItem);
		}

		protected override Guid? GetLinkUID(DataAccess.Employee employee)
		{
			return employee.PositionUID;
		}

		public PositionSynchroniser Synchroniser;
	}
}