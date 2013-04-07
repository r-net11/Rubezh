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
			DirectionState = directionState;
			DirectionState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("DirectionState");
			OnPropertyChanged("ToolTip");
			OnPropertyChanged("DelayTimeLeft");
			OnPropertyChanged("ShowDelayTimeLeft");
			OnPropertyChanged("HoldTimeLeft");
			OnPropertyChanged("ShowHoldTimeLeft");
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

		public int DelayTimeLeft
		{
			get
			{
				var additionalStateProperty = DirectionState.AdditionalStateProperties.FirstOrDefault(x => x.Name == "Задержка");
				if (additionalStateProperty != null)
				{
					return additionalStateProperty.Value;
				}
				return 0;
			}
		}
		public bool ShowDelayTimeLeft
		{
			get { return DelayTimeLeft > 0; }
		}

		public int HoldTimeLeft
		{
			get
			{
				var additionalStateProperty = DirectionState.AdditionalStateProperties.FirstOrDefault(x => x.Name == "Удержание");
				if (additionalStateProperty != null)
				{
					return additionalStateProperty.Value;
				}
				return 0;
			}
		}
		public bool ShowHoldTimeLeft
		{
			get { return HoldTimeLeft > 0; }
		}

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            DialogService.ShowWindow(new DirectionDetailsViewModel(Direction));
        }
	}
}