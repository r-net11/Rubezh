using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Ink;
using RubezhAPI;
using RubezhAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Practices.Prism;

namespace FiltersModule.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        public JournalFilter Filter { get; private set; }
        public ObservableCollection<NameViewModel> Names { get; private set; }
        public ObservableCollection<ObjectViewModel> Objects { get; private set; }
       

        public FilterViewModel(JournalFilter filter)
		{
			Filter = filter;
            Names= new ObservableCollection<NameViewModel>();
            Objects= new ObservableCollection<ObjectViewModel>();
            SetFilterMenu();      
		}
      
        public void Update(JournalFilter filter)
		{
			Filter = filter;
            OnPropertyChanged(() => Filter);
            Names.Clear();
            Objects.Clear();
            SetFilterMenu();
		}

        void SetFilterMenu( )
        {

			ObjectsViewModel _objectsViewModel = new ObjectsViewModel(Filter);

            foreach (var count in Filter.JournalSubsystemTypes)
            {
                Names.Add(new NameViewModel(count));
            }
           
            foreach (var count in Filter.JournalEventNameTypes)
            {
                Names.Add(new NameViewModel(count));
            }

			foreach (var count in Filter.JournalEventDescriptionTypes)
            {
                Names.Add(new NameViewModel(count, count.ToDescription()));
            }
           
			Objects = new ObservableCollection<ObjectViewModel>(_objectsViewModel.AllObjects.Where(c => c.IsRealChecked));
                
			OnPropertyChanged(() => Objects);
        }
    }
}