using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common;
using DevicesModule.ViewModels;
using FiresecClient;

namespace InstructionsModule.ViewModels
{
    public class InstructionZonesViewModel : BaseViewModel
    {
        public InstructionZonesViewModel()
        {
            //Команды
        }

        string _instructionName;
        Instruction _instruction;

        public void Inicialized(Instruction instruction)
        {
            _instruction = instruction;
            AddedZones = new ObservableCollection<InstructionZone>(_instruction.InstructionZones);
            if (AddedZones.Count > 0)
            {
                SelectedAddedZone = AddedZones[0];
            }
        }

        void InicializedZonesAndAddedZones()
        {
            var ZonesNo = new List<int>(from zone in FiresecManager.DeviceConfiguration.Zones
                                        orderby (int.Parse(zone.No))
                                        select (int.Parse(zone.No)));
        
        }

        ObservableCollection<InstructionZone> _zones;
        public ObservableCollection<InstructionZone> Zones
        { 
            get { return _zones; }
            set 
            {
                _zones = value;
                OnPropertyChanged("Zones");
            }
        }

        ObservableCollection<InstructionZone> _addedZones;
        public ObservableCollection<InstructionZone> AddedZones
        {
            get { return _addedZones; }
            set
            {
                _addedZones = value;
                OnPropertyChanged("AddedZones");
            }
        }

        InstructionZone _selectedAddedZone;
        public InstructionZone SelectedAddedZone
        {
            get { return _selectedAddedZone; }
            set
            {
                _selectedAddedZone = value;
                OnPropertyChanged("SelectedAddedZone");
            }
        }

        ObservableCollection<DeviceViewModel> _devicesSelectedAddedZone;
        public ObservableCollection<DeviceViewModel> DevicesSelectedAddedZone
        {
            get { return _devicesSelectedAddedZone; }
            set
            {
                DevicesSelectedAddedZone = value;
                OnPropertyChanged("DevicesSelectedAddedZone");
            }
        }

        DeviceViewModel _selectedDeviceAddedZone;
        public DeviceViewModel SelectedDeviceAddedZone
        {
            get { return _selectedDeviceAddedZone; }
            set
            {
                _selectedDeviceAddedZone = value;
                OnPropertyChanged("SelectedDeviceAddedZone");
            }
        }


    }
}
