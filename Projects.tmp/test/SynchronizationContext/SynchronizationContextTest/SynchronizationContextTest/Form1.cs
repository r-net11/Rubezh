using System;
using System.Threading;
using System.Windows.Forms;

namespace SynchronizationContextTest
{
	public partial class Form1 : Form
	{
		public static SynchronizationContext SyncContext { get; private set; }
		public Form1()
		{
			InitializeComponent();
			SyncContext = SynchronizationContext.Current;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var thread1 = new Thread(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					SyncContext.Post(state => { button1.Text = state.ToString(); }, i);
					Thread.Sleep(100);
				}
			});
			var thread2 = new Thread(() =>
			{
				for (int i = 10000; i < 10100; i++)
				{
					SyncContext.Post(state => { button1.Text = state.ToString(); }, i);
					Thread.Sleep(100);
				}
			});

			thread1.Start();
			thread2.Start();
		}
	}
}
