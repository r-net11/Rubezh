using System;
using System.Windows.Controls;
using RubezhAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Client.Plans.Presenter
{
	public abstract class BasePointPainter<T, TShowEvent> : PointPainter, IBasePainter<T, TShowEvent>
		where T : IStateProvider
		where TShowEvent : CompositePresentationEvent<Guid>, new()
	{
		protected PresenterPainterHelper<T, TShowEvent> Helper;
		public RelayCommand ShowInTreeCommand { get; private set; }
		public RelayCommand ShowPropertiesCommand { get; private set; }
		protected T Item
		{
			get { return Helper.Item; }
		}

		public BasePointPainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			Helper = new PresenterPainterHelper<T, TShowEvent>(presenterItem, this);
			Helper.Initialize();
		}

		protected abstract T CreateItem(PresenterItem presenterItem);
		protected abstract StateTooltipViewModel<T> CreateToolTip();
		protected abstract ContextMenu CreateContextMenu();
		protected abstract WindowBaseViewModel CreatePropertiesViewModel();
		//protected abstract Brush GetBrush();

		public override object GetToolTip(string title)
		{
			return Helper.Tooltip;
		}

		#region IBasePainter<T,TShowEvent> Members

		T IBasePainter<T, TShowEvent>.CreateItem(PresenterItem presenterItem)
		{
			return CreateItem(presenterItem);
		}

		StateTooltipViewModel<T> IBasePainter<T, TShowEvent>.CreateToolTip()
		{
			return CreateToolTip();
		}

		ContextMenu IBasePainter<T, TShowEvent>.CreateContextMenu()
		{
			return CreateContextMenu();
		}

		WindowBaseViewModel IBasePainter<T, TShowEvent>.CreatePropertiesViewModel()
		{
			return CreatePropertiesViewModel();
		}

		RelayCommand IBasePainter<T, TShowEvent>.ShowInTreeCommand
		{
			get { return ShowInTreeCommand; }
			set { ShowInTreeCommand = value; }
		}

		bool IBasePainter<T, TShowEvent>.IsPoint
		{
			get { return true; }
		}

		RelayCommand IBasePainter<T, TShowEvent>.ShowPropertiesCommand
		{
			get { return ShowPropertiesCommand; }
			set { ShowPropertiesCommand = value; }
		}

		#endregion
	}
}