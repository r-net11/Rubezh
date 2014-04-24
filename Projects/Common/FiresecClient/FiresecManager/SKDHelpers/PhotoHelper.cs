using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace FiresecClient.SKDHelpers
{
	public static class PhotoHelper
	{
		public static bool Save(Photo photo)
		{
			var operationResult = FiresecManager.FiresecService.SavePhotos(new List<Photo> { photo });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Photo GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new PhotoFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetPhotos(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}
		public static IEnumerable<Photo> Get(List<Guid> uids)
		{
			var filter = new PhotoFilter()
			{
				UIDs = uids
			};
			var operationResult = FiresecManager.FiresecService.GetPhotos(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static BitmapSource GetBitmapSource(Photo photo)
		{
			if (photo == null)
				return null;
			try
			{
				return BitmapFrame.Create(new MemoryStream(photo.Data));
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return null;
			}
		}
	}
}
