﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Designer.ElementProperties.ViewModels;

namespace SKDModule.PassCardDesigner.ViewModels
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

		public ObservableCollection<ShortAdditionalColumnType> AdditionalColumnTypes { get; private set; }
		private ShortAdditionalColumnType _selectedAdditionalColumnType;
		public ShortAdditionalColumnType SelectedAdditionalColumnType
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
				LogicalDeletationType = LogicalDeletationType.Active,
				Type = AdditionalColumnDataType.Graphics,
				OrganisationUIDs = OrganisationHelper.Get(new OrganisationFilter()).Select(x => x.UID).ToList()
			};
			AdditionalColumnTypes = new ObservableCollection<ShortAdditionalColumnType>(AdditionalColumnTypeHelper.Get(filter));
			base.CopyProperties();
			var element = (ElementPassCardImageProperty)ElementRectangle;
			SelectedPropertyType = element.PropertyType;
			SelectedAdditionalColumnType = SelectedPropertyType == PassCardImagePropertyType.Additional ? AdditionalColumnTypes.FirstOrDefault(item => item.UID == element.AdditionalColumnUID) : null;
			SelectedStretch = element.Stretch;
		}
		protected override bool Save()
		{
			var element = (ElementPassCardImageProperty)ElementRectangle;
			element.Text = SelectedPropertyType.ToDescription();
			if (SelectedPropertyType == PassCardImagePropertyType.Additional)
				element.Text += string.Format("\n({0})", SelectedAdditionalColumnType == null ? string.Empty : SelectedAdditionalColumnType.Name);
			element.PropertyType = SelectedPropertyType;
			element.AdditionalColumnUID = SelectedAdditionalColumnType == null ? Guid.Empty : SelectedAdditionalColumnType.UID;
			element.Stretch = SelectedStretch;
			return base.Save();
		}
	}
}