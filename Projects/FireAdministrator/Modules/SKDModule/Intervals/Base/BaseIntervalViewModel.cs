using Controls;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.Intervals.Base
{
	public class BaseIntervalViewModel : BaseViewModel
	{
		public int Index { get; private set; }

		public BaseIntervalViewModel(int index)
		{
			Index = index;
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
		}
		protected virtual void Activate()
		{
			Update();
		}
	}
}
