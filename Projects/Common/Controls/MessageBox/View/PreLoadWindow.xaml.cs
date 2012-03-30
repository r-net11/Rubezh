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
using System.ComponentModel;

namespace Controls.MessageBox
{
    /// <summary>
    /// Логика взаимодействия для PreLoadWindow.xaml
    /// </summary>
    public partial class PreLoadWindow : Window, INotifyPropertyChanged
    {
        public PreLoadWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        string _preLoadText;
        public string PreLoadText
        {
            get { return _preLoadText; }
            set
            {
                _preLoadText = value;
                OnPropertyChanged("PreLoadText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
