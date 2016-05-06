using System.Linq;
using Common;
using EntitiesValidation;
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
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				var dayIntervalPart = dayIntervalPartDetailsViewModel.DayIntervalPart;
				Model.DayIntervalParts.Add(dayIntervalPart);
				var dayIntervalPartViewModel = new DayIntervalPartViewModel(dayIntervalPart);
				DayIntervalParts.Add(dayIntervalPartViewModel);
				DayIntervalParts.Sort(item => item.BeginTime);
				SelectedDayIntervalPart = dayIntervalPartViewModel;
			}
		}
		bool CanAdd()
		{
			return !IsOrganisation && !IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit);
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var validationResult = DayIntervalPartValidator.ValidateGeneralDayIntervalPartsLengthOnEditingOrDeleting(Model.DayIntervalParts.Where(dayIntervalPart => dayIntervalPart.UID != SelectedDayIntervalPart.DayIntervalPart.UID), Model.SlideTime);
			if (validationResult.HasError)
			{
				MessageBoxService.ShowWarning(validationResult.Error);
				return;
			}
			
			if (!DayIntervalPartHelper.Remove(SelectedDayIntervalPart.DayIntervalPart, Model.Name)) return;

			Model.DayIntervalParts.RemoveAll(x => x.UID == SelectedDayIntervalPart.DayIntervalPart.UID);
			DayIntervalParts.Remove(SelectedDayIntervalPart);
		}
		bool CanDelete()
		{
			return SelectedDayIntervalPart != null && !IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dayIntervalPartDetailsViewModel = new DayIntervalPartDetailsViewModel(Model, SelectedDayIntervalPart.DayIntervalPart);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				DayIntervalPartHelper.Save(SelectedDayIntervalPart.DayIntervalPart, false, Model.Name);
				Model.DayIntervalParts.RemoveAll(x => x.UID == SelectedDayIntervalPart.DayIntervalPart.UID);
				Model.DayIntervalParts.Add(SelectedDayIntervalPart.DayIntervalPart);
				SelectedDayIntervalPart.Update();
				var selectedDayIntervalPart = SelectedDayIntervalPart;
				DayIntervalParts.Sort(item => item.BeginTime);
				SelectedDayIntervalPart = selectedDayIntervalPart;
			}
		}
		bool CanEdit()
		{
			return SelectedDayIntervalPart != null && !IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit);
		}

		public override void Update()
		{
			base.Update();
			if(!IsOrganisation)
				ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Publish(Model.UID);
		}
	}
}