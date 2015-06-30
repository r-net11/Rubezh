﻿using System.Windows;
using MultiClientAdministrator.ViewModels;

namespace MultiClientAdministrator
{
	public partial class ShellView : Window
	{
		public ShellView()
		{
			InitializeComponent();
			Closing += new System.ComponentModel.CancelEventHandler(ShellView_Closing);
		}

		void ShellView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			(DataContext as ShellViewModel).SaveOnClose();
		}
	}
}