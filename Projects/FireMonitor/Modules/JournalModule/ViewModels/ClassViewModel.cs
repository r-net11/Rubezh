using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ClassViewModel : BaseViewModel
    {
        public ClassViewModel(int id)
        {
            Id = id;
            _isEnable = false;
        }

        public int Id { get; private set; }

        bool? _isEnable;
        public bool? IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged("IsEnable");
            }
        }
    }
}
