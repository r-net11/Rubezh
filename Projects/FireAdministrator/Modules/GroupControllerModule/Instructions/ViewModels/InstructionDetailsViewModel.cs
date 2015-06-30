using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;

namespace GKModule.ViewModels
{
	public class InstructionDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKInstruction Instruction { get; private set; }

		public InstructionDetailsViewModel(GKInstruction instruction = null)
		{
			SelectZoneCommand = new RelayCommand(OnSelectZoneCommand, CanSelect);
			SelectDeviceCommand = new RelayCommand(OnSelectDeviceCommand, CanSelect);
			SelectDirectionCommand = new RelayCommand(OnSelectDirectionCommand, CanSelect);
			GetMediaCommand = new RelayCommand(OnGetMedia);
			RemoveMediaCommand = new RelayCommand(OnRemoveMedia);
			if (instruction != null)
			{
				Title = "Редактирование инструкции";
				Instruction = instruction;
				CopyProperties();
			}
			else
			{
				Title = "Новая инструкция";
				Instruction = new GKInstruction();
				CopyProperties();
			}
		}

		void CopyProperties()
		{
			InstructionZones = new ObservableCollection<Guid>();
			InstructionDevices = new ObservableCollection<Guid>();
			InstructionDirections = new ObservableCollection<Guid>();
			Name = Instruction.Name;
			Text = Instruction.Text;
			AlarmType = Instruction.AlarmType;
			InstructionType = Instruction.InstructionType;
			switch (InstructionType)
			{
				case GKInstructionType.Details:
					if (Instruction.ZoneUIDs.IsNotNullOrEmpty())
						InstructionZones = new ObservableCollection<Guid>(Instruction.ZoneUIDs);
					if (Instruction.Devices.IsNotNullOrEmpty())
						InstructionDevices = new ObservableCollection<Guid>(Instruction.Devices);
					if (Instruction.Directions.IsNotNullOrEmpty())
						InstructionDirections = new ObservableCollection<Guid>(Instruction.Directions);
					break;

				case GKInstructionType.General:
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
				OnPropertyChanged(() => Name);
			}
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		public bool IsDetails
		{
			get { return InstructionType == GKInstructionType.Details; }
		}

		GKAlarmType _alarmType;
		public GKAlarmType AlarmType
		{
			get { return _alarmType; }
			set
			{
				_alarmType = value;
				OnPropertyChanged(() => AlarmType);
			}
		}
		public List<GKAlarmType> AvailableAlarmTypes
		{
			get { return Enum.GetValues(typeof(GKAlarmType)).Cast<GKAlarmType>().ToList(); }
		}

		GKInstructionType _instructionType;
		public GKInstructionType InstructionType
		{
			get { return _instructionType; }
			set
			{
				_instructionType = value;
				OnPropertyChanged(() => InstructionType);
				OnPropertyChanged("IsDetails");
			}
		}
		public List<GKInstructionType> AvailableInstructionsType
		{
			get { return new List<GKInstructionType>(Enum.GetValues(typeof(GKInstructionType)).OfType<GKInstructionType>()); }
		}

		ObservableCollection<Guid> _instructionZones;
		public ObservableCollection<Guid> InstructionZones
		{
			get { return _instructionZones; }
			set
			{
				_instructionZones = value;
				OnPropertyChanged(() => InstructionZones);
			}
		}

		ObservableCollection<Guid> _instructionDevices;
		public ObservableCollection<Guid> InstructionDevices
		{
			get { return _instructionDevices; }
			set
			{
				_instructionDevices = value;
				OnPropertyChanged(() => InstructionDevices);
			}
		}

		ObservableCollection<Guid> _instructionDirections;
		public ObservableCollection<Guid> InstructionDirections
		{
			get { return _instructionDirections; }
			set
			{
				_instructionDirections = value;
				OnPropertyChanged("InstructionDirections");
			}
		}

		bool CanSelect()
		{
			return (InstructionType != GKInstructionType.General);
		}

		public RelayCommand SelectZoneCommand { get; private set; }
		void OnSelectZoneCommand()
		{
			var zones = new List<GKZone>();
			foreach (var uid in InstructionZones)
			{
				var zone = GKManager.Zones.FirstOrDefault(x => x.UID == uid);
				if (zone != null)
					zones.Add(zone);
			}
			var zonesSelectationViewModel = new ZonesSelectationViewModel(zones);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				var uids = zonesSelectationViewModel.Zones.Select(x => x.UID).ToList();
				InstructionZones = new ObservableCollection<Guid>(uids);
			}
		}

		public RelayCommand SelectDeviceCommand { get; private set; }
		void OnSelectDeviceCommand()
		{
			var devices = new List<GKDevice>();
			foreach (var uid in InstructionDevices)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == uid);
				if (device != null)
					devices.Add(device);
			}
			var sourceDevices = new List<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (device.Driver.IsDeviceOnShleif)
					sourceDevices.Add(device);
			}
			var devicesSelectationViewModel = new DevicesSelectationViewModel(devices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				var uids = devicesSelectationViewModel.Devices.Select(x => x.UID).ToList();
				InstructionDevices = new ObservableCollection<Guid>(uids);
			}
		}

		public RelayCommand SelectDirectionCommand { get; private set; }
		void OnSelectDirectionCommand()
		{
			var directions = new List<GKDirection>();
			foreach (var uid in InstructionDirections)
			{
				var direction = GKManager.Directions.FirstOrDefault(x => x.UID == uid);
				if (direction != null)
					directions.Add(direction);
			}
			var directionsSelectationViewModel = new DirectionsSelectationViewModel(directions);
			if (DialogService.ShowModalWindow(directionsSelectationViewModel))
			{
				var uids = directionsSelectationViewModel.TargetDirections.Select(x => x.UID).ToList();
				InstructionDirections = new ObservableCollection<Guid>(uids);
			}
		}

		public RelayCommand GetMediaCommand { get; private set; }
		void OnGetMedia()
		{
			try
			{
				var openDialog = new OpenFileDialog();
				openDialog.DefaultExt = ".wmv";
				openDialog.Filter = "wmv видео (*.wmv)|*.wmv|wav аудио (*.wav)|*.wav| все файлы|*.*";
				if (openDialog.ShowDialog().Value)
				{
					Instruction.MediaSource = openDialog.FileName;
					OnPropertyChanged(() => Instruction);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "InstructionDetailsView.GetAudio");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
		}

		public RelayCommand RemoveMediaCommand { get; private set; }
		void OnRemoveMedia()
		{
			Instruction.MediaSource = null;
			OnPropertyChanged(() => Instruction);
		}

		protected override bool CanSave()
		{
			if (string.IsNullOrWhiteSpace(Text) && !Instruction.HasMedia)
				return false;
			else
				return InstructionType == GKInstructionType.General ? true : (InstructionDevices.IsNotNullOrEmpty() || InstructionZones.IsNotNullOrEmpty() || InstructionDirections.IsNotNullOrEmpty());
		}

		protected override bool Save()
		{
			Instruction.Name = Name;
			Instruction.Text = Text;
			Instruction.AlarmType = AlarmType;
			Instruction.InstructionType = InstructionType;
			if (InstructionType == GKInstructionType.Details)
			{
				Instruction.Devices = InstructionDevices.ToList();
				Instruction.ZoneUIDs = InstructionZones.ToList();
				Instruction.Directions = InstructionDirections.ToList();
			}
			else
			{
				Instruction.Devices = new List<Guid>();
				Instruction.ZoneUIDs = new List<Guid>();
				Instruction.Directions = new List<Guid>();
			}
			return base.Save();
		}
	}
}