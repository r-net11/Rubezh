using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class FilterEventNameViewModel : BaseViewModel
    {
        public FilterEventNameViewModel(string eventName)
        {
            EventName = eventName;
        }

        public string EventName { get; private set; }
        public bool IsChecked { get; set; }
    }
}