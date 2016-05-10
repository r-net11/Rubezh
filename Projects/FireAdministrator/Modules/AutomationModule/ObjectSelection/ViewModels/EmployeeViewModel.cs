using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.SKD;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class EmployeeViewModel : TreeNodeViewModel<EmployeeViewModel>
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

		private string _tabelNo;
		/// <summary>
		/// Табельный номер сотрудника
		/// </summary>
		public string TabelNo
		{
			get { return _tabelNo; }
			set
			{
				if (_tabelNo == value)
					return;
				_tabelNo = value;
				OnPropertyChanged(() => TabelNo);
			}
		}

		public bool IsEmployee { get; private set; }

		public EmployeeViewModel(ShortEmployee employee)
		{
			Uid = employee.UID;
			Name = employee.Name;
			TabelNo = employee.TabelNo;
			IsEmployee = true;
		}

		public EmployeeViewModel(Organisation organisation)
		{
			Uid = organisation.UID;
			Name = organisation.Name;
			IsEmployee = false;
		}
	}
}
