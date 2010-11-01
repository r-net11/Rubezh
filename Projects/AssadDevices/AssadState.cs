using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AssadDdevices
{
    public class AssadState : INotifyPropertyChanged
    {
        public string Name { get { return "Состояние"; } }

        public string State
        {
            get
            {
                switch (stateId)
                {
                    case 0:
                        return "Тревога";
                    case 1:
                        return "Внимание (предтревожное)";
                    case 2:
                        return "Неисправность";
                    case 3:
                        return "Требуется обслуживание";
                    case 4:
                        return "Обход устройств";
                    case 5:
                        return "Неизвестно";
                    case 6:
                        return "Норма(*)";
                    case 7:
                        return "Норма";
                    case 8:
                        return "Нет состояния";
                    default:
                        return "";
                }
            }
        }

        int stateId = 8;
        public int StateId
        {
            set
            {
                if (stateId != value)
                {
                    stateId = value;
                    OnTempPropertyChanged(this);
                }
                OnPropertyChanged("State");
            }
        }

        public delegate void TempPropertyChangedDelegate(AssadState sender);
        public event TempPropertyChangedDelegate TempPropertyChanged;
        void OnTempPropertyChanged(AssadState sender)
        {
            if (TempPropertyChanged != null)
                TempPropertyChanged(sender);
        }

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
