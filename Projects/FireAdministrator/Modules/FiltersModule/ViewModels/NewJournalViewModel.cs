using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;

namespace JournalModule.ViewModels
{
    public class NewJournalViewModel : DialogContent
    {
        public NewJournalViewModel()
        {
            Title = "Настройка представления";

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
            
            Events = new ObservableCollection<EventViewModel>();
            Events.Add(new EventViewModel("Тревога"));
            Events.Add(new EventViewModel("Внимание"));
            Events.Add(new EventViewModel("Неисправность"));
            Events.Add(new EventViewModel("Требуется обслуживание"));
            Events.Add(new EventViewModel("Тревоги отключены"));
            Events.Add(new EventViewModel("Информация"));
            Events.Add(new EventViewModel("Прочие"));

            Categories = new ObservableCollection<CategoryViewModel>();
            Categories.Add(new CategoryViewModel("Прочие устройства"));
            Categories.Add(new CategoryViewModel("Прибор"));
            Categories.Add(new CategoryViewModel("Датчик"));
            Categories.Add(new CategoryViewModel("Исполнительное устройство"));
            Categories.Add(new CategoryViewModel("Сеть передачи данных"));
            Categories.Add(new CategoryViewModel("Удаленный сервер"));
            Categories.Add(new CategoryViewModel("[Без устройства]"));

            Count = 100;
            LastDays = 10;
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        int _lastDays;
        public int LastDays
        {
            get { return _lastDays; }
            set
            {
                _lastDays = value;
                OnPropertyChanged("LastDays");
            }
        }

        ObservableCollection<EventViewModel> _events;
        public ObservableCollection<EventViewModel> Events
        {
            get { return _events; }
            set
            {
                _events = value;
                OnPropertyChanged("Events");
            }
        }

        ObservableCollection<CategoryViewModel> _categories;
        public ObservableCollection<CategoryViewModel> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged("Categories");
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
