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
		public ConsumersFolderDetailsViewModel(Consumer consumer, bool isReadOnly, bool isNew = false)
		{
			Title = isNew ? "Создание группы абонентов" : "Редактирование группы абонентов";
			Update(consumer);
			IsReadOnly = isReadOnly;
		}

		public bool IsReadOnly { get; private set; }

		public Guid Uid { get; private set; }
		public Guid? ParentUid { get; private set; }

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

		public void Update(Consumer consumer)
		{
			Uid = consumer.UID;
			ParentUid = consumer.ParentUID;
			Name = consumer.Name;
			Description = consumer.Description;
		}

		public Consumer GetConsumer()
		{
			return new Consumer
			{
				Description = this.Description,
				IsFolder = true,
				Name = this.Name,
				ParentUID = this.ParentUid,
				UID = this.Uid
			};
		}
		
		protected override bool Save()
		{
			DbCache.SaveConsumer(GetConsumer());
			return base.Save();
		}
	}
}