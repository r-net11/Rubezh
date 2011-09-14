using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace Logger.Views
{
    public partial class XmlViewer : UserControl
    {
        private XmlDocument _xmldocument;
        public XmlViewer()
        {
            InitializeComponent();
        }

        public XmlDocument xmlDocument
        {
            get { return _xmldocument; }
            set
            {
                _xmldocument = value;
                BindXMLDocument();
            }
        }

        private void BindXMLDocument()
        {
            if (_xmldocument == null)
            {
                xmlTree.ItemsSource = null;
                return;
            }

            var provider = new XmlDataProvider();
            provider.Document = _xmldocument;
            var binding = new Binding();
            binding.Source = provider;
            binding.XPath = "child::node()";
            xmlTree.SetBinding(TreeView.ItemsSourceProperty, binding);
        }
    }
}