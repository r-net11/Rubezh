﻿using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public Zone Zone { get; private set; }

		public ZoneViewModel(Zone zone)
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
				ServiceFactory.SaveService.FSChanged = true;
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
				ServiceFactory.SaveService.FSChanged = true;
			}
		}
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(Zone zone)
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