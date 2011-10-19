using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardLevelViewModel : BaseViewModel
    {
        public GuardLevelViewModel(GuardLevel guardLevel)
        {
            GuardLevel = guardLevel;
        }

        GuardLevel _guardLevel;
        public GuardLevel GuardLevel
        {
            get { return _guardLevel; }
            set
            {
                _guardLevel = value;
                OnPropertyChanged("GuardLevel");
            }
        }
    }
}
