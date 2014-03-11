using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD.PassCardLibrary;
using Infrastructure.Designer.ElementProperties.ViewModels;
using System.Windows.Media;

namespace SKDModule.PassCard.ViewModels
{
	public class PassCardImagePropertyViewModel : RectanglePropertiesViewModel
	{
		public PassCardImagePropertyViewModel(ElementPassCardImageProperty element)
			: base(element)
		{
			Title = "Свойства фигуры: Графическое свойство";
			PropertyTypes = new ObservableCollection<PassCardImagePropertyType>(Enum.GetValues(typeof(PassCardImagePropertyType)).Cast<PassCardImagePropertyType>());
			StretchTypes = new ObservableCollection<Stretch>(Enum.GetValues(typeof(Stretch)).Cast<Stretch>());
		}

		public ObservableCollection<PassCardImagePropertyType> PropertyTypes { get; private set; }
		private PassCardImagePropertyType _selectedPropertyType;
		public PassCardImagePropertyType SelectedPropertyType
		{
			get { return _selectedPropertyType; }
			set
			{
				_selectedPropertyType = value;
				OnPropertyChanged(() => SelectedPropertyType);
			}
		}

		public ObservableCollection<Stretch> StretchTypes { get; private set; }
		private Stretch _selectedStretch;
		public Stretch SelectedStretch
		{
			get { return _selectedStretch; }
			set
			{
				_selectedStretch = value;
				OnPropertyChanged(() => SelectedStretch);
			}
		}

		protected override void CopyProperties()
		{
			base.CopyProperties();
			SelectedPropertyType = ((ElementPassCardImageProperty)ElementRectangle).PropertyType;
		}

		protected override bool Save()
		{
			//Text = SelectedPropertyType.ToDescription();
			((ElementPassCardImageProperty)ElementRectangle).PropertyType = SelectedPropertyType;
			return base.Save();
		}
	}
}
