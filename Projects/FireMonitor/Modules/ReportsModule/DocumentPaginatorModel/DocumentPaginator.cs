using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ReportsModule.DocumentPaginatorModel
{
	public class DocumentPaginatorWrapper : DocumentPaginator
	{
		Size m_PageSize;
		Size m_Margin;
		DocumentPaginator m_Paginator;
		Typeface m_Typeface;

		DateTime _dateTime;

		public DocumentPaginatorWrapper(DocumentPaginator paginator, Size pageSize, Size margin)
		{
			m_PageSize = pageSize;
			m_Margin = margin;
			m_Paginator = paginator;
			m_Paginator.PageSize = new Size(m_PageSize.Width - margin.Width * 2, m_PageSize.Height - margin.Height * 2);
			_dateTime = DateTime.Now;
		}

		Rect Move(Rect rect)
		{
			if (rect.IsEmpty)
			{
				return rect;
			}
			else
			{
				return new Rect(rect.Left + m_Margin.Width, rect.Top + m_Margin.Height, rect.Width, rect.Height);
			}
		}

		public override DocumentPage GetPage(int pageNumber)
		{
			DocumentPage page = m_Paginator.GetPage(pageNumber);
			// Create a wrapper visual for transformation and add extras
			ContainerVisual newpage = new ContainerVisual();
			DrawingVisual title = new DrawingVisual();
			using (DrawingContext ctx = title.RenderOpen())
			{
				if (m_Typeface == null)
				{
					m_Typeface = new Typeface("Times New Roman");
				}
				LineSegment line = new LineSegment(new Point(100, 570), false);
				FormattedText titleText = new FormattedText("Список устройств конфигурации", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
					m_Typeface, 20, Brushes.Black);
				FormattedText text = new FormattedText("Страница " + (pageNumber + 1), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
					m_Typeface, 14, Brushes.Black);
				FormattedText textDate = new FormattedText(_dateTime.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
					m_Typeface, 14, Brushes.Black);
				var pen = new Pen();
				pen.Brush = Brushes.Black;
				pen.Thickness = 2;
				ctx.DrawLine(pen, new Point(10, 570), new Point(650, 570));
				ctx.DrawText(text, new Point(580, 580));
				ctx.DrawText(textDate, new Point(10, 580));
				if (pageNumber == 0) 
					ctx.DrawText(titleText, new Point(200, 5));

			}
			//DrawingVisual background = new DrawingVisual();
			//using (DrawingContext ctx = background.RenderOpen())
			//{
			//	ctx.DrawRectangle(new SolidColorBrush(Color.FromRgb(240, 240, 240)), null, page.ContentBox);
			//}
			//newpage.Children.Add(background); // Scale down page and center
			ContainerVisual smallerPage = new ContainerVisual();
			smallerPage.Children.Add(page.Visual);
			smallerPage.Transform = new MatrixTransform(0.95, 0, 0, 0.95, 0.025 * page.ContentBox.Width, 0.025 * page.ContentBox.Height);
			newpage.Children.Add(smallerPage);
			newpage.Children.Add(title);
			newpage.Transform = new TranslateTransform(m_Margin.Width, m_Margin.Height);
			return new DocumentPage(newpage, m_PageSize, Move(page.BleedBox), Move(page.ContentBox));
		}

		public override bool IsPageCountValid
		{
			get { return m_Paginator.IsPageCountValid; }
		}

		public override int PageCount
		{
			get { return m_Paginator.PageCount; }
		}

		public override Size PageSize
		{
			get { return m_Paginator.PageSize; }
			set
			{
				m_Paginator.PageSize = value;
			}
		}

		public override IDocumentPaginatorSource Source
		{
			get { return m_Paginator.Source; }
		}
	}
}
