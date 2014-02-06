using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.InstrumentAdorners;
using Infrustructure.Plans.Designer;

namespace Infrastructure.Designer.ViewModels
{
	public class ToolboxViewModel : BaseViewModel
	{
		private IInstrument _defaultInstrument;

		public ToolboxViewModel(DesignerCanvas designerCanvas)
		{
			DesignerCanvas = designerCanvas;
			RegisterInstruments();
			EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyEventHandler), true);
		}

		public DesignerCanvas DesignerCanvas { get; private set; }
		public bool AcceptKeyboard { get; set; }
		public bool IsRightPanel { get; set; }

		private ObservableCollection<IInstrument> _instruments;
		public ObservableCollection<IInstrument> Instruments
		{
			get { return _instruments; }
			set
			{
				_instruments = value;
				OnPropertyChanged("Instruments");
			}
		}

		private IInstrument _activeInstrument;
		public IInstrument ActiveInstrument
		{
			get { return _activeInstrument; }
			set
			{
				if (ActiveInstrument != null && ActiveInstrument.Adorner != null)
					ActiveInstrument.Adorner.Hide();
				_activeInstrument = value;
				OnPropertyChanged(() => ActiveInstrument);
				if (ActiveInstrument.Autostart)
					ApplicationService.BeginInvoke(() => Apply(null));
			}
		}

		private bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}

		public void Apply(Point? point)
		{
			DesignerCanvas.DeselectAll();
			if (ActiveInstrument.Adorner != null)
				using (new WaitWrapper())
				using (new TimeCounter("\t\tInstrumentAdorner.Show: {0}"))
					ActiveInstrument.Adorner.Show(point);
			else if (ActiveInstrument.Command != null)
			{
				ExecuteCommand(ActiveInstrument.Command);
				SetDefault();
				OnPropertyChanged(() => ActiveInstrument);
			}
		}
		public void SetDefault()
		{
			if (ActiveInstrument != _defaultInstrument)
				ActiveInstrument = _defaultInstrument;
		}
		public void UpdateZoom()
		{
			if (ActiveInstrument.Adorner != null)
				ActiveInstrument.Adorner.UpdateZoom();
		}

		public void RegisterInstruments(IEnumerable<IInstrument> instruments)
		{
			if (instruments != null)
			{
				foreach (IInstrument instrument in instruments)
					Instruments.Add(instrument);
				SortInstruments();
			}
		}
		private void RegisterInstruments()
		{
			Instruments = new ObservableCollection<IInstrument>()
			{
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Cursor.png",
					ToolTip="Указатель",
					Adorner = new RubberbandAdorner(DesignerCanvas),
					Index = 0,
					Autostart = false
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Pen.png",
					ToolTip="Нож",
					Index = 1,
					Adorner = new PointsAdorner(DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Line.png",
					ToolTip="Линия",
					Index = 1011,
					Adorner = new PolylineAdorner(DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Rectangle.png",
					ToolTip="Прямоугольник",
					Index = 1012,
					Adorner = new RectangleAdorner(DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Ellipse.png",
					ToolTip="Эллипс",
					Index = 1013,
					Adorner = new ElipseAdorner(DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Polygon.png",
					ToolTip="Многоугольник",
					Index = 1014,
					Adorner = new PolygonAdorner(DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Font.png",
					ToolTip="Текст",
					Index = 1005,
					Adorner = new TextBoxAdorner(DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/GridLineHorizontal.png",
					ToolTip="Добавить горизонтальную линию привязки",
					Index = 1501,
					Adorner = new GridLineAdorner(DesignerCanvas, Orientation.Horizontal),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/GridLineVertical.png",
					ToolTip="Добавить вертикальную линию привязки",
					Index = 1502,
					Adorner = new GridLineAdorner(DesignerCanvas, Orientation.Vertical),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/GridLineEdit.png",
					ToolTip="Удалить линии привязки",
					Index = 1503,
					Command = DesignerCanvas.RemoveGridLinesCommand,
				},
			};
			SortInstruments();
			_defaultInstrument = Instruments.FirstOrDefault(item => item.Index == 0);
			ActiveInstrument = _defaultInstrument;
		}
		private void SortInstruments()
		{
			var sortedItems = Instruments.OrderBy(item => item.Index);
			int index = 0;
			foreach (var item in sortedItems)
			{
				Instruments.Move(Instruments.IndexOf(item), index);
				index++;
			}
		}

		private void OnKeyEventHandler(object sender, KeyEventArgs e)
		{
			if (!AcceptKeyboard || !ApplicationService.ApplicationWindow.IsKeyboardFocusWithin || (IsRightPanel && !ApplicationService.Layout.IsRightPanelFocused))
				return;

			if (ActiveInstrument == null || ActiveInstrument == _defaultInstrument)
			{
				if (Keyboard.Modifiers == ModifierKeys.Control)
					switch (e.Key)
					{
						case Key.C:
							ExecuteCommand(DesignerCanvas.PlanDesignerViewModel.CopyCommand);
							break;
						case Key.X:
							ExecuteCommand(DesignerCanvas.PlanDesignerViewModel.CutCommand);
							break;
						case Key.V:
							ExecuteCommand(DesignerCanvas.PlanDesignerViewModel.PasteCommand);
							break;
						case Key.Z:
							ExecuteCommand(DesignerCanvas.PlanDesignerViewModel.UndoCommand);
							break;
						case Key.Y:
							ExecuteCommand(DesignerCanvas.PlanDesignerViewModel.RedoCommand);
							break;
						case Key.A:
							if (DesignerCanvas != null)
								using (new WaitWrapper())
								using (new TimeCounter("DesignerCanvas.SelectAll: {0}"))
									DesignerCanvas.SelectAll();
							break;
						case Key.D:
							if (DesignerCanvas != null)
								using (new WaitWrapper())
								using (new TimeCounter("DesignerCanvas.DeselectAll: {0}"))
									DesignerCanvas.DeselectAll();
							break;
					}
				DefaultKeyHandler(e);
			}
			else if (ActiveInstrument != null && ActiveInstrument.Adorner != null && !ActiveInstrument.Adorner.KeyboardInput(e.Key))
				DefaultKeyHandler(e);
		}
		private void DefaultKeyHandler(KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				SetDefault();
			else if (e.Key == Key.Delete)
			{
				if (DesignerCanvas != null)
					DesignerCanvas.RemoveAllSelected();
			}
		}
		private void ExecuteCommand(ICommand command)
		{
			if (command.CanExecute(null))
				command.Execute(null);
		}
	}
}