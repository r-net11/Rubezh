using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace StrazhModule.ViewModels
{
	public class DoorDayIntervalsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public DoorDayIntervalsViewModel()
		{
			Menu = new DoorDayIntervalsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			DayIntervals = new ObservableCollection<DoorDayIntervalViewModel>();
			foreach (var dayInterval in SKDManager.TimeIntervalsConfiguration.DoorDayIntervals)
			{
				var dayIntervalViewModel = new DoorDayIntervalViewModel(dayInterval);
				DayIntervals.Add(dayIntervalViewModel);
			}
			SelectedDayInterval = DayIntervals.FirstOrDefault();
		}

		SKDDoorDayInterval _copiedDayInterval;
		public SKDDoorDayInterval CopiedDayInterval
		{
			get { return _copiedDayInterval; }
			set
			{
				if (_copiedDayInterval == value)
					return;
				_copiedDayInterval = value;
				OnPropertyChanged(() => CopiedDayInterval);
			}
		}

		ObservableCollection<DoorDayIntervalViewModel> _dayIntervals;
		public ObservableCollection<DoorDayIntervalViewModel> DayIntervals
		{
			get { return _dayIntervals; }
			set
			{
				_dayIntervals = value;
				OnPropertyChanged(() => DayIntervals);
			}
		}

		DoorDayIntervalViewModel _selectedDayInterval;
		public DoorDayIntervalViewModel SelectedDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				_selectedDayInterval = value;
				OnPropertyChanged(() => SelectedDayInterval);
				UpdateRibbonItems();
			}
		}

		public void Select(Guid dayIntervalUID)
		{
			if (dayIntervalUID != Guid.Empty)
			{
				var dayIntervalViewModel = DayIntervals.FirstOrDefault(x => x.DayInterval.UID == dayIntervalUID);
				SelectedDayInterval = dayIntervalViewModel;
			}
		}

		bool CanEditRemove()
		{
			return SelectedDayInterval != null && SelectedDayInterval.IsEnabled;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var dayIntervalDetailsViewModel = new DoorDayIntervalDetailsViewModel(DayIntervals);
			if (DialogService.ShowModalWindow(dayIntervalDetailsViewModel))
			{
				SKDManager.TimeIntervalsConfiguration.DoorDayIntervals.Add(dayIntervalDetailsViewModel.DayInterval);
				var dayIntervalViewModel = new DoorDayIntervalViewModel(dayIntervalDetailsViewModel.DayInterval);
				DayIntervals.Add(dayIntervalViewModel);
				SelectedDayInterval = dayIntervalViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (SelectedDayInterval.ConfirmRemoval())
			{
				// Вместо удаляемого дневного графика замка подставляем предустановленный дневной график замка <Карта>
				var predefinedDayIntervalCard =
					SKDManager.TimeIntervalsConfiguration.DoorDayIntervals.FirstOrDefault(
						x => x.Name == TimeIntervalsConfiguration.PredefinedIntervalNameCard);
				foreach (var weeklyInterval in SelectedDayInterval.GetLinkedWeeklyIntervals())
				{
					foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts.Where(part => part.DayIntervalUID == SelectedDayInterval.DayInterval.UID))
					{
						weeklyIntervalPart.DayIntervalUID = predefinedDayIntervalCard == null ? Guid.Empty : predefinedDayIntervalCard.UID;
					}
					
				}

				var index = DayIntervals.IndexOf(SelectedDayInterval);
				SKDManager.TimeIntervalsConfiguration.DoorDayIntervals.Remove(SelectedDayInterval.DayInterval);
				DayIntervals.Remove(SelectedDayInterval);
				index = Math.Min(index, DayIntervals.Count - 1);
				if (index > -1)
					SelectedDayInterval = DayIntervals[index];
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dayIntervalDetailsViewModel = new DoorDayIntervalDetailsViewModel(DayIntervals.Except(new List<DoorDayIntervalViewModel>() { SelectedDayInterval }), SelectedDayInterval.DayInterval);
			if (DialogService.ShowModalWindow(dayIntervalDetailsViewModel))
			{
				SelectedDayInterval.Update(dayIntervalDetailsViewModel.DayInterval);
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			CopiedDayInterval = CopyDayInterval(SelectedDayInterval.DayInterval);
		}
		bool CanCopy()
		{
			return SelectedDayInterval != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var pasteDayInterval = CopyDayInterval(CopiedDayInterval);
			pasteDayInterval.Name = GenerateNewNameBeforePaste(pasteDayInterval.Name);
			SKDManager.TimeIntervalsConfiguration.DoorDayIntervals.Add(pasteDayInterval);
			var dayIntervalViewModel = new DoorDayIntervalViewModel(pasteDayInterval);
			DayIntervals.Add(dayIntervalViewModel);
			SelectedDayInterval = dayIntervalViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanPaste()
		{
			return CopiedDayInterval != null;
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), () =>
			{
				if (SelectedDayInterval != null)
				{
					if (AddCommand.CanExecute(null))
						AddCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), () =>
			{
				if (SelectedDayInterval != null)
				{
					if (DeleteCommand.CanExecute(null))
						DeleteCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), () =>
			{
				if (SelectedDayInterval != null)
				{
					if (EditCommand.CanExecute(null))
						EditCommand.Execute();
				}
			});
		}

		public override void OnShow()
		{
			SelectedDayInterval = SelectedDayInterval;
			base.OnShow();
		}
		public override void OnHide()
		{
			base.OnHide();
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][0].Command = AddCommand;
			RibbonItems[0][1].Command = SelectedDayInterval == null ? null : EditCommand;
			RibbonItems[0][2].Command = SelectedDayInterval == null ? null : DeleteCommand;
		}
		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", "BEdit"),
					new RibbonMenuItemViewModel("Удалить", "BDelete"),
				}, "BEdit") { Order = 1 }
			};
		}

		private SKDDoorDayInterval CopyDayInterval(SKDDoorDayInterval dayInterval)
		{
			var copiedDayInterval = new SKDDoorDayInterval() { Name = dayInterval.Name, Description = dayInterval.Description };
			foreach (var part in dayInterval.DayIntervalParts)
			{
				var copiedDayIntervalPart = new SKDDoorDayIntervalPart()
				{
					StartMilliseconds = part.StartMilliseconds,
					EndMilliseconds = part.EndMilliseconds,
					DoorOpenMethod = part.DoorOpenMethod
				};
				copiedDayInterval.DayIntervalParts.Add(copiedDayIntervalPart);
			}
			return copiedDayInterval;
		}

		private string GenerateNewNameBeforePaste(string name)
		{
			string newName;
			var i = 1;

			do
				newName = String.Format("{0} ({1})", name, i++);
			while (DayIntervals.Any(x => x.Name == newName));

			return newName;
		}
	}
}