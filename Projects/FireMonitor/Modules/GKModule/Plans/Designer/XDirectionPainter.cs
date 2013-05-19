using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using System.Windows.Controls;
using Controls.Converters;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using FiresecAPI.Models;

namespace GKModule.Plans.Designer
{
	class XDirectionPainter : RectangleZonePainter, IPainter
	{
		private PresenterItem _presenterItem;
		private XDirection XDirection;
		private ContextMenu _contextMenu;

		public XDirectionPainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			XDirection = Helper.GetXDirection((ElementXDirection)_presenterItem.Element);
			if (XDirection != null)
				XDirection.DirectionState.StateChanged += OnPropertyChanged;
			_presenterItem.Title = GetDirectionTooltip();
		}

		private void OnPropertyChanged()
		{
			_presenterItem.Title = GetDirectionTooltip();
			_presenterItem.InvalidatePainter();
			_presenterItem.DesignerCanvas.Refresh();
		}
		private string GetDirectionTooltip()
		{
			if (XDirection == null)
				return null;
			var sb = new StringBuilder();
			sb.AppendLine(XDirection.PresentationName);
			sb.AppendLine("Состояние: " + XDirection.DirectionState.GetStateType().ToDescription());
			return sb.ToString().TrimEnd();
		}

		#region IPainter Members

		protected override Brush GetBrush()
		{
			//return PainterCache.GetTransparentBrush(GetStateColor());
			//return new LinearGradientBrush(GetStateColor(), Colors.Blue, 45);
			return new RadialGradientBrush(GetStateColor(), Colors.Blue);
		}

		#endregion

		public Color GetStateColor()
		{
			var stateType = XDirection.DirectionState.GetStateType();
			switch (stateType)
			{
				case StateType.Fire:
					return Colors.Red;

				case StateType.Attention:
					return Colors.Yellow;

				case StateType.Failure:
					return Colors.Pink;

				case StateType.Service:
					return Colors.Yellow;

				case StateType.Off:
					return Colors.Yellow;

				case StateType.Unknown:
					return Colors.Gray;

				case StateType.Info:
					return Colors.LightBlue;

				case StateType.Norm:
					return Colors.LightGreen;

				case StateType.No:
					return Colors.White;

				default:
					return Colors.Black;
			}
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(XDirection.UID);
		}
		bool CanShowInTree()
		{
			return XDirection != null;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel(XDirection);
			DialogService.ShowWindow(directionDetailsViewModel);
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);
				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в списке",
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Свойства",
					Command = ShowPropertiesCommand
				});
			}
			return _contextMenu;
		}
	}
}