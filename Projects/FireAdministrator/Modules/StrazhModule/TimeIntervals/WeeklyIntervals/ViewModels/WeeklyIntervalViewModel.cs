using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows;
using StrazhModule.Intervals.Base.ViewModels;

namespace StrazhModule.ViewModels
{
	public class WeeklyIntervalViewModel : BaseIntervalViewModel<WeeklyIntervalPartViewModel, SKDWeeklyInterval>
	{
		readonly WeeklyIntervalsViewModel _weeklyIntervalsViewModel;

		public WeeklyIntervalViewModel(int index, SKDWeeklyInterval weeklyInterval, WeeklyIntervalsViewModel weeklyIntervalsViewModel)
			: base(index, weeklyInterval)
		{
			_weeklyIntervalsViewModel = weeklyIntervalsViewModel;
			Initialize();
			Update();
		}

		void Initialize()
		{
			Parts = new ObservableCollection<WeeklyIntervalPartViewModel>();
			if (Model != null)
				foreach (var weeklyIntervalPart in Model.WeeklyIntervalParts)
				{
					var weeklyIntervalPartViewModel = new WeeklyIntervalPartViewModel(_weeklyIntervalsViewModel, weeklyIntervalPart);
					Parts.Add(weeklyIntervalPartViewModel);
				}
		}

		public override void Update()
		{
			base.Update();
			Name = IsActive ? Model.Name : string.Format("Понедельный график {0}", Index);
			Description = IsActive ? Model.Description : string.Empty;
		}
		protected override void Activate()
		{
			if (IsActive && Model == null)
			{
				Model = new SKDWeeklyInterval(true)
				{
					ID = Index,
					Name = Name,
				};

				// Для вновь активированного недельного графика для всех дней используем дневной график <Никогда>
				var dayIntervalCard = SKDManager.TimeIntervalsConfiguration.DayIntervals.FirstOrDefault(x => x.Name == TimeIntervalsConfiguration.PredefinedIntervalNameNever);
				if (dayIntervalCard != null)
					Model.WeeklyIntervalParts.ForEach(day => day.DayIntervalUID = dayIntervalCard.UID);

				Initialize();
				SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Add(Model);
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
			else if (!IsActive && Model != null)
			{
				if (ConfirmDeactivation())
				{
					SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Remove(Model);
					Model = null;
					Initialize();
					SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.ForEach(week => week.InvalidateWeekIntervals());
					ServiceFactory.SaveService.SKDChanged = true;
					ServiceFactory.SaveService.TimeIntervalChanged();
				}
				else
					IsActive = true;
			}
			base.Activate();
		}

		public override void Paste(SKDWeeklyInterval interval)
		{
			IsActive = true;
			Model.Name = GenerateNewNameBeforePaste(interval.Name);
			for (int i = 0; i < interval.WeeklyIntervalParts.Count; i++)
			{
				Model.WeeklyIntervalParts[i].DayIntervalUID = interval.WeeklyIntervalParts[i].DayIntervalUID;
			}
			Initialize();
			ServiceFactory.SaveService.SKDChanged = true;
			ServiceFactory.SaveService.TimeIntervalChanged();
			Update();
		}

		private string GenerateNewNameBeforePaste(string name)
		{
			string newName;
			var i = 1;

			do
				newName = String.Format("{0} ({1})", name, i++);
			while (_weeklyIntervalsViewModel.Intervals.Any(x => x.Name == newName));

			return newName;
		}

		bool ConfirmDeactivation()
		{
			return MessageBoxService.ShowQuestion(String.Format("Недельный график доступа \"{0}\" может использоваться для определения временных критериев прохода через точки доступа для уже выданных пропусков. При его удалении он будет заменен на критерий прохода, соответствующий недельному графику доступа \"Никогда\". Вы действительно хотите его удалить?", Name));
		}

		public override bool IsPredefined
		{
			get
			{
				return Name == TimeIntervalsConfiguration.PredefinedIntervalNameNever || Name == TimeIntervalsConfiguration.PredefinedIntervalNameAlways;
			}
		}
	}
}