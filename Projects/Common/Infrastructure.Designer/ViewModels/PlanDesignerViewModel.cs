using System;
using Common;
using FiresecAPI.Models;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;

namespace Infrastructure.Designer.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		public event EventHandler Updated;
		public event EventHandler IsCollapsedChanged;
		public DesignerCanvas DesignerCanvas { get; set; }

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

		public PlanDesignerViewModel()
		{
			IsNotEmpty = false;
			InitializeHistory();
			InitializeZIndexCommands();
			InitializeAlignCommands();
			InitializeCopyPasteCommands();
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
		{ get { return true; } }

		public bool AlwaysShowScroll
		{
			get { return true; }
		}

		public bool CanCollapse
		{
			get { return true; }
		}

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
	}
}