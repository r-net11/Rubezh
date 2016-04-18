using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Controls.PDF
{
	public class PDFDocument : IDisposable
	{
		public Document Document { get; private set; }
		public PdfWriter Writer { get; private set; }
		public PageEventHelper PageEventHelper { get; private set; }
		public Stream DocumentStream { get; private set; }

		public PDFDocument(Stream stream)
			: this(stream, PageSize.A4)
		{
		}
		public PDFDocument(Stream stream, Rectangle pageSize)
		{
			DocumentStream = stream;
			Document = new Document(pageSize);
			PageEventHelper = new PageEventHelper();
			Writer = PdfWriter.GetInstance(Document, stream);
			Writer.PageEvent = PageEventHelper;
			Document.Open();
			Document.AddCreationDate();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (!Writer.PageEmpty)
				Document.Close();
			DocumentStream.Close();
		}

		#endregion
	}
}
