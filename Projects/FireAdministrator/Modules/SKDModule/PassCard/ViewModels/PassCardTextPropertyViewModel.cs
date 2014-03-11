using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD.PassCardLibrary;
using Infrastructure.Designer.ElementProperties.ViewModels;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardTextPropertyViewModel : TextBlockPropertiesViewModel
	{
		public PassCardTextPropertyViewModel(ElementPassCardTextProperty element)
			: base(element)
		{
			Title = "Свойства фигуры: Текстовое свойство";
			PropertyTypes = new ObservableCollection<PassCardTextPropertyType>(Enum.GetValues(typeof(PassCardTextPropertyType)).Cast<PassCardTextPropertyType>());
		}

		public ObservableCollection<PassCardTextPropertyType> PropertyTypes { get; private set; }
		private PassCardTextPropertyType _selectedPropertyType;
		public PassCardTextPropertyType SelectedPropertyType
		{
			get { return _selectedPropertyType; }
			set
			{
				_selectedPropertyType = value;
				OnPropertyChanged(() => SelectedPropertyType);
			}
		}


		protected override void CopyProperties()
		{
			base.CopyProperties();
			SelectedPropertyType = ((ElementPassCardTextProperty)ElementTextBlock).PropertyType;
		}

		protected override bool Save()
		{
			Text = SelectedPropertyType.ToDescription();
			((ElementPassCardTextProperty)ElementTextBlock).PropertyType = SelectedPropertyType;
			return base.Save();
		}
	}
}
