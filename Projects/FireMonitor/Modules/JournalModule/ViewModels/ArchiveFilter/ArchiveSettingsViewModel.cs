using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;
using JournalModule.Events;

namespace JournalModule.ViewModels
{
	public class ArchiveSettingsViewModel : SaveCancelDialogViewModel
	{
		public ArchiveSettingsViewModel()
		{
			Title = "Настройки";

			AdditionalColumns = new List<JournalColumnTypeViewModel>();
			foreach (JournalColumnType journalColumnType in Enum.GetValues(typeof(JournalColumnType)))
			{
				var journalColumnTypeViewModel = new JournalColumnTypeViewModel(journalColumnType);
				AdditionalColumns.Add(journalColumnTypeViewModel);
				if (ClientSettings.ArchiveDefaultState.AdditionalJournalColumnTypes.Any(x => x == journalColumnType))
				{
					journalColumnTypeViewModel.IsChecked = true;
				}
			}

			PageSize = ClientSettings.ArchiveDefaultState.PageSize;
		}

		public List<JournalColumnTypeViewModel> AdditionalColumns { get; private set; }

		int _pageSize;
		public int PageSize
		{
			get { return _pageSize; }
			set
			{
				_pageSize = value;
				OnPropertyChanged(() => PageSize);
			}
		}

		protected override bool Save()
		{
			ClientSettings.ArchiveDefaultState.AdditionalJournalColumnTypes = new List<JournalColumnType>();
			foreach (var journalColumnTypeViewModel in AdditionalColumns)
			{
				if (journalColumnTypeViewModel.IsChecked)
					ClientSettings.ArchiveDefaultState.AdditionalJournalColumnTypes.Add(journalColumnTypeViewModel.JournalColumnType);
			}

			if (PageSize < 10)
				PageSize = 10;
			if (PageSize > 1000)
				PageSize = 1000;
			ClientSettings.ArchiveDefaultState.PageSize = PageSize;
			return base.Save();
		}
	}
}