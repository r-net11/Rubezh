using Common;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.ServiceModel;
using System.Windows.Forms;

namespace FiresecClient
{
	public partial class Form1 : Form
	{
		public Guid ClientUID { get; private set; }
		ChannelFactory<IFiresecService> _factory;
		public Form1()
		{
			InitializeComponent();

			ClientUID = Guid.NewGuid();
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

		private void Test_Click(object sender, EventArgs e)
		{
			var result = ClientManager.FiresecService.Test("Test");
		}

		private void Connect2_Click(object sender, EventArgs e)
		{
			var service = _factory.CreateChannel();
			using (service as IDisposable)
				service.Connect(new ClientCredentials { ClientType = ClientType.Administrator, ClientUID = ClientUID, Login = textBox2.Text, Password = textBox3.Text });
		}

		private void NewChannelFactory_Click(object sender, EventArgs e)
		{
			var tcpUri = new Uri(textBox1.Text);
			var endpointAddress = new EndpointAddress(tcpUri);
			var binding = BindingHelper.CreateNetTcpBinding();
			_factory = new ChannelFactory<IFiresecService>(binding, endpointAddress);
		}

		private void Disconnect2_Click(object sender, EventArgs e)
		{
			var service = _factory.CreateChannel();
			using (service as IDisposable)
				service.Disconnect(ClientUID);
		}

		private void Poll2_Click(object sender, EventArgs e)
		{
			new System.Threading.Thread(() =>
				{
					var service = _factory.CreateChannel();
					using (service as IDisposable)
						service.Poll(ClientUID, 999);
				}
				).Start();
		}

		private void Test2_Click(object sender, EventArgs e)
		{
			new System.Threading.Thread(() =>
				{
					var service = _factory.CreateChannel();
					using (service as IDisposable)
						service.Test(ClientUID, "1234567890");
				}
				).Start();
		}
	}
}
