using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Common.PDF
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
			Document.Close();
			DocumentStream.Close();
		}

		#endregion
	}
}
