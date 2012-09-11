using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceLogicViewModel : SaveCancelDialogViewModel
	{
		public XDevice Device { get; private set; }

		public DeviceLogicViewModel(XDevice device)
		{
			Title = "Настройка логики устройства";
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<StateLogicViewModel>(OnRemove);

			Device = device;
			if (device.DeviceLogic.StateLogics.Count == 0)
			{
				var stateLogic = new StateLogic();
				var clause = new XClause();
				stateLogic.Clauses.Add(clause);
				device.DeviceLogic.StateLogics.Add(stateLogic);
			}

			StateLogics = new ObservableCollection<StateLogicViewModel>();
			foreach (var stateLogic in device.DeviceLogic.StateLogics)
			{
				var stateLogicViewModel = new StateLogicViewModel(this, stateLogic);
				StateLogics.Add(stateLogicViewModel);
			}
		}

		public ObservableCollection<StateLogicViewModel> StateLogics { get; private set; }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var stateLogic = new StateLogic();
			var stateLogicViewModel = new StateLogicViewModel(this, stateLogic);
			StateLogics.Add(stateLogicViewModel);
		}

		public RelayCommand<StateLogicViewModel> RemoveCommand { get; private set; }
		void OnRemove(StateLogicViewModel stateLogicViewModel)
		{
			StateLogics.Remove(stateLogicViewModel);
		}

		protected override bool Save()
		{
			var deviceLogic = new XDeviceLogic();
			foreach (var stateLogicViewModel in StateLogics)
			{
				var stateLogic = new StateLogic();
				stateLogic.StateType = stateLogicViewModel.SelectedStateType;
				foreach (var clauseViewModel in stateLogicViewModel.Clauses)
				{
					var clause = new XClause()
					{
						StateType = clauseViewModel.SelectedStateType,
						Devices = clauseViewModel.Devices.ToList(),
						Zones = clauseViewModel.Zones.ToList(),
						ClauseJounOperationType = clauseViewModel.SelectedClauseJounOperationType,
						ClauseOperationType = clauseViewModel.SelectedClauseOperationType
					};
					foreach (var deviceUID in clause.Devices)
					{
						var decvice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x=>x.UID == deviceUID);
						clause.XDevices.Add(decvice);
					}
					foreach (var zoneUID in clause.Zones)
					{
						var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
						clause.XZones.Add(zone);
					}
					if ((clause.Devices.Count > 0) || (clause.Zones.Count > 0))
						stateLogic.Clauses.Add(clause);
				}
				if (deviceLogic.StateLogics.Any(x => x.StateType == stateLogic.StateType))
				{
					MessageBoxService.ShowWarning("Логика для состояние " + stateLogic.StateType.ToDescription() + " дублируется");
					return false;
				}
				if (stateLogic.Clauses.Count > 0)
					deviceLogic.StateLogics.Add(stateLogic);

				Device.DeviceLogic = deviceLogic;
			}
			return base.Save();
		}
	}
}