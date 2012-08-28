using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System;

namespace GKModule.ViewModels
{
    public class ZonesSelectationViewModel : SaveCancelDialogViewModel
    {
        public List<Guid> Zones { get; private set; }
		XDevice Device;

        public ZonesSelectationViewModel(XDevice device, List<Guid> zones)
        {
            Title = "Выбор зон";
            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

			Device = device;
            Zones = zones;
            TargetZones = new ObservableCollection<XZone>();
            SourceZones = new ObservableCollection<XZone>();

			foreach (var zone in XManager.GetGKZones(device))
            {
                if (Zones.Contains(zone.UID))
                    TargetZones.Add(zone);
                else
                    SourceZones.Add(zone);
            }

			SelectedTargetZone = TargetZones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
        }

        public ObservableCollection<XZone> SourceZones { get; private set; }

        XZone _selectedSourceZone;
        public XZone SelectedSourceZone
        {
            get { return _selectedSourceZone; }
            set
            {
                _selectedSourceZone = value;
                OnPropertyChanged("SelectedSourceZone");
            }
        }

        public ObservableCollection<XZone> TargetZones { get; private set; }

        XZone _selectedTargetZone;
        public XZone SelectedTargetZone
        {
            get { return _selectedTargetZone; }
            set
            {
                _selectedTargetZone = value;
                OnPropertyChanged("SelectedTargetZone");
            }
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            TargetZones.Add(SelectedSourceZone);
            SelectedTargetZone = SelectedSourceZone;
            SourceZones.Remove(SelectedSourceZone);
			SelectedSourceZone = SourceZones.FirstOrDefault();
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            SourceZones.Add(SelectedTargetZone);
            SelectedSourceZone = SelectedTargetZone;
            TargetZones.Remove(SelectedTargetZone);
			SelectedTargetZone = TargetZones.FirstOrDefault();
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var zoneViewModel in SourceZones)
            {
                TargetZones.Add(zoneViewModel);
            }
            SourceZones.Clear();
			SelectedTargetZone = TargetZones.FirstOrDefault();
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var zoneViewModel in TargetZones)
            {
                SourceZones.Add(zoneViewModel);
            }
            TargetZones.Clear();
			SelectedSourceZone = SourceZones.FirstOrDefault();
        }

        bool CanAdd()
        {
            return SelectedSourceZone != null;
        }

        bool CanRemove()
        {
            return SelectedTargetZone != null;
        }

		protected override bool Save()
		{
			Zones = new List<Guid>(TargetZones.Select(x => x.UID));
			return base.Save();
		}
    }
}