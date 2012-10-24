using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class StateTypeViewModel : BaseViewModel
    {
        public StateTypeViewModel(XStateType stateType)
        {
            StateType = stateType;
        }

        public XStateType StateType { get; private set; }
        public bool IsChecked { get; set; }
    }
}