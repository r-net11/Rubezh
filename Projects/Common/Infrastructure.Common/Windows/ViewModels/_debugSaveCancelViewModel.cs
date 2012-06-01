using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Navigation;
using System.Windows.Data;

namespace Infrastructure.Common.Windows.ViewModels
{
	//ResourceService srv = new ResourceService();
	//srv.AddResource(new ResourceDescription(typeof(WindowBaseView).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
	//_debugSaveCancelViewModel model = new _debugSaveCancelViewModel();
	//model.Title = "Test";
	//WindowBaseView win = new WindowBaseView(model);
	//win.ShowDialog();
	//return;

	public class _debugSaveCancelViewModel : ShellViewModel
	{
		public _debugSaveCancelViewModel()
		{
			NavigationItems = new List<Navigation.NavigationItem>()
			{
				new NavigationItem("test1", null, new List<NavigationItem>()
					{
						new NavigationItem("go1"),
						new NavigationItem("go2"),
						new NavigationItem("go3"),
						new NavigationItem("go4"),
						new NavigationItem("go5"),
					}),
				new NavigationItem("test2"),
				new NavigationItem("test3"),
				new NavigationItem("test4"),
				new NavigationItem("test5", null, new List<NavigationItem>()
					{
						new NavigationItem("go1"),
						new NavigationItem("go2"),
						new NavigationItem("go3", null, new List<NavigationItem>()
					{
						new NavigationItem("go1"),
						new NavigationItem("go2"),
						new NavigationItem("go3"),
						new NavigationItem("go4"),
						new NavigationItem("go5"),
					}),
						new NavigationItem("go4", null, new List<NavigationItem>()
					{
						new NavigationItem("go1"),
						new NavigationItem("go2"),
						new NavigationItem("go3"),
						new NavigationItem("go4"),
						new NavigationItem("go5"),
					}),
						new NavigationItem("go5"),
					}),
				new NavigationItem("test6"),
				new NavigationItem("test7"),
			};
			ContentFotter = new _debugReplacer("footter...");
			ContentHeader = new _debugReplacer("header...");
			MainContent = new _debugReplacer("MAIN CONTENT");
			Toolbar = new _debugReplacer("main toolbar");
			MinHeight = 200;
			MinWidth = 100;
		}
	}

	public class _debugReplacer : BaseViewModel
	{
		public string Value { get; private set; }
		public _debugReplacer(string value)
		{
			Value = value;
		}
		public override string ToString()
		{
			return Value;
		}
	}
}
