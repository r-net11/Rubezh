using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DayIntervalViewModel : OrganisationElementViewModel<DayIntervalViewModel, DayInterval>, IEditingViewModel
	{
		bool _isInitialized;
		
		public DayIntervalViewModel(): base()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			_isInitialized = false;
		}
		
		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (!IsOrganisation)
				{
					DayIntervalParts = new SortableObservableCollection<DayIntervalPartViewModel>();
					foreach (var dayIntervalPart in Model.DayIntervalParts)
					{
						var dayIntervalPartViewModel = new DayIntervalPartViewModel(dayIntervalPart);
						DayIntervalParts.Add(dayIntervalPartViewModel);
					}
					SelectedDayIntervalPart = DayIntervalParts.FirstOrDefault();
				}
				OnPropertyChanged(() => DayIntervalParts);
			}
		}
		
		public SortableObservableCollection<DayIntervalPartViewModel> DayIntervalParts { get; private set; }

		DayIntervalPartViewModel _selectedDayIntervalPart;
		public DayIntervalPartViewModel SelectedDayIntervalPart
		{
			get { return _selectedDayIntervalPart; }
			set
			{
				_selectedDayIntervalPart = value;
				OnPropertyChanged(() => SelectedDayIntervalPart);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var dayIntervalPartDetailsViewModel = new DayIntervalPartDetailsViewModel(Model);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel) && AddSave(dayIntervalPartDetailsViewModel.DayIntervalPart))
			{
				var dayIntervalPart = dayIntervalPartDetailsViewModel.DayIntervalPart;
				var dayIntervalPartViewModel = new DayIntervalPartViewModel(dayIntervalPart);
				DayIntervalParts.Add(dayIntervalPartViewModel);
				DayIntervalParts.Sort(item => item.BeginTime);
				SelectedDayIntervalPart = dayIntervalPartViewModel;
			}
		}
		bool CanAdd()
		{
			return !IsOrganisation && !IsDeleted && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit);
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (DeleteSave(SelectedDayIntervalPart.DayIntervalPart))
			{
				Model.DayIntervalParts.Remove(SelectedDayIntervalPart.DayIntervalPart);
				DayIntervalParts.Remove(SelectedDayIntervalPart);
			}
		}
		bool CanDelete()
		{
			return SelectedDayIntervalPart != null && DayIntervalParts.Count > 1 && !IsDeleted && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dayIntervalPartDetailsViewModel = new DayIntervalPartDetailsViewModel(Model, SelectedDayIntervalPart.DayIntervalPart);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				EditSave(SelectedDayIntervalPart.DayIntervalPart);
				SelectedDayIntervalPart.Update();
				var selectedDayIntervalPart = SelectedDayIntervalPart;
				DayIntervalParts.Sort(item => item.BeginTime);
				SelectedDayIntervalPart = selectedDayIntervalPart;
			}
		}
		bool CanEdit()
		{
			return SelectedDayIntervalPart != null && !IsDeleted && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit);
		}

		bool AddSave(DayIntervalPart dayIntervalPart)
		{
			Model.DayIntervalParts.Add(dayIntervalPart);
			return DayIntervalHelper.Save(Model, false);
		}

		bool EditSave(DayIntervalPart dayIntervalPart)
		{
			Model.DayIntervalParts.RemoveAll(x => x.UID == dayIntervalPart.UID);
			Model.DayIntervalParts.Add(dayIntervalPart);
			return DayIntervalHelper.Save(Model, false);
		}

		bool DeleteSave(DayIntervalPart dayIntervalPart)
		{
			Model.DayIntervalParts.RemoveAll(x => x.UID == dayIntervalPart.UID);
			return DayIntervalHelper.Save(Model, false);
		}

		public override void Update()
		{
			base.Update();
			if(!IsOrganisation)
				ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Publish(Model.UID);
		}
	}
}