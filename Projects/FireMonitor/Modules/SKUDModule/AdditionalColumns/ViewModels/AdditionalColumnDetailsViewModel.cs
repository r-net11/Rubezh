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
	public class AdditionalColumnDetailsViewModel : SaveCancelDialogViewModel
	{
		AdditionalColumnsViewModel AdditionalColumnsViewModel;
		public AdditionalColumn AdditionalColumn { get; private set; }

		public AdditionalColumnDetailsViewModel(AdditionalColumnsViewModel additionalColumnsViewModel, AdditionalColumn additionalColumn = null)
		{
			AdditionalColumnsViewModel = additionalColumnsViewModel;
			if (additionalColumn == null)
			{
				Title = "Создание дополнительной колонки";
				additionalColumn = new AdditionalColumn()
				{
					Name = "Новая дополнительная колонка",
				};
				CanChangeAdditionalColumnType = true;
			}
			else
			{
				Title = string.Format("Дополнительная колонка: {0}", additionalColumn.Name);
				CanChangeAdditionalColumnType = false;
			}
			AdditionalColumn = additionalColumn;
			AvailableAdditionalColumnTypes = new ObservableCollection<AdditionalColumnType>(Enum.GetValues(typeof(AdditionalColumnType)).Cast<AdditionalColumnType>());
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = AdditionalColumn.Name;
			Description = AdditionalColumn.Description;
			AdditionalColumnType = AvailableAdditionalColumnTypes.FirstOrDefault(x => x == AdditionalColumn.Type);
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

		public ObservableCollection<AdditionalColumnType> AvailableAdditionalColumnTypes { get; private set; }

		AdditionalColumnType _additionalColumnType;
		public AdditionalColumnType AdditionalColumnType
		{
			get { return _additionalColumnType; }
			set
			{
				_additionalColumnType = value;
				OnPropertyChanged("AdditionalColumnType");
			}
		}

		public bool CanChangeAdditionalColumnType { get; private set; }

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (AdditionalColumnsViewModel.AdditionalColumns.Any(x => x.AdditionalColumn.Name == Name && x.AdditionalColumn.UID != AdditionalColumn.UID))
			{
				MessageBoxService.ShowWarning("Серия и номер карты совпадает с введеннымы ранее");
				return false;
			}

			AdditionalColumn = new AdditionalColumn()
			{
				Name = Name,
				Description = Description,
				Type = AdditionalColumnType
			};
			return true;
		}
	}
}