using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class LevelViewModel : BaseViewModel
    {
        public LevelViewModel(GuardLevel guardLevel)
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
