using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;

namespace PlansModule.Kursk.ViewModels
{
	public class TankPropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementRectangleTank _element;
		private TanksViewModel _tanksViewModel;

		public TankPropertiesViewModel(ElementRectangleTank element, TanksViewModel tankViewModel)
		{
			_tanksViewModel = tankViewModel;
			_element = element;
			Title = "Свойства фигуры: Бак";
			//var directions = XManager.DeviceConfiguration.Directions;
			//XDirections = new ObservableCollection<DirectionViewModel>();
			//foreach (var direction in directions)
			//{
			//    var directionViewModel = new DirectionViewModel(direction);
			//    XDirections.Add(directionViewModel);
			//}
			//if (_element.DirectionUID != Guid.Empty)
			//    SelectedXDirection = XDirections.FirstOrDefault(x => x.Direction.UID == _element.DirectionUID);
		}
	}
}
