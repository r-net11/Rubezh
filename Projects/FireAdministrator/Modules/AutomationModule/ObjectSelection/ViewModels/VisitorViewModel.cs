using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class VisitorViewModel : TreeNodeViewModel<VisitorViewModel>
	{
		private Guid _uid;
		/// <summary>
		/// Идентификатор объекта
		/// </summary>
		public Guid Uid
		{
			get { return _uid; }
			set
			{
				if (_uid == value)
					return;
				_uid = value;
				OnPropertyChanged(() => Uid);
			}
		}

		private string _name;
		/// <summary>
		/// Название организации / ФОИ сотрудника
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name == value)
					return;
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public bool IsVisitor { get; private set; }

		public VisitorViewModel(ShortEmployee employee)
		{
			Uid = employee.UID;
			Name = employee.Name;
			IsVisitor = true;
		}

		public VisitorViewModel(Organisation organisation)
		{
			Uid = organisation.UID;
			Name = organisation.Name;
			IsVisitor = false;
		}
	}
}
