using FiresecAPI;
using FiresecAPI.SKD;
using System.Linq;

namespace StrazhDAL
{
	public class PhotoTranslator : WithFilterTranslator<DataAccess.Photo, Photo, PhotoFilter>
	{
		public PhotoTranslator(SKDDatabaseService databaseService)
			: base(databaseService) { }

		protected override Photo Translate(DataAccess.Photo tableItem)
		{
			var result = base.Translate(tableItem);
			result.Data = tableItem.Data.ToArray();
			return result;
		}

		protected override void TranslateBack(DataAccess.Photo tableItem, Photo apiItem)
		{
			tableItem.Data = apiItem.Data;
		}

		public OperationResult SaveOrDelete(Photo photo)
		{
			if (photo != null)
			{
				if (photo.Data != null && photo.Data.Count() > 0)
				{
					var photoSaveResult = DatabaseService.PhotoTranslator.Save(photo);
					if (photoSaveResult.HasError)
						return photoSaveResult;
				}
				else
				{
					var photoDeleteResult = DatabaseService.PhotoTranslator.Delete(photo.UID);
					if (photoDeleteResult.HasError)
						return photoDeleteResult;
				}
			}
			return new OperationResult();
		}
	}
}