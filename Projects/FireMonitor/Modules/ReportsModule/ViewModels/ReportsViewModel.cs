using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;
using Infrastructure.Common;
using ReportsModule.Reports;

namespace ReportsModule.ViewModels
{
    public class ReportsViewModel : RegionViewModel
    {
        public ReportsViewModel()
        {
            ShowReport1Command = new RelayCommand(OnShowReport1);
        }

        public RelayCommand ShowReport1Command { get; private set; }
        void OnShowReport1()
        {
            ContentControl cc = _documentViewer.Template.FindName("PART_FindToolBarHost", _documentViewer) as ContentControl;
            cc.Visibility = Visibility.Collapsed;

            Report1 report1 = new Report1();
            XpsDocument xpsDocument = report1.CreateReport();
            _documentViewer.Document = xpsDocument.GetFixedDocumentSequence();
        }

        public void Initialize()
        {
            _documentViewer = new DocumentViewer();
            ReportContent = _documentViewer;
        }

        DocumentViewer _documentViewer;

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