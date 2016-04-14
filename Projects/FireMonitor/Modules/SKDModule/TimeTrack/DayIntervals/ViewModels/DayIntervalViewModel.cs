using System.Linq;
using Common;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DayIntervalViewModel : OrganisationElementViewModel<DayIntervalViewModel, DayInterval>
	{
		bool _isInitialized;

		public DayIntervalViewModel()
			: base()
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
					DayIntervalParts.Sort(item => item.BeginTime);
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
			var dayIntervalPartDetailsViewModel = new DayIntervalPartDetailsViewModel(Model, OrganisationUID);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				var dayIntervalPart = dayIntervalPartDetailsViewModel.DayIntervalPart;
				var selected = dayIntervalPart.BeginTime;
				Model.DayIntervalParts.Add(dayIntervalPart);
				Sort();
				DayIntervalHelper.Save(Model, false);
				SelectedDayIntervalPart = DayIntervalParts.FirstOrDefault(item => item.BeginTime == selected);
			}
		}
		bool CanAdd()
		{
			return !IsOrganisation && !IsDeleted && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit);
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dayIntervalPart = SelectedDayIntervalPart.DayIntervalPart;
			Model.DayIntervalParts.RemoveAll(x => x.UID == dayIntervalPart.UID);
			Sort();
			DayIntervalHelper.Save(Model, false);
			SelectedDayIntervalPart = DayIntervalParts.FirstOrDefault();
		}
		bool CanDelete()
		{
			return SelectedDayIntervalPart != null && !IsDeleted && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dayIntervalPartDetailsViewModel = new DayIntervalPartDetailsViewModel(Model, OrganisationUID, SelectedDayIntervalPart.DayIntervalPart);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				var dayIntervalPart = SelectedDayIntervalPart.DayIntervalPart;
				var selected = dayIntervalPart.BeginTime;
				var interval = Model.DayIntervalParts.FirstOrDefault(x => x.UID == dayIntervalPart.UID);
				interval = dayIntervalPart;
				Sort();
				DayIntervalHelper.Save(Model, false);
				SelectedDayIntervalPart = DayIntervalParts.FirstOrDefault(item => item.BeginTime == selected);
			}
		}
		bool CanEdit()
		{
			return SelectedDayIntervalPart != null && !IsDeleted && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit);
		}
		public override void Update()
		{
			base.Update();
			if (!IsOrganisation)
				ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Publish(Model.UID);
		}

		void Sort()
		{
			int i = 1;
			DayIntervalParts = new SortableObservableCollection<DayIntervalPartViewModel>();
			Model.DayIntervalParts = Model.DayIntervalParts.OrderBy(item => item.BeginTime).ToList();
			foreach (var item in Model.DayIntervalParts)
			{
				item.Number = i;
				var dayIntervalPartViewModel = new DayIntervalPartViewModel(item);
				DayIntervalParts.Add(dayIntervalPartViewModel);
				i++;
			}
			OnPropertyChanged(() => DayIntervalParts);
		}
}
}