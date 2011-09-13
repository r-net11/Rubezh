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
        public ReportPaginator(FlowDocument flowDocument, Definition def)
        {
            MemoryStream stream = new MemoryStream();
            TextRange sourceDocument = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
            sourceDocument.Save(stream, DataFormats.Xaml);
            FlowDocument copy = new FlowDocument();
            TextRange copyDocumentRange = new TextRange(copy.ContentStart, copy.ContentEnd);
            copyDocumentRange.Load(stream, DataFormats.Xaml);
            _paginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;
            _definition = def;
            _paginator.PageSize = def.ContentSize;

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
}
