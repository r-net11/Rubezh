using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Processor;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ChangeParentViewModel : SaveCancelDialogViewModel
	{
		public ChangeParentViewModel(Guid exceptApartmentUid)
		{
			Title = "Выбор группы для перемещения";
			BuildTree(exceptApartmentUid);
			if (RootApartment != null)
			{
				SelectedApartment = RootApartment;
				RootApartment.IsExpanded = true;
				foreach (var child in RootApartment.Children)
					child.IsExpanded = true;
			}

			foreach (var apartmentViewModel in AllApartments)
			{
				if (true)
					apartmentViewModel.ExpandToThis();
			}

			OnPropertyChanged(() => RootApartments);
		}

		ApartmentViewModel _selectedApartment;
		public ApartmentViewModel SelectedApartment
		{
			get { return _selectedApartment; }
			set
			{
				_selectedApartment = value;
				OnPropertyChanged(() => SelectedApartment);
			}
		}

		ApartmentViewModel _rootApartment;
		public ApartmentViewModel RootApartment
		{
			get { return _rootApartment; }
			private set
			{
				_rootApartment = value;
				OnPropertyChanged(() => RootApartment);
			}
		}

		public ApartmentViewModel[] RootApartments
		{
			get { return new[] { RootApartment }; }
		}

		void BuildTree(Guid exceptApartmentUid)
		{
			RootApartment = AddApartmentInternal(DBCash.RootApartment, null, exceptApartmentUid);
			FillAllApartments();
		}

		private ApartmentViewModel AddApartmentInternal(Apartment apartment, ApartmentViewModel parentApartmentViewModel, Guid exceptApartmentUid)
		{
			var apartmentViewModel = new ApartmentViewModel(apartment);
			if (parentApartmentViewModel != null)
				parentApartmentViewModel.AddChild(apartmentViewModel);

			foreach (var childApartment in apartment.Children.Where(x => x.IsFolder && x.UID != exceptApartmentUid))
				AddApartmentInternal(childApartment, apartmentViewModel, exceptApartmentUid);
			return apartmentViewModel;
		}

		public List<ApartmentViewModel> AllApartments;

		public void FillAllApartments()
		{
			AllApartments = new List<ApartmentViewModel>();
			AddChildPlainApartments(RootApartment);
		}

		void AddChildPlainApartments(ApartmentViewModel parentViewModel)
		{
			AllApartments.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainApartments(childViewModel);
		}

		public void Select(Guid apartmentUID)
		{
			if (apartmentUID != Guid.Empty)
			{
				FillAllApartments();
				var apartmentViewModel = AllApartments.FirstOrDefault(x => x.Apartment.UID == apartmentUID);
				if (apartmentViewModel != null)
					apartmentViewModel.ExpandToThis();
				SelectedApartment = apartmentViewModel;
			}
		}
	}
}