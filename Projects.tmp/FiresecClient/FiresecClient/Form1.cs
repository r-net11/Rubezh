using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Windows.Forms;

namespace FiresecClient
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Connect_Click(object sender, EventArgs e)
		{
			ClientManager.Connect(ClientType.Administrator, textBox1.Text, textBox2.Text, textBox3.Text);
		}

		private void StartPoll_Click(object sender, EventArgs e)
		{
			ClientManager.StartPoll();
		}

		private void Poll_Click(object sender, EventArgs e)
		{
			new System.Threading.Thread(() => ClientManager.FiresecService.Poll(FiresecServiceFactory.UID, 9999)).Start();
		}

		private void Disconnect_Click(object sender, EventArgs e)
		{
			ClientManager.Disconnect();
		}
	}
}
