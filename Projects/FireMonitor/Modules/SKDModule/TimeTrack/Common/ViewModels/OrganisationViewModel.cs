using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public abstract class OrganisationViewModel<TViewModel, TElement> : ViewPartViewModel, ISelectable<Guid>
		where TViewModel : BaseObjectViewModel<TElement>
		where TElement : OrganisationElementBase
	{
		public Organisation Organisation { get; private set; }

		public OrganisationViewModel(Organisation organisation)
		{
			Organisation = organisation;
		}

		public virtual void Initialize(IEnumerable<TElement> models)
		{
			ViewModels = new SortableObservableCollection<TViewModel>();
			foreach (var model in models)
			{
				var viewModel = CreateViewModel(model);
				ViewModels.Add(viewModel);
			}
			SelectedViewModel = ViewModels.FirstOrDefault();
		}
		protected abstract TViewModel CreateViewModel(TElement model);

		private SortableObservableCollection<TViewModel> _viewModels;
		public SortableObservableCollection<TViewModel> ViewModels
		{
			get { return _viewModels; }
			set
			{
				_viewModels = value;
				OnPropertyChanged(() => ViewModels);
			}
		}

		private TViewModel _selectedViewModel;
		public TViewModel SelectedViewModel
		{
			get { return _selectedViewModel; }
			set
			{
				_selectedViewModel = value;
				OnPropertyChanged(() => SelectedViewModel);
			}
		}

		public void Select(Guid modelUID)
		{
			if (modelUID != Guid.Empty)
			{
				var viewModel = ViewModels.FirstOrDefault(x => x.Model.UID == modelUID);
				if (viewModel != null)
					SelectedViewModel = viewModel;
			}
		}

		public RelayCommand AddCommand { get; protected set; }
		public RelayCommand DeleteCommand { get; protected set; }
		public RelayCommand EditCommand { get; protected set; }
	}
}
