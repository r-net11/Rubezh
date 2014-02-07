using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnDetailsViewModel : SaveCancelDialogViewModel
	{
		public AdditionalColumn AdditionalColumn { get; private set; }

		public AdditionalColumnDetailsViewModel(AdditionalColumn additionalColumn = null)
		{
			if (additionalColumn == null)
			{
				Title = "Создание дополнительной колонки";
				additionalColumn = new AdditionalColumn()
				{
					Name = "Новая дополнительная колонка",
				};
			}
			else
			{
				Title = string.Format("Дополнительная колонка: {0}", additionalColumn.Name);
			}
			AdditionalColumn = additionalColumn;
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = AdditionalColumn.Name;
			Description = AdditionalColumn.Description;
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

		protected override bool CanSave()
		{
			return true;
		}

		protected override bool Save()
		{
			AdditionalColumn = new AdditionalColumn()
			{
				Name = Name,
				Description = Description
			};
			return true;
		}
	}
}