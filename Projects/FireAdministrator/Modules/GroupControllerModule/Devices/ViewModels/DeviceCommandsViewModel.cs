using GKModule.Converter;
using GKModule.Database;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.Models
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel _devicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			ConvertFromFiresecCommand = new RelayCommand(OnConvertFromFiresec);
			ConvertToBinCommand = new RelayCommand(OnConvertToBin);
			CheckConnectionCommand = new RelayCommand(OnCheckConnection);
			GetKAUVersionCommand = new RelayCommand(OnGetKAUVersion);
			GetBlocklVersionCommand = new RelayCommand(OnGetBlocklVersion);
			WriteBlockInfoCommand = new RelayCommand(OnWriteBlockInfo);
			SetTimeCommand = new RelayCommand(OnSetTime);
			GetTimeCommand = new RelayCommand(OnGetTime);
			GetLastJournalIndexCommand = new RelayCommand(OnGetLastJournalIndex);
			GetJournalItemByIndexCommand = new RelayCommand(OnGetJournalItemByIndex);
			EraseJournalCommand = new RelayCommand(OnEraseJournal);
			GetParametersCommand = new RelayCommand(OnGetParameters);
			WriteParametersCommand = new RelayCommand(OnWriteParameters);
			StartWorkingProgramCommand = new RelayCommand(OnStartWorkingProgram);
			GetObjectInfoCommand = new RelayCommand(OnGetObjectInfo);
			ExecuteObjectCommand = new RelayCommand(OnExecuteObject);
			GoToTechnologicalProgrammCommand = new RelayCommand(OnGoToTechnologicalProgramm);
			EraseDatabaseCommand = new RelayCommand(OnEraseDatabase);
			EraseWorkingProgramCommand = new RelayCommand(OnEraseWorkingProgram);
			WriteDatabaseCommand = new RelayCommand(OnWriteDatabase);
			WriteProgramCommand = new RelayCommand(OnWriteProgram);
			GetDeviceParameterCommand = new RelayCommand(OnGetDeviceParameter);
			WriteDeviceParameterCommand = new RelayCommand(OnWriteDeviceParameter);

			_devicesViewModel = devicesViewModel;
		}

		public DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
		}

		public RelayCommand ConvertFromFiresecCommand { get; private set; }
		void OnConvertFromFiresec()
		{
			var configurationConverter = new ConfigurationConverter();
			configurationConverter.Convert();

			DevicesViewModel.Current.Initialize();
			ZonesViewModel.Current.Initialize();
			ServiceFactory.SaveService.XDevicesChanged = true;
		}

		public RelayCommand ConvertToBinCommand { get; private set; }
		void OnConvertToBin()
		{
			DatabaseProcessor.Convert();

			var deviceConverterViewModel = new DatabasesViewModel();
			DialogService.ShowModalWindow(deviceConverterViewModel);
		}

		public RelayCommand CheckConnectionCommand { get; private set; }
		void OnCheckConnection()
		{
			CommandManager.Send();
		}

		public RelayCommand GetKAUVersionCommand { get; private set; }
		void OnGetKAUVersion()
		{
		}

		public RelayCommand GetBlocklVersionCommand { get; private set; }
		void OnGetBlocklVersion()
		{
		}

		public RelayCommand WriteBlockInfoCommand { get; private set; }
		void OnWriteBlockInfo()
		{
		}

		public RelayCommand SetTimeCommand { get; private set; }
		void OnSetTime()
		{
		}

		public RelayCommand GetTimeCommand { get; private set; }
		void OnGetTime()
		{
		}

		public RelayCommand GetLastJournalIndexCommand { get; private set; }
		void OnGetLastJournalIndex()
		{
		}

		public RelayCommand GetJournalItemByIndexCommand { get; private set; }
		void OnGetJournalItemByIndex()
		{
		}

		public RelayCommand EraseJournalCommand { get; private set; }
		void OnEraseJournal()
		{
		}

		public RelayCommand GetParametersCommand { get; private set; }
		void OnGetParameters()
		{
		}

		public RelayCommand WriteParametersCommand { get; private set; }
		void OnWriteParameters()
		{
		}

		public RelayCommand StartWorkingProgramCommand { get; private set; }
		void OnStartWorkingProgram()
		{
		}

		public RelayCommand GetObjectInfoCommand { get; private set; }
		void OnGetObjectInfo()
		{
		}

		public RelayCommand ExecuteObjectCommand { get; private set; }
		void OnExecuteObject()
		{
		}

		public RelayCommand GoToTechnologicalProgrammCommand { get; private set; }
		void OnGoToTechnologicalProgramm()
		{
		}

		public RelayCommand EraseDatabaseCommand { get; private set; }
		void OnEraseDatabase()
		{
		}

		public RelayCommand EraseWorkingProgramCommand { get; private set; }
		void OnEraseWorkingProgram()
		{
		}

		public RelayCommand WriteDatabaseCommand { get; private set; }
		void OnWriteDatabase()
		{
		}

		public RelayCommand WriteProgramCommand { get; private set; }
		void OnWriteProgram()
		{
		}

		public RelayCommand GetDeviceParameterCommand { get; private set; }
		void OnGetDeviceParameter()
		{
		}

		public RelayCommand WriteDeviceParameterCommand { get; private set; }
		void OnWriteDeviceParameter()
		{
		}
	}
}