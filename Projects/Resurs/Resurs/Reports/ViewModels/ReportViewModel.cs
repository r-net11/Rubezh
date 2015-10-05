using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ReportViewModel
	{
		public string Title { get; set; }

		public ReportViewModel(XtraReport model)
		{
			Model = model;
			Title = Model.GetType().ToString();
		}
		public void Reset()
		{
		}
		public XtraReport Model { get; private set; }
	}
}