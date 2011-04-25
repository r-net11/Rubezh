using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RubezhAX
{
    /// <summary>
    /// Interaction logic for AXPropertyPage.xaml
    /// </summary>
    public partial class AXPropertyPage : Window
    {
        protected string myMetadataDriverID;
        public string MyMetadataDriverID
        {
            get { return myMetadataDriverID; }
            set { myMetadataDriverID = value; }

        }

        public AXPropertyPage()
        {
            InitializeComponent();
        }

    }
}
