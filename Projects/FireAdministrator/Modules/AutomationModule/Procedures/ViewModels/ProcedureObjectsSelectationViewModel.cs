using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureObjectsSelectationViewModel : SaveCancelDialogViewModel
	{
		public ProcedureInputObject ProcedureInputObject { get; private set; }

		public ProcedureObjectsSelectationViewModel()
		{
			Title = "Выбор входных объектов";
			DeviceSelectationViewModel = new DeviceSelectationViewModel();
			CameraSelectationViewModel = new CameraSelectationViewModel();
			AvailableProcedureObjectTypes = new ObservableCollection<ProcedureObjectType>();
			AvailableProcedureObjectTypes.Add(ProcedureObjectType.XDevice);
			AvailableProcedureObjectTypes.Add(ProcedureObjectType.Camera);
			SelectedProcedureObjectType = AvailableProcedureObjectTypes.FirstOrDefault();
		}

		public ObservableCollection<ProcedureObjectType> AvailableProcedureObjectTypes { get; private set; }

		ProcedureObjectType _selectedProcedureObjectType;
		public ProcedureObjectType SelectedProcedureObjectType
		{
			get { return _selectedProcedureObjectType; }
			set
			{
				_selectedProcedureObjectType = value;
				OnPropertyChanged("SelectedProcedureObjectType");
				switch (value)
				{
					case ProcedureObjectType.XDevice:
						IsXDeviceSelected = true;
						IsCameraSelected = false;
						break;

					case ProcedureObjectType.Camera:
						IsXDeviceSelected = false;
						IsCameraSelected = true;
						break;
				}
			}
		}

		bool _isXDeviceSelected;
		public bool IsXDeviceSelected
		{
			get { return _isXDeviceSelected; }
			set
			{
				_isXDeviceSelected = value;
				OnPropertyChanged("IsXDeviceSelected");
			}
		}

		bool _isCameraSelected;
		public bool IsCameraSelected
		{
			get { return _isCameraSelected; }
			set
			{
				_isCameraSelected = value;
				OnPropertyChanged("IsCameraSelected");
			}
		}

		public DeviceSelectationViewModel DeviceSelectationViewModel { get; private set; }
		public CameraSelectationViewModel CameraSelectationViewModel { get; private set; }

		protected override bool Save()
		{
			ProcedureInputObject = new ProcedureInputObject()
			{
				ProcedureObjectType = SelectedProcedureObjectType
			};
			switch (SelectedProcedureObjectType)
			{
				case ProcedureObjectType.XDevice:
					if (DeviceSelectationViewModel.SelectedDevice != null)
						ProcedureInputObject.UID = DeviceSelectationViewModel.SelectedDevice.Device.UID;
					break;

				case ProcedureObjectType.Camera:
					if(CameraSelectationViewModel.SelectedCamera != null)
						ProcedureInputObject.UID = CameraSelectationViewModel.SelectedCamera.Camera.UID;
					break;
			}
			return base.Save();
		}
	}
}