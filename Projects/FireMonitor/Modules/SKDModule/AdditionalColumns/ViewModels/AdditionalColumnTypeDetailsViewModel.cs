using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<AdditionalColumnType>
	{
		Organisation Organisation { get; set; }
		bool _isNew;
		public AdditionalColumnType AdditionalColumnType { get; private set; }
		public AdditionalColumnType Model
		{
			get
			{
				return new AdditionalColumnType
				{
					UID = AdditionalColumnType.UID,
					Name = AdditionalColumnType.Name,
					Description = AdditionalColumnType.Description,
					OrganisationUID = AdditionalColumnType.OrganisationUID, 
					IsInGrid = AdditionalColumnType.IsInGrid,
					DataType = AdditionalColumnType.DataType
				};
			}
		}

		public AdditionalColumnTypeDetailsViewModel() { }

		public bool Initialize(Organisation organisation, AdditionalColumnType model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			_isNew = model == null;
			if (_isNew)
			{
				Title = "Создание дополнительной колонки";
				AdditionalColumnType = new AdditionalColumnType()
				{
					Name = "Новая дополнительная колонка",
					OrganisationUID = Organisation.UID
				};
				CanChangeDataType = true;
			}
			else
			{
				AdditionalColumnType = AdditionalColumnTypeHelper.GetDetails(model.UID);
				Title = string.Format("Свойства дополнительной колонки: {0}", AdditionalColumnType.Name);
			}
			AvailableDataTypes = new ObservableCollection<AdditionalColumnDataType>(Enum.GetValues(typeof(AdditionalColumnDataType)).OfType<AdditionalColumnDataType>());
			CopyProperties();
			return true;
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
			AdditionalColumnType.IsInGrid = IsInGrid;
			AdditionalColumnType.OrganisationUID = Organisation.UID;
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			var saveResult = AdditionalColumnTypeHelper.Save(AdditionalColumnType, _isNew);
			if (saveResult)
			{
				ServiceFactory.Events.GetEvent<EditAdditionalColumnEvent>().Publish(null);
			}
			return saveResult;
		}
	}
}