﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace InstructionsModule.ViewModels
{
	public class InstructionDetailsViewModel : SaveCancelDialogViewModel
	{
		public Instruction Instruction { get; private set; }

		public InstructionDetailsViewModel(Instruction instruction = null)
		{
			SelectZoneCommand = new RelayCommand(OnSelectZoneCommand, CanSelect);
			SelectDeviceCommand = new RelayCommand(OnSelectDeviceCommand, CanSelect);

			if (instruction != null)
			{
				Title = "Редактирование инструкции";
				Instruction = instruction;
				CopyProperties();
			}
			else
			{
				Title = "Новая инструкция";
				Instruction = new Instruction();
				CopyProperties();
			}
		}

		void CopyProperties()
		{
			InstructionZones = new ObservableCollection<Guid>();
			InstructionDevices = new ObservableCollection<Guid>();

			Name = Instruction.Name;
			Text = Instruction.Text;
			StateType = Instruction.StateType;
			InstructionType = Instruction.InstructionType;
			switch (InstructionType)
			{
				case InstructionType.Details:
					if (Instruction.ZoneUIDs.IsNotNullOrEmpty())
						InstructionZones = new ObservableCollection<Guid>(Instruction.ZoneUIDs);
					if (Instruction.Devices.IsNotNullOrEmpty())
						InstructionDevices = new ObservableCollection<Guid>(Instruction.Devices);
					break;

				case InstructionType.General:
					break;
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		public bool IsDetails
		{
			get { return InstructionType == InstructionType.Details; }
		}

		StateType _stateType;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				OnPropertyChanged("StateTypeFilter");
			}
		}
		public List<StateType> AvailableStates
		{
			get
			{
				var stateTypes = new List<StateType>();
				stateTypes.Add(StateType.Fire);
				stateTypes.Add(StateType.Attention);
				stateTypes.Add(StateType.Failure);
				stateTypes.Add(StateType.Service);
				stateTypes.Add(StateType.Off);
				stateTypes.Add(StateType.Info);
				return stateTypes;
			}
		}

		InstructionType _instructionType;
		public InstructionType InstructionType
		{
			get { return _instructionType; }
			set
			{
				_instructionType = value;
				OnPropertyChanged("InstructionType");
				OnPropertyChanged("IsDetails");
			}
		}
		public List<InstructionType> AvailableInstructionsType
		{
			get { return new List<InstructionType>(Enum.GetValues(typeof(InstructionType)).OfType<InstructionType>()); }
		}

		ObservableCollection<Guid> _instructionZones;
		public ObservableCollection<Guid> InstructionZones
		{
			get { return _instructionZones; }
			set
			{
				_instructionZones = value;
				OnPropertyChanged("InstructionZones");
			}
		}

		ObservableCollection<Guid> _instructionDevices;
		public ObservableCollection<Guid> InstructionDevices
		{
			get { return _instructionDevices; }
			set
			{
				_instructionDevices = value;
				OnPropertyChanged("InstructionDevices");
			}
		}

		bool CanSelect()
		{
			return (InstructionType != InstructionType.General);
		}

		public RelayCommand SelectZoneCommand { get; private set; }
		void OnSelectZoneCommand()
		{
			var instructionZonesViewModel = new InstructionZonesViewModel(InstructionZones.ToList());
			if (DialogService.ShowModalWindow(instructionZonesViewModel))
			{
				InstructionZones = new ObservableCollection<Guid>(instructionZonesViewModel.InstructionZonesList);
			}
		}

		public RelayCommand SelectDeviceCommand { get; private set; }
		void OnSelectDeviceCommand()
		{
			var instructionDevicesViewModel = new InstructionDevicesViewModel(InstructionDevices.ToList());
			if (DialogService.ShowModalWindow(instructionDevicesViewModel))
			{
				InstructionDevices = new ObservableCollection<Guid>(instructionDevicesViewModel.InstructionDevicesList);
			}
		}

		protected override bool CanSave()
		{
			if (string.IsNullOrWhiteSpace(Text))
				return false;
			else
				return InstructionType == InstructionType.General ? true : (InstructionDevices.IsNotNullOrEmpty() || InstructionZones.IsNotNullOrEmpty());
		}

		protected override bool Save()
		{
			Instruction.Name = Name;
			Instruction.Text = Text;
			Instruction.StateType = StateType;
			Instruction.InstructionType = InstructionType;
			if (InstructionType == InstructionType.Details)
			{
				Instruction.Devices = InstructionDevices.ToList();
				Instruction.ZoneUIDs = InstructionZones.ToList();
			}
			else
			{
				Instruction.Devices = new List<Guid>();
                Instruction.ZoneUIDs = new List<Guid>();
			}
			return base.Save();
		}
	}
}