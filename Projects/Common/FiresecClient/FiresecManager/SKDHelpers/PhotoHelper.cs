using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class PhotoHelper
	{
		public static bool Save(Photo photo)
		{
			var operationResult = FiresecManager.FiresecService.SavePhotos(new List<Photo> { photo });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Photo photo)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedPhotos(new List<Photo> { photo });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Photo Get(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new PhotoFilter();
			filter.Uids.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetPhotos(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}
		public static IEnumerable<Photo> Get(List<Guid> uids)
		{
			var filter = new PhotoFilter()
			{
				Uids = uids
			};
			var operationResult = FiresecManager.FiresecService.GetPhotos(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
