using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation { get; set; }
		public AdditionalColumnType AdditionalColumnType { get; private set; }
		public ShortAdditionalColumnType ShortAdditionalColumnType
		{
			get
			{
				return new ShortAdditionalColumnType
				{
					UID = AdditionalColumnType.UID,
					Name = AdditionalColumnType.Name,
					Description = AdditionalColumnType.Description,
					OrganisationUID = AdditionalColumnType.OrganisationUID
				};
			}
		}

		public AdditionalColumnTypeDetailsViewModel(Organisation orgnaisation, Guid? additionalColumnTypeUID = null)
		{
			Organisation = orgnaisation;
			if (additionalColumnTypeUID == null)
			{
				Title = "Создание дополнительной колонки";
				AdditionalColumnType = new AdditionalColumnType()
				{
					Name = "Новая дополнительная колонка",
					OrganisationUID = Organisation.UID
				};
			}
			else
			{
				AdditionalColumnType = AdditionalColumnTypeHelper.GetDetails(additionalColumnTypeUID);
				Title = string.Format("Свойства дополнительной колонки: {0}", AdditionalColumnType.Name);
			}
			AvailableDataTypes = new ObservableCollection<AdditionalColumnDataType>(Enum.GetValues(typeof(AdditionalColumnDataType)).OfType<AdditionalColumnDataType>());
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = AdditionalColumnType.Name;
			Description = AdditionalColumnType.Description;
			DataType = AvailableDataTypes.FirstOrDefault(x => x == AdditionalColumnType.DataType);
			if (IsTextType)
				IsInGrid = AdditionalColumnType.IsInGrid;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(() => Name);
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged(() => Description);
				}
			}
		}

		public ObservableCollection<AdditionalColumnDataType> AvailableDataTypes { get; private set; }

		AdditionalColumnDataType _dataType;
		public AdditionalColumnDataType DataType
		{
			get { return _dataType; }
			set
			{
				_dataType = value;
				OnPropertyChanged(() => DataType);
				OnPropertyChanged(() => IsTextType);
			}
		}

		public bool IsTextType { get { return DataType == AdditionalColumnDataType.Text; } }

		bool _isInGrid;
		public bool IsInGrid
		{
			get { return _isInGrid; }
			set
			{
				_isInGrid = value;
				OnPropertyChanged(() => IsInGrid);
			}
		}

		public bool CanChangeDataType { get; private set; }

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (Name == "Имя" || Name == "Фамилия" || Name == "Отчество")
			{
				MessageBoxService.ShowWarning("Запрещенное название дополнительной колонки");
				return false;
			}

			AdditionalColumnType.Name = Name;
			AdditionalColumnType.Description = Description;
			AdditionalColumnType.DataType = DataType;
			if (IsTextType)
				AdditionalColumnType.IsInGrid = IsInGrid;
			AdditionalColumnType.OrganisationUID = Organisation.UID;
			return AdditionalColumnTypeHelper.Save(AdditionalColumnType);
		}
	}
}