using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using System.Windows.Input;
using Common;
using System;
using GKProcessor.Events;
using GKProcessor;

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