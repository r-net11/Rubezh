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
            Directions = new ObservableCollection<DirectionViewModel>();

            if (FiresecManager.CoreConfig.part != null)
            {
                foreach (var direction in FiresecManager.CurrentConfiguration.Directions)
                {
                    DirectionViewModel directionViewModel = new DirectionViewModel();
                    directionViewModel.Initialize(direction);
                    Directions.Add(directionViewModel);
                }
            }

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

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            DirectionDetailsViewModel directionDetailsViewModel = new DirectionDetailsViewModel();
            directionDetailsViewModel.Initialize();
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(directionDetailsViewModel);
            if (result)
            {
                DirectionViewModel directionViewModel = new DirectionViewModel();
                directionViewModel.Initialize(directionDetailsViewModel.Direction);
                Directions.Add(directionViewModel);
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            if (SelectedDirection != null)
            {
                var direction = FiresecManager.CurrentConfiguration.Directions.FirstOrDefault(x => x.Id == SelectedDirection.Id);
                FiresecManager.CurrentConfiguration.Directions.Remove(direction);
                Directions.Remove(SelectedDirection);
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            if (SelectedDirection != null)
            {
                var direction = FiresecManager.CurrentConfiguration.Directions.FirstOrDefault(x => x.Id == SelectedDirection.Id);
                DirectionDetailsViewModel directionDetailsViewModel = new DirectionDetailsViewModel();
                directionDetailsViewModel.Initialize(direction);
                bool result = ServiceFactory.UserDialogs.ShowModalWindow(directionDetailsViewModel);
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
