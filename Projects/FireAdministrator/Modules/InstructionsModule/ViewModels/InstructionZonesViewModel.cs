using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionZonesViewModel : DialogContent
    {
        public InstructionZonesViewModel()
        {
            AddZoneCommand = new RelayCommand(OnAddZone, CanAddAvailableZone);
            RemoveZoneCommand = new RelayCommand(OnRemoveZone, CanRemoveZone);
            AddInstructionCommand = new RelayCommand(OnAddInstruction);
        }

        public void Inicialize(Instruction instruction)
        {
            _instruction = instruction;
            UpdateAvailableZonesAndZones();
            if (Zones.Count > 0)
            {
                SelectedZone = Zones[0];
            }
        }

        Instruction _instruction;

        string _text;
        public string Text 
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        void UpdateAvailableZonesAndZones()
        {
            Zones = new ObservableCollection<InstructionZoneViewModel>();
            AvailableZones = new ObservableCollection<Zone>();
            var instructionZone = new InstructionZone();
            if (_instruction.InstructionZones == null)
            {
                foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
                {
                    AvailableZones.Add(zone);
                }
            }
            else
            {
                foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
                {
                    instructionZone = _instruction.InstructionZones.FirstOrDefault(x => x.ZoneNo == zone.No);
                    if (instructionZone != null)
                    {
                        Zones.Add(new InstructionZoneViewModel(instructionZone));
                    }
                    else
                    {
                        AvailableZones.Add(zone);
                    }
                }
            }
            
            if (Zones.Count > 0)
            {
                SelectedZone = Zones[0];
            }
            if (AvailableZones.Count > 0)
            {
                SelectedAvailableZone = AvailableZones[0];
            }
        }

        public ObservableCollection<Zone> AvailableZones { get; set; }

        public ObservableCollection<InstructionZoneViewModel> Zones { get; set; } 

        Zone _selectedAvailableZone;
        public Zone SelectedAvailableZone
        {
            get { return _selectedAvailableZone; }
            set
            {
                _selectedAvailableZone = value;
                OnPropertyChanged("SelectedAvailableZone");
            }
        }

        InstructionZoneViewModel _selectedZone;
        public InstructionZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                OnPropertyChanged("SelectedZone");
            }
        }

        public bool CanAddAvailableZone(object obj)
        {
            return (SelectedAvailableZone != null);
        }

        public bool CanRemoveZone(object obj)
        {
            return (SelectedZone != null);
        }

        public void SaveInstruction()
        {

        }

        public RelayCommand AddZoneCommand { get; private set; }
        void OnAddZone()
        {
            if (CanAddAvailableZone(null))
            {
                var instructionZoneViewModel = new InstructionZoneViewModel(SelectedAvailableZone);
                Zones.Add(instructionZoneViewModel);
                AvailableZones.Remove(SelectedAvailableZone);
                if (AvailableZones.Count != 0)
                {
                    SelectedAvailableZone = AvailableZones[0];
                }
                if (Zones.Count != 0)
                {
                    SelectedZone = Zones[0];
                }
            }
        }

        public RelayCommand RemoveZoneCommand { get; private set; }
        void OnRemoveZone()
        {
            if (CanRemoveZone(null))
            {
                var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == SelectedZone.ZoneNo);
                AvailableZones.Add(zone);
                Zones.Remove(SelectedZone);
                if (AvailableZones.Count != 0)
                {
                    SelectedAvailableZone = AvailableZones[0];
                }
                if (Zones.Count != 0)
                {
                    SelectedZone = Zones[0];
                }
            }
        }

        public RelayCommand AddInstructionCommand { get; private set; }
        void OnAddInstruction()
        {
            foreach (var zone in Zones)
            {
                if (zone.IsChecked)
                {
                    zone.Text = Text;
                }
            }
        }
    }
}

//void InicializeDevicesSelectedZone()
//{
//    var availableDevices = new List<Device>();
//    foreach (var device in FiresecManager.DeviceConfiguration.Devices)
//    {
//        if (device.Driver.IsZoneDevice)
//        {
//            if (device.ZoneNo == SelectedZone.ZoneNo)
//            {
//                device.AllParents.ForEach(x => { availableDevices.Add(x); });
//                availableDevices.Add(device);
//            }
//        }
//    }

//    DevicesSelectedZone = new ObservableCollection<DeviceViewModel>();
//    foreach (var device in availableDevices)
//    {
//        DeviceViewModel deviceViewModel = new DeviceViewModel();
//        deviceViewModel.Initialize(device, DevicesSelectedZone);
//        deviceViewModel.IsExpanded = true;
//        DevicesSelectedZone.Add(deviceViewModel);
//    }

//    foreach (var device in DevicesSelectedZone)
//    {
//        if (device.Device.Parent != null)
//        {
//            var parent = DevicesSelectedZone.FirstOrDefault(x => x.Device.Id == device.Device.Parent.Id);
//            device.Parent = parent;
//            parent.Children.Add(device);
//        }
//    }
//}
//public ObservableCollection<DeviceViewModel> DevicesSelectedZone { get; set; }

//DeviceViewModel _selectedDeviceZone;
//public DeviceViewModel SelectedDeviceZone
//{
//    get { return _selectedDeviceZone; }
//    set
//    {
//        _selectedDeviceZone = value;
//        OnPropertyChanged("SelectedDeviceZone");
//    }
//}