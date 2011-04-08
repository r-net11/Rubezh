using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WpfApplication4
{
    public class ViewModel : BaseViewModel
    {
        public ViewModel()
        {
            Current = this;

            MainAlarms = new ObservableCollection<MainAlarm>();
            MainAlarms.Add(new MainAlarm() { Name = "Пожар", AlarmType = AlarmType.Alarm });
            MainAlarms.Add(new MainAlarm() { Name = "Внимание", AlarmType = AlarmType.Attention });
            MainAlarms.Add(new MainAlarm() { Name = "Неисправность", AlarmType = AlarmType.Failure });
            MainAlarms.Add(new MainAlarm() { Name = "Отключение", AlarmType = AlarmType.Off });
            MainAlarms.Add(new MainAlarm() { Name = "Информация", AlarmType = AlarmType.Info });
            MainAlarms.Add(new MainAlarm() { Name = "Обслуживание", AlarmType = AlarmType.Service });
            MainAlarms.Add(new MainAlarm() { Name = "Автоматика", AlarmType = AlarmType.Auto });

            DetailAlarms = new ObservableCollection<DetailAlarm>();

            AddAlarm(new Alarm() { AlarmType = AlarmType.Alarm, Name = "ИП 212-64", Description = "Сработал дымовой датчик" });
            AddAlarm(new Alarm() { AlarmType = AlarmType.Alarm, Name = "ИПР", Description = "Сработал ручной извещатель" });
            AddAlarm(new Alarm() { AlarmType = AlarmType.Alarm, Name = "ИП 101-29", Description = "Сработал тепловой извещатель" });
            AddAlarm(new Alarm() { AlarmType = AlarmType.Failure, Name = "Вскрытие", Description = "Всрытие прибора Рубеж-2АМ" });
            AddAlarm(new Alarm() { AlarmType = AlarmType.Info, Name = "Вход", Description = "Вход пользователя в систему" });
        }

        public static ViewModel Current { get; set; }

        public void AddAlarm(Alarm alarm)
        {
            DetailAlarm detailAlarm = new DetailAlarm();
            detailAlarm.Initialize(alarm);
            DetailAlarms.Add(detailAlarm);
            UpdateMainAlarms();
        }

        public void RemoveAlarm(DetailAlarm detailAlarm)
        {
            DetailAlarms.Remove(detailAlarm);
            UpdateMainAlarms();
        }

        void UpdateMainAlarms()
        {
            foreach (MainAlarm mainAlarm in MainAlarms)
            {
                mainAlarm.Update();
            }
        }

        object content;
        public object Content
        {
            get { return content; }
            set
            {
                content = value;
                OnPropertyChanged("Content");
            }
        }

        public ObservableCollection<MainAlarm> MainAlarms { get; set; }

        ObservableCollection<DetailAlarm> detailAlarms;
        public ObservableCollection<DetailAlarm> DetailAlarms
        {
            get { return detailAlarms; }
            set
            {
                detailAlarms = value;
                OnPropertyChanged("DetailAlarms");
            }
        }
        
    }
}
