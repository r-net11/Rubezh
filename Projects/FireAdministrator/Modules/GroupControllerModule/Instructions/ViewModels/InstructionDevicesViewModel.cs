using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections;

namespace GKModule.ViewModels
{
    public class InstructionDevicesViewModel : SaveCancelDialogViewModel
    {
        public InstructionDevicesViewModel(List<Guid> instructionDevicesList)
        {
            AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
            Title = "Выбор устройства";

            InstructionDevicesList = new List<Guid>(instructionDevicesList);

            UpdateDevices();
        }

        void UpdateDevices()
        {
            SourceDevices = new ObservableCollection<XDevice>();
            TargetDevices = new ObservableCollection<XDevice>();
            SelectedSourceDevice = null;
            SelectedTargetDevice = null;

            foreach (var device in XManager.Devices)
            {
                if (device.IsRealDevice && device.Driver.IsDeviceOnShleif)
                {
                    if (InstructionDevicesList.Contains(device.UID))
                        TargetDevices.Add(device);
                    else
                        SourceDevices.Add(device);
                }
            }

			SelectedTargetDevice = TargetDevices.FirstOrDefault();
			SelectedSourceDevice = SourceDevices.FirstOrDefault();

			OnPropertyChanged("SourceDevices");
			OnPropertyChanged("TargetDevices");
        }

        public List<Guid> InstructionDevicesList { get; set; }
        public ObservableCollection<XDevice> SourceDevices { get; private set; }
        public ObservableCollection<XDevice> TargetDevices { get; private set; }

        XDevice _selectedAvailableDevice;
        public XDevice SelectedSourceDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
				OnPropertyChanged("SelectedSourceDevice");
            }
        }

        XDevice _selectedInstructionDevice;
        public XDevice SelectedTargetDevice
        {
            get { return _selectedInstructionDevice; }
            set
            {
                _selectedInstructionDevice = value;
				OnPropertyChanged("SelectedTargetDevice");
            }
        }

        public bool CanAddAll()
        {
            return (SourceDevices.Count > 0);
        }

        public bool CanRemoveAll()
        {
            return (TargetDevices.Count > 0);
        }

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceDevices;
		void OnAdd(object parameter)
		{
			var index = SourceDevices.IndexOf(SelectedSourceDevice);

			SelectedSourceDevices = (IList)parameter;
			var SourceDeviceViewModels = new List<XDevice>();
			foreach (var SourceDevice in SelectedSourceDevices)
			{
				var SourceDeviceViewModel = SourceDevice as XDevice;
				if (SourceDeviceViewModel != null)
					SourceDeviceViewModels.Add(SourceDeviceViewModel);
			}
			foreach (var SourceDeviceViewModel in SourceDeviceViewModels)
			{
				TargetDevices.Add(SourceDeviceViewModel);
				SourceDevices.Remove(SourceDeviceViewModel);
			}
			SelectedTargetDevice = TargetDevices.LastOrDefault();
			OnPropertyChanged("SourceDevices");

			index = Math.Min(index, SourceDevices.Count - 1);
			if (index > -1)
				SelectedSourceDevice = SourceDevices[index];
		}
		public bool CanAdd(object parameter)
		{
			return SelectedSourceDevice != null;
		}

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var deviceViewModel in SourceDevices)
            {
                InstructionDevicesList.Add(deviceViewModel.UID);
            }
            UpdateDevices();
        }

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedDevices;
		void OnRemove(object parameter)
		{
			var index = TargetDevices.IndexOf(SelectedTargetDevice);

			SelectedDevices = (IList)parameter;
			var deviceViewModels = new List<XDevice>();
			foreach (var device in SelectedDevices)
			{
				var deviceViewModel = device as XDevice;
				if (deviceViewModel != null)
					deviceViewModels.Add(deviceViewModel);
			}
			foreach (var deviceViewModel in deviceViewModels)
			{
				SourceDevices.Add(deviceViewModel);
				TargetDevices.Remove(deviceViewModel);
			}
			SelectedSourceDevice = SourceDevices.LastOrDefault();
			OnPropertyChanged("TargetDevices");

			index = Math.Min(index, TargetDevices.Count - 1);
			if (index > -1)
				SelectedTargetDevice = TargetDevices[index];
		}
		public bool CanRemove(object parameter)
		{
			return SelectedTargetDevice != null;
		}

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            InstructionDevicesList.Clear();
            UpdateDevices();
        }
    }
}