﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using StrazhModule.Events;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DayIntervalViewModel : BaseViewModel
	{
		public SKDDayInterval DayInterval { get; private set; }
		public DayIntervalPartsViewModel DayIntervalPartsViewModel { get; private set; }

		public DayIntervalViewModel(SKDDayInterval dayInterval)
		{
			DayInterval = dayInterval;
			DayIntervalPartsViewModel = new DayIntervalPartsViewModel(dayInterval);
			Update(DayInterval);
		}

		public void Update(SKDDayInterval dayInterval)
		{
			DayInterval = dayInterval;
			OnPropertyChanged(() => DayInterval);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}

		public string Name
		{
			get { return DayInterval.Name; }
		}

		public string Description
		{
			get { return DayInterval.Description; }
		}

		public bool IsEnabled
		{
			get
			{
				return Name != TimeIntervalsConfiguration.PredefinedIntervalNameNever && Name != TimeIntervalsConfiguration.PredefinedIntervalNameAlways;
			}
		}

		/// <summary>
		/// Проверяет связь с недельными графиками доступа и выводит соответствующее предупреждающее окно перед процедурой удаления дневного графика доступа
		/// </summary>
		/// <returns>true - нужно удалить дневной график доступа, false - не нужно удалять дневной график доступа</returns>
		public bool ConfirmRemoval()
		{
			var hasReference = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Any(item => item.WeeklyIntervalParts.Any(part => part.DayIntervalUID == DayInterval.UID));
			return hasReference
				? MessageBoxService.ShowQuestion(String.Format("Дневной график доступа \"{0}\" используется в одном или нескольких недельных графиках доступа. При его удалении он будет заменен в недельных графиках доступа на дневной график \"Никогда\". Вы действительно хотите его удалить?", Name), null, MessageBoxImage.Warning)
				: MessageBoxService.ShowQuestion(String.Format("Вы действительно хотите удалить дневной график доступа \"{0}\"?", Name));
		}

		/// <summary>
		/// Возвращает коллекцию недельных графиков доступа, которые ссылаются на текущий дневной график доступа
		/// </summary>
		/// <returns>Коллекция недельных графиков доступа</returns>
		public IEnumerable<SKDWeeklyInterval> GetLinkedWeeklyIntervals()
		{
			return SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Where(item => item.WeeklyIntervalParts.Any(part => part.DayIntervalUID == DayInterval.UID)).ToList();
		}
	}
}