using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace AutomationModule.ViewModels
{
	public class MasksViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public MasksViewModel()
		{
			Menu = new MasksMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
		}

		public void Initialize()
		{
			Masks = new ObservableCollection<MaskViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.Masks == null)
				FiresecClient.FiresecManager.SystemConfiguration.Masks = new List<Mask>();
			foreach (var Mask in FiresecClient.FiresecManager.SystemConfiguration.Masks)
			{
				var maskViewModel = new MaskViewModel(Mask);
				Masks.Add(maskViewModel);
			}
			SelectedMask = Masks.FirstOrDefault();
		}

		ObservableCollection<MaskViewModel> _masks;
		public ObservableCollection<MaskViewModel> Masks
		{
			get { return _masks; }
			set
			{
				_masks = value;
				OnPropertyChanged("Masks");
			}
		}

		MaskViewModel _selectedMask;
		public MaskViewModel SelectedMask
		{
			get { return _selectedMask; }
			set
			{
				_selectedMask = value;
				OnPropertyChanged("SelectedMask");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var maskDetailsViewModel = new MaskDetailsViewModel();
			if (DialogService.ShowModalWindow(maskDetailsViewModel))
			{
				FiresecClient.FiresecManager.SystemConfiguration.Masks.Add(maskDetailsViewModel.Mask);
				ServiceFactory.SaveService.AutomationChanged = true;
				var maskViewModel = new MaskViewModel(maskDetailsViewModel.Mask);
				Masks.Add(maskViewModel);
				SelectedMask = maskViewModel;
			}
		}

		bool CanAdd()
		{
			return true;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			FiresecClient.FiresecManager.SystemConfiguration.Masks.Remove(SelectedMask.Mask);
			Masks.Remove(SelectedMask);
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var maskDetailsViewModel = new MaskDetailsViewModel(SelectedMask.Mask);
			if (DialogService.ShowModalWindow(maskDetailsViewModel))
			{
				SelectedMask.Update(maskDetailsViewModel.Mask);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedMask != null;
		}

		public void Select(Guid maskUid)
		{
			if (maskUid != Guid.Empty)
			{
				SelectedMask = Masks.FirstOrDefault(item => item.Mask.Uid == maskUid);
			}
		}
	}
}
