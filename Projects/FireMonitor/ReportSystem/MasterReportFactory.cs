using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

namespace ReportSystem
{
	/// <summary>
	/// Класс для создания мульти-колоночного отчёта.
	/// В качестве источника данных использует DataSet и существующий входной отчёт, на основе которого генерируются страницы мультиколоночного отчёта.
	/// Мастер отчёт разбит на колонки и содержит под-отчёты. Количество колонок вычисляется динамически, на основании ширины под-отчёта.
	/// </summary>
	public class MasterReportFactory
	{
		private readonly XtraReport _masterReport;	//мастер-отчёт, который генерируется автоматически
		private readonly DetailBand _detailBand;
		private readonly XtraReport _inputReport; //входной отчёт-шаблон
		private readonly XtraReport _inputReportBack; //входной отчёт-шаблон

		public MasterReportFactory(DataSet dataSet, XtraReport inputReport)
		{
			_detailBand = new DetailBand();
			_masterReport = new XtraReport();
			_inputReport = inputReport;
			MasterReportInit(dataSet);
		}

		public MasterReportFactory(DataSet dataSet, XtraReport inputReportFront, XtraReport inputReportBack)
		{
			_detailBand = new DetailBand();
			_masterReport = new XtraReport();
			_inputReport = inputReportFront;
			_inputReportBack = inputReportBack;

			MasterReportInit(dataSet);
		}

		private void MasterReportInit(DataSet set)
		{
			_detailBand.MultiColumn.ColumnSpacing = 10F;						//Установка отступов между отчётами в колонках
			_detailBand.MultiColumn.Layout = ColumnLayout.AcrossThenDown;		//Метод заполнения колонок отчётами
			_detailBand.MultiColumn.Mode = MultiColumnMode.UseColumnCount;		//Метод, с помощью которого будет печататься отчёт
			_detailBand.MultiColumn.ColumnCount = 3; //CalculateReportColumnCount();	//Количество колонок в отчёте

			_masterReport.ReportUnit = ReportUnit.TenthsOfAMillimeter;			//Установка используемых величин для размеров
			_masterReport.ReportPrintOptions.DetailCount = 1;					//Количество повторений под-отчётов.
			_masterReport.Margins = new Margins(50, 50, 50, 50);				//Установка отступов страницы мастер-отчёта.
			_masterReport.DataSource = set;										//Инициализация мастер отчёта источником данных
			_masterReport.Bands.Add(_detailBand);
		}

		//private int CalculateReportColumnCount()
		//{
		//	var totalWidth = _masterReport.PageWidth - (_masterReport.Margins.Left + _masterReport.Margins.Right);
		//	var reportColumnCount = totalWidth % (_inputReport.)
		//}

		public XtraReport CreateMasterReport()
		{
			//Create a Detail Report
			var detailReport = new DetailReportBand();
			_masterReport.Bands.Add(detailReport);

			detailReport.DataSource = _masterReport.DataSource;
			detailReport.DataMember = "Employees"; //TODO: Replace by TableName property

			//Create bands
			var band = (DetailBand)_inputReport.Bands[BandKind.Detail];
			band.MultiColumn.ColumnSpacing = 50F;
			band.MultiColumn.Layout = ColumnLayout.AcrossThenDown;
			band.MultiColumn.Mode = MultiColumnMode.UseColumnCount;
			band.MultiColumn.ColumnCount = 3;
			detailReport.Bands.Add(band);

			return _masterReport;
		}
	}
}
