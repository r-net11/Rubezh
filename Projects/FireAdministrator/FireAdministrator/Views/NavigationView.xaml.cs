using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Infrastructure.Common.Navigation;

namespace FireAdministrator.Views
{
	public partial class NavigationView : UserControl, INotifyPropertyChanged
	{
		public NavigationView()
		{
			InitializeComponent();
			DataContext = this;
		}

		private List<NavigationItem> _navigation;
		public List<NavigationItem> Navigation
		{
			get { return _navigation; }
			set
			{
				_navigation = value;
				OnPropertyChanged("Navigation");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}