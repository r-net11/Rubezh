using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class PositionsViewModel : ViewPartViewModel
	{
		public PositionsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RefreshCommand = new RelayCommand(OnRefresh);
			EditFilterCommand = new RelayCommand(OnEditFilter);
			Filter = new PositionFilter();
			Initialize();
		}

		PositionFilter Filter;

		public void Initialize()
		{
			Positions = new ObservableCollection<PositionViewModel>();
			var positions = PositionHelper.Get(Filter);
			if (positions == null)
				return;
			foreach (var position in positions)
				Positions.Add(new PositionViewModel(position));
			SelectedPosition = Positions.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<PositionViewModel> _positions;
		public ObservableCollection<PositionViewModel> Positions
		{
			get { return _positions; }
			set
			{
				_positions = value;
				OnPropertyChanged("Positions");
			}
		}

		PositionViewModel _selectedPosition;
		public PositionViewModel SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				OnPropertyChanged("SelectedPosition");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(this);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				var position = positionDetailsViewModel.Position;
				bool saveResult = PositionHelper.Save(position);
				if(!saveResult)
					return;
				var positionViewModel = new PositionViewModel(position);
				Positions.Add(positionViewModel);
				SelectedPosition = positionViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = Positions.IndexOf(SelectedPosition);
			var position = SelectedPosition.Position;
			bool removeResult = PositionHelper.MarkDeleted(position);
			if (!removeResult)
				return;
			Positions.Remove(SelectedPosition);
			index = Math.Min(index, Positions.Count - 1);
			if (index > -1)
				SelectedPosition = Positions[index];
		}
		bool CanRemove()
		{
			return SelectedPosition != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(this, SelectedPosition.Position);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				var position = positionDetailsViewModel.Position;
				bool saveResult = PositionHelper.Save(position);
				if (!saveResult)
					return;
				SelectedPosition.Update(positionDetailsViewModel.Position);
			}
		}
		bool CanEdit()
		{
			return SelectedPosition != null;
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new PositionFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				Initialize();
			}
		}
	}
}