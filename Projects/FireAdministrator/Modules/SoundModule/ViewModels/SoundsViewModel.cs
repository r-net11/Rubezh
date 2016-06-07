using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Infrastructure;
using Infrastructure.Common.Ribbon;
using Infrastructure.ViewModels;

namespace SoundsModule.ViewModels
{
	public class SoundsViewModel : MenuViewPartViewModel
	{
		public SoundsAssignmentViewModel SoundsAssignmentViewModel { get; private set; }

		public SoundFilesViewModel SoundFilesViewModel { get; private set; }

		private int _selectedTabIndex;

		public int SelectedTabIndex
		{
			get { return _selectedTabIndex; }
			set
			{
				if (_selectedTabIndex == value)
					return;
				_selectedTabIndex = value;
				InvalidateMenuAndRibbonItems(_selectedTabIndex == 1);
				OnPropertyChanged(() => SelectedTabIndex);
			}
		}

		public SoundsViewModel()
		{
			SoundsAssignmentViewModel = new SoundsAssignmentViewModel();
			SoundFilesViewModel = new SoundFilesViewModel();

			AddCommand = SoundFilesViewModel.AddCommand;
			EditCommand = SoundFilesViewModel.EditCommand;
			DeleteCommand = SoundFilesViewModel.DeleteCommand;
			PlaySoundCommand = SoundFilesViewModel.PlaySoundCommand;

			SelectedTabIndex = 0;
		}

		public ICommand AddCommand { get; private set; }

		public ICommand EditCommand { get; private set; }

		public ICommand DeleteCommand { get; private set; }
		public ICommand PlaySoundCommand { get; private set; }

		private void CreateMenuAndRibbonItems()
		{
			if (Menu == null)
				Menu = new SoundsMenuViewModel(this);
			if (RibbonItems == null)
				RibbonItems = new List<RibbonMenuItemViewModel>
				{
					new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
					{
						new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
						new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
						new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
						new RibbonMenuItemViewModel("Воспроизвести", PlaySoundCommand, "BPlay"),
					}, "BEdit") { Order = 2 }
				};
		}

		private void InvalidateMenuAndRibbonItems(bool show)
		{
			CreateMenuAndRibbonItems();
			if (show)
			{
				ServiceFactory.MenuService.Show(Menu);
				UpdateRibbonItems();
				ServiceFactory.RibbonService.AddRibbonItems(RibbonItems);
			}
			else
			{
				ServiceFactory.MenuService.Show(null);
				ServiceFactory.RibbonService.RemoveRibbonItems(RibbonItems);
			}
		}
	}
}