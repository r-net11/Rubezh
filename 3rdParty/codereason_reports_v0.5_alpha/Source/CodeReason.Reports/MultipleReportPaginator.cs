using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace CodeReason.Reports
{
	/// <summary>
	/// Paginator to concat multiple reports
	/// </summary>
	public class MultipleReportPaginator : DocumentPaginator
	{
		private List<ReportPaginator> _reportPaginators = new List<ReportPaginator>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="report">report document</param>
		/// <param name="data">multiple report data</param>
		/// <exception cref="ArgumentException">Need at least two ReportData objects</exception>
		public MultipleReportPaginator(ReportDocument report, IEnumerable<ReportData> data)
		{
			if (data == null)
				throw new ArgumentException("Need at least two ReportData objects");

			// create a list of report paginators and compute page counts
			_pageCount = 0;
			foreach (ReportData rd in data)
			{
				if (rd == null)
					continue;
				ReportPaginator paginator = new ReportPaginator(report, rd);
				_reportPaginators.Add(paginator);
				paginator.PageShift = _pageCount;
				paginator.TotalPageCount = _pageCount;
				DocumentPage dp = paginator.GetPage(0);
				if ((dp != DocumentPage.Missing) && (dp.Size != Size.Empty))
					_pageSize = paginator.PageSize;
				_pageCount += paginator.PageCount;
			}
			foreach (ReportPaginator paginator in _reportPaginators)
				paginator.TotalPageCount = _pageCount;
			if (_reportPaginators.Count <= 0)
				throw new ArgumentException("Need at least two ReportData objects");
		}

		/// <summary>
		/// Gets a document page of the appropriate generated report
		/// </summary>
		/// <param name="pageNumber">page number</param>
		/// <returns>parsed DocumentPage</returns>
		public override DocumentPage GetPage(int pageNumber)
		{
			// find the appropriate paginator for the page
			int currentPage = 0;
			ReportPaginator pagePaginator = null;
			foreach (ReportPaginator paginator in _reportPaginators)
			{
				int pageCount = paginator.PageCount;
				if (pageNumber >= currentPage + pageCount)
				{
					currentPage += pageCount;
					continue;
				}
				pagePaginator = paginator;
				break;
			}
			if (pagePaginator == null)
				return DocumentPage.Missing;
			pagePaginator.PageShift = currentPage;
			pagePaginator.TotalPageCount = PageCount;
			DocumentPage dp = pagePaginator.GetPage(pageNumber - currentPage);
			if (dp == DocumentPage.Missing)
				return DocumentPage.Missing;
			//_pageSize = dp.Size;
			return dp;
		}

		/// <summary>
		/// Determines if the current page count is valid
		/// </summary>
		public override bool IsPageCountValid
		{
			get { return true; }
		}

		private int _pageCount = 0;
		/// <summary>
		/// Gets the total page count
		/// </summary>
		public override int PageCount
		{
			get { return _pageCount; }
		}

		private Size _pageSize = Size.Empty;
		/// <summary>
		/// Gets or sets the page size
		/// </summary>
		public override Size PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value; }
		}

		/// <summary>
		/// We don't have only one paginator source
		/// </summary>
		public override IDocumentPaginatorSource Source
		{
			get { return null; }
		}
	}
}
