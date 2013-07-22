using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Infrustructure.Plans.Painters;

namespace Infrastructure.Common.Services.Content
{
	public class ContentService : IContentService
	{
		private const string ContentFolderRelativePath = @"Configuration\Unzip\Content";
		public string ContentFolder { get; private set; }
		private Dictionary<string, Stream> _streams;
		private Dictionary<string, BitmapImage> _images;

		public ContentService(string applicationName)
		{
			ContentFolder = AppDataFolderHelper.GetLocalFolder(Path.Combine(applicationName, ContentFolderRelativePath));
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
		public bool CheckIfExists(string guid)
		{
			var fileName = GetContentFileName(guid);
			return File.Exists(fileName);
		}
		public Stream GetContentStream(string guid)
		{
			if (_streams.ContainsKey(guid))
				return _streams[guid];
			var fileName = GetContentFileName(guid);
			if (!File.Exists(fileName))
				return null;
			var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			_streams.Add(guid, stream);
			return stream;
		}
		public BitmapImage GetBitmapContent(Guid guid)
		{
			return GetBitmapContent(guid.ToString());
		}
		public BitmapImage GetBitmapContent(string guid)
		{
			if (_images.ContainsKey(guid))
				return _images[guid];
			BitmapImage bitmap = new BitmapImage();
			try
			{
				bitmap.BeginInit();
				var contentStream = GetContentStream(guid);
				if (contentStream != null)
					bitmap.StreamSource = contentStream;
				bitmap.EndInit();
				_images.Add(guid, bitmap);
			}
			catch (Exception e)
			{
				Logger.Error(e, "ContentService.GetBitmapContent");
			}
			return bitmap;
		}
		public T GetObject<T>(Guid guid)
		{
			return GetObject<T>(guid.ToString());
		}
		public T GetObject<T>(string guid)
		{
			return (T)XamlReader.Load(GetContentStream(guid));
		}
		public Drawing GetDrawing(Guid guid)
		{
			return GetObject<Drawing>(guid);
		}
		public Drawing GetDrawing(string guid)
		{
			return GetObject<Drawing>(guid);
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
			using (var fileStream = new FileStream(contentFile, FileMode.CreateNew, FileAccess.Write))
				while ((count = fileStream.Read(buffer, 0, buffer.Length)) > 0)
					fileStream.Write(buffer, 0, count);
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
		public Guid AddContent(object data)
		{
			var guid = Guid.NewGuid();
			var contentFile = Path.Combine(ContentFolder, guid.ToString());
			using (var fileStream = new FileStream(contentFile, FileMode.CreateNew, FileAccess.Write))
				XamlWriter.Save(data, fileStream);
			return guid;
		}

		public void RemoveContent(Guid guid)
		{
			RemoveContent(guid.ToString());
		}
		public void RemoveContent(string guid)
		{
			if (_images.ContainsKey(guid))
				_images.Remove(guid);
			if (_streams.ContainsKey(guid))
			{
				_streams[guid].Dispose();
				_streams.Remove(guid);
			}
			var contentFile = Path.Combine(ContentFolder, guid);
			if (File.Exists(contentFile))
				File.Delete(contentFile);
		}

		public void Invalidate()
		{
			Close();
			_streams = new Dictionary<string, Stream>();
			_images = new Dictionary<string, BitmapImage>();
			if (!Directory.Exists(ContentFolder))
				Directory.CreateDirectory(ContentFolder);
		}
		public void Close()
		{
			if (_streams != null)
			{
				foreach (var stream in _streams.Values)
					stream.Dispose();
				_streams = null;
			}
			if (_images != null)
				_images = null;
			PainterCache.Dispose();
		}

		#endregion
	}
}