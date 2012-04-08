using System.ComponentModel;
using System.Windows;

namespace Controls.MessageBox
{
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