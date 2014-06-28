using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Common;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Client.Plans.Presenter
{
	public abstract class BaseZonePainter<T, TShowEvent> : PolygonZonePainter, IPainter, IBasePainter<T, TShowEvent>
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

		public BaseZonePainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			Helper = new PresenterPainterHelper<T, TShowEvent>(presenterItem, this);
			Helper.Initialize();
		}

		protected abstract Guid ItemUID { get; }
		protected abstract T CreateItem(PresenterItem presenterItem);
		protected abstract StateTooltipViewModel<T> CreateToolTip();
		protected abstract ContextMenu CreateContextMenu();
		protected abstract WindowBaseViewModel CreatePropertiesViewModel();

		#region IPainter Members
		public override object GetToolTip(string title)
		{
			return Helper.Tooltip;
		}
		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(GetStateColor());
		}
		#endregion

		protected virtual Color GetStateColor()
		{
			switch (Helper.Item.StateClass.StateType)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.TechnologicalRegime:
				case XStateClass.ConnectionLost:
				case XStateClass.HasNoLicense:
					return Colors.DarkGray;

				case XStateClass.Fire1:
				case XStateClass.Fire2:
					return Colors.Red;

				case XStateClass.Attention:
					return Colors.Yellow;

				case XStateClass.Ignore:
					return Colors.Yellow;

				case XStateClass.Norm:
					return Colors.Green;

				default:
					return Colors.White;
			}
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
			get { return false; }
		}

		RelayCommand IBasePainter<T, TShowEvent>.ShowPropertiesCommand
		{
			get { return ShowPropertiesCommand; }
			set { ShowPropertiesCommand = value; }
		}

		Guid IBasePainter<T, TShowEvent>.ItemUID
		{
			get { return ItemUID; }
		}

		#endregion
	}
}