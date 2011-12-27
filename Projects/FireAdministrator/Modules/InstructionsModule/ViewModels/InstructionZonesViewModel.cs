using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionZonesViewModel : SaveCancelDialogContent
    {
        public InstructionZonesViewModel(List<ulong?> instructionZonesList)
        {
            Title = "Выбор зоны";

            InstructionZonesList = new List<ulong?>(instructionZonesList);
            InstructionZones = new ObservableCollection<ZoneViewModel>();
            AvailableZones = new ObservableCollection<ZoneViewModel>();

            InicializeZones();
            if (InstructionZones.IsNotNullOrEmpty())
                SelectedInstructionZone = InstructionZones[0];

            AddOneCommand = new RelayCommand(OnAddOne, CanAddOne);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
        }

        void InicializeZones()
        {
            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                var zoneViewModel = new ZoneViewModel(zone);
                if (InstructionZonesList.IsNotNullOrEmpty())
                {
                    var instructionZone = InstructionZonesList.FirstOrDefault(x => x.Value == zoneViewModel.No.Value);
                    if (instructionZone != null)
                        InstructionZones.Add(zoneViewModel);
                    else
                        AvailableZones.Add(zoneViewModel);
                }
                else
                {
                    AvailableZones.Add(zoneViewModel);
                }
            }

            if (InstructionZones.IsNotNullOrEmpty())
                SelectedInstructionZone = InstructionZones[0];
            if (AvailableZones.IsNotNullOrEmpty())
                SelectedAvailableZone = AvailableZones[0];
        }

        public List<ulong?> InstructionZonesList { get; set; }
        public ObservableCollection<ZoneViewModel> AvailableZones { get; set; }
        public ObservableCollection<ZoneViewModel> InstructionZones { get; set; }

        ZoneViewModel _selectedAvailableZone;
        public ZoneViewModel SelectedAvailableZone
        {
            get { return _selectedAvailableZone; }
            set
            {
                _selectedAvailableZone = value;
                OnPropertyChanged("SelectedAvailableZone");
            }
        }

        ZoneViewModel _selectedInstructionZone;
        public ZoneViewModel SelectedInstructionZone
        {
            get { return _selectedInstructionZone; }
            set
            {
                _selectedInstructionZone = value;
                OnPropertyChanged("SelectedInstructionZone");
            }
        }

        public bool CanAddOne()
        {
            return SelectedAvailableZone != null;
        }

        public bool CanAddAll()
        {
            return AvailableZones.IsNotNullOrEmpty();
        }

        public bool CanRemoveOne()
        {
            return SelectedInstructionZone != null;
        }

        public bool CanRemoveAll()
        {
            return InstructionZones.IsNotNullOrEmpty();
        }

        //public bool CanSaveInstruction()
        //{
        //    return (InstructionZones.IsNotNullOrEmpty());
        //}

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            InstructionZones.Add(SelectedAvailableZone);
            AvailableZones.Remove(SelectedAvailableZone);
            if (AvailableZones.IsNotNullOrEmpty())
                SelectedAvailableZone = AvailableZones[0];
            if (InstructionZones.IsNotNullOrEmpty())
                SelectedInstructionZone = InstructionZones[0];
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var availableZone in AvailableZones)
            {
                InstructionZones.Add(availableZone);
            }

            AvailableZones.Clear();
            SelectedAvailableZone = null;
            if (InstructionZones.IsNotNullOrEmpty())
                SelectedInstructionZone = InstructionZones[0];
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var instructionZone in InstructionZones)
            {
                AvailableZones.Add(instructionZone);
            }

            InstructionZones.Clear();
            SelectedInstructionZone = null;
            if (AvailableZones.IsNotNullOrEmpty())
                SelectedAvailableZone = AvailableZones[0];
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            AvailableZones.Add(SelectedInstructionZone);
            InstructionZones.Remove(SelectedInstructionZone);
            if (AvailableZones.IsNotNullOrEmpty())
                SelectedAvailableZone = AvailableZones[0];
            if (InstructionZones.IsNotNullOrEmpty())
                SelectedInstructionZone = InstructionZones[0];
        }

        protected override void Save(ref bool cancel)
        {
            InstructionZonesList = new List<ulong?>(from zone in InstructionZones select zone.No);
        }
    }
}