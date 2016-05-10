using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class DelayPropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		IElementDelay IElementDelay;
		ElementBaseRectangle ElementBaseRectangle { get; set; }
		public bool CanEditPosition { get; private set; }
		public DelayPropertiesViewModel(IElementDelay element)
		{
			IElementDelay = element;
			ElementBaseRectangle = element as ElementBaseRectangle;
			CanEditPosition = ElementBaseRectangle != null;
			if (CanEditPosition)
			{
				Left = (int)(ElementBaseRectangle.Left * _sensivityFactor);
				Top = (int)(ElementBaseRectangle.Top * _sensivityFactor);
			}
			Title = "Свойства фигуры: Задержка";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			ShowState = element.ShowState;
			ShowDelay = element.ShowDelay;

			Delays = new ObservableCollection<GKDelay>(GKManager.Delays);
			if (element.DelayUID != Guid.Empty)
				SelectedDelay = Delays
					.Where(delay => delay.UID == element.DelayUID)
					.FirstOrDefault();
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}
		private void OnCreate()
		{
			var createDelayEventArg = new CreateGKDelayEventArgs();
			ServiceFactory.Events.GetEvent<CreateGKDelayEvent>().Publish(createDelayEventArg);
			if (createDelayEventArg.Delay != null)
				GKPlanExtension.Instance.RewriteItem(IElementDelay, createDelayEventArg.Delay);
			if (!createDelayEventArg.Cancel)
				Close(true);
		}

		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKDelayEvent>().Publish(this.SelectedDelay.UID);
		}

		private bool CanEdit()
		{
			return this.SelectedDelay != null;
		}
		public RelayCommand CreateCommand { get; private set; }

		public RelayCommand EditCommand { get; private set; }

		public ObservableCollection<GKDelay> Delays { get; private set; }

		private GKDelay _selectedDelay = null;
		public GKDelay SelectedDelay
		{
			get { return _selectedDelay; }
			set
			{
				_selectedDelay = value;
				OnPropertyChanged(() => SelectedDelay);
			}
		}

		private bool _showState;
		public bool ShowState
		{
			get { return _showState; }
			set
			{
				_showState = value;
				OnPropertyChanged(() => ShowState);
			}
		}

		private bool _showDelay;
		public bool ShowDelay
		{
			get { return _showDelay; }
			set
			{
				_showDelay = value;
				OnPropertyChanged(() => ShowDelay);
			}
		}
		protected override bool Save()
		{
			IElementDelay.ShowState = ShowState;
			IElementDelay.ShowDelay = ShowDelay;
			ElementBaseRectangle.Left = (double)Left / _sensivityFactor;
			ElementBaseRectangle.Top = (double)Top / _sensivityFactor;
			GKPlanExtension.Instance.RewriteItem(IElementDelay, SelectedDelay);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedDelay != null;
		}
	}
}