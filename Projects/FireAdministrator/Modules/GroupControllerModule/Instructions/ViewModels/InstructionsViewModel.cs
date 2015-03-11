using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using FiresecAPI.GK;
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
	public class InstructionsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public InstructionsViewModel()
		{
			Menu = new InstructionsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			DeleteAllCommand = new RelayCommand(OnDeleteAll, CanRemoveAll);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Instructions = new ObservableCollection<InstructionViewModel>();
			foreach (var instruction in GKManager.DeviceConfiguration.Instructions)
			{
				if (instruction.InstructionType == GKInstructionType.Details)
				{
					if (instruction.ZoneUIDs == null)
						instruction.ZoneUIDs = new List<Guid>();
					instruction.Devices = new List<Guid>(instruction.Devices.Where(deviceGuid => GKManager.Devices.Any(x => x.UID == deviceGuid)));
					instruction.ZoneUIDs = new List<Guid>(instruction.ZoneUIDs.Where(zoneUID => GKManager.Zones.Any(x => x.UID == zoneUID)));
				}
				Instructions.Add(new InstructionViewModel(instruction));
			}
			SelectedInstruction = Instructions.FirstOrDefault();
		}

		ObservableCollection<InstructionViewModel> _instructions;
		public ObservableCollection<InstructionViewModel> Instructions
		{
			get { return _instructions; }
			set
			{
				_instructions = value;
				OnPropertyChanged(() => Instructions);
			}
		}

		InstructionViewModel _selectedInstruction;
		public InstructionViewModel SelectedInstruction
		{
			get { return _selectedInstruction; }
			set
			{
				_selectedInstruction = value;
				OnPropertyChanged("SelectedInstruction");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var instructionDetailsViewModel = new InstructionDetailsViewModel();
			if (DialogService.ShowModalWindow(instructionDetailsViewModel))
			{
				GKManager.DeviceConfiguration.Instructions.Add(instructionDetailsViewModel.Instruction);
				var instructionViewModel = new InstructionViewModel(instructionDetailsViewModel.Instruction);
				Instructions.Add(instructionViewModel);
				SelectedInstruction = instructionViewModel;
				ServiceFactory.SaveService.GKInstructionsChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var instructionDetailsViewModel = new InstructionDetailsViewModel(SelectedInstruction.Instruction);
			if (DialogService.ShowModalWindow(instructionDetailsViewModel))
			{
				SelectedInstruction.Update();
				ServiceFactory.SaveService.GKInstructionsChanged = true;
			}
		}

		bool CanEditRemove()
		{
			return SelectedInstruction != null;
		}

		bool CanRemoveAll()
		{
			return (Instructions.IsNotNullOrEmpty());
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = Instructions.IndexOf(SelectedInstruction);
			GKManager.DeviceConfiguration.Instructions.Remove(SelectedInstruction.Instruction);
			Instructions.Remove(SelectedInstruction);
			index = Math.Min(index, Instructions.Count - 1);
			if (index > -1)
				SelectedInstruction = Instructions[index];
			ServiceFactory.SaveService.GKInstructionsChanged = true;
		}

		public RelayCommand DeleteAllCommand { get; private set; }
		void OnDeleteAll()
		{
			SelectedInstruction = null;
			Instructions.Clear();
			GKManager.DeviceConfiguration.Instructions.Clear();
			ServiceFactory.SaveService.GKChanged = true;
		}

		#region ISelectable<Guid> Members
		public void Select(Guid instructionUID)
		{
			if (instructionUID != Guid.Empty)
				SelectedInstruction = Instructions.FirstOrDefault(x => x.Instruction.UID == instructionUID);
		}
		#endregion

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}


		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
					new RibbonMenuItemViewModel("Удалить все инструкции", DeleteAllCommand, "BDeleteAll"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}