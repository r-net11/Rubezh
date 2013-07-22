using System;
using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Helper;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
		private VisualizationState _visualizetionState;
		public XZone XZone { get; set; }

        public ZoneViewModel(XZone xZone)
        {
            XZone = xZone;
			Update();
        }

		public string Name
		{
			get { return XZone.Name; }
			set
			{
				XZone.Name = value;
				XZone.OnChanged();
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public string Description
		{
			get { return XZone.Description; }
			set
			{
				XZone.Description = value;
				XZone.OnChanged();
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(XZone xzone)
		{
			XZone = xzone;
			OnPropertyChanged("Zone");
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
			Update();
		}

		public void Update()
		{
			if (XZone.PlanElementUIDs == null)
				XZone.PlanElementUIDs = new List<Guid>();
			_visualizetionState = XZone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (XZone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}
    }
}