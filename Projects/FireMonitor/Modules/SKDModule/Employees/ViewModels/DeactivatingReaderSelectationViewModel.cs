using System;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class DeactivatingReaderSelectationViewModel : TreeNodeViewModel<DeactivatingReaderSelectationViewModel>
	{
		private readonly bool _isReader;
		private Guid _uid;
		private bool _isChecked;
		private string _name;
		private string _zoneName;

		public DeactivatingReaderSelectationViewModel(bool isReader)
		{
			_isReader = isReader;
		}

		/// <summary>
		/// Идентификатор точки доступа / считывателя
		/// </summary>
		public Guid UID
		{
			get { return _uid; }
			set
			{
				if (_uid == value)
					return;
				_uid = value;
				OnPropertyChanged(() => UID);
			}
		}

		public bool IsReader
		{
			get { return _isReader; }
		}

		/// <summary>
		/// Метка выбора на считывателе
		/// </summary>
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				if (_isChecked == value)
					return;
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		/// <summary>
		/// Название точки доступа / считывателя
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

		/// <summary>
		/// Название зоны, куда ведет считыватель
		/// </summary>
		public string ZoneName
		{
			get { return _zoneName; }
			set
			{
				if (_zoneName == value)
					return;
				_zoneName = value;
				OnPropertyChanged(() => ZoneName);
			}
		}
	}
}