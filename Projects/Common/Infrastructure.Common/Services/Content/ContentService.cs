using Common;
using Infrustructure.Plans.Painters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;

namespace Infrastructure.Common.Services.Content
{
	public class ContentService : IContentService
	{
		private const string ContentFolderRelativePath = @"Configuration\Content";
		public string ContentFolder { get; private set; }
		private Dictionary<string, Stream> _streams;
		private Dictionary<string, BitmapImage> _images;
		private Dictionary<string, object> _objects;

		public ContentService(string applicationName)
		{
			ContentFolder = AppDataFolderHelper.GetLocalFolder(Path.Combine(applicationName, ContentFolderRelativePath));
			Invalidate();
		}

		#region IContentService Members

		public bool CheckIfExists(string guid)
		{
			var fileName = GetContentFileName(guid);
			return File.Exists(fileName);
		}
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
			if (_streams.ContainsKey(guid))
			{
				_streams[guid].Position = 0;
				return _streams[guid];
			}
			var fileName = GetContentFileName(guid);
			if (!File.Exists(fileName))
				throw new Exception("File " + fileName + " not found");
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
			try
			{
				return GetObject<T>(guid.ToString());
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public T GetObject<T>(string guid)
		{
			if (_objects.ContainsKey(guid))
				return (T)_objects[guid];
			try
			{
				var obj = XamlServices.Load(GetContentStream(guid));
				_objects.Add(guid, obj);
				return (T)obj;
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "ContentService.GetObject<T>, Не удалось считать файл с изображением заднего плана.");
				return default(T);
			}
		}
		public Visual GetVisual(Guid guid)
		{
			try
			{
				return GetObject<Visual>(guid);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public Visual GetVisual(string guid)
		{
			try
			{
				return GetObject<Visual>(guid);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public Drawing GetDrawing(Guid guid)
		{
			try
			{
				return GetObject<Drawing>(guid);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public Drawing GetDrawing(string guid)
		{
			try
			{
				return GetObject<Drawing>(guid);
			}
			catch (Exception ex)
			{
				throw ex;
			}
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
		public Guid AddContent(byte[] data, Guid? guid = null)
		{
			if (guid == null)
				guid = Guid.NewGuid();
			else
			{
				RemoveContent(guid.Value);
			}
			var contentFile = Path.Combine(ContentFolder, guid.ToString());
			using (var fileStream = new FileStream(contentFile, FileMode.CreateNew, FileAccess.Write))
				fileStream.Write(data, 0, data.Length);
			return guid.Value;
		}
		public Guid AddContent(object data)
		{
			var guid = Guid.NewGuid();
			var contentFile = Path.Combine(ContentFolder, guid.ToString());
			using (var fileStream = new FileStream(contentFile, FileMode.CreateNew, FileAccess.Write))
				XamlServices.Save(fileStream, data);
			_objects.Add(guid.ToString(), data);
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
			if (_objects.ContainsKey(guid))
				_objects.Remove(guid);
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
			_objects = new Dictionary<string, object>();
			if (!Directory.Exists(ContentFolder))
				Directory.CreateDirectory(ContentFolder);
		}

		public void Clear()
		{
			Invalidate();
			if (Directory.Exists(ContentFolder))
				Directory.Delete(ContentFolder, true);
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
			if (_objects != null)
				_objects = null;
			PainterCache.Dispose();
		}

		public void RemoveAllBut(IEnumerable<string> uids)
		{
			foreach (var filePath in Directory.GetFiles(ContentFolder))
			{
				var fileName = Path.GetFileName(filePath);
				if (!uids.Contains(fileName))
				{
					RemoveContent(fileName);
					File.Delete(filePath);
				}
			}
		}

		#endregion
	}
}