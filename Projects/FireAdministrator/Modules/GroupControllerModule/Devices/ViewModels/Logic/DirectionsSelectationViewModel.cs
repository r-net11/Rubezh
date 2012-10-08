using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DirectionsSelectationViewModel : SaveCancelDialogViewModel
    {
		public List<XDirection> Directions { get; private set; }

		public DirectionsSelectationViewModel(List<XDirection> directions)
        {
            Title = "Выбор зон";
            AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

			Directions = directions;
			TargetDirections = new ObservableCollection<XDirection>();
			SourceDirections = new ObservableCollection<XDirection>();

			foreach (var direction in XManager.DeviceConfiguration.Directions)
            {
				if (Directions.Contains(direction))
                    TargetDirections.Add(direction);
                else
                    SourceDirections.Add(direction);
            }

			SelectedTargetDirection = TargetDirections.FirstOrDefault();
			SelectedSourceDirection = SourceDirections.FirstOrDefault();
        }

		public ObservableCollection<XDirection> SourceDirections { get; private set; }

		XDirection _selectedSourceDirection;
		public XDirection SelectedSourceDirection
        {
            get { return _selectedSourceDirection; }
            set
            {
                _selectedSourceDirection = value;
				OnPropertyChanged("SelectedSourceDirection");
            }
        }

		public ObservableCollection<XDirection> TargetDirections { get; private set; }

		XDirection _selectedTargetDirection;
		public XDirection SelectedTargetDirection
        {
            get { return _selectedTargetDirection; }
            set
            {
                _selectedTargetDirection = value;
				OnPropertyChanged("SelectedTargetDirection");
            }
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            TargetDirections.Add(SelectedSourceDirection);
            SelectedTargetDirection = SelectedSourceDirection;
            SourceDirections.Remove(SelectedSourceDirection);
			SelectedSourceDirection = SourceDirections.FirstOrDefault();
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            SourceDirections.Add(SelectedTargetDirection);
            SelectedSourceDirection = SelectedTargetDirection;
            TargetDirections.Remove(SelectedTargetDirection);
			SelectedTargetDirection = TargetDirections.FirstOrDefault();
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var zoneViewModel in SourceDirections)
            {
                TargetDirections.Add(zoneViewModel);
            }
            SourceDirections.Clear();
			SelectedTargetDirection = TargetDirections.FirstOrDefault();
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var zoneViewModel in TargetDirections)
            {
                SourceDirections.Add(zoneViewModel);
            }
            TargetDirections.Clear();
			SelectedSourceDirection = SourceDirections.FirstOrDefault();
        }

        bool CanAdd()
        {
            return SelectedSourceDirection != null;
        }

        bool CanRemove()
        {
            return SelectedTargetDirection != null;
        }

		protected override bool Save()
		{
			Directions = new List<XDirection>(TargetDirections);
			return base.Save();
		}
    }
}