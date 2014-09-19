using System;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ChiefViewModel : BaseViewModel
	{
		EmployeeFilter _filter;

		public ChiefViewModel(Guid chiefUID, EmployeeFilter filter)
		{
			SelectChiefCommand = new RelayCommand(OnSelectChief);
			Chief = EmployeeHelper.GetSingleShort(chiefUID);
			_filter = filter;
		}

		ShortEmployee _chief;
		public ShortEmployee Chief
		{
			get { return _chief; }
			set
			{
				_chief = value;
				OnPropertyChanged(() => Chief);
				OnPropertyChanged(() => HasChief);
			}
		}
		public bool HasChief
		{
			get { return Chief != null; }
		}
		public Guid ChiefUID
		{
			get { return HasChief ? Chief.UID : Guid.Empty; }
		}

		public RelayCommand SelectChiefCommand { get; private set; }
		void OnSelectChief()
		{
			var positionSelectionViewModel = new EmployeeSelectionViewModel(HasChief ? Chief.UID : Guid.Empty, _filter);
			if (DialogService.ShowModalWindow(positionSelectionViewModel))
			{
				Chief = positionSelectionViewModel.SelectedEmployee;
			}
		}
	}
}
