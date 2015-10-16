using System;
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

		public void Update()
		{
			OnPropertyChanged(() => Zone);
			_visualizetionState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
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

		VisualizationState _visualizetionState;
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
	}
}