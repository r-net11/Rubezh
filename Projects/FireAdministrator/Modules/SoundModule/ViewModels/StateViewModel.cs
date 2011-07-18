using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace SoundsModule.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public StateViewModel(string name)
        {
            Name = name;
            IsContinious = false;
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

        string _sound;
        public string Sound
        {
            get { return _sound; }
            set
            {
                _sound = value;
                OnPropertyChanged("Sound");
            }
        }

        string _speaker;
        public string Speaker
        {
            get { return _speaker; }
            set
            {
                _speaker = value;
                OnPropertyChanged("Speaker");
            }
        }

        bool _isContinious;
        public bool IsContinious
        {
            get { return _isContinious; }
            set
            {
                _isContinious = value;
                OnPropertyChanged("IsContinious");
            }
        }

        public ObservableCollection<string> AvailableSouns
        {
            get
            {
                ObservableCollection<string> availableSouns = new ObservableCollection<string>();
                availableSouns.Add("<Нет>");
                availableSouns.Add("Sound1.wav");
                availableSouns.Add("Sound2.wav");
                availableSouns.Add("Sound3.wav");
                availableSouns.Add("Sound4.wav");
                availableSouns.Add("Sound5.wav");
                availableSouns.Add("Sound6.wav");
                availableSouns.Add("Sound7.wav");
                availableSouns.Add("Sound8.wav");
                availableSouns.Add("Sound9.wav");
                return availableSouns;
            }
        }

        public ObservableCollection<string> AvailableSpeakers
        {
            get
            {
                ObservableCollection<string> availableSpeakers = new ObservableCollection<string>();
                availableSpeakers.Add("<Нет>");
                availableSpeakers.Add("Тревога");
                availableSpeakers.Add("Внимание");
                return availableSpeakers;
            }
        }
    }
}
