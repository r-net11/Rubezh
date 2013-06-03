using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;

namespace USBTest
{
	public partial class MainWindow : Window
	{
		Test Test;

		public MainWindow()
		{
			InitializeComponent();
			Test = new Test();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Test.Start();
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			Test.Stop();
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			CancellationTokenSource cts = new CancellationTokenSource();

			cts.Token.Register(() =>
			{
				button1.IsEnabled = true;
				button1.Content = "Cancelled!";
			}, true);

			Task t = Task.Factory.StartNew(() =>
			{
				button1.IsEnabled = false;
				button1.Content = "In progress";
			},
			CancellationToken.None,
			TaskCreationOptions.None,
			TaskScheduler.FromCurrentSynchronizationContext());

			t.ContinueWith((Task previous) =>
			{
				cts.Cancel();
			})
			.ContinueWith((Task previous) =>
			{
				button1.Content = previous.Status;
			},
			CancellationToken.None,
			TaskContinuationOptions.OnlyOnFaulted,
			TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			TasksCancellation(10);
		}


		private static void TasksCancellation(int items)
		{
			CancellationTokenSource cts = new CancellationTokenSource();


			Task[] tasks = new Task[items];


			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i] = Task.Factory.StartNew(() =>
				{
					cts.Token.WaitHandle.WaitOne();
					Console.WriteLine("TaskId: {0} Token has been cancelled!\n", Task.CurrentId);
				}, cts.Token);
			}


			Console.WriteLine("Press <Enter> to cancel the operations.");
			Console.ReadLine();
			cts.Cancel();


			try
			{
				Task.WaitAll(tasks);
			}
			catch (AggregateException ex)
			{
				foreach (var e in ex.InnerExceptions)
				{
					Console.WriteLine("{0}", e.Message);
				}
			}
		}
	}
}