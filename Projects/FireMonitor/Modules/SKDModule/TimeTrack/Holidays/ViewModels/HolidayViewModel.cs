using RubezhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class HolidayViewModel :OrganisationElementViewModel<HolidayViewModel, Holiday>
	{
		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => ReductionTime);
			OnPropertyChanged(() => TransitionDate);
			OnPropertyChanged(() => Model);
		}

		public string ReductionTime
		{
			get
			{
				if (Model != null && Model.Type == HolidayType.BeforeHoliday)
					return Model.Reduction.ToString("hh\\-mm");
				return null;
			}
		}
		public string TransitionDate
		{
			get
			{
				if (Model != null && Model.Type == HolidayType.WorkingHoliday && Model.TransferDate.HasValue)
					return Model.TransferDate.Value.ToString("dd-MM");
				return null;
			}
		}
	}
}