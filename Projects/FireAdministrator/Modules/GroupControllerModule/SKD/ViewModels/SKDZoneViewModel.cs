using System;
using System.Linq;
using System.Collections.Generic;
using RubezhAPI;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using GKModule.Events;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
using RubezhClient;

namespace GKModule.ViewModels
{
	public class SKDZoneViewModel : BaseViewModel
	{
		public GKSKDZone Zone { get; private set; }

		public SKDZoneViewModel(GKSKDZone zone)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);

			Zone = zone;
			Update();
		}
		List<GKDoor> _doors;
		public List<GKDoor> Doors
		{
			get { return _doors; }
			set
			{
				_doors = value;
				OnPropertyChanged(() => Doors);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Zone);
			_visualizationState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);

			Doors = GKManager.Doors.Where(x => x.EnterZoneUID == Zone.UID || x.ExitZoneUID==Zone.UID).ToList();
		}

		public bool IsOnPlan
		{
			get { return Zone.PlanElementUIDs.Count > 0; }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Zone.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Zone.PlanElementUIDs);
		}

		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
		}
	}
}