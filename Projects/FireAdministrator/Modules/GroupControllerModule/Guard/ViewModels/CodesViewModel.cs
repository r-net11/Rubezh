using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Ribbon;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using RubezhAPI.GK;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class CodesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public CodesViewModel()
		{
			Menu = new GuardMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Codes = new ObservableCollection<CodeViewModel>();
			foreach (var code in GKManager.DeviceConfiguration.Codes.OrderBy(x => x.No))
			{
				var codeViewModel = new CodeViewModel(code);
				Codes.Add(codeViewModel);
			}
			SelectedCode = Codes.FirstOrDefault();
		}

		ObservableCollection<CodeViewModel> _codes;
		public ObservableCollection<CodeViewModel> Codes
		{
			get { return _codes; }
			set
			{
				_codes = value;
				OnPropertyChanged(() => Codes);
			}
		}

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
			OnAddResult();
		}

		GKCode OnAddResult()
		{
			var codeDetailsViewModel = new CodeDetailsViewModel();
			if (DialogService.ShowModalWindow(codeDetailsViewModel))
			{
				GKManager.DeviceConfiguration.Codes.Add(codeDetailsViewModel.Code);
				var codeViewModel = new CodeViewModel(codeDetailsViewModel.Code);
				Codes.Add(codeViewModel);
				SelectedCode = codeViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				return codeDetailsViewModel.Code;
			}
			return null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var codeDetailsViewModel = new CodeDetailsViewModel(SelectedCode.Code);
			if (DialogService.ShowModalWindow(codeDetailsViewModel))
			{
				SelectedCode.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return (SelectedCode != null);
		}

		public void CreateCode(CreateGKCodeEventArg createGKCodeEventArg)
		{
			createGKCodeEventArg.Code = OnAddResult();
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить код " + SelectedCode.Code.PresentationName + "?"))
			{
				int oldIndex = Codes.IndexOf(SelectedCode);

				GKManager.DeviceConfiguration.Codes.Remove(SelectedCode.Code);
				Codes.Remove(SelectedCode);
				SelectedCode = Codes.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;

				if (Codes.Count > 0)
					SelectedCode = Codes[System.Math.Min(oldIndex, Codes.Count - 1)];
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все неиспользуемые коды ?"))
			{
				var emptyCodes = Codes.Where(x => !GetOnEmpty().Contains(x.Code.UID));
				if (emptyCodes != null)
				{
					for (var i = emptyCodes.Count() - 1; i >= 0; i--)
					{
						GKManager.DeviceConfiguration.Codes.Remove(emptyCodes.ElementAt(i).Code);
						Codes.Remove(emptyCodes.ElementAt(i));
					}
					ServiceFactory.SaveService.GKChanged = true;
					SelectedCode = Codes.FirstOrDefault();
				}	
			}
		}

		bool CanDeleteAllEmpty()
		{
			return GKManager.DeviceConfiguration.Codes.Any(x => !GetOnEmpty().Contains(x.UID));
		}

		private HashSet<Guid> GetOnEmpty()
		{
			HashSet<Guid> codes = new HashSet<Guid>();
			foreach (var guardZone in GKManager.DeviceConfiguration.GuardZones)
			{
				var guardZoneDevices = guardZone.GuardZoneDevices.Where(x => x.Device.Driver.IsCardReaderOrCodeReader);
				foreach (var code in GKManager.DeviceConfiguration.Codes)
				{
					if (guardZoneDevices.Any(y => y.CodeReaderSettings.AlarmSettings.CodeUIDs.Contains(code.UID) || y.CodeReaderSettings.ChangeGuardSettings.CodeUIDs.Contains(code.UID) ||
						y.CodeReaderSettings.ResetGuardSettings.CodeUIDs.Contains(code.UID) || y.CodeReaderSettings.SetGuardSettings.CodeUIDs.Contains(code.UID)))
					{
						codes.Add(code.UID);
					}
				}
			}

			foreach (var MPT in GKManager.DeviceConfiguration.MPTs)
			{
				var MPTDevices = MPT.MPTDevices.Where(x => x.Device.Driver.IsCardReaderOrCodeReader);
				foreach (var code in GKManager.DeviceConfiguration.Codes)
				{
					if (MPTDevices.Any(y => y.CodeReaderSettings.MPTSettings.CodeUIDs.Contains(code.UID)))
					{
						codes.Add(code.UID);
					}
				}
			}

			return codes;
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
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
					new RibbonMenuItemViewModel("Удалить все пустые коды", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 2 }
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