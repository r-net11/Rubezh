using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Helper;
using System;
using System.Collections.Generic;
using System.Windows;

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

			ZoneName = Name;
		}

		public static readonly DependencyProperty ZoneNameProperty =
			DependencyProperty.Register("ZoneName", typeof(string), typeof(ZoneViewModel),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnZoneNamePropertyChanged)));

		public string ZoneName
		{
			get { return (string)GetValue(ZoneNameProperty); }
			set { SetValue(ZoneNameProperty, value); }
		}

		private static void OnZoneNamePropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			ZoneViewModel zoneViewModel = dp as ZoneViewModel;
			if (zoneViewModel != null)
			{
				zoneViewModel.Name = zoneViewModel.ZoneName;
			}
		}

		public string Name
		{
			get { return Zone.Name; }
			set
			{
				Zone.Name = value;
				Zone.OnChanged();
				OnPropertyChanged("Name");
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
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.FSChanged = true;
			}
		}
		public string DetectorCount
		{
			get
			{
				if (Zone.ZoneType == ZoneType.Fire)
					return Zone.DetectorCount.ToString();
				return null;
			}
		}
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(Zone zone)
		{
			Zone = zone;
			OnPropertyChanged("Zone");
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
			OnPropertyChanged("DetectorCount");
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