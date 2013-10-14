using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Microsoft.Win32;
using Common;
using System.Windows;

namespace GKModule.ViewModels
{
	public class InstructionDetailsViewModel : SaveCancelDialogViewModel
	{
		public XInstruction Instruction { get; private set; }

		public InstructionDetailsViewModel(XInstruction instruction = null)
		{
			SelectZoneCommand = new RelayCommand(OnSelectZoneCommand, CanSelect);
			SelectDeviceCommand = new RelayCommand(OnSelectDeviceCommand, CanSelect);
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
				Instruction = new XInstruction();
				CopyProperties();
			}
		}

		void CopyProperties()
		{
			InstructionZones = new ObservableCollection<Guid>();
			InstructionDevices = new ObservableCollection<Guid>();

			Name = Instruction.Name;
			Text = Instruction.Text;
			AlarmType = Instruction.AlarmType;
			InstructionType = Instruction.InstructionType;
			switch (InstructionType)
			{
				case XInstructionType.Details:
					if (Instruction.ZoneUIDs.IsNotNullOrEmpty())
						InstructionZones = new ObservableCollection<Guid>(Instruction.ZoneUIDs);
					if (Instruction.Devices.IsNotNullOrEmpty())
						InstructionDevices = new ObservableCollection<Guid>(Instruction.Devices);
					break;

				case XInstructionType.General:
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
			get { return InstructionType == XInstructionType.Details; }
		}

		XAlarmType _alarmType;
		public XAlarmType AlarmType
		{
			get { return _alarmType; }
			set
			{
				_alarmType = value;
				OnPropertyChanged("AlarmType");
			}
		}
		public List<XAlarmType> AvailableAlarmTypes
		{
			get { return Enum.GetValues(typeof(XAlarmType)).Cast<XAlarmType>().ToList(); }
		}

		XInstructionType _instructionType;
		public XInstructionType InstructionType
		{
			get { return _instructionType; }
			set
			{
				_instructionType = value;
				OnPropertyChanged("InstructionType");
				OnPropertyChanged("IsDetails");
			}
		}
		public List<XInstructionType> AvailableInstructionsType
		{
			get { return new List<XInstructionType>(Enum.GetValues(typeof(XInstructionType)).OfType<XInstructionType>()); }
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
			return (InstructionType != XInstructionType.General);
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
					OnPropertyChanged("Instruction");
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
			OnPropertyChanged("Instruction");
		}

		protected override bool CanSave()
		{
			if (string.IsNullOrWhiteSpace(Text) && !Instruction.HasMedia)
				return false;
			else
				return InstructionType == XInstructionType.General ? true : (InstructionDevices.IsNotNullOrEmpty() || InstructionZones.IsNotNullOrEmpty());
		}

		protected override bool Save()
		{
			Instruction.Name = Name;
			Instruction.Text = Text;
			Instruction.AlarmType = AlarmType;
			Instruction.InstructionType = InstructionType;
			if (InstructionType == XInstructionType.Details)
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