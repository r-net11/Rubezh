using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace SoundsModule.ViewModels
{
    class SoundListViewModel : BaseViewModel
    {


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
    }
}
