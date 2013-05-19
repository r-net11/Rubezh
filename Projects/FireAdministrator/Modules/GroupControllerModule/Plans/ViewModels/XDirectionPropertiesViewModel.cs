using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using XFiresecAPI;
using FiresecAPI.Models;

namespace GKModule.Plans.ViewModels
{
	public class XDirectionPropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementXDirection _element;

		public XDirectionPropertiesViewModel(ElementXDirection element)
		{
			_element = element;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: ГК Направление";
			XDirections = new ObservableCollection<XDirection>(XManager.DeviceConfiguration.Directions);
			if (element.DirectionUID != Guid.Empty)
				SelectedXDirection = XDirections.FirstOrDefault(x => x.UID == element.DirectionUID);
		}

		public ObservableCollection<XDirection> XDirections { get; private set; }

		private XDirection _selectedXDirection;
		public XDirection SelectedXDirection
		{
			get { return _selectedXDirection; }
			set
			{
				_selectedXDirection = value;
				OnPropertyChanged("SelectedXDirection");
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			var createXDirectionEventArg = new CreateXDirectionEventArg();
			ServiceFactory.Events.GetEvent<CreateXDirectionEvent>().Publish(createXDirectionEventArg);
			_element.DirectionUID = createXDirectionEventArg.DirectionUID;
			Helper.BuildMap();
			Helper.SetXDirection(_element);
			if (!createXDirectionEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditXDirectionEvent>().Publish(SelectedXDirection.UID);
			OnPropertyChanged("Directions");
		}
		private bool CanEdit()
		{
			return SelectedXDirection != null;
		}

		protected override bool Save()
		{
			Helper.SetXDirection(_element, SelectedXDirection);
			return base.Save();
		}
	}
}