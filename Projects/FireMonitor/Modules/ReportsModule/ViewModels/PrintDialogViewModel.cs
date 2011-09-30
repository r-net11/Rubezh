using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace ReportsModule.ViewModels
{
    public class PrintDialogViewModel
    {
        public PrintDialogViewModel()
        {
            commandBinding = new CommandBinding();
            commandBinding.Command = ApplicationCommands.Print;
            commandBinding.Executed += new ExecutedRoutedEventHandler(OnPrint);
        }

        public CommandBinding commandBinding;
        
        public void OnPrint(object sender, ExecutedRoutedEventArgs e)
        {
            var documentViewer = sender as DocumentViewer;
            if (documentViewer == null) { return; }
            var printDialog = new PrintDialog();
            printDialog.MaxPage = (uint)documentViewer.Document.DocumentPaginator.PageCount;
            printDialog.MinPage = 1;
            printDialog.UserPageRangeEnabled = true;
            printDialog.PageRangeSelection = PageRangeSelection.AllPages;
            printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            DocumentPage page;
            if (printDialog.ShowDialog() == true)
            {
                if (printDialog.PageRangeSelection == PageRangeSelection.UserPages)
                {
                    var pageRange = printDialog.PageRange;
                    for (int i = pageRange.PageFrom; i <= pageRange.PageTo; i++)
                    {
                        page = documentViewer.Document.DocumentPaginator.GetPage(i);
                        printDialog.PrintVisual(page.Visual, "PrintPage № " + i.ToString());
                    }
                }
                else
                {
                    printDialog.PrintDocument(documentViewer.Document.DocumentPaginator, "Print All Document");
                }
            }
        }
    }
}
