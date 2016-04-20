using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;
using System;

namespace GKModule.ViewModels
{
	public class DoorSkdZoneViewModel : BaseViewModel
	{
		public SKDZoneViewModel SkdZoneViewModel { get; private set; }
		public GKDoor Door { get; private set; }
		public DoorSkdZoneViewModel(GKDoor door, SKDZoneViewModel skdZoneViewModel)
		{
			RemoveOutputDoorCommand = new RelayCommand(OnRemoveOutputDoor);
			RemoveInputDoorCommand = new RelayCommand(OnRemoveInputDoor);

			Door = door;
			SkdZoneViewModel = skdZoneViewModel;
		}
		public RelayCommand RemoveOutputDoorCommand { get; private set; }
		void OnRemoveOutputDoor()
		{
			Door.EnterZoneUID = Guid.Empty;
			SkdZoneViewModel.Update();
			ServiceFactory.SaveService.GKChanged = true;
		}
		public RelayCommand RemoveInputDoorCommand { get; private set; }
		void OnRemoveInputDoor()
		{
			Door.ExitZoneUID = Guid.Empty;
			SkdZoneViewModel.Update();
			ServiceFactory.SaveService.GKChanged = true;
		}
	}
}