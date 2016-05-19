using System.Collections.Generic;
using RubezhAPI.Models;

namespace VideoModule.Views
{
	public partial class _2X2GridView
	{
		public Dictionary<int, Camera> Dictionary { get; set; }
		public _2X2GridView()
		{
			InitializeComponent();
			Dictionary = new Dictionary<int, Camera>();
		}
	}
}