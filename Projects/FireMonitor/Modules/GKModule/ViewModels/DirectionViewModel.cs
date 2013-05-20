using System.Collections.Generic;
using System.Text;
using System.Linq;
using Common.GK;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		public XDirectionState DirectionState { get; private set; }
		public XDirection Direction
		{
			get { return DirectionState.Direction; }
		}

		public DirectionViewModel(XDirectionState directionState)
		{
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			DirectionState = directionState;
			DirectionState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("DirectionState");
			OnPropertyChanged("ToolTip");
			OnPropertyChanged("HasOnDelay");
			OnPropertyChanged("HasHoldDelay");
		}

		public string ToolTip
		{
			get
			{
                var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Direction.PresentationName);
				stringBuilder.AppendLine("Состояние: " + DirectionState.StateClass.ToDescription());
				foreach (var stateType in DirectionState.States)
				{
					stringBuilder.AppendLine(stateType.ToDescription());
				}
                stringBuilder.AppendLine("Задержка: " + Direction.Delay.ToString());
				return stringBuilder.ToString();
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowDirection(Direction);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowDirection(Direction);
		}
		
		public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            DialogService.ShowWindow(new DirectionDetailsViewModel(Direction));
        }

		public bool HasOnDelay
		{
			get { return DirectionState.States.Contains(XStateType.TurningOn) && DirectionState.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return DirectionState.States.Contains(XStateType.On) && DirectionState.HoldDelay > 0; }
		}
	}
}