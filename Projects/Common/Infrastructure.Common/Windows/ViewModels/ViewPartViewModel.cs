
namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class ViewPartViewModel : BaseViewModel, IViewPartViewModel
	{
		#region IViewPartViewModel Members

		public virtual void OnShow()
		{
		}

		public virtual void OnHide()
		{
		}

		public virtual string Key
		{
			get { return GetType().FullName; }
		}

		#endregion

		private bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			private set
			{
				_isActive = value;
				OnPropertyChanged("IsActive");
			}
		}

		internal void Show()
		{
			IsActive = true;
			OnShow();
		}
		internal void Hide()
		{
			if (IsActive)
				OnHide();
			IsActive = false;
		}
	}
}
