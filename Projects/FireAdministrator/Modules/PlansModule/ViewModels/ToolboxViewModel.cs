using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using PlansModule.InstrumentAdorners;

namespace PlansModule.ViewModels
{
	public class ToolboxViewModel : BaseViewModel
	{
		private IInstrument _defaultInstrument;

		public ToolboxViewModel(PlansViewModel plansViewModel)
		{
			PlansViewModel = plansViewModel;
			RegisterInstruments();
			EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyEventHandler), true);
		}

		public PlansViewModel PlansViewModel { get; private set; }
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
				OnPropertyChanged("ActiveInstrument");
				if (ActiveInstrument.Autostart)
					Apply(null);
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
			PlansViewModel.DesignerCanvas.DeselectAll();
			if (ActiveInstrument.Adorner != null)
				ActiveInstrument.Adorner.Show(point);
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

		private void RegisterInstruments()
		{
			Instruments = new ObservableCollection<IInstrument>()
			{
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Cursor.png",
					ToolTip="Указатель",
					Adorner = new RubberbandAdorner(PlansViewModel.DesignerCanvas)
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Pen.png",
					ToolTip="Нож",
					Adorner = new PointsAdorner(PlansViewModel.DesignerCanvas),
					Autostart = true
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Line.png",
					ToolTip="Линия",
					Adorner = new PolylineAdorner(PlansViewModel.DesignerCanvas),
					Autostart = true
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Rectangle.png",
					ToolTip="Прямоугольник",
					Adorner = new RectangleAdorner(PlansViewModel.DesignerCanvas),
					Autostart = true
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Ellipse.png",
					ToolTip="Эллипс",
					Adorner = new ElipseAdorner(PlansViewModel.DesignerCanvas),
					Autostart = true
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Polygon.png",
					ToolTip="Многоугольник",
					Adorner = new PolygonAdorner(PlansViewModel.DesignerCanvas),
					Autostart = true
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Font.png",
					ToolTip="Текст",
					Adorner = new TextBoxAdorner(PlansViewModel.DesignerCanvas),
					Autostart = true
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/SubPlanPolygon.png",
					ToolTip="Подплан",
					Adorner = new SubPlanAdorner(PlansViewModel.DesignerCanvas),
					Autostart = true
				},
			};
			_defaultInstrument = Instruments[0];
			ActiveInstrument = Instruments[0];
		}
		private void OnKeyEventHandler(object sender, KeyEventArgs e)
		{
			if (AcceptKeyboard == false)
				return;

			if (Keyboard.Modifiers == ModifierKeys.Control)
				switch (e.Key)
				{
					case Key.C:
						ExecuteCommand(PlansViewModel.CopyCommand);
						break;
					case Key.X:
						ExecuteCommand(PlansViewModel.CutCommand);
						break;
					case Key.V:
						ExecuteCommand(PlansViewModel.PasteCommand);
						break;
					case Key.Z:
						ExecuteCommand(PlansViewModel.UndoCommand);
						break;
					case Key.Y:
						ExecuteCommand(PlansViewModel.RedoCommand);
						break;
					case Key.A:
						var designerCanvas = PlansViewModel.DesignerCanvas;
						if (designerCanvas != null)
							using (new WaitWrapper())
							using (new TimeCounter("DesignerCanvas.SelectAll: {0}"))
								designerCanvas.SelectAll();
						break;
				}
			else if (e.Key == Key.Delete)
			{
				var designerCanvas = PlansViewModel.DesignerCanvas;
				if (designerCanvas != null)
					designerCanvas.RemoveAllSelected();
			}
			else if (e.Key == Key.Escape)
				SetDefault();
			else if (ActiveInstrument != null && ActiveInstrument.Adorner != null)
				ActiveInstrument.Adorner.KeyboardInput(e.Key);
		}
		private void ExecuteCommand(ICommand command)
		{
			if (command.CanExecute(null))
				command.Execute(null);
		}
	}
}
