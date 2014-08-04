using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		DayTimeTrack DayTimeTrack;

		public TimeTrackDetailsViewModel(DayTimeTrack dayTimeTrack)
		{
			Title = "Время в течение дня";
			DayTimeTrack = dayTimeTrack;

			AvailableExcuseDocuments = new ObservableCollection<ExcuseDocumentEnum>(Enum.GetValues(typeof(ExcuseDocumentEnum)).OfType<ExcuseDocumentEnum>());

			DayTimeTrackParts = new ObservableCollection<DayTimeTrackPartViewModel>();
			foreach (var dayTimeTrackPart in DayTimeTrack.TimeTrackParts)
			{
				var employeeTimeTrackPartViewModel = new DayTimeTrackPartViewModel(dayTimeTrackPart);
				DayTimeTrackParts.Add(employeeTimeTrackPartViewModel);
			}
		}

		public ObservableCollection<ExcuseDocumentEnum> AvailableExcuseDocuments { get; private set; }

		ExcuseDocumentEnum _selectedExcuseDocument;
		public ExcuseDocumentEnum SelectedExcuseDocument
		{
			get { return _selectedExcuseDocument; }
			set
			{
				_selectedExcuseDocument = value;
				OnPropertyChanged(() => SelectedExcuseDocument);
			}
		}

		public ObservableCollection<DayTimeTrackPartViewModel> DayTimeTrackParts { get; private set; }
	}

	public class DayTimeTrackPartViewModel : BaseViewModel
	{
		public DayTimeTrackPart DayTimeTrackPart { get; private set; }

		public DayTimeTrackPartViewModel(DayTimeTrackPart dayTimeTrackPart)
		{
			DayTimeTrackPart = dayTimeTrackPart;
		}
	}
}