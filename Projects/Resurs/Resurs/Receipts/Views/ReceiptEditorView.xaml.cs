using Resurs.Reports.Templates;
using System.Windows.Controls;

namespace Resurs.Views
{
	/// <summary>
	/// Interaction logic for ReceiptEditorView.xaml
	/// </summary>
	public partial class ReceiptEditorView : UserControl
	{
		public static ReceiptEditorView Current { get; private set; }
		public ReceiptEditorView()
		{
			InitializeComponent();
			Current = this;
		}
		public static void OpenDocument(ReceiptTemplate receipt)
		{
			Current.designer.OpenDocument(receipt);
		}
		public static void CloseAllDocuments()
		{
			for (int i = 0; i < Current.designer.Documents.Count; i++)
			{
				Current.designer.Documents[i].Close(true);
			}
		}
		public static ReceiptTemplate GetFirstReceiptTemplate()
		{
			if (Current.designer.Documents.Count == 0)
				return null;
			return (ReceiptTemplate)Current.designer.Documents[0].GetReportCopy();
		}
		public static bool HasChangesFirstReceiptTemplate
		{
			get
			{
				if (Current.designer.Documents.Count == 0)
					return false;
				return Current.designer.Documents[0].HasChanges;
			}
		}
	}
}