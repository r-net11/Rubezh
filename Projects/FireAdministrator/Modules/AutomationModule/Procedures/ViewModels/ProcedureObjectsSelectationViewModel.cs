using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI.Models;

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
				switch(value)
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
			switch(SelectedProcedureObjectType)
			{
				case ProcedureObjectType.XDevice:
					ProcedureInputObject.UID = Guid.NewGuid();
					break;

				case ProcedureObjectType.Camera:
					ProcedureInputObject.UID = Guid.NewGuid();
					break;
			}
			return base.Save();
		}
	}
}