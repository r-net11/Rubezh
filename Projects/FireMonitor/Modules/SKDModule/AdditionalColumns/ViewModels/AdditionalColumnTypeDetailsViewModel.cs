using System;
using System.Collections.ObjectModel;
using System.Linq;
using Localization.SKD.Common;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortAdditionalColumnType>
	{
		Organisation Organisation { get; set; }
		bool _isNew;
		public AdditionalColumnType AdditionalColumnType { get; private set; }
		public ShortAdditionalColumnType Model
		{
			get
			{
				return new ShortAdditionalColumnType
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

		public bool Initialize(Organisation organisation, ShortAdditionalColumnType model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			_isNew = model == null;
			if (_isNew)
			{
				Title = CommonViewModels.CreateAdditionalColumn;
				AdditionalColumnType = new AdditionalColumnType()
				{
					Name = CommonViewModels.NewAdditionalColumn,
					OrganisationUID = Organisation.UID
				};
				CanChangeDataType = true;
			}
			else
			{
				AdditionalColumnType = AdditionalColumnTypeHelper.GetDetails(model.UID);
				Title = string.Format(CommonViewModels.AdditionalColumnProperties, AdditionalColumnType.Name);
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
			if (Name == CommonResources.SecondName || Name == CommonResources.FirstName || Name == CommonResources.ThirdName)
			{
				MessageBoxService.ShowWarning(CommonViewModels.AdditionalColumnForbiddenName);
				return false;
			}

			AdditionalColumnType.Name = Name;
			AdditionalColumnType.Description = Description;
			AdditionalColumnType.DataType = DataType;
			AdditionalColumnType.IsInGrid = IsInGrid;
			AdditionalColumnType.OrganisationUID = Organisation.UID;

			var saveResult = AdditionalColumnTypeHelper.Save(AdditionalColumnType, _isNew);
			if (saveResult)
			{
				ServiceFactory.Events.GetEvent<EditAdditionalColumnEvent>().Publish(null);
			}
			return saveResult;
		}
	}
}