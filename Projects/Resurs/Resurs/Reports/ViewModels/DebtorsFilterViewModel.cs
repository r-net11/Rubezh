namespace Resurs.ViewModels
{
	class DebtorsFilterViewModel : ReportFilterViewModel
	{
		double _minDebt;
		public double MinDebt
		{
			get { return _minDebt; }
			set
			{
				_minDebt = value;
				Filter.MinDebt = _minDebt;
				OnPropertyChanged(() => MinDebt);
			}
		}
	}
}