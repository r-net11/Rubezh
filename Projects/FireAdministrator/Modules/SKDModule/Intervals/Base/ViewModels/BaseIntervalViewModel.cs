using Controls;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace SKDModule.Intervals.Base.ViewModels
{
	public class BaseIntervalViewModel<TPart, TModel> : BaseViewModel
		where TPart : BaseIntervalPartViewModel
	{
		public int Index { get; private set; }
		public TModel Model { get; protected set; }

		public BaseIntervalViewModel(int index, TModel model)
		{
			Model = model;
			Index = index;
			_isActive = Model != null;
		}

		public ObservableCollection<TPart> Parts { get; protected set; }

		private TPart _selectedPart;
		public TPart SelectedPart
		{
			get { return _selectedPart; }
			set
			{
				_selectedPart = value;
				OnPropertyChanged(() => SelectedPart);
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		private bool _isActive;
		public bool IsActive
		{
			get { return IsDefault ? true : _isActive; }
			set
			{
				_isActive = IsDefault ? true : value;
				OnPropertyChanged(() => IsActive);
				Activate();
			}
		}

		public bool IsEnabled
		{
			get { return !IsDefault && IsActive; }
		}
		public bool IsDefault
		{
			get { return Index == 0; }
		}

		public string ActivateActionTitle
		{
			get { return IsActive ? "Деактивировать" : "Активировать"; }
		}
		public string ActivateActionImage
		{
			get { return GetActiveImage(!IsActive, false); }
		}

		public string ActiveTitle
		{
			get { return IsActive ? "Активно" : "Не активно"; }
		}
		public string ActiveImage
		{
			get { return GetActiveImage(!IsActive, false); }
		}

		public string GetActiveImage(bool isActive, bool isBlack)
		{
			var map = isActive ? "MapOn" : "MapOff";
			if (isBlack)
				map = "B" + map;
			return map.ToImagePath();
		}

		public virtual void Update()
		{
			OnPropertyChanged(() => ActivateActionTitle);
			OnPropertyChanged(() => ActivateActionImage);
			OnPropertyChanged(() => ActiveTitle);
			OnPropertyChanged(() => ActiveImage);
			OnPropertyChanged(() => IsEnabled);
			OnPropertyChanged(() => Model);
			OnPropertyChanged(() => Parts);
		}
		protected virtual void Activate()
		{
			Update();
		}

		public virtual void Paste(TModel source)
		{
		}
	}
}
