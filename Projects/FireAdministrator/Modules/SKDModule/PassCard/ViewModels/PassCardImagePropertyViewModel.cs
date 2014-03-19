using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD.PassCardLibrary;
using Infrastructure.Designer.ElementProperties.ViewModels;
using System.Windows.Media;
using FiresecAPI;
using FiresecClient.SKDHelpers;

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
				OnPropertyChanged(() => IsAdditionalColumn);
				if (SelectedPropertyType != PassCardImagePropertyType.Additional)
					SelectedAdditionalColumnType = null;
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

		public ObservableCollection<AdditionalColumnType> AdditionalColumnTypes { get; private set; }
		private AdditionalColumnType _selectedAdditionalColumnType;
		public AdditionalColumnType SelectedAdditionalColumnType
		{
			get { return _selectedAdditionalColumnType; }
			set
			{
				_selectedAdditionalColumnType = value;
				OnPropertyChanged(() => SelectedAdditionalColumnType);
			}
		}

		public bool IsAdditionalColumn
		{
			get { return SelectedPropertyType == PassCardImagePropertyType.Additional; }
		}

		protected override void CopyProperties()
		{
			var filter = new AdditionalColumnTypeFilter()
			{
				WithDeleted = DeletedType.Not,
				Type = DataType.Graphics,
			};
			AdditionalColumnTypes = new ObservableCollection<AdditionalColumnType>(AdditionalColumnTypeHelper.Get(filter));
			base.CopyProperties();
			SelectedPropertyType = ((ElementPassCardImageProperty)ElementRectangle).PropertyType;
			SelectedAdditionalColumnType = SelectedPropertyType == PassCardImagePropertyType.Additional ? AdditionalColumnTypes.FirstOrDefault(item => item.UID == ((ElementPassCardImageProperty)ElementRectangle).AdditionalColumn) : null;
		}
		protected override bool Save()
		{
			var element = (ElementPassCardImageProperty)ElementRectangle;
			element.Text = SelectedPropertyType.ToDescription();
			if (SelectedPropertyType == PassCardImagePropertyType.Additional)
				element.Text += string.Format("({0})", SelectedAdditionalColumnType == null ? string.Empty : SelectedAdditionalColumnType.Name);
			element.PropertyType = SelectedPropertyType;
			element.AdditionalColumn = SelectedAdditionalColumnType == null ? Guid.Empty : SelectedAdditionalColumnType.UID;
			return base.Save();
		}
	}
}