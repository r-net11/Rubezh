using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections;

namespace GKModule.ViewModels
{
    public class InstructionZonesViewModel : SaveCancelDialogViewModel
    {
		public InstructionZonesViewModel(List<Guid> instructionZonesList)
        {
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

            Title = "Выбор зоны";

			TargetZonesList = new List<Guid>(instructionZonesList);
            TargetZones = new ObservableCollection<ZoneViewModel>();
            SourceZones = new ObservableCollection<ZoneViewModel>();

            InitializeZones();
            if (TargetZones.IsNotNullOrEmpty())
                SelectedTargetZone = TargetZones[0];
        }

        void InitializeZones()
        {
            foreach (var zone in XManager.Zones)
            {
                var zoneViewModel = new ZoneViewModel(zone);
                if (TargetZonesList.IsNotNullOrEmpty())
                {
                    var instructionZone = TargetZonesList.FirstOrDefault(x => x == zoneViewModel.Zone.UID);
                    if (instructionZone != Guid.Empty)
                        TargetZones.Add(zoneViewModel);
                    else
                        SourceZones.Add(zoneViewModel);
                }
                else
                {
                    SourceZones.Add(zoneViewModel);
                }
            }

            if (TargetZones.IsNotNullOrEmpty())
                SelectedTargetZone = TargetZones[0];
            if (SourceZones.IsNotNullOrEmpty())
                SelectedSourceZone = SourceZones[0];
        }

		public List<Guid> TargetZonesList { get; set; }
        public ObservableCollection<ZoneViewModel> SourceZones { get; set; }
        public ObservableCollection<ZoneViewModel> TargetZones { get; set; }

        ZoneViewModel _selectedAvailableZone;
        public ZoneViewModel SelectedSourceZone
        {
            get { return _selectedAvailableZone; }
            set
            {
                _selectedAvailableZone = value;
				OnPropertyChanged("SelectedSourceZone");
            }
        }

        ZoneViewModel _selectedInstructionZone;
        public ZoneViewModel SelectedTargetZone
        {
            get { return _selectedInstructionZone; }
            set
            {
                _selectedInstructionZone = value;
				OnPropertyChanged("SelectedTargetZone");
            }
        }

        public bool CanAddAll()
        {
            return SourceZones.IsNotNullOrEmpty();
        }

        public bool CanRemoveAll()
        {
            return TargetZones.IsNotNullOrEmpty();
        }

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceZones;
		void OnAdd(object parameter)
		{
			var index = SourceZones.IndexOf(SelectedSourceZone);

			SelectedSourceZones = (IList)parameter;
			var zoneViewModels = new List<ZoneViewModel>();
			foreach (var selectedZone in SelectedSourceZones)
			{
				var zoneViewModel = selectedZone as ZoneViewModel;
				if (zoneViewModel != null)
					zoneViewModels.Add(zoneViewModel);
			}
			foreach (var zoneViewModel in zoneViewModels)
			{
				TargetZones.Add(zoneViewModel);
				SourceZones.Remove(zoneViewModel);
			}
			SelectedTargetZone = TargetZones.LastOrDefault();
			OnPropertyChanged("SourceZones");

			index = Math.Min(index, SourceZones.Count - 1);
			if (index > -1)
				SelectedSourceZone = SourceZones[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetZones;
		void OnRemove(object parameter)
		{
			var index = TargetZones.IndexOf(SelectedTargetZone);

			SelectedTargetZones = (IList)parameter;
			var zoneViewModels = new List<ZoneViewModel>();
			foreach (var selectedZone in SelectedTargetZones)
			{
				var zoneViewModel = selectedZone as ZoneViewModel;
				if (zoneViewModel != null)
					zoneViewModels.Add(zoneViewModel);
			}
			foreach (var zoneViewModel in zoneViewModels)
			{
				SourceZones.Add(zoneViewModel);
				TargetZones.Remove(zoneViewModel);
			}
			SelectedSourceZone = SourceZones.LastOrDefault();
			OnPropertyChanged("TargetZones");

			index = Math.Min(index, TargetZones.Count - 1);
			if (index > -1)
				SelectedTargetZone = TargetZones[index];
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceZone != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetZone != null;
		}

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var availableZone in SourceZones)
            {
                TargetZones.Add(availableZone);
            }

            SourceZones.Clear();
            SelectedSourceZone = null;
            if (TargetZones.IsNotNullOrEmpty())
                SelectedTargetZone = TargetZones[0];
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var instructionZone in TargetZones)
            {
                SourceZones.Add(instructionZone);
            }

            TargetZones.Clear();
            SelectedTargetZone = null;
            if (SourceZones.IsNotNullOrEmpty())
                SelectedSourceZone = SourceZones[0];
        }

        protected override bool Save()
		{
			TargetZonesList = new List<Guid>(from zone in TargetZones select zone.Zone.UID);
			return base.Save();
		}
    }
}