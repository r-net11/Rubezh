using System;
using System.Collections.ObjectModel;
using System.Linq;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class PositionSelectionViewModel : SaveCancelDialogViewModel
	{
		Guid OrganisationUID;

		public PositionSelectionViewModel(Employee employee, ShortPosition shortPosition)
		{
			Title = CommonViewModels.SelectPosition;
			OrganisationUID = employee.OrganisationUID;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			ClearCommand = new RelayCommand(OnClear, CanClear);

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
			var positionDetailsViewModel = new PositionDetailsViewModel();
			positionDetailsViewModel.Initialize(OrganisationUID);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				var position = positionDetailsViewModel.Model;
				Positions.Add(position);
				SelectedPosition = Positions.LastOrDefault();
				ServiceFactory.Events.GetEvent<NewPositionEvent>().Publish(position);
			}
		}
		bool CanAdd()
		{
			return FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Positions_Etit);
		}

		public RelayCommand ClearCommand { get; private set; }

		private bool CanClear()
		{
			return SelectedPosition != null;
		}

		void OnClear()
		{
			SelectedPosition = null;
		}
	}
}