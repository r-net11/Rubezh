using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;
using Infrastructure.Common.Windows.Services.Layout;
using Infrastructure.Common.Windows.Windows;
using RubezhClient;
using Common;

namespace FireMonitor.Layout.ViewModels
{
	public class TemplateContainerPartViewModel : BaseViewModel, ILayoutPartContent
	{
		public TemplateContainerPartViewModel(LayoutPartReferenceProperties properties)
		{
			var layout = ClientManager.LayoutsConfiguration.Layouts.FirstOrDefault(item => item.UID == properties.ReferenceUID);
			LayoutContainer = new LayoutContainer(this, layout);
		}

		public LayoutContainer LayoutContainer { get; private set; }

		#region ILayoutPartContent Members

		public ILayoutPartContainer Container { get; private set; }
		public void SetLayoutPartContainer(ILayoutPartContainer container)
		{
			if (Container != null)
			{
				Container.ActiveChanged -= OnActiveChanged;
				Container.SelectedChanged -= OnSelectedChanged;
			}
			Container = container;
			if (Container != null)
			{
				Container.ActiveChanged += OnActiveChanged;
				Container.SelectedChanged += OnSelectedChanged;
			}
		}

		#endregion

		private void OnSelectedChanged(object sender, EventArgs e)
		{
			LayoutContainer.LayoutParts.ForEach(item => item.FireSelectedChanged());
		}
		private void OnActiveChanged(object sender, EventArgs e)
		{
			LayoutContainer.LayoutParts.ForEach(item => item.FireActiveChanged());
		}
	}
}