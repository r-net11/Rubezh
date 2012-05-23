using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Infrastructure.Common.MessageBox
{
	public partial class PreLoadWindow : Window, INotifyPropertyChanged
	{
		public PreLoadWindow()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += delegate { Mouse.OverrideCursor = Cursors.Wait; };
			Closed += delegate { Mouse.OverrideCursor = null; };
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