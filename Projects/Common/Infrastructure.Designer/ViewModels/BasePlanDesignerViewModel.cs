using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.ViewModels;
using System;

namespace Infrastructure.Designer.ViewModels
{
	public partial class BasePlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		public event EventHandler Updated;
		public event EventHandler IsCollapsedChanged;
		public BaseDesignerCanvas DesignerCanvas { get; set; }
		public bool AllowScalePoint { get; protected set; }

		private bool _isNotEmpty;
		public bool IsNotEmpty
		{
			get { return _isNotEmpty; }
			protected set
			{
				_isNotEmpty = value;
				OnPropertyChanged(() => IsNotEmpty);
			}
		}
		public bool AllowChangePlanZoom
		{
			get { return true; }
		}
		public bool ShowZoomSliders
		{
			get { return true; }
		}
		public bool FullScreenSize { get; protected set; }

		public BasePlanDesignerViewModel()
		{
			IsNotEmpty = false;
			CanCollapse = true;
			InitializeHistory();
			InitializeZIndexCommands();
			InitializeAlignCommands();
			InitializeCopyPaste();

			ServiceFactoryBase.Events.GetEvent<ShowElementEvent>().Subscribe(OnShowElement);
		}

		public void Update()
		{
			OnUpdated();
		}
		private void OnUpdated()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}

		public virtual void RegisterDesignerItem(DesignerItem designerItem)
		{
		}

		#region IPlanDesignerViewModel Members

		public object Toolbox
		{
			get { return DesignerCanvas.Toolbox; }
		}

		public CommonDesignerCanvas Canvas
		{
			get { return DesignerCanvas; }
		}

		public bool HasPermissionsToScale
		{
			get { return true; }
		}

		public bool AlwaysShowScroll
		{
			get { return true; }
		}

		public bool CanCollapse { get; protected set; }

		private bool _isCollapsed;
		public bool IsCollapsed
		{
			get { return _isCollapsed; }
			set
			{
				if (IsCollapsed != value)
				{
					_isCollapsed = value;
					OnPropertyChanged(() => IsCollapsed);
					if (IsCollapsedChanged != null)
						IsCollapsedChanged(this, EventArgs.Empty);
				}
			}
		}

		#endregion

		public void Save()
		{
			if (IsNotEmpty)
				NormalizeZIndex();
		}

		private void OnShowElement(Guid elementUID)
		{
			DesignerCanvas.Toolbox.SetDefault();
			DesignerCanvas.DeselectAll();
			foreach (var designerItem in DesignerCanvas.Items)
				if (designerItem.Element.UID == elementUID && designerItem.IsEnabled)
				{
					designerItem.IsSelected = true;
					break;
				}
		}
	}
}