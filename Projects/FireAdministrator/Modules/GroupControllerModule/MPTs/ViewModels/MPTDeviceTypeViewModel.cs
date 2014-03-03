using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class MPTDeviceTypeViewModel : SaveCancelDialogViewModel
	{
		public MPTDeviceTypeViewModel()
		{
			Title = "Выбор типа устройства";

			AvailableMPTDeviceTypes = new ObservableCollection<MPTDeviceType>();
			AvailableMPTDeviceTypes.Add(MPTDeviceType.DoNotEnterBoard);
			AvailableMPTDeviceTypes.Add(MPTDeviceType.ExitBoard);
			AvailableMPTDeviceTypes.Add(MPTDeviceType.AutomaticOffBoard);
			AvailableMPTDeviceTypes.Add(MPTDeviceType.Speaker);
			AvailableMPTDeviceTypes.Add(MPTDeviceType.Door);
			AvailableMPTDeviceTypes.Add(MPTDeviceType.HandStart);
			AvailableMPTDeviceTypes.Add(MPTDeviceType.HandStop);
			AvailableMPTDeviceTypes.Add(MPTDeviceType.HandAutomatic);
			AvailableMPTDeviceTypes.Add(MPTDeviceType.Bomb);
			SelectedMPTDeviceType = AvailableMPTDeviceTypes.FirstOrDefault();
		}

		public ObservableCollection<MPTDeviceType> AvailableMPTDeviceTypes { get; private set; }

		MPTDeviceType _selectedMPTDeviceType;
		public MPTDeviceType SelectedMPTDeviceType
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