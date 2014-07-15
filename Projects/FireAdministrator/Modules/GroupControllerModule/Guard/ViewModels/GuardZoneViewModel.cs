using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class GuardZoneViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public XGuardZone Zone { get; set; }

		public GuardZoneViewModel(XGuardZone zone)
		{
			Zone = zone;
			Update();
		}

		public string Name
		{
			get { return Zone.Name; }
			set
			{
				Zone.Name = value;
				Zone.OnChanged();
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public string Description
		{
			get { return Zone.Description; }
			set
			{
				Zone.Description = value;
				Zone.OnChanged();
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(XGuardZone zone)
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
			OnPropertyChanged(() => VisualizationState);
		}
	}
}