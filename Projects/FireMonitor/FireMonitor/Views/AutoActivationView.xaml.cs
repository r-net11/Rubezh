using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using FiresecAPI.Journal;

namespace FireMonitor.Views
{
	public partial class AutoActivationView : UserControl
	{
		public AutoActivationView()
		{
			InitializeComponent();
		}
	}
}