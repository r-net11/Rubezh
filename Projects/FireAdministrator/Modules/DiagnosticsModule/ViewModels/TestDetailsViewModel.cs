using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace DiagnosticsModule.ViewModels
{
	public class TestDetailsViewModel : DialogViewModel
	{
		public TestDetailsViewModel()
		{
			Title = "Тест производительности";
			Items = new ObservableCollection<TestItem>();
			var thread = new Thread(OnRun);
			thread.Start();
		}

		public ObservableCollection<TestItem> Items { get; private set; }

		void OnRun()
		{
			for (int i = 1; i < 100000; i++)
			{
				ApplicationService.Invoke(new Action(() =>
				{
					var testItem = new TestItem() { Name = "Name " + i };
					Items.Add(testItem);
				}));
				//Thread.Sleep(10);
			}
		}
	}

	public class TestItem
	{
		public string Name { get; set; }
	}
}