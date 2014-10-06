using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTDeviceTypeSelectationViewModel : SaveCancelDialogViewModel
	{
		public MPTDeviceTypeSelectationViewModel()
		{
			Title = "Выбор типа устройства";

			AvailableMPTDeviceTypes = new ObservableCollection<MPTDeviceTypeViewModel>();
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.DoNotEnterBoard));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.ExitBoard));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.AutomaticOffBoard));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.Speaker));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.Door));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.HandStart));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.HandStop));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.HandAutomatic));
			AvailableMPTDeviceTypes.Add(new MPTDeviceTypeViewModel(GKMPTDeviceType.Bomb));
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
				OnPropertyChanged(() => SelectedMPTDeviceType);
			}
		}

		protected override bool CanSave()
		{
			return SelectedMPTDeviceType != null;
		}
	}
}