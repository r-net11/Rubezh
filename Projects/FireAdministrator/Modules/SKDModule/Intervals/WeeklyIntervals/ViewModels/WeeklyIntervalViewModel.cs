using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Intervals.Base;
using Infrastructure;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalViewModel : BaseIntervalViewModel<WeeklyIntervalPartViewModel, SKDWeeklyInterval>
	{
		private WeeklyIntervalsViewModel _weeklyIntervalsViewModel;

		public WeeklyIntervalViewModel(int index, SKDWeeklyInterval weeklyInterval, WeeklyIntervalsViewModel weeklyIntervalsViewModel)
			: base(index, weeklyInterval)
		{
			_weeklyIntervalsViewModel = weeklyIntervalsViewModel;
			Initialize();
			Update();
		}

		private void Initialize()
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
			Description = IsEnabled ? Model.Description : string.Empty;
		}
		protected override void Activate()
		{
			if (!IsDefault)
			{
				if (IsActive && Model == null)
				{
					Model = new SKDWeeklyInterval()
					{
						ID = Index,
						Name = Name,
					};
					Initialize();
					SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Add(Model);
					ServiceFactory.SaveService.SKDChanged = true;
				}
				else if (!IsActive && Model != null)
				{
					SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Remove(Model);
					Model = null;
					Initialize();
					SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.ForEach(week => week.InvalidateWeekIntervals());
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
			base.Activate();
		}

		public void Paste(SKDWeeklyInterval interval)
		{
			IsActive = true;
			for (int i = 0; i < interval.WeeklyIntervalParts.Count; i++)
				Model.WeeklyIntervalParts[i].TimeIntervalID = interval.WeeklyIntervalParts[i].TimeIntervalID;
			Initialize();
			ServiceFactory.SaveService.SKDChanged = true;
			Update();
		}
	}
}