using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
			//the code is For debug
			var network = new IncotexNetworkControllerVirtual();
			_NetworksManager.Networks.Add(network);
			network.Devices.Add(new Mercury203Virtual());

			InitTreeViewSystem(_treeViewSystem);
		}

		void EventHandlerTreeViewSystem_AfterSelect(
			object sender, TreeViewEventArgs e)
		{
			switch (e.Action)
			{
				case TreeViewAction.ByKeyboard:
				case TreeViewAction.ByMouse:
					{
						if (e.Node.Tag is IDevice)
						{
							_toolStripMenuItemAddDevice.Enabled = true;
							_toolStripMenuItemRemoveDevice.Enabled = true;
							_toolStripMenuItemAddNetwork.Enabled = false;
							_toolStripMenuItemRemoveNetwork.Enabled = false;
						}
						else if (e.Node.Tag is INetwrokController)
						{
							_toolStripMenuItemAddDevice.Enabled = false;
							_toolStripMenuItemRemoveDevice.Enabled = false;
							_toolStripMenuItemAddNetwork.Enabled = true;
							_toolStripMenuItemRemoveNetwork.Enabled = true;
						}
						break;
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

			control.ContextMenuStrip = _contextMenuStripSystem;
			control.AfterSelect += EventHandlerTreeViewSystem_AfterSelect;

		}

		#endregion
	}
}
