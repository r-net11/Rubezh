using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace SKDModule.ViewModels
{
	public class PositionSelectionViewModel : SaveCancelDialogViewModel
	{
		Guid OrganisationUID;
		HRViewModel HRViewModel;

		public PositionSelectionViewModel(Employee employee, ShortPosition shortPosition, HRViewModel hrViewModel)
		{
			Title = "Выбор должности";
			OrganisationUID = employee.OrganisationUID;
			HRViewModel = hrViewModel;
			AddCommand = new RelayCommand(OnAdd);

			Positions = new ObservableCollection<ShortPosition>();
			var positions = PositionHelper.GetByOrganisation(OrganisationUID);
			if (positions != null)
			{
				foreach (var position in positions)
				{
					Positions.Add(position);
				}
			}
			if (shortPosition != null)
			{
				SelectedPosition = Positions.FirstOrDefault(x => x.UID == shortPosition.UID);
			}
		}

		public ObservableCollection<ShortPosition> Positions { get; set; }

		ShortPosition _selectedPosition;
		public ShortPosition SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				OnPropertyChanged(() => SelectedPosition);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(OrganisationUID);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				Positions.Add(positionDetailsViewModel.ShortPosition);
				SelectedPosition = Positions.LastOrDefault();
				HRViewModel.UpdatePositions();
			}
		}
	}
}