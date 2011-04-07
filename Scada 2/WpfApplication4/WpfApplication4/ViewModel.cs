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
            MainAlarms.Add(new MainAlarm() { Name = "Пожарная тревога" });
            MainAlarms.Add(new MainAlarm() { Name = "Внимание" });
            MainAlarms.Add(new MainAlarm() { Name = "Неисправность" });
            MainAlarms.Add(new MainAlarm() { Name = "Отключенное оборудование" });
            MainAlarms.Add(new MainAlarm() { Name = "Информационное сообщение" });
            MainAlarms.Add(new MainAlarm() { Name = "Требуется обслуживание" });
            MainAlarms.Add(new MainAlarm() { Name = "Автоматика" });

            DetailAlarms = new ObservableCollection<DetailAlarm>();
            DetailAlarms.Add(new DetailAlarm() { Name = "Событие 1" });
            DetailAlarms.Add(new DetailAlarm() { Name = "Событие 2" });
            DetailAlarms.Add(new DetailAlarm() { Name = "Событие 3" });
        }

        public static ViewModel Current { get; set; }

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
