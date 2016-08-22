using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Localization.Sounds.ViewModels;
using Localization.Sounds.Common;
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
			SoundFilesViewModel = new SoundFilesViewModel();
			SoundsAssignmentViewModel = new SoundsAssignmentViewModel(SoundFilesViewModel.Sounds);

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
					new RibbonMenuItemViewModel(CommonViewModels.Edition, new ObservableCollection<RibbonMenuItemViewModel>()
					{
						new RibbonMenuItemViewModel(CommonResources.Add, AddCommand, "BAdd"),
						new RibbonMenuItemViewModel(CommonResources.Edit, EditCommand, "BEdit"),
						new RibbonMenuItemViewModel(CommonResources.Delete, DeleteCommand, "BDelete"),
						new RibbonMenuItemViewModel(CommonResources.Play, PlaySoundCommand, "BPlay"),
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

		public override void OnShow()
		{
			base.OnShow();
			InvalidateMenuAndRibbonItems(SelectedTabIndex == 1);
		}
	}
}