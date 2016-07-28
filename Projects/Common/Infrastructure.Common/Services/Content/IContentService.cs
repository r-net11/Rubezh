using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Infrastructure.Common.Services.Content
{
	public interface IContentService
	{
		string ContentFolder { get; }

		bool CheckIfExists(Guid guid);

		bool CheckIfExists(string guid);

		string GetContentFileName(Guid guid);

		string GetContentFileName(string guid);

		Stream GetContentStream(Guid guid);

		Stream GetContentStream(string guid);

		T GetObject<T>(Guid guid);

		T GetObject<T>(string guid);

		Visual GetVisual(Guid guid);

		Visual GetVisual(string guid);

		Drawing GetDrawing(Guid guid);

		Drawing GetDrawing(string guid);

		BitmapImage GetBitmapContent(Guid guid);

		BitmapImage GetBitmapContent(string guid);

		Guid AddContent(string fileName);

		Guid AddContent(Stream stream);

		Guid AddContent(byte[] data, Guid? uid = null);

		Guid AddContent(object data);

		void RemoveContent(Guid guid);

		void RemoveContent(string guid);

		void Clear();

		void Close();
	}
}