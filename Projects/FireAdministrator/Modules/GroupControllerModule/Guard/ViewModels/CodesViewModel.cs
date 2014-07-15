using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class CodesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public CodesViewModel()
		{
			Menu = new GuardMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Codes = new ObservableCollection<CodeViewModel>();
			foreach (var code in XManager.DeviceConfiguration.Codes)
			{
				var codeViewModel = new CodeViewModel(code);
				Codes.Add(codeViewModel);
			}
			SelectedCode = Codes.FirstOrDefault();
		}

		public ObservableCollection<CodeViewModel> Codes { get; private set; }

		CodeViewModel _selectedCode;
		public CodeViewModel SelectedCode
		{
			get { return _selectedCode; }
			set
			{
				_selectedCode = value;
				OnPropertyChanged(() => SelectedCode);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var codeDetailsViewModel = new CodeDetailsViewModel();
			if (DialogService.ShowModalWindow(codeDetailsViewModel))
			{
				XManager.DeviceConfiguration.Codes.Add(codeDetailsViewModel.Code);
				var codeViewModel = new CodeViewModel(codeDetailsViewModel.Code);
				Codes.Add(codeViewModel);
				SelectedCode = codeViewModel;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			int oldIndex = Codes.IndexOf(SelectedCode);

			XManager.DeviceConfiguration.Codes.Remove(SelectedCode.Code);
			Codes.Remove(SelectedCode);
			SelectedCode = Codes.FirstOrDefault();
			ServiceFactory.SaveService.GKChanged = true;

			if (Codes.Count > 0)
				SelectedCode = Codes[System.Math.Min(oldIndex, Codes.Count - 1)];
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var codeDetailsViewModel = new CodeDetailsViewModel(SelectedCode.Code);
			if (DialogService.ShowModalWindow(codeDetailsViewModel))
			{
				SelectedCode.Code = codeDetailsViewModel.Code;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return (SelectedCode != null);
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 2 }
			};
		}

		#region ISelectable<Guid> Members
		public void Select(Guid codeUID)
		{
			if (codeUID != Guid.Empty)
				SelectedCode = Codes.FirstOrDefault(x => x.Code.UID == codeUID);
		}
		#endregion
	}
}