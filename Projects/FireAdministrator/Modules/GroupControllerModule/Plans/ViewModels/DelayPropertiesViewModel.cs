﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;

namespace GKModule.Plans.ViewModels
{
	public class DelayPropertiesViewModel : SaveCancelDialogViewModel
	{
		public DelayPropertiesViewModel(IElementDelay element, DelaysViewModel delaysViewModel)
		{
			_delaysViewModel = delaysViewModel;
			_element = element;
			Title = "Свойства фигуры: ГК Задержка";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			ShowState = element.ShowState;
			this.ShowDelay = element.ShowDelay;

			this.Delays = new ObservableCollection<DelayViewModel>(GKManager.Delays.Select(delay => new DelayViewModel(delay)));
			if (element.DelayUID != Guid.Empty)
				this.SelectedDelay = this.Delays
					.Where(delay => delay.Delay.UID == element.DelayUID)
					.FirstOrDefault();
		}

		private void OnCreate()
		{
			Guid delayUID = _element.DelayUID;
			var createDelayEventArg = new CreateGKDelayEventArgs();
			ServiceFactory.Events.GetEvent<CreateGKDelayEvent>().Publish(createDelayEventArg);
			if (createDelayEventArg.Delay != null)
				_element.DelayUID = createDelayEventArg.Delay.UID;
			GKPlanExtension.Instance.Cache.BuildSafe<GKDelay>();
			GKPlanExtension.Instance.SetItem<GKDelay>(_element);
			UpdateDelays(delayUID);
			if (!createDelayEventArg.Cancel)
				Close(true);
		}

		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKDelayEvent>().Publish(this.SelectedDelay.Delay.UID);
			this.SelectedDelay.Update(this.SelectedDelay.Delay);
		}

		private bool CanEdit()
		{
			return this.SelectedDelay != null;
		}

		private void Update(Guid delayUID)
		{
			DelayViewModel delay = this._delaysViewModel.Delays
				.Where(x => x.Delay.UID == delayUID)
				.FirstOrDefault();
			if (delay != null)
				delay.Update();
		}

		private void UpdateDelays(Guid delayUID)
		{
			if (this._delaysViewModel == null)
				return;
			if (delayUID != _element.DelayUID)
				this.Update(delayUID);
			this._delaysViewModel.LockedSelect(_element.DelayUID);
		}

		protected override bool Save()
		{
			Guid delayUID = _element.DelayUID;
			GKPlanExtension.Instance.SetItem<GKDelay>(_element, this.SelectedDelay == null ? null : this.SelectedDelay.Delay);
			UpdateDelays(delayUID);
			return base.Save();
		}

		#region Properties

		public RelayCommand CreateCommand { get; private set; }

		public RelayCommand EditCommand { get; private set; }

		public ObservableCollection<DelayViewModel> Delays { get; private set; }

		public DelayViewModel SelectedDelay
		{
			get { return this.selectedDelay; }
			set
			{
				this.selectedDelay = value;
				base.OnPropertyChanged(() => SelectedDelay);
			}
		}

		public bool ShowState
		{
			get { return _showState; }
			set
			{
				_showState = value;
				OnPropertyChanged(() => ShowState);
			}
		}

		public bool ShowDelay
		{
			get { return _showDelay; }
			set
			{
				_showDelay = value;
				OnPropertyChanged(() => ShowDelay);
			}
		}

		#endregion

		#region Fields
		private IElementDelay _element;
		private DelaysViewModel _delaysViewModel;
		private DelayViewModel selectedDelay = null;
		private bool _showState;
		private bool _showDelay;

		#endregion
	}
}
