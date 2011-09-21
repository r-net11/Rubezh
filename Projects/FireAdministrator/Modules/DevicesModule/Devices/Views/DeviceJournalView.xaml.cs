using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class DeviceJournalView : UserControl
    {
        public DeviceJournalView()
        {
            InitializeComponent();
        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var webBrowser = sender as WebBrowser;
            if (webBrowser == null)
            {
                return;
            }
            //webBrowser.Document = {mshtml.HTMLDocumentClass}
            //HTMLDocumentClass xxx = webBrowser.Document;
            //var doc = (IHTMLDocument2)webBrowser.Document;

            //doc.charset = "utf-8";
            //webBrowser.Refresh();
        }
    }
}
