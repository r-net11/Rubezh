using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class MPTDeviceTypeSelectationViewModel : SaveCancelDialogViewModel
	{
		public MPTDeviceTypeSelectationViewModel()
		{
			Title = "Выбор типа устройства";

			AvailableMPTDeviceTypes = new ObservableCollection<MPTDeviceTypeViewModel>();
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(MPTDeviceType.DoNotEnterBoard));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(MPTDeviceType.ExitBoard));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(MPTDeviceType.AutomaticOffBoard));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(MPTDeviceType.Speaker));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(MPTDeviceType.Door));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(MPTDeviceType.HandStart));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(MPTDeviceType.HandStop));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(MPTDeviceType.HandAutomatic));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(MPTDeviceType.Bomb));
			SelectedMPTDeviceType = AvailableMPTDeviceTypes.FirstOrDefault();
		}

		public ObservableCollection<MPTDeviceTypeViewModel> AvailableMPTDeviceTypes { get; private set; }

		MPTDeviceTypeViewModel _selectedMPTDeviceType;
		public MPTDeviceTypeViewModel SelectedMPTDeviceType
		{
			get { return _selectedMPTDeviceType; }
			set
			{
				_selectedMPTDeviceType = value;
				OnPropertyChanged("SelectedMPTDeviceType");
			}
		}

		protected override bool CanSave()
		{
			return SelectedMPTDeviceType != null;
		}
	}
}