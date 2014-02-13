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
	public class AdditionalColumnsViewModel : ViewPartViewModel
	{
		public AdditionalColumnsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		void Initialize()
		{
			AdditionalColumns = new ObservableCollection<AdditionalColumnViewModel>();
			//var additionalColumns = FiresecManager.GetPositions(null);
			var additionalColumns = new List<AdditionalColumn>();
			foreach (var additionalColumn in additionalColumns)
			{
				var additionalColumnViewModel = new AdditionalColumnViewModel(additionalColumn);
				AdditionalColumns.Add(additionalColumnViewModel);
			}
			SelectedAdditionalColumn = AdditionalColumns.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<AdditionalColumnViewModel> _additionalColumns;
		public ObservableCollection<AdditionalColumnViewModel> AdditionalColumns
		{
			get { return _additionalColumns; }
			set
			{
				_additionalColumns = value;
				OnPropertyChanged("AdditionalColumns");
			}
		}

		AdditionalColumnViewModel _selectedAdditionalColumn;
		public AdditionalColumnViewModel SelectedAdditionalColumn
		{
			get { return _selectedAdditionalColumn; }
			set
			{
				_selectedAdditionalColumn = value;
				OnPropertyChanged("SelectedAdditionalColumn");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var additionalColumnDetailsViewModel = new AdditionalColumnDetailsViewModel(this);
			if (DialogService.ShowModalWindow(additionalColumnDetailsViewModel))
			{
				var additionalColumn = additionalColumnDetailsViewModel.AdditionalColumn;
				var additionalColumnViewModel = new AdditionalColumnViewModel(additionalColumn);
				AdditionalColumns.Add(additionalColumnViewModel);
				SelectedAdditionalColumn = additionalColumnViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = AdditionalColumns.IndexOf(SelectedAdditionalColumn);
			AdditionalColumns.Remove(SelectedAdditionalColumn);
			index = Math.Min(index, AdditionalColumns.Count - 1);
			if (index > -1)
				SelectedAdditionalColumn = AdditionalColumns[index];
		}
		bool CanRemove()
		{
			return SelectedAdditionalColumn != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var additionalColumnDetailsViewModel = new AdditionalColumnDetailsViewModel(this, SelectedAdditionalColumn.AdditionalColumn);
			if (DialogService.ShowModalWindow(additionalColumnDetailsViewModel))
			{
				SelectedAdditionalColumn.Update(additionalColumnDetailsViewModel.AdditionalColumn);
			}
		}
		bool CanEdit()
		{
			return SelectedAdditionalColumn != null;
		}
	}
}