using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public SKDZone Zone { get; private set; }

		public ZoneViewModel(SKDZone zone)
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);

			Zone = zone;
			zone.Changed += OnChanged;
			Update();
		}

		void OnChanged()
		{
			OnPropertyChanged(() => Name);
		}

		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(SKDZone zone)
		{
			Zone = zone;
			OnPropertyChanged(() => Zone);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}

		public void Update()
		{
			if (Zone.PlanElementUIDs == null)
				Zone.PlanElementUIDs = new List<Guid>();
			_visualizetionState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => IsOnPlan);
			OnPropertyChanged(() => VisualizationState);
		}

		public string Name
		{
			get { return Zone.Name; }
		}

		public string Description
		{
			get { return Zone.Description; }
		}

		public bool IsOnPlan
		{
			get { return Zone.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return true; }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Zone.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Zone.PlanElementUIDs);
		}

		public bool IsBold { get; set; }
	}
}