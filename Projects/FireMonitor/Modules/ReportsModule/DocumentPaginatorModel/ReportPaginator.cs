using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using StrazhAPI;

namespace ReportsModule.DocumentPaginatorModel
{
	class ReportPaginator : DocumentPaginator
	{
		public ReportPaginator(DocumentPaginator documentPaginator)
		{
			_paginator = documentPaginator;
			_reportDefinition = new ReportDefinition();

			_paginator.PageSize = _reportDefinition.ContentSize;
		}

		DocumentPaginator _paginator;
		ReportDefinition _reportDefinition;
		ContainerVisual _currentHeader = null;

		public override bool IsPageCountValid
		{
			get { return _paginator.IsPageCountValid; }
		}

		public override DocumentPage GetPage(int pageNumber)
		{
			Visual originalPage = _paginator.GetPage(pageNumber).Visual;

			ContainerVisual visual = new ContainerVisual();
			ContainerVisual pageVisual = new ContainerVisual()
			{
				Transform = new TranslateTransform(_reportDefinition.ContentOrigin.X, _reportDefinition.ContentOrigin.Y)
			};
			pageVisual.Children.Add(originalPage);
			visual.Children.Add(pageVisual);

			if (_reportDefinition.Header != null)
			{
				visual.Children.Add(CreateHeaderFooterVisual(_reportDefinition.Header, _reportDefinition.HeaderRect, pageNumber));
			}

			if (_reportDefinition.Footer != null)
			{
				visual.Children.Add(CreateHeaderFooterVisual(_reportDefinition.Footer, _reportDefinition.FooterRect, pageNumber));
			}

			if (_reportDefinition.RepeatTableHeaders)
			{
				ContainerVisual table;
				if (PageStartWithTable(originalPage, out table) && _currentHeader != null)
				{
					Rect headerBounds = VisualTreeHelper.GetDescendantBounds(_currentHeader);
					Vector offset = VisualTreeHelper.GetOffset(_currentHeader);
					ContainerVisual tableHeaderVisual = new ContainerVisual();

					tableHeaderVisual.Transform = new ScaleTransform(_reportDefinition.ContentOrigin.X,
						_reportDefinition.ContentOrigin.Y - headerBounds.Top);

					double yScale = (_reportDefinition.ContentSize.Height - headerBounds.Height) / _reportDefinition.ContentSize.Height;
					TransformGroup group = new TransformGroup();
					group.Children.Add(new ScaleTransform(1.0, yScale));
					group.Children.Add(new TranslateTransform(
						_reportDefinition.ContentOrigin.X,
						_reportDefinition.ContentOrigin.Y + headerBounds.Height
					));
					pageVisual.Transform = group;

					ContainerVisual cp = VisualTreeHelper.GetParent(_currentHeader) as ContainerVisual;
					if (cp != null)
					{
						cp.Children.Remove(_currentHeader);
					}
					tableHeaderVisual.Children.Add(_currentHeader);
					visual.Children.Add(tableHeaderVisual);
				}

				ContainerVisual newTable, newHeader;
				if (PageEndsWithTable(originalPage, out newTable, out newHeader))
				{
					if (newTable == table)
					{
						// Still the same table so don't change the repeating header
						_currentHeader = newHeader;
					}
					else
					{
						// We've found a new table. Repeat the header on the next page
						_currentHeader = newHeader;
					}
				}
				else
				{
					// There was no table at the end of the page
					_currentHeader = null;
				}
			}

			return new DocumentPage(visual, _reportDefinition.PageSize,
				new Rect(new Point(), _reportDefinition.PageSize),
				new Rect(_reportDefinition.ContentOrigin, _reportDefinition.ContentSize));
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

		bool PageEndsWithTable(DependencyObject element, out ContainerVisual tableVisual, out ContainerVisual headerVisual)
		{
			tableVisual = null;
			headerVisual = null;
			if (element.GetType().Name == "RowVisual")
			{
				tableVisual = (ContainerVisual)VisualTreeHelper.GetParent(element);
				headerVisual = (ContainerVisual)VisualTreeHelper.GetChild(tableVisual, 0);
				return true;
			}
			int children = VisualTreeHelper.GetChildrenCount(element);
			if (element.GetType() == typeof(ContainerVisual))
			{
				for (int i = children - 1; i >= 0; i--)
				{
					DependencyObject child = VisualTreeHelper.GetChild(element, i);
					if (PageEndsWithTable(child, out tableVisual, out headerVisual))
					{
						return true;
					}
				}
			}
			else if (children > 0)
			{
				DependencyObject child = VisualTreeHelper.GetChild(element, children - 1);
				if (PageEndsWithTable(child, out tableVisual, out headerVisual))
				{
					return true;
				}
			}
			return false;
		}

		bool PageStartWithTable(DependencyObject element, out ContainerVisual tableVisual)
		{
			tableVisual = null;
			if (element.GetType().Name == "RowVisual")
			{
				tableVisual = (ContainerVisual)VisualTreeHelper.GetParent(element);
				return true;
			}
			int children = VisualTreeHelper.GetChildrenCount(element);
			if (children > 0)
			{
				DependencyObject child = VisualTreeHelper.GetChild(element, 0);
				if (PageStartWithTable(child, out tableVisual))
				{
					return true;
				}
			}
			return false;
		}

		class ReportDefinition
		{
			//Size _pageSize = new Size(11.69 * 96, 8.27 * 96);
			Size _pageSize = new Size(793.5987, 1122.3987);
			public Size PageSize
			{
				get { return _pageSize; }
				set { _pageSize = value; }
			}

			Thickness _margins = new Thickness(96);
			public Thickness Margins
			{
				get { return _margins; }
				set { _margins = value; }
			}

			double _headerHeight;
			public double HeaderHeight
			{
				get { return _headerHeight; }
				set { _headerHeight = value; }
			}

			double _footerHeight;
			public double FooterHeight
			{
				get { return _footerHeight; }
				set { _footerHeight = value; }
			}

			bool _repeatTableHeaders = true;
			public bool RepeatTableHeaders
			{
				get { return _repeatTableHeaders; }
				set { _repeatTableHeaders = value; }
			}

			public DrawHeaderFooter Header, Footer;

			internal Size ContentSize
			{
				get { return PageSize.Subtract(new Size(Margins.Left + Margins.Right, Margins.Top + Margins.Bottom + HeaderHeight + FooterHeight)); }
			}

			internal Point ContentOrigin
			{
				get { return new Point(Margins.Left, Margins.Top + HeaderRect.Height); }
			}

			internal Rect HeaderRect
			{
				get { return new Rect(Margins.Left, Margins.Top, ContentSize.Width, HeaderHeight); }
			}

			internal Rect FooterRect
			{
				get { return new Rect(Margins.Left, ContentOrigin.Y + ContentSize.Height, ContentSize.Width, FooterHeight); }
			}
		}

		public delegate void DrawHeaderFooter(DrawingContext context, Rect bounds, int pageNumber);
	}
}