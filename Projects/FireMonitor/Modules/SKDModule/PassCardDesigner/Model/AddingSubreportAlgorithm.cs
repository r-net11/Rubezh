using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.UI;

namespace SKDModule.PassCardDesigner.Model
{
	public class AddingSubreportAlgorithm
	{
		private float _reportWidthTotal;
		private float _reportWidthRemain;
		private float _reportHeightTotal;
		private float _reportHeightRemain;
		private float _subreportMargin;
		private int x_coor;
		private int y_coor;

		private List<ReportLine> _lines;

		private readonly XtraReport _masterReport;

		public AddingSubreportAlgorithm(float reportWidth, float reportHeight, XtraReport masterReport)
		{
			_lines = new List<ReportLine>();
			_reportWidthTotal = reportWidth;
			_reportHeightTotal = reportHeight;
			_masterReport = masterReport;
			x_coor = masterReport.Margins.Left;
			y_coor = masterReport.Margins.Top;
		}

		public XtraReport AddSubReport(XRSubreport subreport, XtraReport masterReport)
		{
			var detailBand = masterReport.Bands[BandKind.Detail] as DetailBand;
			if(detailBand == null)
				throw new InvalidOperationException("Empty detail band");

			detailBand.Controls.Add(subreport);

			subreport.Location = new Point(x_coor, y_coor);

			var detailReport = new XtraReport { DataSource = masterReport.DataSource, DataMember = masterReport.DataMember };

			subreport.ReportSource = detailReport;

			detailReport.Bands.Add(_masterReport.Bands[BandKind.Detail]);

			CalculateNextCoords(subreport);

			return masterReport;
		}

		/// <summary>
		/// Helper method to calculate coordinates of next sub-report based on current sub-report.
		/// </summary>
		/// <param name="subReport">Current sub-report</param>
		private void CalculateNextCoords(XRSubreport subReport)
		{
			x_coor += subReport.Width;
			y_coor += subReport.Height;
		}

		public void Reset()
		{
			x_coor = 0;
			y_coor = 0;
		}
	}

	internal class ReportLine
	{
		public bool IsFilled { get; set; }

		public int ReportsCount
		{
			get { return _subreports == null ? default(int) : _subreports.Count; }
		}

		private int _reportsCount;
		private int _lineHeight;
		private int _lineWidth;

		private readonly List<XRSubreport> _subreports;

		public ReportLine()
		{
			_subreports = new List<XRSubreport>();
		}

		public bool AddSubreport(XRSubreport subreport)
		{
			if (subreport == null)
				return false;

			_subreports.Add(subreport);

			if (subreport.Height > _lineHeight)
				_lineHeight = subreport.Height;

			_lineWidth += subreport.Width;

			return true;
		}
	}

	internal class ReportPage
	{
		private int _widthTotal;
		private int _heightTotal;
		private int _widthRemain;

		public int HeightRemain { get; set; }

		private List<ReportLine> _lines;
		private ReportLine _currentLine;

		public ReportPage(int widthTotal, int heightTotal)
		{
			_widthTotal = _widthRemain = widthTotal;
			_heightTotal = HeightRemain = heightTotal;
			_lines = new List<ReportLine>();
		}

		public bool AddSubreport(XRSubreport subreport)
		{
			var currentLine = _lines.LastOrDefault();

			if (currentLine == null)
				return false;

			if (subreport.Width > _widthRemain)
			{
				currentLine.IsFilled = true;
				currentLine = new ReportLine();
			}

			var addingResult = currentLine.AddSubreport(subreport);
			if (addingResult)
			{
				_widthRemain -= subreport.Width;
			}

			return true;
		}
	}

	internal class Report
	{
		private List<ReportPage> _pages;
		private int _heightTotal;
		private int _heightRemain;
		private int _widthTotal;

		public Report(int heightTotal, int widthTotal)
		{
			_heightTotal = _heightRemain = heightTotal;
			_widthTotal = widthTotal;
			_pages = new List<ReportPage>();
		}

		public bool AddSubreport(XRSubreport subreport)
		{
			var currentPage = _pages.LastOrDefault();

			if (currentPage == null)
				return false;

			if (subreport.Height > currentPage.HeightRemain)
			{
				currentPage = new ReportPage(_widthTotal, _heightTotal);
			}

			var addingResult = currentPage.AddSubreport(subreport);
			return true;
		}
	}
}
