﻿using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class DelayViewModel : BaseViewModel
	{
		public GKDelay Delay { get; set; }
		public DelayViewModel(GKDelay delay)
		{
			ShowLogicCommand = new RelayCommand(OnShowLogic);
			Delay = delay;
			Delay.Changed += Update;
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => Delay);
			OnPropertyChanged(() => PresentationLogic);
		}

		public void Update(GKDelay delay)
		{
			this.Delay = delay;
			this.Update();
		}

		public string PresentationLogic
		{
			get
			{
				var presentationLogic = GKManager.GetPresentationLogic(Delay.Logic);
				IsLogicGrayed = string.IsNullOrEmpty(presentationLogic);
				if (string.IsNullOrEmpty(presentationLogic))
				{
					presentationLogic = "Нажмите для настройки логики";
				}
				return presentationLogic;
			}
		}

		bool _isLogicGrayed;
		public bool IsLogicGrayed
		{
			get { return _isLogicGrayed; }
			set
			{
				_isLogicGrayed = value;
				OnPropertyChanged(() => IsLogicGrayed);
			}
		}

		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
			DelaysViewModel.Current.SelectedDelay = this;
			var logicViewModel = new LogicViewModel(Delay, Delay.Logic, true, hasStopClause: true);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetDelayLogic(Delay, logicViewModel.GetModel());
				OnPropertyChanged(() => PresentationLogic);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}