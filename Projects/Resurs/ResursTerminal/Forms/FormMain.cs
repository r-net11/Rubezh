using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using ResursNetwork.Networks;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.ApplicationLayer.Devices;


namespace ResursTerminal.Forms
{
	public partial class FormMain : Form
	{
		#region Fields And Properties

		NetworksManager _NetworksManager = NetworksManager.Instance;

		#endregion

		#region Constructors

		public FormMain()
		{
			InitializeComponent();
			Load += EventHandler_FormMain_Load;
		}

		#endregion

		#region Events Handler

		void EventHandler_FormMain_Load(object sender, EventArgs e)
		{
			#region the code is For debug

			var connection = 
				new ResursNetwork.Incotex.NetworkControllers.DataLinkLayer.ComPort
			{
				PortName = "COM2",
				BaudRate = 9600,
			};

			var network = new IncotexNetworkController
			{
				Connection = connection
			};

			var device = new Mercury203
			{
				Address = 23801823
			};

			network.Devices.Add(device);

			_NetworksManager.Networks.Add(network);

			device.Start();
			network.Start();

			#endregion
			
			InitTreeViewSystem(_treeViewSystem);
			InitContextMenuSystem();
		}

		void EventHandler_ContextMenuSystem_Click(
			object sender, EventArgs e)
		{
			ToolStripMenuItem control = (ToolStripMenuItem)sender;
			
			if (control.Equals(_toolStripMenuItemAddDevice))
			{
				Debug.WriteLine(_toolStripMenuItemAddDevice.Name);
			}
			else if (control.Equals(_toolStripMenuItemAddNetwork))
			{
				Debug.WriteLine(_toolStripMenuItemAddNetwork.Name);
			}
			else if (control.Equals(_toolStripMenuItemRemoveDevice))
			{
				Debug.WriteLine(_toolStripMenuItemRemoveDevice.Name);
			}
			else if (control.Equals(_toolStripMenuItemRemoveNetwork))
			{
				Debug.WriteLine(_toolStripMenuItemRemoveNetwork.Name);
			}
		}

		void EventHandlerTreeViewSystem_AfterSelect(
			object sender, TreeViewEventArgs e)
		{
			switch (e.Action)
			{
				case TreeViewAction.ByKeyboard:
				case TreeViewAction.ByMouse:
					{
						if (e.Node.Equals(_treeViewSystem.TopNode))
						{
							_contextMenuStripSystem.Enabled = true;
							_toolStripMenuItemAddDevice.Enabled = false;
							_toolStripMenuItemRemoveDevice.Enabled = false;
							_toolStripMenuItemAddNetwork.Enabled = true;
							_toolStripMenuItemRemoveNetwork.Enabled = false;

							ShowObjectPropeties();
							SetCommands();
						}
						else if (e.Node.Tag is IDevice)
						{
							_contextMenuStripSystem.Enabled = true;
							_toolStripMenuItemAddDevice.Enabled = false;
							_toolStripMenuItemRemoveDevice.Enabled = true;
							_toolStripMenuItemAddNetwork.Enabled = false;
							_toolStripMenuItemRemoveNetwork.Enabled = false;

							ShowObjectPropeties(e.Node.Tag);
							SetCommands((IDevice)e.Node.Tag);
						}
						else if (e.Node.Tag is INetwrokController)
						{
							_contextMenuStripSystem.Enabled = true;
							_toolStripMenuItemAddDevice.Enabled = true;
							_toolStripMenuItemRemoveDevice.Enabled = false;
							_toolStripMenuItemAddNetwork.Enabled = false;
							_toolStripMenuItemRemoveNetwork.Enabled = true;

							ShowObjectPropeties(e.Node.Tag);
							SetCommands();
						}
						else
						{
							_contextMenuStripSystem.Enabled = false;
							ShowObjectPropeties();
							SetCommands();
						}
						break;
					}
			}
		}

		private void EventHandler_buttonExecute_Click(
			object sender, EventArgs e)
		{
			if (_treeViewSystem.SelectedNode.Tag is IDevice)
			{
				var device = (IDevice)_treeViewSystem.SelectedNode.Tag;
				switch (_comboBoxCommands.SelectedItem.ToString())
				{
					case "Прочитать время и дату":
						{
							var result = device.ReadParameter(
								ResursAPI.ParameterNames.ParameterNamesMercury203.DateTime);
							var dt = ((IncotexDateTime)result.Value).ToDateTime();
							break;
						}
					case "Прочитать групповой адерс":
						{
							var result = device.ReadParameter(
								ResursAPI.ParameterNames.ParameterNamesMercury203.GADDR);
							break;
						}
				}
			}
		}

		#endregion

		#region Initialization Methods
		
		void InitTreeViewSystem(TreeView control)
		{
			TreeNode node;

			control.FullRowSelect = true;
			control.ShowRootLines = false;
			control.Nodes.Clear();


			node = new TreeNode
			{
				Name = "_treeNodeRoot",
				Text = "Контроллеры сетей"
			};
			control.Nodes.Add(node);
			control.TopNode = node;

			foreach (var network in _NetworksManager.Networks)
			{
				node = new TreeNode
					{
						Text = network.Id.ToString(),
						Tag = network
					};
				//control.TopNode.Nodes.Add(node);
				(control.Nodes.Find(control.TopNode.Name, false))[0].Nodes.Add(node);

				var devices = network.Devices.Select(p =>
					new TreeNode { Text = p.Address.ToString(), Tag = p }).ToArray();
				node.Nodes.AddRange(devices);
			}

			control.ExpandAll();
			control.ContextMenuStrip = _contextMenuStripSystem;
			control.AfterSelect += EventHandlerTreeViewSystem_AfterSelect;

		}

		void InitContextMenuSystem()
		{
			var control = _contextMenuStripSystem;

			//foreach (ToolStripMenuItem item in control.Items)
			//{
			//	item.Click += EventHandler_ContextMenuSystem_Click;
			//}

			_toolStripMenuItemAddDevice.Click += 
				EventHandler_ContextMenuSystem_Click;
			_toolStripMenuItemAddNetwork.Click +=
				EventHandler_ContextMenuSystem_Click;
			_toolStripMenuItemRemoveDevice.Click +=
				EventHandler_ContextMenuSystem_Click;
			_toolStripMenuItemRemoveNetwork.Click +=
				EventHandler_ContextMenuSystem_Click;
		}

		#endregion

		#region Methods

		void ShowObjectPropeties(Object component = null)
		{
			_propertyGrid.SelectedObject = null;

			if (component != null)
			{
				_propertyGrid.SelectedObject = component;
			}
		}

		void SetCommands(IDevice device = null)
		{
			_comboBoxCommands.Enabled = device == null ? false : true;
				
			if (device != null)
			{
				_comboBoxCommands.Items.Clear();
				
				switch (device.DeviceModel)
				{
					case DeviceModel.Mercury203:
						{
							_comboBoxCommands.Items.AddRange(new String[] 
							{
								"Прочитать время и дату",
								"Прочитать групповой адерс"
							});
							break;
						}
				}
			}
		}
		#endregion

	}
}
