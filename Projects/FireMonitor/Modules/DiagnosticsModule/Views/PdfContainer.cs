using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DiagnosticsModule.Views
{
	public partial class PdfContainer : UserControl
	{
		public PdfContainer()
		{
			InitializeComponent();
		}
		private void PdfContainer_Load(object sender, EventArgs e)
		{
			axAcroPDF.OnError += new EventHandler(axAcroPDF_OnError);
			axAcroPDF.OnMessage += new EventHandler(axAcroPDF_OnMessage);
			axAcroPDF.setShowToolbar(false);
			axAcroPDF.setShowScrollbars(false);
			axAcroPDF.setPageMode("none");
			axAcroPDF.setLayoutMode("SinglePage");
		}

		void axAcroPDF_OnMessage(object sender, EventArgs e)
		{
		}

		void axAcroPDF_OnError(object sender, EventArgs e)
		{
		}
		public void LoadFile(string fileName)
		{
			axAcroPDF.LoadFile(fileName);
			axAcroPDF.setShowToolbar(true);
			axAcroPDF.setShowScrollbars(true);
		}
	}
}
