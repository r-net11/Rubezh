using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace ClientFS2.ViewModels
{
	class PropertiesViewModel : DialogViewModel
	{
		public PropertiesViewModel(List<Property> properties)
		{
			Title = "Список свойств";

			Properties = new ObservableCollection<Property>();
			foreach (var property in properties)
			{
				Properties.Add(property);
			}
		}
		public ObservableCollection<Property> Properties { get; set; }
	}
}