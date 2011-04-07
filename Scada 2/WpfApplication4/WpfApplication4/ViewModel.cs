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
            MainAlarms.Add(new MainAlarm() { Name = "Пожарная тревога", Color="Red" });
            MainAlarms.Add(new MainAlarm() { Name = "Внимание", Color = "Yellow" });
            MainAlarms.Add(new MainAlarm() { Name = "Неисправность", Color = "White" });
            MainAlarms.Add(new MainAlarm() { Name = "Отключенное оборудование", Color = "SkyBlue" });
            MainAlarms.Add(new MainAlarm() { Name = "Информационное сообщение", Color = "Green" });
            MainAlarms.Add(new MainAlarm() { Name = "Требуется обслуживание", Color = "YellowGreen" });
            MainAlarms.Add(new MainAlarm() { Name = "Автоматика", Color = "Green" });

            DetailAlarms = new ObservableCollection<DetailAlarm>();
            DetailAlarms.Add(new DetailAlarm() { Name = "Событие 1", Color = "Red" });
            DetailAlarms.Add(new DetailAlarm() { Name = "Событие 2", Color = "Yellow" });
            DetailAlarms.Add(new DetailAlarm() { Name = "Событие 3", Color = "Green" });
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
