using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DeviveModelManager
{
	public class ViewModel : BaseViewModel
	{
		public static string StaticVersion { get; set; }

		public ViewModel()
		{
			GenarateCommand = new RelayCommand(OnGenarateCommand);
			Version = "5";
		}

		ObservableCollection<TreeItem> _devices;
		public ObservableCollection<TreeItem> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
		}

		TreeItem _selectedDevice;
		public TreeItem SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		string _version;
		public string Version
		{
			get { return _version; }
			set
			{
				_version = value;
				StaticVersion = _version;
				OnPropertyChanged("Version");
			}
		}

		public RelayCommand GenarateCommand { get; private set; }
		void OnGenarateCommand()
		{
			var assadTreeBuilder = new TreeBuilder();
			assadTreeBuilder.Build();
			Devices = new ObservableCollection<TreeItem>();
			Devices.Add(assadTreeBuilder.RootTreeItem);
		}
	}
}