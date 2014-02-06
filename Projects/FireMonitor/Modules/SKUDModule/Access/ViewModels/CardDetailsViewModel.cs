using System;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDCard Card { get; private set; }

		public CardDetailsViewModel()
		{
			Title = "Выдача карт доступа";
		}

		int _idFamily;
		public int IDFamily
		{
			get { return _idFamily; }
			set
			{
				_idFamily = value;
				OnPropertyChanged("IDFamily");
			}
		}

		int _idNo;
		public int IDNo
		{
			get { return _idNo; }
			set
			{
				_idNo = value;
				OnPropertyChanged("IDNo");
			}
		}

		DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged("StartDate");
			}
		}

		DateTime _endDate;
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = value;
				OnPropertyChanged("EndDate");
			}
		}

		protected override bool Save()
		{
			Card = new SKDCard()
			{
				Series = IDFamily,
				Number = IDNo,
				ValidFrom = StartDate,
				ValidTo = EndDate
			};
			return true;
		}
	}
}