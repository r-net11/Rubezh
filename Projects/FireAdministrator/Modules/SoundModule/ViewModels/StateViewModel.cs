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

        public ObservableCollection<string> AvailableSounds
        {
            get
            {
                return new ObservableCollection<string>(){
                "<Нет>",
                "Sound1.wav",
                "Sound2.wav",
                "Sound3.wav",
                "Sound4.wav",
                "Sound5.wav",
                "Sound6.wav",
                "Sound7.wav",
                "Sound8.wav",
                "Sound9.wav"
                };
            }
        }

        public ObservableCollection<string> AvailableSpeakers
        {
            get
            {
                return new ObservableCollection<string>(){
                "<Нет>",
                "Тревога",
                "Внимание"
                };
            }
        }
    }
}
