using System;
using System.Windows;
using System.Windows.Controls;
using FireAdministrator.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace FireAdministrator.Views
{
	public partial class MenuView : UserControl
	{
		public MenuView()
		{
			InitializeComponent();
			DataContextChanged += new DependencyPropertyChangedEventHandler(MenuView_DataContextChanged);
			ServiceFactory.SaveService.Changed += new Action(SaveService_Changed);
		}

		void MenuView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is MenuViewModel)
				((MenuViewModel)e.NewValue).SetNewConfigEvent += (s, ee) => { ee.Cancel = !ConfigManager.SetNewConfig(); };
		}

		void SaveService_Changed()
		{
			_saveButton.IsEnabled = ServiceFactory.SaveService.HasChanges;
		}
	}
}