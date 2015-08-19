using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows;
using StrazhModule.Intervals.Base.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorWeeklyIntervalViewModel : BaseIntervalViewModel<DoorWeeklyIntervalPartViewModel, SKDDoorWeeklyInterval>
	{
		DoorWeeklyIntervalsViewModel _weeklyIntervalsViewModel;

		public DoorWeeklyIntervalViewModel(int index, SKDDoorWeeklyInterval weeklyInterval, DoorWeeklyIntervalsViewModel weeklyIntervalsViewModel)
			: base(index, weeklyInterval)
		{
			_weeklyIntervalsViewModel = weeklyIntervalsViewModel;
			Initialize();
			Update();
		}

		void Initialize()
		{
			Parts = new ObservableCollection<DoorWeeklyIntervalPartViewModel>();
			if (Model != null)
				foreach (var weeklyIntervalPart in Model.WeeklyIntervalParts)
				{
					var weeklyIntervalPartViewModel = new DoorWeeklyIntervalPartViewModel(_weeklyIntervalsViewModel, weeklyIntervalPart);
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
				Model = new SKDDoorWeeklyInterval(true)
				{
					ID = Index,
					Name = Name,
				};
				Initialize();
				SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals.Add(Model);
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
			else if (!IsActive && Model != null)
			{
				if (ConfirmDeactivation())
				{
					SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals.Remove(Model);
					Model = null;
					Initialize();
					//SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.ForEach(week => week.InvalidateWeekIntervals());
					ServiceFactory.SaveService.SKDChanged = true;
					ServiceFactory.SaveService.TimeIntervalChanged();
				}
				else
					IsActive = true;
			}
			base.Activate();
		}

		public override void Paste(SKDDoorWeeklyInterval interval)
		{
			IsActive = true;
			for (int i = 0; i < interval.WeeklyIntervalParts.Count; i++)
			{
				Model.WeeklyIntervalParts[i].DayIntervalUID = interval.WeeklyIntervalParts[i].DayIntervalUID;
			}
			Initialize();
			ServiceFactory.SaveService.SKDChanged = true;
			ServiceFactory.SaveService.TimeIntervalChanged();
			Update();
		}

		bool ConfirmDeactivation()
		{
			var hasReference = SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.Any(item => item.WeeklyIntervalIDs.Contains(Index));
			return !hasReference || MessageBoxService.ShowConfirmation("Данный недельный график используется в одном или нескольких скользящих недельных графиках, Вы уверены что хотите его деактивировать?");
		}

		public override bool IsPredefined
		{
			get
			{
				return Name == TimeIntervalsConfiguration.PredefinedIntervalNameCard
					|| Name == TimeIntervalsConfiguration.PredefinedIntervalNamePassword
					|| Name == TimeIntervalsConfiguration.PredefinedIntervalNameCardAndPassword;
			}
		}
	}
}