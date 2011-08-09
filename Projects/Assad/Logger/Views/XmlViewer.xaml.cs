using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace Logger.Views
{
    /// <summary>
    /// Логика взаимодействия для XmlViewer.xaml
    /// </summary>
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

            XmlDataProvider provider = new XmlDataProvider();
            provider.Document = _xmldocument;
            Binding binding = new Binding();
            binding.Source = provider;
            binding.XPath = "child::node()";
            xmlTree.SetBinding(TreeView.ItemsSourceProperty, binding);
        }
    }
}