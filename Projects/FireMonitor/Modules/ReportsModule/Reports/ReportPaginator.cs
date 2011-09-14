using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.IO;

namespace ReportsModule.Reports
{
    public class ReportPaginator : DocumentPaginator
    {
        public ReportPaginator(FlowDocument flowDocument)
        {
            MemoryStream stream = new MemoryStream();
            TextRange sourceDocument = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
            sourceDocument.Save(stream, DataFormats.Xaml);
            FlowDocument copy = new FlowDocument();
            TextRange copyDocumentRange = new TextRange(copy.ContentStart, copy.ContentEnd);
            copyDocumentRange.Load(stream, DataFormats.Xaml);
            stream.Close();
            _paginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;
            _definition = new Definition();
            _paginator.PageSize = _definition.ContentSize;

            copy.ColumnWidth = double.MaxValue; // Prevent columns
            copy.PageWidth = _definition.ContentSize.Width;
            copy.PageHeight = _definition.ContentSize.Height;
            copy.PagePadding = new Thickness(0);
        }

        DocumentPaginator _paginator;
        Definition _definition;

        public override DocumentPage GetPage(int pageNumber)
        {
            Visual originalPage = _paginator.GetPage(pageNumber).Visual;

            ContainerVisual visual = new ContainerVisual();
            ContainerVisual pageVisual = new ContainerVisual()
            {
                Transform = new TranslateTransform(
                    _definition.ContentOrigin.X,
                    _definition.ContentOrigin.Y
                )
            };
            pageVisual.Children.Add(originalPage);
            visual.Children.Add(pageVisual);

            if (_definition.Header != null)
            {
                visual.Children.Add(CreateHeaderFooterVisual(_definition.Header, _definition.HeaderRect, pageNumber));
            }
            if (_definition.Footer != null)
            {
                visual.Children.Add(CreateHeaderFooterVisual(_definition.Footer, _definition.FooterRect, pageNumber));
            }

            return null;
        }

        public override bool IsPageCountValid
        {
            get { return _paginator.IsPageCountValid; }
        }

        public override int PageCount
        {
            get { return _paginator.PageCount; }
        }

        public override Size PageSize
        {
            get { return _paginator.PageSize; }
            set { _paginator.PageSize = value; }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return _paginator.Source; }
        }
        
        Visual CreateHeaderFooterVisual(DrawHeaderFooter draw, Rect bounds, int pageNumber)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                draw(context, bounds, pageNumber);
            }
            return visual;
        }

        public class Definition
        {
            public Definition()
            {
                PageSize = new Size(793.5987, 1122.3987);
                Margins = new Thickness(96);
            }

            public DrawHeaderFooter Header, Footer;

            public Size PageSize { get; set; }
            public Thickness Margins { get; set; }

            public double HeaderHeight { get; set; }
            public double FooterHeight { get; set; }

            public bool RepeatTableHeader { get; set; }

            public Size ContentSize
            {
                get
                {
                    return PageSize.Subtract(new Size(
                        Margins.Left + Margins.Right,
                        Margins.Top + Margins.Bottom + HeaderHeight + FooterHeight
                    ));
                }
            }

            public Point ContentOrigin
            {
                get
                {
                    return new Point(Margins.Left, Margins.Top + HeaderRect.Height);
                }
            }

            public Rect HeaderRect
            {
                get
                {
                    return new Rect(Margins.Left, Margins.Top, ContentSize.Width, HeaderHeight);
                }
            }

            public Rect FooterRect
            {
                get
                {
                    return new Rect(Margins.Left, ContentOrigin.Y + ContentSize.Height, ContentSize.Width, FooterHeight);
                }
            }
        }

        public delegate void DrawHeaderFooter(DrawingContext context, Rect bounds, int pageNr);
    }

    public class VisualDocumentPaginator : DocumentPaginator
    {
        Size m_PageSize;
        Size m_Margin;
        DocumentPaginator m_Paginator = null;
        int m_PageCount;
        Size m_ContentSize;
        ContainerVisual m_PageContent;
        ContainerVisual m_SmallerPage;
        ContainerVisual m_SmallerPageContainer;
        ContainerVisual m_NewPage;

        public VisualDocumentPaginator(DocumentPaginator paginator,
               Size pageSize, Size margin)
        {
            m_PageSize = pageSize;
            m_Margin = margin;
            m_Paginator = paginator;
            m_ContentSize = new Size(pageSize.Width - 2 * margin.Width,
                                     pageSize.Height - 2 * margin.Height);
            m_PageCount = (int)Math.Ceiling(m_Paginator.PageSize.Height /
                                            m_ContentSize.Height);
            m_Paginator.PageSize = m_ContentSize;
            m_PageContent = new ContainerVisual();
            m_SmallerPage = new ContainerVisual();
            m_NewPage = new ContainerVisual();
            m_SmallerPageContainer = new ContainerVisual();
        }

        Rect Move(Rect rect)
        {
            if (rect.IsEmpty)
            {
                return rect;
            }
            else
            {
                return new Rect(rect.Left + m_Margin.Width,
                                rect.Top + m_Margin.Height,
                                rect.Width, rect.Height);
            }
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            m_PageContent.Children.Clear();
            m_SmallerPage.Children.Clear();
            m_NewPage.Children.Clear();
            m_SmallerPageContainer.Children.Clear();
            DrawingVisual title = new DrawingVisual();
            using (DrawingContext ctx = title.RenderOpen())
            {
                FontFamily font = new FontFamily("Times New Roman");
                Typeface typeface =
                  new Typeface(font, FontStyles.Normal,
                               FontWeights.Bold, FontStretches.Normal);
                FormattedText text = new FormattedText("Page " +
                    (pageNumber + 1) + " of " + m_PageCount,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface, 14, Brushes.Black);
                ctx.DrawText(text, new Point(0, 0));
            }

            DocumentPage page = m_Paginator.GetPage(0);
            m_PageContent.Children.Add(page.Visual);
            RectangleGeometry clip = new RectangleGeometry(
              new Rect(0, m_ContentSize.Height * pageNumber,
                       m_ContentSize.Width, m_ContentSize.Height));
            m_PageContent.Clip = clip;
            m_PageContent.Transform =
              new TranslateTransform(0, -m_ContentSize.Height * pageNumber);
            m_SmallerPage.Children.Add(m_PageContent);
            m_SmallerPage.Transform = new ScaleTransform(0.95, 0.95);
            m_SmallerPageContainer.Children.Add(m_SmallerPage);
            m_SmallerPageContainer.Transform = new TranslateTransform(0, 24);
            m_NewPage.Children.Add(title);
            m_NewPage.Children.Add(m_SmallerPageContainer);
            m_NewPage.Transform =
                      new TranslateTransform(m_Margin.Width, m_Margin.Height);
            return new DocumentPage(m_NewPage, m_PageSize,
                       Move(page.BleedBox), Move(page.ContentBox));
        }

        public override bool IsPageCountValid
        {
            get
            {
                return true;
            }
        }

        public override int PageCount
        {
            get
            {
                return m_PageCount;
            }
        }

        public override Size PageSize
        {
            get
            {
                return m_Paginator.PageSize;
            }
            set
            {
                m_Paginator.PageSize = value;
            }
        }

        public override IDocumentPaginatorSource Source
        {
            get
            {
                if (m_Paginator != null)
                    return m_Paginator.Source;
                return null;
            }
        }
    }

}
