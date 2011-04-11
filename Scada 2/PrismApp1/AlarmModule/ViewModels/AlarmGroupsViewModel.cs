using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure;

namespace AlarmModule.ViewModels
{
    public class AlarmGroupsViewModel : RegionViewModel
    {
        public AlarmGroupsViewModel()
        {
            MainAlarms = new ObservableCollection<AlarmGroupViewModel>();
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Пожар", AlarmType = AlarmType.Alarm });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Внимание", AlarmType = AlarmType.Attention });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Неисправность", AlarmType = AlarmType.Failure });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Отключение", AlarmType = AlarmType.Off });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Информация", AlarmType = AlarmType.Info });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Обслуживание", AlarmType = AlarmType.Service });
            MainAlarms.Add(new AlarmGroupViewModel() { Name = "Автоматика", AlarmType = AlarmType.Auto });
        }

        public ObservableCollection<AlarmGroupViewModel> MainAlarms { get; set; }

        public override void Dispose()
        {
        }
    }
}
