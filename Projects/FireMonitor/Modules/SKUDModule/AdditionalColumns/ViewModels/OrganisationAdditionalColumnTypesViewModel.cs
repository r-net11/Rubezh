using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using System.Collections.Generic;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class OrganisationAdditionalColumnTypesViewModel : ViewPartViewModel
	{
		public Organization Organization { get; private set; }

		public OrganisationAdditionalColumnTypesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(Organization organization, List<AdditionalColumnType> additionalColumnTypes)
		{
			Organization = organization;

			AdditionalColumnTypes = new ObservableCollection<AdditionalColumnTypeViewModel>();
			foreach (var additionalColumnType in additionalColumnTypes)
			{
				var additionalColumnViewModel = new AdditionalColumnTypeViewModel(additionalColumnType);
				AdditionalColumnTypes.Add(additionalColumnViewModel);
			}
			SelectedAdditionalColumnType = AdditionalColumnTypes.FirstOrDefault();
		}

		ObservableCollection<AdditionalColumnTypeViewModel> _additionalColumns;
		public ObservableCollection<AdditionalColumnTypeViewModel> AdditionalColumnTypes
		{
			get { return _additionalColumns; }
			set
			{
				_additionalColumns = value;
				OnPropertyChanged(() => AdditionalColumnTypes);
			}
		}

		AdditionalColumnTypeViewModel _selectedAdditionalColumnType;
		public AdditionalColumnTypeViewModel SelectedAdditionalColumnType
		{
			get { return _selectedAdditionalColumnType; }
			set
			{
				_selectedAdditionalColumnType = value;
				OnPropertyChanged(()=>SelectedAdditionalColumnType);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var additionalColumnDetailsViewModel = new AdditionalColumnTypeDetailsViewModel(this);
			if (DialogService.ShowModalWindow(additionalColumnDetailsViewModel))
			{
				var additionalColumnType = additionalColumnDetailsViewModel.AdditionalColumnType;
				var saveResult = AdditionalColumnTypeHelper.Save(additionalColumnType);
				if (!saveResult)
					return;
				var additionalColumnViewModel = new AdditionalColumnTypeViewModel(additionalColumnType);
				AdditionalColumnTypes.Add(additionalColumnViewModel);
				SelectedAdditionalColumnType = additionalColumnViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var additionalColumnType = SelectedAdditionalColumnType.AdditionalColumnType;
			var removeResult = AdditionalColumnTypeHelper.MarkDeleted(additionalColumnType);
			if (!removeResult)
				return;
			var index = AdditionalColumnTypes.IndexOf(SelectedAdditionalColumnType);
			AdditionalColumnTypes.Remove(SelectedAdditionalColumnType);
			index = Math.Min(index, AdditionalColumnTypes.Count - 1);
			if (index > -1)
				SelectedAdditionalColumnType = AdditionalColumnTypes[index];
		}
		bool CanRemove()
		{
			return SelectedAdditionalColumnType != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var additionalColumnDetailsViewModel = new AdditionalColumnTypeDetailsViewModel(this, SelectedAdditionalColumnType.AdditionalColumnType);
			if (DialogService.ShowModalWindow(additionalColumnDetailsViewModel))
			{
				var additionalColumnType = additionalColumnDetailsViewModel.AdditionalColumnType;
				var saveResult = AdditionalColumnTypeHelper.Save(additionalColumnType);
				if (!saveResult)
					return;
				SelectedAdditionalColumnType.Update(additionalColumnType);
			}
		}
		bool CanEdit()
		{
			return SelectedAdditionalColumnType != null;
		}
	}
}