using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class GUDsViewModel : ViewPartViewModel
	{
		public GUDsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			GUDs = new ObservableCollection<GUDViewModel>();
			var guds = new List<GUD>();
			foreach (var gud in guds)
			{
				var gudViewModel = new GUDViewModel(gud);
				GUDs.Add(gudViewModel);
			}
			SelectedGUD = GUDs.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<GUDViewModel> _guds;
		public ObservableCollection<GUDViewModel> GUDs
		{
			get { return _guds; }
			set
			{
				_guds = value;
				OnPropertyChanged("GUDs");
			}
		}

		GUDViewModel _selectedGUD;
		public GUDViewModel SelectedGUD
		{
			get { return _selectedGUD; }
			set
			{
				_selectedGUD = value;
				OnPropertyChanged("SelectedGUD");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var gudDetailsViewModel = new GUDDetailsViewModel(this);
			if (DialogService.ShowModalWindow(gudDetailsViewModel))
			{
				var gud = gudDetailsViewModel.GUD;
				var gudViewModel = new GUDViewModel(gud);
				GUDs.Add(gudViewModel);
				SelectedGUD = gudViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = GUDs.IndexOf(SelectedGUD);
			GUDs.Remove(SelectedGUD);
			index = Math.Min(index, GUDs.Count - 1);
			if (index > -1)
				SelectedGUD = GUDs[index];
		}
		bool CanRemove()
		{
			return SelectedGUD != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var gudDetailsViewModel = new GUDDetailsViewModel(this, SelectedGUD.GUD);
			if (DialogService.ShowModalWindow(gudDetailsViewModel))
			{
				SelectedGUD.Update(gudDetailsViewModel.GUD);
			}
		}
		bool CanEdit()
		{
			return SelectedGUD != null;
		}
	}
}