using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using SKDModule.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class OrganisationPositionsViewModel : BaseViewModel
	{
		public OrganisationPositionsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(string name, List<Position> positions)
		{
			Name = name;

			Positions = new ObservableCollection<PositionViewModel>();
			foreach (var position in positions)
			{
				var positionViewModel = new PositionViewModel(position);
				Positions.Add(positionViewModel);
			}
			SelectedPosition = Positions.FirstOrDefault();
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
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
				if (!saveResult)
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
	}
}