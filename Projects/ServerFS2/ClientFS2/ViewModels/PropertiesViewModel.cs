using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace ClientFS2.ViewModels
{
    class PropertiesViewModel: DialogViewModel
    {
        public PropertiesViewModel(List<Property> properties)
        {
            Title = "Список свойств";
            PropertiesInitialize(properties);
        }
        void PropertiesInitialize(List<Property> properties)
        {
            Properties = new ObservableCollection<Property>();
            foreach (var property in properties)
            {
                Properties.Add(property);
            }
            OnPropertyChanged("Properties");
        }
        public ObservableCollection<Property> Properties { get; set; }
    }
}
