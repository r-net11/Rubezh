using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.GK
{
	public static class DiagnosticsManager
	{
		static DiagnosticsManager()
		{
			DiagnosticsItems = new List<DiagnosticsItem>();
		}

		public static List<DiagnosticsItem> DiagnosticsItems { get; private set; }

		public static void Add(string name)
		{
			var diagnosticsItem = new DiagnosticsItem(name);
			DiagnosticsItems.Add(diagnosticsItem);
			OnNewDiagnosticsItem(diagnosticsItem);
		}

		public static event Action<DiagnosticsItem> NewDiagnosticsItem;
		static void OnNewDiagnosticsItem(DiagnosticsItem diagnosticsItem)
		{
			if (NewDiagnosticsItem != null)
				NewDiagnosticsItem(diagnosticsItem);
		}
	}
}