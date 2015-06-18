using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Ink;
using FiresecAPI;
using FiresecAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Practices.Prism;

namespace FiltersModule.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        public JournalFilter Filter { get; private set; }
        public ObservableCollection<NameViewModel> Names { get; private set; }
        public ObservableCollection<ObjectViewModel> Objects { get; private set; }
        private ObjectsViewModel _objectsViewModel;

        public FilterViewModel(JournalFilter filter)
		{
			Filter = filter;
            _objectsViewModel = new ObjectsViewModel(filter);
            Names= new ObservableCollection<NameViewModel>();
            Objects= new ObservableCollection<ObjectViewModel>();
            SetFilterMenu();      
		}
      
        public void Update(JournalFilter filter)
		{
			Filter = filter;
            _objectsViewModel = new ObjectsViewModel(filter);
			OnPropertyChanged(() => Filter);
            Names.Clear();
            Objects.Clear();
            SetFilterMenu();
		}

        private void SetFilterMenu( )
        {
            foreach (var count in Filter.JournalSubsystemTypes)
                {
                   //if (string.Compare(count.ToDescription(),"Видео",false, new CultureInfo("Ru-ru")) != 0)
                   Names.Add(new NameViewModel(count));
                }
           
            foreach (var count in Filter.JournalEventNameTypes)
                {
                   Names.Add(new NameViewModel(count));
                }
               
            foreach (var count in Filter.JournalEventDescriptionTypes)
                {
                   Names.Add(new NameViewModel(count,count.ToDescription()));
                }
           
            //foreach (var item in _objectsViewModel.RootObjects)
            //{
            //    if (item.IsRealChecked)
            //    {
            //        ObjectJournalObjectType.Add(item);
            //    }
            //}

            //foreach (var item in _objectsViewModel.AllObjects)
            //{
            //    if (item.IsRealChecked)
            //    {
            //        ObjectJournalObjectType.Add(item);
            //    }
            //}
            Objects = new ObservableCollection<ObjectViewModel>(_objectsViewModel.AllObjects.Where(c => c.IsRealChecked));
                //.AddRange(_objectsViewModel.RootObjects.Where(c => c.IsRealChecked));
            OnPropertyChanged(() => Objects);
        }
    }
}