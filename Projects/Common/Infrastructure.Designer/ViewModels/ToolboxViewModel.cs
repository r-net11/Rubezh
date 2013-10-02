using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using Infrastructure.Client.Plans;
using Infrastructure.Designer.InstrumentAdorners;

namespace Infrastructure.Designer.ViewModels
{
	public class ToolboxViewModel : BaseViewModel
	{
		private IInstrument _defaultInstrument;

		public ToolboxViewModel(PlanDesignerViewModel planDesignerViewModel)
		{
			PlanDesignerViewModel = planDesignerViewModel;
			RegisterInstruments();
			EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyEventHandler), true);
		}

		public PlanDesignerViewModel PlanDesignerViewModel { get; private set; }
		public bool AcceptKeyboard { get; set; }

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
			PlanDesignerViewModel.DesignerCanvas.DeselectAll();
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
					Adorner = new RubberbandAdorner(PlanDesignerViewModel.DesignerCanvas),
					Index = 0,
					Autostart = false
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Pen.png",
					ToolTip="Нож",
					Index = 1,
					Adorner = new PointsAdorner(PlanDesignerViewModel.DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Line.png",
					ToolTip="Линия",
					Index = 311,
					Adorner = new PolylineAdorner(PlanDesignerViewModel.DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Rectangle.png",
					ToolTip="Прямоугольник",
					Index = 312,
					Adorner = new RectangleAdorner(PlanDesignerViewModel.DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Ellipse.png",
					ToolTip="Эллипс",
					Index = 313,
					Adorner = new ElipseAdorner(PlanDesignerViewModel.DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Polygon.png",
					ToolTip="Многоугольник",
					Index = 314,
					Adorner = new PolygonAdorner(PlanDesignerViewModel.DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Font.png",
					ToolTip="Текст",
					Index = 305,
					Adorner = new TextBoxAdorner(PlanDesignerViewModel.DesignerCanvas),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/GridLineHorizontal.png",
					ToolTip="Добавить горизонтальную линию привязки",
					Index = 501,
					Adorner = new GridLineAdorner(PlanDesignerViewModel.DesignerCanvas, Orientation.Horizontal),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/GridLineVertical.png",
					ToolTip="Добавить вертикальную линию привязки",
					Index = 502,
					Adorner = new GridLineAdorner(PlanDesignerViewModel.DesignerCanvas, Orientation.Vertical),
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/GridLineEdit.png",
					ToolTip="Удалить линии привязки",
					Index = 503,
					Command = PlanDesignerViewModel.DesignerCanvas.RemoveGridLinesCommand,
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
			if (!AcceptKeyboard || !ApplicationService.ApplicationWindow.IsKeyboardFocusWithin || !ApplicationService.Layout.IsRightPanelFocused)
				return;

			if (ActiveInstrument == null || ActiveInstrument == _defaultInstrument)
			{
				if (Keyboard.Modifiers == ModifierKeys.Control)
					switch (e.Key)
					{
						case Key.C:
							ExecuteCommand(PlanDesignerViewModel.CopyCommand);
							break;
						case Key.X:
							ExecuteCommand(PlanDesignerViewModel.CutCommand);
							break;
						case Key.V:
							ExecuteCommand(PlanDesignerViewModel.PasteCommand);
							break;
						case Key.Z:
							ExecuteCommand(PlanDesignerViewModel.UndoCommand);
							break;
						case Key.Y:
							ExecuteCommand(PlanDesignerViewModel.RedoCommand);
							break;
						case Key.A:
							if (PlanDesignerViewModel.DesignerCanvas != null)
								using (new WaitWrapper())
								using (new TimeCounter("DesignerCanvas.SelectAll: {0}"))
									PlanDesignerViewModel.DesignerCanvas.SelectAll();
							break;
						case Key.D:
							if (PlanDesignerViewModel.DesignerCanvas != null)
								using (new WaitWrapper())
								using (new TimeCounter("DesignerCanvas.DeselectAll: {0}"))
									PlanDesignerViewModel.DesignerCanvas.DeselectAll();
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
				var designerCanvas = PlanDesignerViewModel.DesignerCanvas;
				if (designerCanvas != null)
					designerCanvas.RemoveAllSelected();
			}
		}
		private void ExecuteCommand(ICommand command)
		{
			if (command.CanExecute(null))
				command.Execute(null);
		}
	}
}