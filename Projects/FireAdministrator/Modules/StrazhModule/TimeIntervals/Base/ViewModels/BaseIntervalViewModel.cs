using Controls;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Localization.Strazh.Common;
using Localization.Strazh.ViewModels;

namespace StrazhModule.Intervals.Base.ViewModels
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
			get { return _isActive && !IsPredefined; }
			set
			{
				_isActive = value;
				OnPropertyChanged(() => IsActive);
				Activate();
			}
		}

		public string ActivateActionTitle
		{
			get { return IsActive ? CommonViewModels.Deactivate : CommonResources.Activate; }
		}
		public string ActivateActionImage
		{
			get { return GetActiveImage(!IsActive, false); }
		}

		public string ActiveTitle
		{
            get { return IsActive ? CommonViewModels.Active : CommonViewModels.Inactive; }
		}
		public string ActiveImage
		{
			get { return GetActiveImage(!IsActive, false); }
		}

		public string GetActiveImage(bool isActive, bool isBlack)
		{
			var map = isActive ? "Map" : "MapOff";
			if (isBlack)
				map = "B" + map;
			return map;
		}

		public virtual void Update()
		{
			OnPropertyChanged(() => ActivateActionTitle);
			OnPropertyChanged(() => ActivateActionImage);
			OnPropertyChanged(() => ActiveTitle);
			OnPropertyChanged(() => ActiveImage);
			OnPropertyChanged(() => IsActive);
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

		public virtual bool IsPredefined
		{
			get { return false; }
		}
	}
}