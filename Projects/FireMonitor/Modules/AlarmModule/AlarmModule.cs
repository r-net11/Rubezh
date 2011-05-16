using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure.Common;
using Infrastructure;
using AlarmModule.ViewModels;
using AlarmModule.Imitator;

namespace AlarmModule
{
    public class AlarmModule : IModule
    {
        public void Initialize()
        {
            RegisterResources();

            AlarmGroupListViewModel alarmGroupListViewModel = new AlarmGroupListViewModel();
            ServiceFactory.Layout.AddAlarmGroups(alarmGroupListViewModel);

            //ShowImitatorView();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void ShowImitatorView()
        {
            AlarmImitatorView alarmImitatorView = new AlarmImitatorView();
            AlarmImitatorViewModel alarmImitatorViewModel = new AlarmImitatorViewModel();
            alarmImitatorView.DataContext = alarmImitatorViewModel;
            alarmImitatorView.Show();
        }
    }
}
