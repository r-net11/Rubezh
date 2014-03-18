using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeDetailsViewModel : SaveCancelDialogViewModel
	{
		OrganisationAdditionalColumnTypesViewModel OrganisationAdditionalColumnTypesViewModel;
		public AdditionalColumnType AdditionalColumnType { get; private set; }

		public AdditionalColumnTypeDetailsViewModel(OrganisationAdditionalColumnTypesViewModel organisationAdditionalColumnsViewModel, AdditionalColumnType additionalColumnType = null)
		{
			OrganisationAdditionalColumnTypesViewModel = organisationAdditionalColumnsViewModel;
			if (additionalColumnType == null)
			{
				Title = "Создание дополнительной колонки";
				additionalColumnType = new AdditionalColumnType()
				{
					Name = "Новая дополнительная колонка",
				};
				CanChangeDataType = true;
			}
			else
			{
				Title = string.Format("Дополнительная колонка: {0}", additionalColumnType.Name);
				CanChangeDataType = false;
			}
			AdditionalColumnType = additionalColumnType;
			AvailableDataTypes = new ObservableCollection<DataType>(Enum.GetValues(typeof(DataType)).Cast<DataType>());
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = AdditionalColumnType.Name;
			Description = AdditionalColumnType.Description;
			DataType = AvailableDataTypes.FirstOrDefault(x => x == AdditionalColumnType.DataType);
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
					OnPropertyChanged("Name");
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
					OnPropertyChanged("Description");
				}
			}
		}

		public ObservableCollection<DataType> AvailableDataTypes { get; private set; }

		DataType _dataType;
		public DataType DataType
		{
			get { return _dataType; }
			set
			{
				_dataType = value;
				OnPropertyChanged(()=>DataType);
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
			if (OrganisationAdditionalColumnTypesViewModel.AdditionalColumnTypes.Any(x => x.AdditionalColumnType.Name == Name && x.AdditionalColumnType.UID != AdditionalColumnType.UID))
			{
				MessageBoxService.ShowWarning("Название дополнительной колонки совпадает с введенным ранее");
				return false;
			}

			AdditionalColumnType.Name = Name;
			AdditionalColumnType.Description = Description;
			AdditionalColumnType.DataType = DataType;
			AdditionalColumnType.OrganizationUID = OrganisationAdditionalColumnTypesViewModel.Organization.UID;
			return true;
		}
	}
}