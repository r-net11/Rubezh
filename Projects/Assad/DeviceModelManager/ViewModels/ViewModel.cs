using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using Infrastructure.Common;

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
            AssadTreeBuilder assadTreeBuilder = new AssadTreeBuilder();
            assadTreeBuilder.Build();
            TreeItem RootTreeItem = assadTreeBuilder.RootTreeItem;
            Devices = new ObservableCollection<TreeItem>();
            Devices.Add(RootTreeItem);
            return;
        }
    }
}
