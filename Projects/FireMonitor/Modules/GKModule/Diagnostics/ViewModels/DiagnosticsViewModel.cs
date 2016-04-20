using System;
using System.Collections.ObjectModel;
using GKProcessor;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DiagnosticsViewModel : DialogViewModel
	{
		public DiagnosticsViewModel()
		{
			Title = "Диагностика";
			DiagnosticsItems = new ObservableCollection<DiagnosticsItem>(DiagnosticsManager.DiagnosticsItems);
			DiagnosticsManager.NewDiagnosticsItem += new Action<DiagnosticsItem>(OnNewDiagnosticsItem);
		}

		public ObservableCollection<DiagnosticsItem> DiagnosticsItems { get; private set; }

		void OnNewDiagnosticsItem(DiagnosticsItem diagnosticsItem)
		{
			DiagnosticsItems.Add(diagnosticsItem);
		}
	}
}