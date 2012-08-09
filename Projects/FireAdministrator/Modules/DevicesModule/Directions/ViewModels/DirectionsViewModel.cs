using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Views;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DirectionsViewModel : ViewPartViewModel, IEditingViewModel
	{
		public DirectionsViewModel()
		{
			DeleteCommand = new RelayCommand(OnDelete, CanEditOrDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditOrDelete);
			AddCommand = new RelayCommand(OnAdd);
		}

		public void Initialize()
		{
			Directions = new ObservableCollection<DirectionViewModel>(
				from direction in FiresecManager.Directions
				select new DirectionViewModel(direction));
			SelectedDirection = Directions.FirstOrDefault(); ;
		}

		ObservableCollection<DirectionViewModel> _directions;
		public ObservableCollection<DirectionViewModel> Directions
		{
			get { return _directions; }
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
			FiresecManager.Directions.Remove(SelectedDirection.Direction);
			Directions.Remove(SelectedDirection);

			ServiceFactory.SaveService.DevicesChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel(SelectedDirection.Direction);
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
			{
				SelectedDirection.Update();

				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

		bool CanEditOrDelete()
		{
			return SelectedDirection != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel();
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
			{
				FiresecManager.Directions.Add(directionDetailsViewModel.Direction);
				Directions.Add(new DirectionViewModel(directionDetailsViewModel.Direction));

				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

		public override void OnShow()
		{
			ServiceFactory.Layout.ShowMenu(new DirectionsMenuViewModel(this));

			if (DirectionsMenuView.Current != null)
				DirectionsMenuView.Current.AcceptKeyboard = true;
		}

		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);

			if (DirectionsMenuView.Current != null)
				DirectionsMenuView.Current.AcceptKeyboard = false;
		}
	}
}