using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace FiresecClient.SKDHelpers
{
	public static class PhotoHelper
	{
		public static BitmapSource GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new PhotoFilter();
			filter.Uids.Add((Guid)uid);
			var operationResult = FiresecManager.FiresecService.GetPhotos(filter);
			var photo = Common.ShowErrorIfExists(operationResult).FirstOrDefault();
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
