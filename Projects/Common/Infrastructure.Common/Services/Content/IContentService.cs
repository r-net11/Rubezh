using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;

namespace Infrastructure.Common.Services.Content
{
	public interface IContentService
	{
		string ContentFolder { get; }

		string GetContentFileName(Guid guid);
		string GetContentFileName(string guid);
		Stream GetContentStream(Guid guid);
		Stream GetContentStream(string guid);
		BitmapImage GetBitmapContent(Guid guid);
		BitmapImage GetBitmapContent(string guid);

		Guid AddContent(string fileName);
		Guid AddContent(Stream stream);
		Guid AddContent(byte[] bytes);

		void RemoveContent(Guid guid);
		void RemoveContent(string guid);

		void Invalidate();
	}
}
