using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ConsumersFolderDetailsViewModel : SaveCancelDialogViewModel
	{
		Consumer _consumer;
		public Consumer Consumer
		{
			get { return _consumer; }
			set
			{
				_consumer = value;
				if (_consumer != null)
				{
					Name = _consumer.Name;
					Description = _consumer.Description;
				}
			}
		}

		public ConsumersFolderDetailsViewModel(Consumer consumer, bool isNew = false, bool isReadOnly = false)
		{
			Title = isNew ? "Создание группы абонентов" : "Редактирование группы абонентов";
			Consumer = consumer;
			IsReadOnly = isReadOnly;
		}

		bool _isReadOnly;
		public bool IsReadOnly
		{
			get { return _isReadOnly; }
			set
			{
				_isReadOnly = value;
				OnPropertyChanged(() => IsReadOnly);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		protected override bool Save()
		{
			Consumer.Name = Name;
			Consumer.Description = Description;

			DBCash.SaveConsumer(Consumer);

			return base.Save();
		}
	}
}