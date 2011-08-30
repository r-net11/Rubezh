using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using DevicesModule.ViewModels;
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
            AddAllZoneCommand = new RelayCommand(OnAddAllZone, CanAddAllAvailableZone);
            RemoveAllZoneCommand = new RelayCommand(OnRemoveAllZone, CanRemoveAllZone);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

            InstructionZones = new ObservableCollection<ZoneViewModel>();
            AvailableZones = new ObservableCollection<ZoneViewModel>();
            InstructionZonesList = new List<string>();
        }

        public void Inicialize(List<string> instructionZonesList)
        {
            if (instructionZonesList.IsNotNullOrEmpty())
            {
                InstructionZonesList = new List<string>(instructionZonesList);
            }
            InicializeZones();
            if (InstructionZones.IsNotNullOrEmpty())
            {
                SelectedInstructionZone = InstructionZones[0];
            }
        }

        void InicializeZones()
        {
            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                var zoneViewModel = new ZoneViewModel(zone);
                if (InstructionZonesList.IsNotNullOrEmpty())
                {
                    var instructionZone = InstructionZonesList.FirstOrDefault(x => x == zoneViewModel.No);
                    if (instructionZone != null)
                    {
                        InstructionZones.Add(zoneViewModel);
                    }
                    else
                    {
                        AvailableZones.Add(zoneViewModel);
                    }
                }
                else
                {
                    AvailableZones.Add(zoneViewModel);
                }
            }

            if (InstructionZones.IsNotNullOrEmpty())
            {
                SelectedInstructionZone = InstructionZones[0];
            }
            if (AvailableZones.IsNotNullOrEmpty())
            {
                SelectedAvailableZone = AvailableZones[0];
            }
        }

        public List<string> InstructionZonesList { get; set; }
        public ObservableCollection<ZoneViewModel> AvailableZones { get; set; }
        public ObservableCollection<ZoneViewModel> InstructionZones { get; set; }
        public ZoneViewModel SelectedAvailableZone { get; set; }
        public ZoneViewModel SelectedInstructionZone { get; set; }

        public bool CanAddAvailableZone()
        {
            return (SelectedAvailableZone != null);
        }

        public bool CanAddAllAvailableZone()
        {
            return (AvailableZones.IsNotNullOrEmpty());
        }

        public bool CanRemoveZone()
        {
            return (SelectedInstructionZone != null);
        }

        public bool CanRemoveAllZone()
        {
            return (InstructionZones.IsNotNullOrEmpty());
        }

        //public bool CanSaveInstruction()
        //{
        //    return (InstructionZones.IsNotNullOrEmpty());
        //}

        public RelayCommand AddZoneCommand { get; private set; }
        void OnAddZone()
        {
            InstructionZones.Add(SelectedAvailableZone);
            AvailableZones.Remove(SelectedAvailableZone);
            if (AvailableZones.IsNotNullOrEmpty())
            {
                SelectedAvailableZone = AvailableZones[0];
            }
            if (InstructionZones.IsNotNullOrEmpty())
            {
                SelectedInstructionZone = InstructionZones[0];
            }
        }

        public RelayCommand AddAllZoneCommand { get; private set; }
        void OnAddAllZone()
        {
            foreach (var availableZone in AvailableZones)
            {
                InstructionZones.Add(availableZone);
            }
            AvailableZones.Clear();
            SelectedAvailableZone = null;
            if (InstructionZones.IsNotNullOrEmpty())
            {
                SelectedInstructionZone = InstructionZones[0];
            }
        }

        public RelayCommand RemoveAllZoneCommand { get; private set; }
        void OnRemoveAllZone()
        {
            foreach (var instructionZone in InstructionZones)
            {
                AvailableZones.Add(instructionZone);
            }
            InstructionZones.Clear();
            SelectedInstructionZone = null;
            if (AvailableZones.IsNotNullOrEmpty())
            {
                SelectedAvailableZone = AvailableZones[0];
            }
        }

        public RelayCommand RemoveZoneCommand { get; private set; }
        void OnRemoveZone()
        {
            AvailableZones.Add(SelectedInstructionZone);
            InstructionZones.Remove(SelectedInstructionZone);
            if (AvailableZones.IsNotNullOrEmpty())
            {
                SelectedAvailableZone = AvailableZones[0];
            }
            if (InstructionZones.IsNotNullOrEmpty())
            {
                SelectedInstructionZone = InstructionZones[0];
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Save();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }

        public void Save()
        {
            var instructionZones = new List<string>(
                from zone in InstructionZones
                select zone.No);
            InstructionZonesList = instructionZones;
        }
    }
}