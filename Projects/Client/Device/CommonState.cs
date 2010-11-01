using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class CommonState
    {
        public string Name { get { return "Состояние"; } }

        public string CurrentState
        {
            get
            {
                switch (currentClass)
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

        int currentClass = 8;
        public int CurrentClass
        {
            set
            {
                if (currentClass != value)
                {
                    currentClass = value;
                    OnPropertyChanged(this);
                }
            }
        }

        public delegate void PropertyChangedDelegate(CommonState sender);
        public event PropertyChangedDelegate PropertyChanged;
        void OnPropertyChanged(CommonState sender)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender);
        }
    }
}
