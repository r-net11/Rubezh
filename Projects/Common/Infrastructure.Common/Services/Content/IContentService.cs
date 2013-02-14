using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Infrastructure.Common.Services.Content
{
	public interface IContentService
	{
		string ContentFolder { get; }

		string GetContentFileName(Guid guid);
		string GetContentFileName(string guid);
		Stream GetContentStream(Guid guid);
		Stream GetContentStream(string guid);
		T GetObject<T>(Guid guid);
		T GetObject<T>(string guid);
		Drawing GetDrawing(Guid guid);
		Drawing GetDrawing(string guid);
		BitmapImage GetBitmapContent(Guid guid);
		BitmapImage GetBitmapContent(string guid);

		Guid AddContent(string fileName);
		Guid AddContent(Stream stream);
		Guid AddContent(byte[] bytes);
		Guid AddContent(object data);

		void RemoveContent(Guid guid);
		void RemoveContent(string guid);

		void Invalidate();
		void Close();
	}
}
