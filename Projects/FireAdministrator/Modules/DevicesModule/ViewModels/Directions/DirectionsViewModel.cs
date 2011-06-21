using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public class DirectionsViewModel : RegionViewModel
    {
        public DirectionsViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete);
            EditCommand = new RelayCommand(OnEdit);
        }

        public void Initialize()
        {
            Directions = new ObservableCollection<DirectionViewModel>(from direction in FiresecManager.Configuration.Directions select new DirectionViewModel(direction));

            if (Directions.Count > 0)
                SelectedDirection = Directions[0];
        }

        ObservableCollection<DirectionViewModel> _directions;
        public ObservableCollection<DirectionViewModel> Directions
        {
            get{return _directions;}
            set
            {
                _directions = value;
                OnPropertyChanged("Directions");
            }
        }

        DirectionViewModel _selectedDirection;
        public DirectionViewModel SelectedDirection
        {
            get { return _selectedDirection; }
            set
            {
                _selectedDirection = value;
                OnPropertyChanged("SelectedDirection");
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            if (SelectedDirection != null)
            {
                FiresecManager.Configuration.Directions.Remove(SelectedDirection.Direction);
                Directions.Remove(SelectedDirection);
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            DirectionDetailsViewModel directionDetailsViewModel = new DirectionDetailsViewModel();
            directionDetailsViewModel.Initialize();
            var result = ServiceFactory.UserDialogs.ShowModalWindow(directionDetailsViewModel);
            if (result)
            {
                FiresecManager.Configuration.Directions.Add(directionDetailsViewModel.Direction);
                DirectionViewModel directionViewModel = new DirectionViewModel(directionDetailsViewModel.Direction);
                Directions.Add(directionViewModel);
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            if (SelectedDirection != null)
            {
                DirectionDetailsViewModel directionDetailsViewModel = new DirectionDetailsViewModel();
                directionDetailsViewModel.Initialize(SelectedDirection.Direction);
                var result = ServiceFactory.UserDialogs.ShowModalWindow(directionDetailsViewModel);
                if (result)
                {
                    SelectedDirection.Update();
                }
            }
        }

        public override void Dispose()
        {
        }
    }
}
