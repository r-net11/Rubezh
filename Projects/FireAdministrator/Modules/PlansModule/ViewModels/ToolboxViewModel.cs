using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace PlansModule.ViewModels
{
	public class ToolboxViewModel : BaseViewModel
	{
		public ToolboxViewModel(PlansViewModel plansViewModel)
		{
			PlansViewModel = plansViewModel;
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
			ActiveInstrument = Instruments[0];
		}

		public PlansViewModel PlansViewModel { get; private set; }

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
	}
}
