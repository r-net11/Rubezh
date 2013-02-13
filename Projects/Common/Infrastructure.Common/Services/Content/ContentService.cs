using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;

namespace Infrastructure.Common.Services.Content
{
	public class ContentService : IContentService
	{
		private const string ContentFolderRelativePath = @"Configuration\Unzip\Content";
		public string ContentFolder { get; private set; }

		public ContentService(string applicationName)
		{
			ContentFolder = AppDataFolderHelper.GetFolder(Path.Combine(applicationName, ContentFolderRelativePath));
			Invalidate();
		}

		#region IContentService Members

		public string GetContentFileName(Guid guid)
		{
			return GetContentFileName(guid.ToString());
		}
		public string GetContentFileName(string guid)
		{
			return Path.Combine(ContentFolder, guid);
		}
		public Stream GetContentStream(Guid guid)
		{
			return GetContentStream(guid.ToString());
		}
		public Stream GetContentStream(string guid)
		{
			var fileName = GetContentFileName(guid);
			return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
		}
		public BitmapImage GetBitmapContent(Guid guid)
		{
			return GetBitmapContent(guid.ToString());
		}
		public BitmapImage GetBitmapContent(string guid)
		{
			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.StreamSource = GetContentStream(guid);
			bitmap.EndInit();
			return bitmap;
		}

		public Guid AddContent(string fileName)
		{
			var guid = Guid.NewGuid();
			var contentFile = Path.Combine(ContentFolder, guid.ToString());
			File.Copy(fileName, contentFile);
			return guid;
		}
		public Guid AddContent(Stream stream)
		{
			var guid = Guid.NewGuid();
			var contentFile = Path.Combine(ContentFolder, guid.ToString());
			byte[] buffer = new byte[byte.MaxValue];
			int count = 0;
			using (var fs = new FileStream(contentFile, FileMode.CreateNew, FileAccess.Write))
				while ((count = fs.Read(buffer, 0, buffer.Length)) > 0)
					fs.Write(buffer, 0, count);
			return guid;
		}
		public Guid AddContent(byte[] bytes)
		{
			var guid = Guid.NewGuid();
			var contentFile = Path.Combine(ContentFolder, guid.ToString());
			using (var fs = new FileStream(contentFile, FileMode.CreateNew, FileAccess.Write))
				fs.Write(bytes, 0, bytes.Length);
			return guid;
		}

		public void RemoveContent(Guid guid)
		{
			RemoveContent(guid.ToString());
		}
		public void RemoveContent(string guid)
		{
			var contentFile = Path.Combine(ContentFolder, guid.ToString());
			if (File.Exists(contentFile))
				File.Delete(contentFile);
		}

		#endregion

		#region IContentService Members

		public void Invalidate()
		{
			if (!Directory.Exists(ContentFolder))
				Directory.CreateDirectory(ContentFolder);
		}

		#endregion
	}
}
