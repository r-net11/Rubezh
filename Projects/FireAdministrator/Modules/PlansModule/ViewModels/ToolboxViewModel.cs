using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PlansModule.ViewModels
{
	public class ToolboxViewModel : BaseViewModel
	{
		private InstrumentViewModel _defaultInstrument;

		public ToolboxViewModel(PlansViewModel plansViewModel)
		{
			PlansViewModel = plansViewModel;
			RegisterInstruments();
			EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyEventHandler), true);
		}

		public PlansViewModel PlansViewModel { get; private set; }
		public bool AcceptKeyboard { get; set; }

		private ObservableCollection<InstrumentViewModel> _instruments;
		public ObservableCollection<InstrumentViewModel> Instruments
		{
			get { return _instruments; }
			set
			{
				_instruments = value;
				OnPropertyChanged("Instruments");
			}
		}

		private InstrumentViewModel _activeInstrument;
		public InstrumentViewModel ActiveInstrument
		{
			get { return _activeInstrument; }
			set
			{
				_activeInstrument = value;
				OnPropertyChanged("ActiveInstrument");
			}
		}

		private void RegisterInstruments()
		{
			Instruments = new ObservableCollection<InstrumentViewModel>()
			{
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Help.png",
					ToolTip="Указатель"
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Line.png",
					ToolTip="Линия"
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Rectangle.png",
					ToolTip="Прямоугольник"
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Ellipse.png",
					ToolTip="Эллипс"
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Polygon.png",
					ToolTip="Многоугольник"
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/Font.png",
					ToolTip="Текст"
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/ZoneRectangle.png",
					ToolTip="Зона"
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/ZonePolygon.png",
					ToolTip="Зона"
				},
				new InstrumentViewModel()
				{
					ImageSource="/Controls;component/Images/SubPlanPolygon.png",
					ToolTip="Подплан"
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
							designerCanvas.SelectAll();
						break;
				}
			else if (e.Key == Key.Delete)
			{
				var designerCanvas = PlansViewModel.DesignerCanvas;
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
