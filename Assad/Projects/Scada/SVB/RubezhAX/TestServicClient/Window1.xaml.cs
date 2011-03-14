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

namespace TestServiceClient
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        protected string myMetadataDriverID;
        public string MyMetadataDriverID
        {
            get { return myMetadataDriverID; }
            set { myMetadataDriverID = value;}

        }
        
        public Window1()
        {
            InitializeComponent();
//            MyMetadataDriverID = "1EDE7282-0003-424E-B76C-BB7B413B4F3B";
        }  

        private void Connect(object sender, RoutedEventArgs e)
        {

        }

    }
}
