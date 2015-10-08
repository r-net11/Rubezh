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
	public class ApartmentsViewModel : BaseViewModel
	{
		public ApartmentsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddFolderCommand = new RelayCommand(OnAddFolder, CanAddFolder);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ChangeParentCommand = new RelayCommand(OnChangeParent, CanChangeParent);

			BuildTree();
			if (RootApartment != null)
			{
				SelectedApartment = RootApartment;
				RootApartment.IsExpanded = true;
				foreach (var child in RootApartment.Children)
					child.IsExpanded = true;
			}

			foreach (var apartment in AllApartments)
			{
				if (true)
					apartment.ExpandToThis();
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

		void BuildTree()
		{
			RootApartment = AddApartmentInternal(DBCash.RootApartment, null);
			FillAllApartments();
		}

		public ApartmentViewModel AddApartment(Apartment apartment, ApartmentViewModel parentApartmentViewModel)
		{
			var apartmentViewModel = AddApartmentInternal(apartment, parentApartmentViewModel);
			FillAllApartments();
			return apartmentViewModel;
		}
		private ApartmentViewModel AddApartmentInternal(Apartment apartment, ApartmentViewModel parentApartmentViewModel)
		{
			var apartmentViewModel = new ApartmentViewModel(apartment);
			if (parentApartmentViewModel != null)
				parentApartmentViewModel.AddChild(apartmentViewModel);

			foreach (var childApartment in apartment.Children)
				AddApartmentInternal(childApartment, apartmentViewModel);
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

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var apartmentDetailsViewModel = new ApartmentDetailsViewModel(new Apartment() { Parent = SelectedApartment.Apartment }, true);
			if (DialogService.ShowModalWindow(apartmentDetailsViewModel))
			{
				DBCash.SaveApartment(apartmentDetailsViewModel.Apartment);

				var apartmentViewModel = new ApartmentViewModel(apartmentDetailsViewModel.Apartment);
				SelectedApartment.AddChild(apartmentViewModel);
				SelectedApartment.IsExpanded = true;
				AllApartments.Add(apartmentViewModel);
				SelectedApartment = apartmentViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedApartment != null;
		}

		public RelayCommand AddFolderCommand { get; private set; }
		void OnAddFolder()
		{
			var apartmentsFolderDetailsViewModel = new ApartmentsFolderDetailsViewModel(new Apartment() { IsFolder = true, Parent = SelectedApartment.Apartment });
			if (DialogService.ShowModalWindow(apartmentsFolderDetailsViewModel))
			{
				DBCash.SaveApartment(apartmentsFolderDetailsViewModel.Apartment);

				var apartmentViewModel = new ApartmentViewModel(apartmentsFolderDetailsViewModel.Apartment);
				SelectedApartment.AddChild(apartmentViewModel);
				SelectedApartment.IsExpanded = true;
				AllApartments.Add(apartmentViewModel);
				SelectedApartment = apartmentViewModel;
			}
		}
		bool CanAddFolder()
		{
			return SelectedApartment != null && SelectedApartment.Apartment.IsFolder;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var apartment = SelectedApartment.Apartment.IsFolder ? 
				SelectedApartment.ApartmentsFolderDetails.Apartment : 
				SelectedApartment.ApartmentDetails.Apartment;
			var dialogViewModel = SelectedApartment.Apartment.IsFolder ?
				(SaveCancelDialogViewModel)new ApartmentsFolderDetailsViewModel(apartment) :
				new ApartmentDetailsViewModel(apartment);
			if (DialogService.ShowModalWindow(dialogViewModel))
			{
				SelectedApartment.Update(apartment);
			}
		}
		bool CanEdit()
		{
			return SelectedApartment != null && DBCash.CurrentUser.UserPermissions.Any(x=> x.PermissionType == PermissionType.EditApartment);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (SelectedApartment.ChildrenCount > 0)
			{
				MessageBoxService.Show(string.Format("Группа \"{0}\" содержит абонентов. Удаление невозможно.", SelectedApartment.Apartment.Name));
				return;
			}

			if (MessageBoxService.ShowQuestion(string.Format("Вы уверенны, что хотите удалить {0} \"{1}\"?", SelectedApartment.Apartment.IsFolder ? "группу" : "абонента", SelectedApartment.Apartment.Name)))
			{
				var selectedApartment = SelectedApartment;
				var parent = selectedApartment.Parent;
				if (parent != null)
				{
					DBCash.DeleteApartment(selectedApartment.Apartment);

					var index = selectedApartment.VisualIndex;
					parent.Nodes.Remove(selectedApartment);
					index = Math.Min(index, parent.ChildrenCount - 1);
					SelectedApartment = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
				}
			}
		}
		
		bool CanRemove()
		{
			return SelectedApartment != null && SelectedApartment.Parent != null;
		}

		public RelayCommand ChangeParentCommand { get; private set; }
		void OnChangeParent()
		{
			var apartmentsFoldersViewModel = new ChangeParentViewModel(SelectedApartment.Apartment.UID);
			if (DialogService.ShowModalWindow(apartmentsFoldersViewModel) && apartmentsFoldersViewModel.SelectedApartment != null)
			{
				var parentApartmentViewModel = AllApartments.FirstOrDefault(x => x.Apartment.UID == apartmentsFoldersViewModel.SelectedApartment.Apartment.UID);
				if (parentApartmentViewModel != null)
				{
					SelectedApartment.Apartment.Parent.Children.RemoveAll(x => x.UID == SelectedApartment.Apartment.UID);
					SelectedApartment.Apartment.Parent = apartmentsFoldersViewModel.SelectedApartment.Apartment;
					SelectedApartment.Apartment.Parent.Children.Add(SelectedApartment.Apartment);

					DBCash.SaveApartment(SelectedApartment.Apartment);

					var apartmentViewModel = SelectedApartment;
					SelectedApartment.Parent.RemoveChild(SelectedApartment);

					parentApartmentViewModel.AddChild(apartmentViewModel);
					apartmentViewModel.ExpandToThis();

					SelectedApartment = apartmentViewModel;
				}
			}
		}

		bool CanChangeParent()
		{
			return SelectedApartment != null && SelectedApartment.Parent != null;
		}

		public bool IsVisibility
		{
			get { return DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.Apartment); }
		}
	}
}