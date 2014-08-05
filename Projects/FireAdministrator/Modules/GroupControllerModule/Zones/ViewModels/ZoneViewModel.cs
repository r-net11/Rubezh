using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public XZone Zone { get; set; }

		public ZoneViewModel(XZone zone)
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
				OnPropertyChanged("Name");
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
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public bool IsOnPlan
		{
			get { return Zone.PlanElementUIDs.Count > 0; }
		}
		public void Update(XZone zone)
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

		#region OPC
		public bool IsOPCUsed
		{
			get { return Zone.IsOPCUsed; }
			set
			{
				Zone.IsOPCUsed = value;
				OnPropertyChanged("IsOPCUsed");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		#endregion
	}
}