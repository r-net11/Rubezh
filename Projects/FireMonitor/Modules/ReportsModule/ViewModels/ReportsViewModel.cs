using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Printing;
using Infrastructure.Common;
using ReportsModule.Reports;
using System.Windows.Input;
using System;
using System.Windows.Threading;
using System.Windows.Documents;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media;
using ReportsModule.Views;

namespace ReportsModule.ViewModels
{
    public class ReportsViewModel : RegionViewModel
    {
        public ReportsViewModel()
        {
            ShowReportTypesCountCommand = new RelayCommand(OnShowReportTypesCountCommand);
            ShowReportDeviceParamsCommand = new RelayCommand(OnShowReportDeviceParams);
            ShowReportDeviceListCommand = new RelayCommand(OnShowReportDeviceListCommand);
            ShowReportIndicationBlockCommand = new RelayCommand(OnShowReportIndicationBlockCommand);
            ShowReportJournalCommand = new RelayCommand(OnShowReportJournalCommand);
            ShowReportTestCommand = new RelayCommand(OnShowReportTestCommand);
            ShowReportDataGridCommand = new RelayCommand(OnShowReportDataGridCommand);
        }

        public RelayCommand ShowReportDeviceParamsCommand { get; private set; }
        void OnShowReportDeviceParams()
        {
            ReportDeviceParams  reportDeviceParams = new ReportDeviceParams();
            xpsDocument = reportDeviceParams.CreateReport();
            _documentViewer.Document = xpsDocument.GetFixedDocumentSequence();
            xpsDocument.Close();
        }

        public RelayCommand ShowReportTypesCountCommand { get; private set; }
        void OnShowReportTypesCountCommand()
        {
            ContentControl cc = _documentViewer.Template.FindName("PART_FindToolBarHost", _documentViewer) as ContentControl;
            cc.Visibility = Visibility.Collapsed;
            ReportTypesCount reportTypesCount = new ReportTypesCount();
            xpsDocument = reportTypesCount.CreateReport();
            _documentViewer.Document = xpsDocument.GetFixedDocumentSequence();
            xpsDocument.Close();
        }

        public RelayCommand ShowReportIndicationBlockCommand { get; private set; }
        void OnShowReportIndicationBlockCommand()
        {
            ContentControl cc = _documentViewer.Template.FindName("PART_FindToolBarHost", _documentViewer) as ContentControl;
            cc.Visibility = Visibility.Collapsed;

            var reportIndicationBlock = new ReportIndicationBlock();
            xpsDocument = reportIndicationBlock.CreateReport();
            _documentViewer.Document = xpsDocument.GetFixedDocumentSequence();
            xpsDocument.Close();
        }

        public RelayCommand ShowReportJournalCommand { get; private set; }
        void OnShowReportJournalCommand()
        {
            ContentControl cc = _documentViewer.Template.FindName("PART_FindToolBarHost", _documentViewer) as ContentControl;
            cc.Visibility = Visibility.Collapsed;

            var reportJournal = new ReportJournal();
            xpsDocument = reportJournal.CreateReport();
            _documentViewer.Document = xpsDocument.GetFixedDocumentSequence();
            xpsDocument.Close();
        }

        public RelayCommand ShowReportDeviceListCommand { get; private set; }
        void OnShowReportDeviceListCommand()
        {
            ContentControl cc = _documentViewer.Template.FindName("PART_FindToolBarHost", _documentViewer) as ContentControl;
            cc.Visibility = Visibility.Visible;
            var a = _documentViewer.CommandBindings;
            CommandBinding commandBinding = new CommandBinding();
            commandBinding.Command = ApplicationCommands.Print;
            commandBinding.Executed += new ExecutedRoutedEventHandler(OnPrint);
            _documentViewer.CommandBindings.Add(commandBinding);
            var reportDeviceList = new ReportDevicesList();
            xpsDocument = reportDeviceList.CreateReport();
            _documentViewer.Document = xpsDocument.GetFixedDocumentSequence();
            xpsDocument.Close();
            
        }

        public RelayCommand ShowReportTestCommand { get; private set; }
        void OnShowReportTestCommand()
        {
            //_documentViewer.Document = null;
            //var dataTable = TestReport.CreateDataTable();
            //PrintDialog printDialog = new PrintDialog();
            //printDialog.UserPageRangeEnabled = true;
            //if (printDialog.ShowDialog() == true)
            //{
            //    StoreDataSetPaginator paginator = new StoreDataSetPaginator(dataTable, new Typeface("Calibri"), 24, 96 * 0.75,
            //        new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
            //    printDialog.PrintDocument(paginator, "Custom-Printed Pages");
            //}
            //CommandBinding commandBinding = new CommandBinding();
            //commandBinding.Command = ApplicationCommands.Print;
            //commandBinding.Executed += new ExecutedRoutedEventHandler(OnPrint);

            File.Delete("fileName.xps");
            xpsDocument = new XpsDocument("fileName.xps", FileAccess.ReadWrite);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            string path = @"H:\Rubezh\Projects\FireMonitor\Modules\ReportsModule\ReportTemplates\TestFlowDocument.xaml";
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                FlowDocument flowDocument = (FlowDocument)XamlReader.Load(fs);
                //StoreDataSetPaginator storeDataSetPaginator = new StoreDataSetPaginator(flowDocument);
                var visualDocumentPaginator = new VisualDocumentPaginator(((IDocumentPaginatorSource)flowDocument).DocumentPaginator,
                    new Size(816, 1056), new Size(50, 50));
                writer.Write(((IDocumentPaginatorSource)flowDocument).DocumentPaginator);
                _documentViewer.Document = xpsDocument.GetFixedDocumentSequence();
                xpsDocument.Close();
            }
        }

        public RelayCommand ShowReportDataGridCommand { get; private set; }
        void OnShowReportDataGridCommand()
        {
            FlowDocumentViewModel flowDocumentViewModel = new FlowDocumentViewModel();
            FlowDocumentDataGrid flowDocumentView = new FlowDocumentDataGrid();
            flowDocumentView.DataContext = flowDocumentViewModel;
            _flowDocumentReader.Document = flowDocumentView;
        }

        public void OnPrint(object sender, ExecutedRoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            printDialog.MaxPage = 18;
            printDialog.MinPage = 1;
            printDialog.UserPageRangeEnabled = true;
            printDialog.PageRangeSelection = PageRangeSelection.UserPages;
            printDialog.PageRangeSelection = PageRangeSelection.AllPages;
            printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            DocumentPage page = _documentViewer.Document.DocumentPaginator.GetPage(11);
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(page.Visual, "PrintPAge");
                //printDialog.PrintDocument(_documentViewer.Document.DocumentPaginator, "Print document");
            }
        }

        public void Initialize()
        {
            _documentViewer = new DocumentViewer();
            _flowDocumentReader = new FlowDocumentReader();
            ReportContent = _documentViewer;
            //ReportContent = _flowDocumentReader;
        }

        DocumentViewer _documentViewer;
        FlowDocumentReader _flowDocumentReader;
        XpsDocument xpsDocument;

        object _reportContent;
        public object ReportContent
        {
            get { return _reportContent; }
            set
            {
                _reportContent = value;
                OnPropertyChanged("ReportContent");
            }
        }
    }
}