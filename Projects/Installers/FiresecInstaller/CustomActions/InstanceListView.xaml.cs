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
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace CustomActions
{
    /// <summary>
    /// Логика взаимодействия для InstanceListView.xaml
    /// </summary>
    public partial class InstanceListView : Window
    {
        public InstanceListView()
        {
            InitializeComponent();
            InstanceList = new ObservableCollection<string>();
            GenSQLInstanceList();
        }

        ObservableCollection<string> _instanceList;
        public ObservableCollection<string> InstanceList 
        {
            get 
            {
                if (_instanceList.Count == 0)
                {
                    _instanceList.Add("SQLEXPRESS");
                }
                return _instanceList;
            }
            set { _instanceList = value; }
        }

        public string SelectedInstance { get; set; }

        public void GenSQLInstanceList()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names");
            foreach (string sk in key.GetSubKeyNames())
            {
                RegistryKey rkey = key.OpenSubKey(sk);

                foreach (string s in rkey.GetValueNames())
                {
                    InstanceList.Add(s);
                }
            }
            SelectedInstance = InstanceList[0];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
