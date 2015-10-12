using Infrastructure.Common.Windows.ViewModels;
using Resurs.Reports;
using ResursAPI;
using System;
using System.Linq;
using System.Collections.Generic;
using ResursDAL;
using Infrastructure.Common.Windows;

namespace Resurs.ViewModels
{
	public class ReportFilterViewModel : SaveCancelDialogViewModel
	{
		public ReportFilter Filter { get; set; }
		public ReportFilterViewModel()
		{	
			Filter = new ReportFilter();
		}
	}
}