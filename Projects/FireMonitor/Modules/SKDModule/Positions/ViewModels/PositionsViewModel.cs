using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;

namespace SKDModule.ViewModels
{
	public class PositionsViewModel : ViewPartViewModel
	{
		ShortPosition _clipboard;

		public PositionsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(PositionFilter filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;
			var positions = PositionHelper.Get(filter);
			if (positions == null)
				return;
			AllPositions = new List<PositionViewModel>();
			Organisations = new List<PositionViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new PositionViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllPositions.Add(organisationViewModel);
				foreach (var position in positions)
				{
					if (position.OrganisationUID == organisation.UID)
					{
						var positionViewModel = new PositionViewModel(organisation, position);
						organisationViewModel.AddChild(positionViewModel);
						AllPositions.Add(positionViewModel);
					}
				}
			}
			OnPropertyChanged(() => Organisations);
			SelectedPosition = Organisations.FirstOrDefault();
		}

		public List<PositionViewModel> Organisations { get; private set; }
		List<PositionViewModel> AllPositions { get; set; }

		PositionViewModel _selectedPosition;
		public PositionViewModel SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged(() => SelectedPosition);
			}
		}

		public PositionViewModel ParentOrganisation
		{
			get
			{
				PositionViewModel OrganisationViewModel = SelectedPosition;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedPosition.Parent;

				if (OrganisationViewModel.Organisation != null)
					return OrganisationViewModel;

				return null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(SelectedPosition.Organisation.UID);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				var positionViewModel = new PositionViewModel(SelectedPosition.Organisation, positionDetailsViewModel.ShortPosition);

				PositionViewModel OrganisationViewModel = SelectedPosition;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedPosition.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(positionViewModel);
				SelectedPosition = positionViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedPosition != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			PositionViewModel OrganisationViewModel = SelectedPosition;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedPosition.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedPosition);
			var position = SelectedPosition.Position;
			bool removeResult = PositionHelper.MarkDeleted(position.UID);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedPosition);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedPosition = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedPosition = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedPosition != null && !SelectedPosition.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(SelectedPosition.Organisation.UID, SelectedPosition.Position.UID);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				SelectedPosition.Update(positionDetailsViewModel.ShortPosition);
			}
		}
		bool CanEdit()
		{
			return SelectedPosition != null && SelectedPosition.Parent != null && !SelectedPosition.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_clipboard = CopyPosition(SelectedPosition.Position, false);
		}
		private bool CanCopy()
		{
			return SelectedPosition != null && !SelectedPosition.IsOrganisation;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newShortPosition = CopyPosition(_clipboard);
			var position = new Position()
			{
				UID = newShortPosition.UID,
				Name = newShortPosition.Name,
				Description = newShortPosition.Description
			};
			if (PositionHelper.Save(position))
			{
				var positionViewModel = new PositionViewModel(SelectedPosition.Organisation, newShortPosition);
				if (ParentOrganisation != null)
				{
					ParentOrganisation.AddChild(positionViewModel);
					AllPositions.Add(positionViewModel);
				}
				SelectedPosition = positionViewModel;
			}
		}
		private bool CanPaste()
		{
			return SelectedPosition != null && _clipboard != null;
		}

		ShortPosition CopyPosition(ShortPosition source, bool newName = true)
		{
			var copy = new ShortPosition();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
			copy.Description = source.Description;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			return copy;
		}
	}
}