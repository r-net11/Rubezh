using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ClassViewModel : BaseViewModel
    {
        public ClassViewModel(int id)
        {
            Id = id;
            IsEnable = false;
        }

        public int Id { get; private set; }
        public bool? IsEnable { get; set; }
    }
}