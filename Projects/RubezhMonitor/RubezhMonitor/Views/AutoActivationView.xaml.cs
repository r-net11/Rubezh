using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using RubezhAPI.Journal;

namespace RubezhMonitor.Views
{
	public partial class AutoActivationView : UserControl
	{
		public AutoActivationView()
		{
			InitializeComponent();
		}
	}
}